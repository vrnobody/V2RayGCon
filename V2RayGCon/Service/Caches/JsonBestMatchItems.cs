using AutocompleteMenuNS;
using ScintillaNET;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace V2RayGCon.Service.Caches
{
    internal sealed class JsonBestMatchItems :
        IEnumerable<AutocompleteItem>
    {
        Scintilla editor;
        string searchPattern =
            VgcApis.Models.Consts.Patterns.JsonSnippetSearchPattern;

        List<string> keywords;

        public JsonBestMatchItems(
            Scintilla editor,
            string matchPattern,
            IEnumerable<string> rawKeywords)
        {
            this.searchPattern = matchPattern;
            this.editor = editor;
            this.keywords = rawKeywords.ToList();
        }

        #region private methods


        private IEnumerable<AutocompleteItem> BuildList()
        {
            var fragment = VgcApis.Libs.Utils.GetFragment(
                editor, searchPattern);

            var table = new Dictionary<string, long>();

            foreach (var keyword in keywords)
            {
                var marks = VgcApis.Libs.Utils.MeasureSimilarityCi(
                    keyword, fragment);

                if (marks > 0 && !table.ContainsKey(keyword))
                {
                    table.Add(keyword, marks);
                }
            }

            var sorted = table
                .OrderBy(kv => kv.Value)
                .ThenBy(kv => kv.Key)
                .Select(kv => kv.Key)
                .ToList();


            //return autocomplete items
            foreach (var word in sorted)
                yield return new JsonKeywordItems(word);
        }

        #endregion

        #region IEnumerable thinggy
        public IEnumerator<AutocompleteItem> GetEnumerator() =>
            BuildList().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        #endregion
    }
}
