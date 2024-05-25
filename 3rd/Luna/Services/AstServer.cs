﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using Moq;
using Newtonsoft.Json.Linq;
using NLua;

namespace Luna.Services
{
    internal class AstServer : VgcApis.BaseClasses.Disposable
    {
        public event Action OnFileChanged;
        Libs.LuaSnippet.SnippetsCache snpCache;

        public AstServer() { }

        public void Run()
        {
            this.snpCache = new Libs.LuaSnippet.SnippetsCache();
            UpdateRequireModuleNameCache();
            fsWatcher = CreateFileSystemWatcher(@"lua");
        }

        #region constants
        public static string KEY_PARAMS = "params";
        public static string KEY_PROPERTY = "props";
        public static string KEY_FUNCTION = "funcs";
        public static string KEY_VARS = "vars";
        public static string KEY_MODULES = "modules";
        public static string KEY_LINE_NUM = "line"; // line number
        public static string KEY_METHODS = "methods";
        public static string KEY_SUB_FUNCS = "subs";
        #endregion

        #region file watching

        void StopFileSystemWatcher()
        {
            if (fsWatcher == null)
            {
                return;
            }
            fsWatcher.EnableRaisingEvents = false;
            fsWatcher.Dispose();
            fsWatcher = null;
        }

        FileSystemWatcher CreateFileSystemWatcher(string relativeFileName)
        {
            if (!Directory.Exists(relativeFileName))
            {
                return null;
            }

            var fsw = new FileSystemWatcher
            {
                IncludeSubdirectories = true,
                Path = relativeFileName,
                Filter = "*.lua",
            };

            fsw.Changed += LuaDirFileChangedHandler;
            fsw.Created += LuaDirFileChangedHandler;
            fsw.Deleted += LuaDirFileChangedHandler;
            fsw.Renamed += LuaDirFileChangedHandler;

            fsw.EnableRaisingEvents = true;

            return fsw;
        }

        void LuaDirFileChangedHandler(object sender, FileSystemEventArgs e)
        {
            UpdateRequireModuleNameCache();
            var mn = VgcApis.Misc.Utils.GetLuaModuleName(e.FullPath);
            if (string.IsNullOrWhiteSpace(mn))
            {
                return;
            }

            lock (moduleCacheLock)
            {
                astModuleCache.TryRemove(mn, out _);
                astModuleExCache.TryRemove(mn, out _);
            }

            try
            {
                OnFileChanged?.Invoke();
            }
            catch { }
        }

        #endregion

        #region properties
        FileSystemWatcher fsWatcher;
        readonly ConcurrentQueue<string> hotCacheKeys = new ConcurrentQueue<string>();
        readonly ConcurrentDictionary<string, JObject> astCodeCache =
            new ConcurrentDictionary<string, JObject>();
        readonly ConcurrentDictionary<string, JObject> astModuleCache =
            new ConcurrentDictionary<string, JObject>();
        readonly ConcurrentDictionary<string, JObject> astModuleExCache =
            new ConcurrentDictionary<string, JObject>();

        internal enum AnalyzeModes
        {
            SourceCode,
            Module,
            ModuleEx
        }
        #endregion

        #region public methods

        public Libs.LuaSnippet.BestMatchSnippets CreateBestMatchSnippet(
            ScintillaNET.Scintilla editor
        ) => snpCache?.CreateBestMatchSnippets(editor);

        public List<Dictionary<string, string>> GetWebUiLuaStaticSnippets() =>
            snpCache?.GetWebUiLuaStaticSnippets();

        public JObject AnalyzeModule(string moduleName, bool isExMode)
        {
            var cache = isExMode ? astModuleExCache : astModuleCache;
            if (cache.TryGetValue(moduleName, out var ast))
            {
                return ast;
            }

            var mode = isExMode ? AnalyzeModes.ModuleEx : AnalyzeModes.Module;
            var mAst = AnalyzeModuleCore(moduleName, mode);
            if (mAst != null)
            {
                lock (moduleCacheLock)
                {
                    TrimModuleAstCache(cache);
                    cache.TryAdd(moduleName, mAst);
                }
            }
            return mAst;
        }

        public JObject AnalyzeCode(string code)
        {
            if (astCodeCache.TryGetValue(code, out var cachedAst))
            {
                hotCacheKeys.Enqueue(code);
                return cachedAst;
            }

            var ast = AnalyzeCore(code, AnalyzeModes.SourceCode);
            AddToAstCodeCache(code, ast);
            return ast;
        }

        public ReadOnlyCollection<string> GetRequireModuleNames() =>
            requireModuleNamesCache.AsReadOnly();
        #endregion

        #region private methods
        readonly object moduleCacheLock = new object();
        readonly object codeCacheLock = new object();

        List<string> requireModuleNamesCache;

        void UpdateRequireModuleNameCache()
        {
            var requiresNames = new List<string>();
            try
            {
                string[] fileArray = Directory.GetFiles(
                    @"lua",
                    "*.lua",
                    SearchOption.AllDirectories
                );
                foreach (var file in fileArray)
                {
                    if (!string.IsNullOrEmpty(file) || !file.ToLower().EndsWith(".lua"))
                    {
                        var mn = file.Replace("\\", ".")
                            .Replace("/", ".")
                            .Substring(0, file.Length - ".lua".Length);
                        requiresNames.Add($"require('{mn}')");
                    }
                }
            }
            catch { }

            lock (moduleCacheLock)
            {
                requireModuleNamesCache = requiresNames;
            }
        }

        void TrimModuleAstCache<TKey, TValue>(ConcurrentDictionary<TKey, TValue> cache)
        {
            try
            {
                var keep = 100;
                var keys = cache.Keys.ToList();
                if (keys.Count > keep * 2)
                {
                    var cut = keys.Count - keep;
                    for (int i = 0; i < cut; i++)
                    {
                        cache.TryRemove(keys[i], out _);
                    }
                }
            }
            catch { }
        }

        JObject AnalyzeCore(string code, AnalyzeModes analyzeMode)
        {
            using (Lua state = CreateAnalyser())
            {
                state["code"] = code;
                var script = CreateAnalyzeTemplateScript(analyzeMode);
                try
                {
                    string r = state.DoString(script)[0]?.ToString();
                    if (!string.IsNullOrEmpty(r))
                    {
                        return JObject.Parse(r);
                    }
                }
                catch { }
            }
            return null;
        }

        string CreateAnalyzeTemplateScript(AnalyzeModes analyzeMode)
        {
            var fn = "analyzeCode";
            if (analyzeMode == AnalyzeModes.Module)
            {
                fn = "analyzeModule";
            }
            else if (analyzeMode == AnalyzeModes.ModuleEx)
            {
                fn = "analyzeModuleEx";
            }

            string tpl =
                @"local analyzer = require('lua.libs.luacheck.analyzer').new();"
                + @"return analyzer.{0}(code)";
            var script = string.Format(tpl, fn);

            var sb = new StringBuilder();
            sb.AppendLine(Resources.Files.Datas.LuaPredefinedFunctions);
            sb.AppendLine(script);
            return sb.ToString();
        }

        void AddToAstCodeCache(string key, JObject value)
        {
            lock (codeCacheLock)
            {
                while (hotCacheKeys.Count > 50)
                {
                    hotCacheKeys.TryDequeue(out _);
                }

                if (astCodeCache.Count > 200)
                {
                    var keys = astCodeCache.Keys;
                    var filterd = keys.Where(k => !hotCacheKeys.Contains(k))
                        .Skip(100 - hotCacheKeys.Count)
                        .ToList();
                    foreach (var k in filterd)
                    {
                        astCodeCache.TryRemove(k, out _);
                    }
                }

                astCodeCache.TryAdd(key, value);
            }
        }

        JObject AnalyzeModuleCore(string moduleName, AnalyzeModes mode)
        {
            try
            {
                var fn = moduleName.Replace('.', Path.DirectorySeparatorChar) + ".lua";
                var code = File.ReadAllText(fn);
                return AnalyzeCore(code, mode);
            }
            catch { }
            return null;
        }

        static readonly Dictionary<string, object> mockApis = new Dictionary<string, object>()
        {
            { "Misc", new Mock<Interfaces.ILuaMisc>().Object },
            { "Signal", new Mock<Interfaces.ILuaSignal>().Object },
            { "Sys", new Mock<Interfaces.ILuaSys>().Object },
            { "Server", new Mock<Interfaces.ILuaServer>().Object },
            { "Web", new Mock<Interfaces.ILuaWeb>().Object },
        };

        Lua CreateAnalyser()
        {
            Lua anz = new Lua();

            anz.State.Encoding = Encoding.UTF8;

            foreach (var kv in mockApis)
            {
                anz[kv.Key] = kv.Value;
            }
            return anz;
        }
        #endregion

        #region
        protected override void Cleanup()
        {
            StopFileSystemWatcher();
            snpCache?.Dispose();
        }
        #endregion
    }
}
