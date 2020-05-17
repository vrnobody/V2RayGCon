using AutocompleteMenuNS;
using ScintillaNET;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Luna.Libs.LuaSnippet
{
    internal sealed class BestMatchSnippets :
        IEnumerable<AutocompleteItem>
    {
        private readonly Scintilla editor;
        string searchPattern = VgcApis.Models.Consts.Patterns.LuaSnippetSearchPattern;

        List<ApiFunctionSnippets> apiFunctions;
        List<LuaFuncSnippets> luaFunctions;
        List<LuaKeywordSnippets> luaKeywords;
        List<LuaSubFuncSnippets> luaSubFunctions;

        List<LuaImportClrSnippets> luaImportClrs;

        List<LuaImportClrSnippets> customRequireModuleSnippets = new List<LuaImportClrSnippets>();
        List<MatchItemBase> customScriptSnippets = new List<MatchItemBase>();

        public BestMatchSnippets(
            Scintilla editor,

            List<ApiFunctionSnippets> apiFunctions,
            List<LuaFuncSnippets> luaFunctions,
            List<LuaKeywordSnippets> luaKeywords,
            List<LuaSubFuncSnippets> luaSubFunctions,
            List<LuaImportClrSnippets> luaImportClrs)
        {
            this.editor = editor;

            this.apiFunctions = apiFunctions;
            this.luaFunctions = luaFunctions;
            this.luaKeywords = luaKeywords;
            this.luaSubFunctions = luaSubFunctions;
            this.luaImportClrs = luaImportClrs;
        }

        #region public methods
        public void UpdateCustomScriptSnippets(List<MatchItemBase> snippets)
        {
            if (snippets != null)
            {
                this.customScriptSnippets = snippets;
            }
        }

        public void UpdateRequireModuleSnippets(List<LuaImportClrSnippets> snippets)
        {
            if (snippets != null)
            {
                this.customRequireModuleSnippets = snippets;
            }
        }

        #endregion

        #region private methods

        private IEnumerable<AutocompleteItem> BuildList()
        {
            var fragment = "";
            VgcApis.Misc.UI.Invoke(() =>
            {
                fragment = VgcApis.Misc.Utils.GetFragment(editor, searchPattern);
            });

            var cache = customScriptSnippets;

            List<MatchItemBase> candidates = cache
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
                items.AddRange(customRequireModuleSnippets);
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
