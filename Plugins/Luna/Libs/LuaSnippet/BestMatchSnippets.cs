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

        List<MatchItemBase> autoCompleteVarSnippets = new List<MatchItemBase>();
        List<MatchItemBase> autoCompleteFuncSnippets = new List<MatchItemBase>();
        List<MatchItemBase> autoCompleteEquationSnippets = new List<MatchItemBase>();

        ConcurrentDictionary<string, List<string>> autoCompleteModulesCache = new ConcurrentDictionary<string, List<string>>();

        List<LuaImportClrSnippets> luaImportClrs;
        List<LuaImportClrSnippets> luaRequireModules;

        VgcApis.Libs.Tasks.LazyGuy lazyAnalyser;

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
            luaRequireModules = new List<LuaImportClrSnippets>();

            BindEvents();
            UpdateLuaRequireModuleSnippets();
        }

        #region public methods
        bool isEnableCodeAnalyze;
        public void SetIsEnableCodeAnalyze(bool enable)
        {
            isEnableCodeAnalyze = enable;
            if (!enable)
            {
                autoCompleteEquationSnippets.Clear();
            }
            else
            {
                AnalyzeScriptLater(this, EventArgs.Empty);
            }
        }

        #endregion

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
            fsw.Renamed += FileSystemEventHandler;

            fsw.EnableRaisingEvents = true;

            return fsw;
        }

        void UpdateLuaRequireModuleSnippets()
        {
            try
            {
                List<LuaImportClrSnippets> snps = new List<LuaImportClrSnippets>();
                string[] fileArray = Directory.GetFiles(@"lua", "*.lua", SearchOption.AllDirectories);
                foreach (var file in fileArray)
                {
                    if (!string.IsNullOrEmpty(file) || !file.ToLower().EndsWith(".lua"))
                    {
                        var mn = file.Replace("\\", ".")
                            .Replace("/", ".")
                            .Substring(0, file.Length - ".lua".Length);

                        var scr = $"require('{mn}')";
                        snps.Add(new LuaImportClrSnippets(scr));
                    }
                }
                luaRequireModules = snps;
            }
            catch { }
        }

        void FileSystemEventHandler(object sender, FileSystemEventArgs e)
        {
            UpdateLuaRequireModuleSnippets();
            var mn = VgcApis.Misc.Utils.GetLuaModuleName(e.FullPath);
            if (string.IsNullOrWhiteSpace(mn))
            {
                return;
            }
            autoCompleteModulesCache.TryRemove(mn, out _);
        }

        void BindEvents()
        {
            lazyAnalyser = new VgcApis.Libs.Tasks.LazyGuy(AnalizeScriptWorker, 1000, 3000);
            this.editor.Disposed += (s, a) => Cleanup();
            fsWatcher = CreateFileSystemWatcher(@"lua");
            editor.TextChanged += AnalyzeScriptLater;
        }

        void AnalyzeScriptLater(object sender, EventArgs e)
        {
            if (isEnableCodeAnalyze)
            {
                lazyAnalyser?.Deadline();
            }
        }

        List<MatchItemBase> AnalyseScript(LuaTable metaData)
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

            return snp;
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
                if (!string.IsNullOrEmpty(mi) && mi.Length > 1)
                {
                    snp.Add(new LuaKeywordSnippets(mi.Substring(1)));
                }
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
            if (!autoCompleteModulesCache.TryGetValue(moduleName, out var moduleCache))
            {
                Lua anz = CreateAnalyser();
                var metaData = CallLuaFunction(anz, "analyzeModule", $"'{moduleName}'");
                if (metaData == null || (metaData["type"] as string) != "table")
                {
                    return;
                }

                moduleCache = ExtractMembersInfo(metaData["table"] as LuaTable);
                autoCompleteModulesCache.TryAdd(moduleName, moduleCache);
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

        string GetAllTextExceptCurLine()
        {
            StringBuilder sb = new StringBuilder();
            var curLine = editor.CurrentLine;
            foreach (var line in editor.Lines)
            {
                if (line.Index == curLine)
                {
                    continue;
                }
                sb.Append(line.Text);
            }
            return sb.ToString();
        }

        void AnalizeScriptWorker()
        {
            string src = "";
            VgcApis.Misc.UI.Invoke(() =>
            {
                src = GetAllTextExceptCurLine();
            });

            autoCompleteVarSnippets = GenAutoCompleteVarSnippets(src);
            autoCompleteFuncSnippets = GenAutoCompleteFuncSnippets(src);

            List<MatchItemBase> snps = GenAutoCompleteEquationSnippets(src);
            if (snps != null && snps.Count() > 0)
            {
                autoCompleteEquationSnippets = snps;
            }
        }

        ConcurrentDictionary<string, List<MatchItemBase>> acmEqCache = new ConcurrentDictionary<string, List<MatchItemBase>>();
        private List<MatchItemBase> GenAutoCompleteEquationSnippets(string src)
        {
            if (!acmEqCache.TryGetValue(src, out var snps))
            {
                CheckCacheSize(acmEqCache);
                Lua state = CreateAnalyser();
                state["code"] = src;
                var result = CallLuaFunction(state, "analyzeSource", "code");
                if (result == null)
                {
                    acmEqCache.TryAdd(src, null);
                }
                else
                {
                    snps = AnalyseScript(result);
                    acmEqCache.TryAdd(src, snps);
                }
            }
            return snps;
        }

        ConcurrentDictionary<string, List<MatchItemBase>> acmFunCache = new ConcurrentDictionary<string, List<MatchItemBase>>();
        List<MatchItemBase> GenAutoCompleteFuncSnippets(string src)
        {
            if (!acmFunCache.TryGetValue(src, out var snps))
            {
                CheckCacheSize(acmFunCache);
                snps = GenAutoCompleteFuncSnippetsWorker(src);
            }
            return snps;
        }

        List<MatchItemBase> GenAutoCompleteFuncSnippetsWorker(string src)
        {
            var gvs = VgcApis.Misc.Utils.ExtractFunctionsFromLuaScript(src);
            var gvsnps = new List<MatchItemBase>();
            foreach (var gv in gvs)
            {
                if (string.IsNullOrWhiteSpace(gv))
                {
                    continue;
                }

                if (gv.Contains(":"))
                {
                    gvsnps.Add(new LuaSubFuncSnippets(gv, ":"));
                }
                else if (gv.Contains("."))
                {
                    gvsnps.Add(new LuaSubFuncSnippets(gv, "."));
                }
                else
                {
                    gvsnps.Add(new LuaFuncSnippets(gv));
                }

            }
            return gvsnps;
        }

        ConcurrentDictionary<string, List<MatchItemBase>> acmVarCache = new ConcurrentDictionary<string, List<MatchItemBase>>();
        List<MatchItemBase> GenAutoCompleteVarSnippets(string src)
        {
            if (!acmVarCache.TryGetValue(src, out var snps))
            {
                CheckCacheSize(acmVarCache);
                snps = GenAutoCompleteVarSnippetsWorker(src);
            }
            return snps;
        }

        List<MatchItemBase> GenAutoCompleteVarSnippetsWorker(string src)
        {
            var gvs = VgcApis.Misc.Utils.ExtractGlobalVarsFromLuaScript(src);
            var gvsnps = new List<MatchItemBase>();
            foreach (var gv in gvs)
            {
                if (string.IsNullOrWhiteSpace(gv))
                {
                    continue;
                }
                gvsnps.Add(new LuaKeywordSnippets(gv));
            }
            return gvsnps;
        }

        void CheckCacheSize(ConcurrentDictionary<string, List<MatchItemBase>> cache)
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


        void Cleanup()
        {
            StopFileSystemWatcher();
            lazyAnalyser?.Dispose();
        }

        private IEnumerable<AutocompleteItem> BuildList()
        {
            var fragment = VgcApis.Misc.Utils.GetFragment(
                editor, searchPattern);

            var cache = autoCompleteEquationSnippets;
            List<MatchItemBase> candidates = cache
                .Concat(autoCompleteFuncSnippets)
                .Concat(GenCandidateList(fragment))
                .ToList();

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
                items.AddRange(luaRequireModules);
                items.AddRange(luaImportClrs);
                items.AddRange(luaFunctions);
            }
            else
            {
                items.AddRange(autoCompleteVarSnippets);
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
