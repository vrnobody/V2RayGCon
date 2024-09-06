using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using VgcApis.Libs.Infr.KwFilterComps;

namespace VgcApis.Libs.Infr
{
    public class KeywordFilter
    {
        public KeywordFilter(string keyword)
        {
            ISimpleFilter f = SimpleIndexFilter.CreateFilter(keyword);

            keyword = keyword?.ToLower() ?? "";
            if (f == null)
            {
                f = SimpleTitleFilter.CreateFilter(keyword);
            }

            string[] tokens = Helpers.Tokenize(keyword);
            if (f == null)
            {
                f = AdvNumberFilter.CreateFilter(tokens);
            }
            if (f == null)
            {
                f = AdvStringFilter.CreateFilter(tokens);
            }

            if (f == null)
            {
                var err = $"Fail to create filter for keyword: \"{keyword}\"";
                Sys.FileLogger.Error(err);
            }

            if (f is SimpleIndexFilter sif)
            {
                isIndexFilter = true;
                parsedIndex = sif.GetIndex();
            }
            this.filter = f;
        }

        #region properties

        readonly bool isIndexFilter = false;
        readonly int parsedIndex = 0;
        readonly ISimpleFilter filter;
        readonly Highlighter emptyHighlighter = new Highlighter();

        #endregion

        #region public methods
        static public ReadOnlyCollection<string> GetNumericTips() => AdvNumberFilter.GetTips();

        public static ReadOnlyCollection<string> GetStringTips() => AdvStringFilter.GetTips();

        public ISimpleFilter GetFilter() => this.filter;

        public Highlighter GetHighlighter()
        {
            return this.filter?.GetHighlighter() ?? emptyHighlighter;
        }

        public bool TryGetIndex(out int index)
        {
            index = parsedIndex;
            return isIndexFilter;
        }

        #endregion

        #region private methods

        #endregion
    }
}
