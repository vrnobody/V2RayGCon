using System.Collections.Generic;

namespace VgcApis.Libs.Infr.KwFilterComps
{
    internal class SimpleTitleFilter : ISimpleFilter
    {
        public SimpleTitleFilter(string keyword)
        {
            var kw = keyword.ToLower().Replace(" ", "");
            this.isEmpty = string.IsNullOrEmpty(kw);
            this.parsedKeyword = kw;
            this.highlighter = new Highlighter(kw);
        }

        #region properties
        readonly bool isEmpty;
        readonly string parsedKeyword;
        readonly Highlighter highlighter;
        #endregion

        #region public methods

        public List<Interfaces.ICoreServCtrl> Filter(List<Interfaces.ICoreServCtrl> coreServs)
        {
            if (isEmpty)
            {
                return coreServs;
            }

            var r = new List<Interfaces.ICoreServCtrl>();
            foreach (var coreServ in coreServs)
            {
                // todo add cache
                var cst = coreServ.GetCoreStates();
                if (MatchCore(cst.GetTitle()))
                {
                    r.Add(coreServ);
                }
            }
            return r;
        }

        public Highlighter GetHighlighter() => highlighter;
        #endregion

        #region creator
        public static SimpleTitleFilter CreateFilter(string kw)
        {
            kw = kw ?? "";
            if (kw.StartsWith("##"))
            {
                kw = kw.Substring(1);
            }
            else if (kw.StartsWith("#"))
            {
                return null;
            }
            return new SimpleTitleFilter(kw);
        }

        #endregion

        #region for unit tests
        internal string GetParsedKeyword() => this.parsedKeyword;

        internal bool MatchCore(string content)
        {
            content = content.ToLower();
            var r = Misc.Utils.PartialMatch(content, parsedKeyword);
            return r;
        }
        #endregion
    }
}
