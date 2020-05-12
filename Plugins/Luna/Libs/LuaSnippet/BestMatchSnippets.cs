using AutocompleteMenuNS;
using Moq;
using NLua;
using ScintillaNET;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Luna.Libs.LuaSnippet
{
    internal sealed class BestMatchSnippets :
        IEnumerable<AutocompleteItem>
    {
        Scintilla editor;
        string searchPattern = @"";

        List<ApiFunctionSnippets> apiFunctions;
        List<LuaFuncSnippets> luaFunctions;
        List<LuaKeywordSnippets> luaKeywords;
        List<LuaSubFuncSnippets> luaSubFunctions;
        private readonly List<LuaImportClrSnippets> luaImportClrs;

        ConcurrentDictionary<string, List<string>> luaModulesCache =
            new ConcurrentDictionary<string, List<string>>();
        List<MatchItemBase> luaScriptCache = new List<MatchItemBase>();

        public BestMatchSnippets(
            Scintilla editor,
            string searchPattern,
            List<ApiFunctionSnippets> apiFunctions,
            List<LuaFuncSnippets> luaFunctions,
            List<LuaKeywordSnippets> luaKeywords,
            List<LuaSubFuncSnippets> luaSubFunctions,
            List<LuaImportClrSnippets> luaImportClrs)
        {
            this.searchPattern = searchPattern;
            this.editor = editor;
            this.apiFunctions = apiFunctions;
            this.luaFunctions = luaFunctions;
            this.luaKeywords = luaKeywords;
            this.luaSubFunctions = luaSubFunctions;
            this.luaImportClrs = luaImportClrs;

            BindEvents();
        }

        #region private methods
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

            fsw.Changed += FileSystemEventHandler;
            fsw.Created += FileSystemEventHandler;
            fsw.Deleted += FileSystemEventHandler;

            fsw.EnableRaisingEvents = true;

            return fsw;
        }

        void FileSystemEventHandler(object sender, FileSystemEventArgs e)
        {
            var mn = VgcApis.Misc.Utils.GetLuaModuleName(e.FullPath);
            if (string.IsNullOrWhiteSpace(mn))
            {
                return;
            }
            luaModulesCache.TryRemove(mn, out _);
        }

        VgcApis.Libs.Tasks.LazyGuy lazyAnalyser;

        void BindEvents()
        {
            lazyAnalyser = new VgcApis.Libs.Tasks.LazyGuy(AnalizeScriptWorker, 1000, 3000);
            this.editor.Disposed += (s, a) => Cleanup();
            fsWatcher = CreateFileSystemWatcher(@"lua");
            editor.TextChanged += InvokeScriptAnalyser;
        }

        void InvokeScriptAnalyser(object sender, EventArgs e) => lazyAnalyser?.Deadline();

        void AnalyseScript(LuaTable metaData)
        {
            List<MatchItemBase> snp = new List<MatchItemBase>();
            var keys = new string[] { "modules", "variables", "instances" };

            foreach (string key in keys)
            {
                var md = metaData[key] as LuaTable;
                switch (key)
                {
                    case "modules":
                        AddModuleSnippets(md, snp);
                        break;
                    case "variables":
                        AddVariableSnippets(md, snp);
                        break;
                    case "instances":
                        AddInstanceSnippets(md, snp);
                        break;
                }
            }
            luaScriptCache = snp;
        }

        void AddInstanceSnippets(LuaTable insts, List<MatchItemBase> snp)
        {
            foreach (string instName in insts?.Keys)
            {
                var inst = insts[instName] as LuaTable;
                var moduleName = inst["module"] as string;
                if (string.IsNullOrWhiteSpace(instName) || string.IsNullOrWhiteSpace(moduleName))
                {
                    continue;
                }

                try
                {
                    AddModulesMembers(snp, instName, moduleName);
                }
                catch { }
            }
        }

        void AddVariableSnippets(LuaTable vs, List<MatchItemBase> snp)
        {
            foreach (string v in vs?.Keys)
            {
                if (string.IsNullOrWhiteSpace(v) || v.Length < 2)
                {
                    continue;
                }

                var mi = ExtractOneMemberInfo(v, vs[v] as LuaTable);
                snp.Add(new LuaKeywordSnippets(mi.Substring(1)));
            }
        }

        void AddModuleSnippets(LuaTable modules, List<MatchItemBase> snp)
        {
            foreach (string variableName in modules?.Keys)
            {
                var module = modules[variableName] as LuaTable;
                var moduleName = module["module"] as string;
                if (string.IsNullOrWhiteSpace(variableName) || string.IsNullOrWhiteSpace(moduleName))
                {
                    continue;
                }

                try
                {
                    AddModulesMembers(snp, variableName, moduleName);
                }
                catch { }
            }
        }

        List<string> ExtractMembersInfo(LuaTable members)
        {
            var info = new List<string>();
            foreach (string memberName in members?.Keys)
            {
                var member = members[memberName] as LuaTable;
                var mi = ExtractOneMemberInfo(memberName, member);
                if (!string.IsNullOrEmpty(mi))
                {
                    info.Add(mi);
                }
            }
            return info;
        }

        string ExtractOneMemberInfo(string memberName, LuaTable member)
        {
            var type = member["type"] as string;
            switch (type)
            {
                case "table":
                    return $".{memberName}[]";
                case "number":
                case "string":
                    return $".{memberName}";
                case "function":
                    var pl = (member[type] as LuaTable)["paramList"] as LuaTable;
                    return GenFuncDefinition(pl, memberName);
            }
            return null;
        }

        string GenFuncDefinition(LuaTable pl, string memberName)
        {
            var list = new List<string>();
            foreach (long p in pl.Keys)
            {
                list.Add(pl[p] as string);
            }
            var sep = ".";
            if (list.FirstOrDefault() == "self")
            {
                list = list.Skip(1).ToList();
                sep = ":";
            }
            return $"{sep}{memberName}({string.Join(", ", list)})";
        }

        void AddModulesMembers(List<MatchItemBase> members, string varName, string moduleName)
        {
            if (!luaModulesCache.TryGetValue(moduleName, out var moduleCache))
            {
                Lua anz = CreateAnalyser();
                var metaData = CallLuaFunction(anz, "analyzeModule", $"'{moduleName}'");
                if (metaData == null || (metaData["type"] as string) != "table")
                {
                    return;
                }

                moduleCache = ExtractMembersInfo(metaData["table"] as LuaTable);
                luaModulesCache.TryAdd(moduleName, moduleCache);
            }

            foreach (var member in moduleCache)
            {
                if (string.IsNullOrEmpty(member) || member.Length < 2)
                {
                    continue;
                }
                var sep = member.Substring(0, 1);
                var snp = new LuaSubFuncSnippets($"{varName}{member}", sep);
                members.Add(snp);
            }
        }

        LuaTable CallLuaFunction(Lua state, string op, string param)
        {
            var script =
                $"local analyze = require('lua.libs.ast.analyze');" +
                $"return analyze.{op}({param});";

            var r = state.DoString(script);

            if (r == null)
            {
                return null;
            }

            return r[0] as LuaTable;
        }

        Lua CreateAnalyser()
        {
            Lua anz = new Lua()
            {
                UseTraceback = true,
            };

            anz.State.Encoding = Encoding.UTF8;

            // phony
            anz["Misc"] = new Mock<VgcApis.Interfaces.Lua.ILuaMisc>().Object;
            anz["Signal"] = new Mock<VgcApis.Interfaces.Lua.ILuaSignal>().Object;
            anz["Sys"] = new Mock<VgcApis.Interfaces.Lua.ILuaSys>().Object;
            anz["Server"] = new Mock<VgcApis.Interfaces.Lua.ILuaServer>().Object;
            anz["Web"] = new Mock<VgcApis.Interfaces.Lua.ILuaWeb>().Object;

            return anz;
        }

        void AnalizeScriptWorker()
        {
            Lua state = CreateAnalyser();
            try
            {
                string text = "";
                VgcApis.Misc.UI.Invoke(() =>
                {
                    text = editor.Text;
                });

                state["code"] = text;
                var result = CallLuaFunction(state, "analyzeSource", "code");
                if (result != null)
                {
                    AnalyseScript(result);
                }
            }
            catch (Exception e)
            {
                // debug
                var ex = e.ToString();
                var err = state?.GetDebugTraceback();
            }
        }

        void Cleanup()
        {
            StopFileSystemWatcher();
            lazyAnalyser?.Dispose();
        }

        private IEnumerable<AutocompleteItem> BuildList()
        {
            var fragment = VgcApis.Misc.Utils.GetFragment(
                editor, searchPattern);

            var cache = luaScriptCache;
            List<MatchItemBase> candidates = cache.Concat(GenCandidateList(fragment)).ToList();

            var table = new Dictionary<MatchItemBase, long>();
            foreach (var candidate in candidates)
            {
                var marks = candidate.MeasureSimilarityCi(fragment);
                if (marks > 0)
                {
                    table.Add(candidate, marks);
                }
            }

            var sorted = table
                .OrderBy(kv => kv.Value)
                .ThenBy(kv => kv.Key.GetLowerText())
                .Select(kv => kv.Key as AutocompleteItem)
                .ToList();

            //return autocomplete items
            foreach (var item in sorted)
                yield return item;
        }

        #endregion

        #region private methods
        List<MatchItemBase> GenCandidateList(string fragment)
        {
            var items = new List<MatchItemBase>();
            if (fragment.Contains(":"))
            {
                items.AddRange(apiFunctions);
            }
            else if (fragment.Contains("."))
            {
                if (fragment.Contains("("))
                {
                    items.AddRange(luaImportClrs);
                }
                items.AddRange(luaSubFunctions);
            }
            else if (fragment.Contains("("))
            {
                items.AddRange(luaImportClrs);
                items.AddRange(luaFunctions);
            }
            else
            {
                items.AddRange(luaKeywords);
            }

            return items;
        }
        #endregion

        #region IEnumerable thinggy
        public IEnumerator<AutocompleteItem> GetEnumerator() =>
            BuildList().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        #endregion
    }
}
