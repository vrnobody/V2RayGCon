using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using VgcApis.Libs.Infr.KwFilterComps;
using VgcApis.UserControls.AcmComboBoxComps;

namespace VgcApis.Libs.Infr
{
    public class KeywordFilter
    {
        public KeywordFilter(string keyword)
        {
            ISimpleFilter f = null;

            foreach (var creator in filterCreators)
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

                // fallback
                // f = new SimpleIndexFilter(0);
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

        #region lookup tables
        static readonly List<AutocompleteItem> tips = CreateTipsCache();

        static readonly List<Func<string, ISimpleFilter>> filterCreators = new List<
            Func<string, ISimpleFilter>
        >()
        {
            (kw) => SimpleIndexFilter.CreateFilter(kw),
            (kw) => AdvStringFilter.CreateFilter(kw),
            (kw) => AdvNumberFilter.CreateFilter(kw),
            (kw) => SimpleTitleFilter.CreateFilter(kw),
        };
        #endregion

        #region public methods

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

        public static ReadOnlyCollection<AutocompleteItem> GetTips() => tips.AsReadOnly();

        #endregion

        #region private methods

        static List<AutocompleteItem> CreateTipsCache()
        {
            var r = AdvNumberFilter.GetTips().Concat(AdvStringFilter.GetTips()).ToList();
            return r;
        }

        #endregion
    }
}
