using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using VgcApis.Interfaces;
using VgcApis.Libs.Infr.KwFilterComps.BoolExprComps;

namespace VgcApis.Libs.Infr.KwFilterComps
{
    // credit: https://stackoverflow.com/questions/17568067/how-to-parse-a-boolean-expression-and-load-it-into-a-class
    internal class BoolExprFilter : ISimpleFilter
    {
        readonly BoolExpr expr;

        public BoolExprFilter(BoolExpr expr)
        {
            this.expr = expr;
        }

        #region properties methods
        Highlighter highlighter = new Highlighter();
        #endregion

        #region private methods
        #endregion

        #region public methods
        public IReadOnlyCollection<ICoreServCtrl> Filter(
            IReadOnlyCollection<ICoreServCtrl> coreServs
        )
        {
            IReadOnlyCollection<ICoreServCtrl> r = null;
            try
            {
                r = expr?.Filter(coreServs);
            }
            catch
            {
                Sys.FileLogger.Error("Call expression filter failed.");
                Sys.FileLogger.Info($"Expression: {this}");
            }
            return r ?? new List<ICoreServCtrl>();
        }

        public Highlighter GetHighlighter() => highlighter;

        #endregion

        #region unit tests


        public override string ToString()
        {
            try
            {
                return expr?.ToString() ?? "";
            }
            catch
            {
                Sys.FileLogger.Error("Get expression failed.");
            }
            return "";
        }

        #endregion

        #region creator
        internal static ReadOnlyCollection<string> GetTips() =>
            new List<string>() { "#@" }.AsReadOnly();

        internal static BoolExprFilter CreateFilter(string keywords)
        {
            try
            {
                return CreateFilterCore(keywords);
            }
            catch { }
            return null;
        }

        static BoolExprFilter CreateFilterCore(string keywords)
        {
            if (string.IsNullOrEmpty(keywords) || !keywords.StartsWith("#@"))
            {
                // #@ (#mk like "a")
                return null;
            }

            keywords = keywords.Substring(2).ToLower();
            var tokens = Helpers.ParseExprToken(keywords);
            if (tokens.Count < 2)
            {
                return null;
            }
            var pe = Helpers.TransformToPolishNotation(tokens.ToList());
            var em = pe.GetEnumerator();
            em.MoveNext();
            var expr = Helpers.MakeExpr(ref em);
            if (expr == null)
            {
                return null;
            }

            var f = new BoolExprFilter(expr);
            return f;
        }

        #endregion
    }
}
