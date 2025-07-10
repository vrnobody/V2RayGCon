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

            this.filter = f;
        }

        #region tables
        static List<Func<string, ISimpleFilter>> creators = new List<Func<string, ISimpleFilter>>()
        {
            (kw) => AdvGotoIndexFilter.CreateFilter(kw),
            (kw) => BoolExprFilter.CreateFilter(kw),
            (kw) => SimpleIndexFilter.CreateFilter(kw),
            (kw) => SimpleTitleFilter.CreateFilter(kw),
        };

        #endregion

        #region properties

        readonly ISimpleFilter filter;
        readonly Highlighter emptyHighlighter = new Highlighter();

        #endregion

        #region public methods

        public static List<ReadOnlyCollection<string>> GetTips() =>
            new List<ReadOnlyCollection<string>>()
            {
                AdvNumberFilter.GetTips(),
                AdvStringFilter.GetTips(),
                AdvOrderByFilter.GetTips(),
                AdvTakeFilter.GetTips(),
                new List<string>() { "#goto" }.AsReadOnly(),
            };

        public ISimpleFilter GetFilter() => this.filter;

        public Highlighter GetHighlighter()
        {
            return this.filter?.GetHighlighter() ?? emptyHighlighter;
        }

        public bool TryGetIndex(out int index)
        {
            index = 0;
            var hasIndex = this.filter is IIndexFilter;
            if (hasIndex)
            {
                index = (this.filter as IIndexFilter).GetIndex();
            }
            return hasIndex;
        }

        #endregion

        #region private methods

        #endregion
    }
}
