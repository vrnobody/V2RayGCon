using System.Collections.Generic;
using System.Linq;

namespace VgcApis.Libs.Infr.KwFilterComps
{
    internal class AdvGotoIndexFilter : IIndexFilter
    {
        public AdvGotoIndexFilter(BoolExprFilter boolExprFilter)
        {
            this.boolExprFilter = boolExprFilter;
            SetIndex(1);
        }

        #region properties
        readonly BoolExprFilter boolExprFilter;
        int index;
        Highlighter highlighter;
        #endregion

        #region public methods

        public int GetIndex() => this.index;

        public IReadOnlyCollection<Interfaces.ICoreServCtrl> Filter(
            IReadOnlyCollection<Interfaces.ICoreServCtrl> coreServs
        )
        {
            var filtered = this.boolExprFilter.Filter(coreServs);
            var index = filtered.FirstOrDefault()?.GetCoreStates().GetIndex() ?? 1;
            SetIndex((int)index);
            return coreServs;
        }

        public Highlighter GetHighlighter() => highlighter;
        #endregion

        #region creator
        public static AdvGotoIndexFilter CreateFilter(string kw)
        {
            var prefix = "#goto ";
            if (string.IsNullOrEmpty(kw) || !kw.ToLower().StartsWith(prefix))
            {
                return null;
            }

            var expr = kw.Substring(prefix.Length);
            var f = BoolExprFilter.CreateFilter(expr);
            if (f == null)
            {
                return null;
            }
            return new AdvGotoIndexFilter(f);
        }

        #endregion

        #region private methods
        void SetIndex(int index)
        {
            this.index = index;
            this.highlighter = new Highlighter(index);
        }
        #endregion
    }
}
