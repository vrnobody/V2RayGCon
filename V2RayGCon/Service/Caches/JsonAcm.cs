using AutocompleteMenuNS;
using ScintillaNET;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace V2RayGCon.Service.Caches
{
    public sealed class JsonAcm
    {
        public JsonAcm() { }

        #region public methods
        public AutocompleteMenu BindToEditor(Scintilla editor)
        {
            const string SearchPattern =
            VgcApis.Models.Consts.Patterns.JsonSnippetSearchPattern;

            var acm = new AutocompleteMenu()
            {
                SearchPattern = SearchPattern,
                MaximumSize = new Size(320, 200),
                ToolTipDuration = 5000,
            };

            acm.TargetControlWrapper = new ScintillaWrapper(editor);
            var snippets = new JsonBestMatchItems(
                    editor, SearchPattern, GetKeywords());
            acm.SetAutocompleteItems(snippets);
            return acm;
        }

        #endregion

        #region private methods
        List<string> keywordCache = null;
        List<string> GetKeywords()
        {
            if (keywordCache == null)
            {
                keywordCache = Resource.Resx.StrConst.ConfigJsonKeywords
                    .Replace("\r", "\0")
                    .Replace("\n", "\0")
                    .Split(new char[] { '\0' }, StringSplitOptions.RemoveEmptyEntries)
                    .ToList();
            }

            return keywordCache;
        }

        #endregion

        #region protected methods

        #endregion
    }
}
