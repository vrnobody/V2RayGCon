using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace VgcApis.Libs.Infr.KwFilterComps
{
    internal class AdvTakeFilter : ISimpleFilter
    {
        static readonly string FILTER_NAME = @"take";

        public AdvTakeFilter(int take, int skip)
        {
            this.skip = skip;
            this.take = take;
        }

        #region properties

        readonly Highlighter highlighter = null;

        #endregion

        #region lookup tables

        readonly static List<string> tips = CreateTipsCache();
        private readonly int skip;
        private readonly int take;

        #endregion

        #region public methods
        public override string ToString()
        {
            var r = $"#{FILTER_NAME} {take} {skip}";
            return r;
        }

        internal static ReadOnlyCollection<string> GetTips() => tips.AsReadOnly();

        public IReadOnlyCollection<Interfaces.ICoreServCtrl> Filter(
            IReadOnlyCollection<Interfaces.ICoreServCtrl> coreServs
        )
        {
            if (take == 0)
            {
                return new List<Interfaces.ICoreServCtrl>();
            }

            if (take < 0)
            {
                var t = coreServs.Reverse();
                t = skip > 0 ? t.Skip(skip) : t;
                return t.Take(-1 * take).Reverse().ToList();
            }

            // clone
            var r = skip > 0 ? coreServs.Skip(skip) : coreServs;
            return r.Take(take).ToList();
        }

        public Highlighter GetHighlighter() => highlighter;

        #endregion

        #region private methods
        static List<string> CreateTipsCache()
        {
            var r = new List<string>() { $"#{FILTER_NAME}", $"#{FILTER_NAME} 10" };
            return r;
        }

        #endregion

        #region creator

        internal static AdvTakeFilter CreateFilter(string kw)
        {
            if (string.IsNullOrEmpty(kw) || !kw.StartsWith("#") || "#".Equals(kw))
            {
                return null;
            }
            var kws = Helpers.ParseLiteral(kw);
            return CreateFilter(kws);
        }

        // remove later
        internal static AdvTakeFilter CreateFilter(string[] kws)
        {
            if (kws.Length < 2)
            {
                // #latency < 100
                return null;
            }

            var name = kws[0];
            if (
                string.IsNullOrEmpty(name)
                || !name.StartsWith("#")
                || name.Length < 2
                || !Misc.Utils.PartialMatchCi($"#{FILTER_NAME}", name)
            )
            {
                return null;
            }

            if (!int.TryParse(kws[1], out var take))
            {
                return null;
            }

            var skip = 0;
            if (kws.Length > 2 && int.TryParse(kws[2], out var sk) && sk > 0)
            {
                skip = sk;
            }

            return new AdvTakeFilter(take, skip);
        }

        #endregion
    }
}
