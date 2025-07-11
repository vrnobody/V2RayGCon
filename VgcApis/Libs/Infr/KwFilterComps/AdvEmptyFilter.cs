using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace VgcApis.Libs.Infr.KwFilterComps
{
    internal class AdvEmptyFilter : ISimpleFilter
    {
        static readonly string FILTER_NAME = @"note";

        public AdvEmptyFilter() { }

        #region properties

        readonly Highlighter highlighter = null;

        #endregion

        #region lookup tables

        readonly static List<string> tips = CreateTipsCache();

        #endregion

        #region public methods
        public override string ToString()
        {
            var r = $"#{FILTER_NAME} ...";
            return r;
        }

        internal static ReadOnlyCollection<string> GetTips() => tips.AsReadOnly();

        public IReadOnlyCollection<Interfaces.ICoreServCtrl> Filter(
            IReadOnlyCollection<Interfaces.ICoreServCtrl> coreServs
        )
        {
            return new List<Interfaces.ICoreServCtrl>();
        }

        public Highlighter GetHighlighter() => highlighter;

        #endregion

        #region private methods
        static List<string> CreateTipsCache()
        {
            var r = new List<string>() { $"#{FILTER_NAME}" };
            return r;
        }

        #endregion

        #region creator

        internal static AdvEmptyFilter CreateFilter(string kw)
        {
            if (string.IsNullOrEmpty(kw) || !kw.StartsWith($"#{FILTER_NAME}"))
            {
                return null;
            }
            var kws = Helpers.ParseLiteral(kw);
            return CreateFilter(kws);
        }

        // remove later
        internal static AdvEmptyFilter CreateFilter(string[] kws)
        {
            if (kws[0] != $"#{FILTER_NAME}")
            {
                return null;
            }
            return new AdvEmptyFilter();
        }

        #endregion
    }
}
