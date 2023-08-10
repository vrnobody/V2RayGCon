using AutocompleteMenuNS;
using ScintillaNET;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NeoLuna.Libs.LuaSnippet
{
    internal sealed class BestMatchSnippets :
        IEnumerable<AutocompleteItem>
    {
        private Scintilla editor;
        string searchPattern = VgcApis.Models.Consts.Patterns.LuaSnippetSearchPattern;

        List<ApiFunctionSnippets> apiFunctions;
        List<LuaFuncSnippets> luaFunctions;
        List<LuaKeywordSnippets> luaKeywords;
        List<LuaSubFuncSnippets> luaSubFunctions;

        List<LuaImportClrSnippets> customRequireModuleSnippets = new List<LuaImportClrSnippets>();
        List<MatchItemBase> customScriptSnippets = new List<MatchItemBase>();

        public BestMatchSnippets(
            Scintilla editor,

            List<ApiFunctionSnippets> apiFunctions,
            List<LuaFuncSnippets> luaFunctions,
            List<LuaKeywordSnippets> luaKeywords,
            List<LuaSubFuncSnippets> luaSubFunctions)
        {
            this.editor = editor;

            this.apiFunctions = apiFunctions;
            this.luaFunctions = luaFunctions;
            this.luaKeywords = luaKeywords;
            this.luaSubFunctions = luaSubFunctions;
        }

        #region public methods
        public void UpdateCustomScriptSnippets(List<MatchItemBase> snippets)
        {
            if (snippets == null)
            {
                return;
            }

            lock (this.customScriptSnippets)
            {
                var old = this.customScriptSnippets;
                this.customScriptSnippets = snippets;
                old.Clear();
            }
        }

        public void UpdateRequireModuleSnippets(List<LuaImportClrSnippets> snippets)
        {
            if (snippets == null)
            {
                return;
            }

            lock (this.customRequireModuleSnippets)
            {
                var old = this.customRequireModuleSnippets;
                this.customRequireModuleSnippets = snippets;
                old.Clear();
            }
        }

        public void Cleanup()
        {
            this.editor = null;
            customRequireModuleSnippets?.Clear();
            customScriptSnippets?.Clear();
        }

        #endregion

        #region private methods
        HashSet<string> ignoredList =
            new HashSet<string>(
                VgcApis.Models.Consts.Lua.NeoLuaKeyWords.Split(' '));

        private IEnumerable<AutocompleteItem> BuildList()
        {
            var fragment = "";
            var editor = this.editor;
            if (editor != null)
            {
                VgcApis.Misc.UI.Invoke(() =>
                {
                    fragment = VgcApis.Misc.Utils.GetFragment(editor, searchPattern);
                });
            }

            var snps = new List<AutocompleteItem>();
            if (!ignoredList.Contains(fragment))
            {
                snps = CreateSnippets(fragment);
            }

            //return autocomplete items
            foreach (var item in snps)
                yield return item;
        }

        private List<AutocompleteItem> CreateSnippets(string fragment)
        {
            List<MatchItemBase> candidates;
            lock (this.customScriptSnippets)
            {
                candidates = customScriptSnippets
                   .Concat(GenCandidateList(fragment))
                   .ToList();
            }

            var table = new Dictionary<MatchItemBase, long>();
            foreach (var candidate in candidates)
            {
                var marks = candidate.MeasureSimilarityCi(fragment);
                if (marks > 0)
                {
                    table.Add(candidate, marks);
                }
            }

            var snps = table
                .OrderBy(kv => kv.Value)
                .ThenBy(kv => kv.Key.GetLowerText())
                .Select(kv => kv.Key as AutocompleteItem)
                .ToList();
            return snps;
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
                items.AddRange(luaSubFunctions);
            }
            else if (fragment.Contains("("))
            {
                lock (customRequireModuleSnippets)
                {
                    items.AddRange(customRequireModuleSnippets);
                }
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
