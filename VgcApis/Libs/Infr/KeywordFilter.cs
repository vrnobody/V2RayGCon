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
            keyword = keyword?.ToLower() ?? "";
            ISimpleFilter f = null;
            foreach (var creator in creators)
            {
                f = creator.Invoke(keyword);
                if (f != null)
                {
                    break;
                }
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

        #region tables
        static List<Func<string, ISimpleFilter>> creators = new List<Func<string, ISimpleFilter>>()
        {
            (kw) => SimpleIndexFilter.CreateFilter(kw),
            (kw) => SimpleTitleFilter.CreateFilter(kw),
            (kw) => BoolExprFilter.CreateFilter(kw),
            (kw) => AdvNumberFilter.CreateFilter(kw),
            (kw) => AdvStringFilter.CreateFilter(kw),
        };

        #endregion

        #region properties

        readonly bool isIndexFilter = false;
        readonly int parsedIndex = 0;
        readonly ISimpleFilter filter;
        readonly Highlighter emptyHighlighter = new Highlighter();

        #endregion

        #region public methods
        static public ReadOnlyCollection<string> GetBoolExprTips() => BoolExprFilter.GetTips();

        public static ReadOnlyCollection<string> GetNumericTips() => AdvNumberFilter.GetTips();

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
