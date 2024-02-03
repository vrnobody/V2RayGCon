using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using Moq;
using Neo.IronLua;
using Newtonsoft.Json.Linq;

namespace NeoLuna.Services
{
    internal class AstServer : VgcApis.BaseClasses.Disposable
    {
        public event Action OnFileChanged;
        Libs.LuaSnippet.SnippetsCache snpCache;
        VgcApis.Libs.Tasks.LazyGuy cacheRecycler;

        public AstServer() { }

        public void Run()
        {
            this.snpCache = new Libs.LuaSnippet.SnippetsCache();
            UpdateRequireModuleNameCache();
            fsWatcher = CreateFileSystemWatcher(@"3rd/neolua");
            cacheRecycler = new VgcApis.Libs.Tasks.LazyGuy(RenewCacheWorker, 20 * 60 * 1000, 1000);
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
        FileSystemWatcher fsWatcher;

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
            UpdateRequireModuleNameCache(); // for require(...) snippets
            var mn = VgcApis.Misc.Utils.GetLuaModuleName(e.FullPath).Replace(".", "/");
            if (string.IsNullOrWhiteSpace(mn))
            {
                return;
            }

            RenewCacheLater();
            astModuleCache.Remove(mn);
            astModuleExCache.Remove(mn);

            try
            {
                OnFileChanged?.Invoke();
            }
            catch { }
        }

        #endregion

        #region caches
        VgcApis.Libs.Infr.StringLruCache<JObject> astCodeCache =
            new VgcApis.Libs.Infr.StringLruCache<JObject>(60, TimeSpan.MinValue);
        VgcApis.Libs.Infr.StringLruCache<JObject> astModuleCache =
            new VgcApis.Libs.Infr.StringLruCache<JObject>();
        VgcApis.Libs.Infr.StringLruCache<JObject> astModuleExCache =
            new VgcApis.Libs.Infr.StringLruCache<JObject>();

        void RenewCacheLater() => cacheRecycler.Postpone();

        void RenewCacheWorker()
        {
            astCodeCache = new VgcApis.Libs.Infr.StringLruCache<JObject>();
            astModuleCache = new VgcApis.Libs.Infr.StringLruCache<JObject>();
            astModuleExCache = new VgcApis.Libs.Infr.StringLruCache<JObject>();
        }

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

        public JObject AnalyzeModule(string path, bool isExMode)
        {
            RenewCacheLater();
            var cache = isExMode ? astModuleExCache : astModuleCache;
            if (cache.TryGet(path, out var ast))
            {
                return ast;
            }

            var mode = isExMode ? AnalyzeModes.ModuleEx : AnalyzeModes.Module;
            var mAst = AnalyzeModuleCore(path, mode);
            if (mAst != null)
            {
                cache.Add(path, mAst);
            }
            return mAst;
        }

        public JObject AnalyzeCode(string code)
        {
            RenewCacheLater();
            if (astCodeCache.TryGet(code, out var cachedAst))
            {
                return cachedAst;
            }

            var ast = AnalyzeCore(code, AnalyzeModes.SourceCode);
            astCodeCache.Add(code, ast);
            return ast;
        }

        public ReadOnlyCollection<string> GetRequireModuleNames() =>
            requireModuleNamesCache.AsReadOnly();
        #endregion

        #region private methods
        List<string> requireModuleNamesCache;

        void UpdateRequireModuleNameCache()
        {
            var requiresNames = new List<string>();
            try
            {
                string[] fileArray = Directory.GetFiles(
                    @"3rd/neolua",
                    "*.lua",
                    SearchOption.AllDirectories
                );
                foreach (var file in fileArray)
                {
                    if (!string.IsNullOrEmpty(file) || !file.ToLower().EndsWith(".lua"))
                    {
                        var mn = file.Replace("\\", "/").Substring(0, file.Length - ".lua".Length);
                        requiresNames.Add($"require('{mn}')");
                    }
                }
            }
            catch { }

            requireModuleNamesCache = requiresNames;
        }

        JObject AnalyzeCore(string code, AnalyzeModes analyzeMode)
        {
            string name = "anz.lua";
            using (Lua state = new Lua())
            {
                try
                {
                    var g = CreateAnalyser(state);
                    g["code"] = code;

                    string script = CreateAnalyzeTemplateScript(analyzeMode);

                    var chunk = state.CompileChunk(
                        script,
                        name,
                        new LuaCompileOptions() { ClrEnabled = false, DebugEngine = null, }
                    );

                    var rs = g.DoChunk(chunk);
                    if (rs != null && rs.Count > 0)
                    {
                        var r = rs[0]?.ToString();
                        if (!string.IsNullOrEmpty(r))
                        {
                            return JObject.Parse(r);
                        }
                    }
                }
                catch
                {
                    // do nothing
                }
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
                @"local analyzer = require('3rd/neolua/libs/luacheck/analyzer').new();"
                + @"return analyzer.{0}(code)";

            var s = string.Format(tpl, fn);
            var sb = new StringBuilder();
            sb.AppendLine(Resources.Files.Datas.LuaPredefinedFunctions);
            sb.AppendLine(s);

            var script = sb.ToString();
            return script;
        }

        JObject AnalyzeModuleCore(string path, AnalyzeModes mode)
        {
            try
            {
                var p = path.Replace('.', Path.DirectorySeparatorChar)
                    .Replace('/', Path.DirectorySeparatorChar);
                var fn = p + ".lua";
                var code = File.ReadAllText(fn);
                return AnalyzeCore(code, mode);
            }
            catch { }
            return null;
        }

        static readonly LuaTable mockApis = new LuaTable()
        {
            { "Misc", new Mock<Interfaces.ILuaMisc>().Object },
            { "Signal", new Mock<Interfaces.ILuaSignal>().Object },
            { "Sys", new Mock<Interfaces.ILuaSys>().Object },
            { "Server", new Mock<Interfaces.ILuaServer>().Object },
            { "Web", new Mock<Interfaces.ILuaWeb>().Object },
        };

        LuaGlobal CreateAnalyser(Lua anz)
        {
            var g = anz.CreateEnvironment<LuaGlobal>();
            g["std"] = mockApis;
            Misc.Patches.FixTableStringMath(g);
            return g;
        }
        #endregion

        #region
        protected override void Cleanup()
        {
            StopFileSystemWatcher();
            snpCache?.Dispose();
            cacheRecycler?.Dispose();
        }
        #endregion
    }
}
