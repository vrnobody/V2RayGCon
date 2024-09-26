using System;
using System.Collections.Generic;
using System.Linq;
using VgcApis.Interfaces;

namespace VgcApis.Libs.Infr.KwFilterComps
{
    internal class TagName
    {
        public readonly bool not;
        public readonly object tag;

        public TagName(object tag, bool not)
        {
            this.tag = tag;
            this.not = not;
        }

        public override string ToString()
        {
            var n = not ? "-" : "";
            switch (tag)
            {
                case StringTagNames stag:
                    return $"{n}{stag.ToString().ToLower()}";
                case NumberTagNames ntag:
                    return $"{n}{ntag.ToString().ToLower()}";
                default:
                    throw new InvalidCastException($"unknow tag name: {tag}");
            }
        }
    }

    internal class ExprToken
    {
        public readonly ExprTokenTypes type;
        public readonly string value;

        public ExprToken(ExprTokenTypes type, string value)
        {
            this.type = type;
            this.value = value;
        }
    }

    internal class BoolExpr : ISimpleFilter
    {
        protected readonly string[] keywords;
        protected readonly BoolExpr left;
        protected readonly BoolExpr right;
        protected Highlighter highlighter;
        protected int pri;

        public BoolExpr(BoolExpr left, BoolExpr right, string[] keywords)
        {
            this.left = left;
            this.right = right;
            this.keywords = keywords;
            this.pri = GetPri(left) + GetPri(right);
            this.highlighter = this.left?.GetHighlighter();
            if (this.highlighter == null)
            {
                this.highlighter = this.right?.GetHighlighter();
            }
        }

        #region public methods
        public Highlighter GetHighlighter() => this.highlighter;

        public BoolExpr GetHightPriExpr() => GetPri(left) < GetPri(right) ? right : left;

        public BoolExpr GetLowPriExpr() => GetPri(left) < GetPri(right) ? left : right;

        public virtual IReadOnlyCollection<ICoreServCtrl> Filter(
            IReadOnlyCollection<ICoreServCtrl> servs
        )
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            var type = this.GetType().Name;
            if (this.keywords != null)
            {
                var s = string.Join(" ", keywords);
                return $"{type}@{pri}({s})";
            }
            var sl = left?.ToString() ?? "Null";
            var sr = right?.ToString() ?? "Null";
            return $"{type}@{pri}({sl}, {sr})";
        }
        #endregion

        #region private methods
        int GetPri(BoolExpr expr)
        {
            if (expr == null)
            {
                return 2;
            }
            return expr.pri;
        }
        #endregion
    }

    internal class NotExpr : BoolExpr
    {
        public NotExpr(BoolExpr left, BoolExpr right)
            : base(left, right, null) { }

        public override IReadOnlyCollection<ICoreServCtrl> Filter(
            IReadOnlyCollection<ICoreServCtrl> servs
        )
        {
            var l = left?.Filter(servs) ?? new List<ICoreServCtrl>();
            if (l.Count < 1)
            {
                return l;
            }

            var r = right?.Filter(l) ?? new List<ICoreServCtrl>();
            if (r.Count < 1)
            {
                return l;
            }

            var uids = new HashSet<string>(r.Select(s => s.GetCoreStates().GetUid()));
            var nt = l.Where(s => !uids.Contains(s.GetCoreStates().GetUid())).ToList();
            return nt;
        }
    }

    internal class AndExpr : BoolExpr
    {
        public AndExpr(BoolExpr left, BoolExpr right)
            : base(left, right, null) { }

        public override IReadOnlyCollection<ICoreServCtrl> Filter(
            IReadOnlyCollection<ICoreServCtrl> servs
        )
        {
            var l = GetHightPriExpr()?.Filter(servs) ?? new List<ICoreServCtrl>();
            if (l.Count < 1)
            {
                return l;
            }
            var r = GetLowPriExpr()?.Filter(l) ?? new List<ICoreServCtrl>();
            return r;
        }
    }

    internal class OrExpr : BoolExpr
    {
        public OrExpr(BoolExpr left, BoolExpr right)
            : base(left, right, null) { }

        public override IReadOnlyCollection<ICoreServCtrl> Filter(
            IReadOnlyCollection<ICoreServCtrl> servs
        )
        {
            var l = left?.Filter(servs) ?? new List<ICoreServCtrl>();
            var r = right?.Filter(servs) ?? new List<ICoreServCtrl>();

            if (l.Count < 1)
            {
                return r;
            }
            else if (r.Count < 1)
            {
                return l;
            }

            var uids = new HashSet<string>(l.Select(s => s.GetCoreStates().GetUid()));
            var patch = r.Where(s => !uids.Contains(s.GetCoreStates().GetUid()));
            var or = l.Concat(patch).OrderBy(s => s).ToList();
            return or;
        }
    }

    internal class LeafExpr : BoolExpr
    {
        readonly ISimpleFilter filter;

        public LeafExpr(List<ExprToken> src)
            : base(null, null, src.Select(tk => tk.value).ToArray())
        {
            this.pri = 0;
            this.filter = AdvOrderByFilter.CreateFilter(this.keywords);
            if (this.filter == null)
            {
                this.pri = 1;
                this.filter = AdvStringFilter.CreateFilter(this.keywords);
            }
            if (this.filter == null)
            {
                this.filter = AdvNumberFilter.CreateFilter(this.keywords);
                this.pri = 2;
            }
            if (this.filter == null)
            {
                this.pri = 4;
            }
            this.highlighter = this.filter?.GetHighlighter();
        }

        public override IReadOnlyCollection<ICoreServCtrl> Filter(
            IReadOnlyCollection<ICoreServCtrl> servs
        )
        {
            // debug: Console.WriteLine($"{ToString()}");
            var r = this.filter?.Filter(servs) ?? new List<ICoreServCtrl>();
            return r;
        }
    }
}
