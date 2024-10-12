using System.Collections.Generic;

namespace VgcApis.Libs.Infr.KwFilterComps
{
    internal class SimpleIndexFilter : ISimpleFilter
    {
        public SimpleIndexFilter(int index)
        {
            this.index = index;
            this.highlighter = new Highlighter(index);
        }

        #region properties

        readonly int index;
        readonly Highlighter highlighter;
        #endregion

        #region public methods

        public int GetIndex() => this.index;

        public IReadOnlyCollection<Interfaces.ICoreServCtrl> Filter(
            IReadOnlyCollection<Interfaces.ICoreServCtrl> coreServs
        )
        {
            return coreServs;
        }

        public Highlighter GetHighlighter() => highlighter;
        #endregion

        #region creator
        public static SimpleIndexFilter CreateFilter(string kw)
        {
            // special case
            if (kw == "#")
            {
                return new SimpleIndexFilter(0);
            }

            if (string.IsNullOrEmpty(kw) || !kw.StartsWith("#") || kw.Length < 2)
            {
                return null;
            }

            if (!int.TryParse(kw.Substring(1), out int idx))
            {
                return null;
            }
            return new SimpleIndexFilter(idx);
        }

        #endregion

        #region private methods
        #endregion
    }
}
