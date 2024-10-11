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

        protected static readonly List<Func<string[], ISimpleFilter>> creators = new List<
            Func<string[], ISimpleFilter>
        >()
        {
            kws => AdvTakeFilter.CreateFilter(kws),
            kws => AdvOrderByFilter.CreateFilter(kws),
            kws => AdvStringFilter.CreateFilter(kws),
            kws => AdvNumberFilter.CreateFilter(kws),
        };

        public BoolExpr(BoolExpr left, BoolExpr right, string[] keywords)
        {
            this.left = left;
            this.right = right;
            this.keywords = keywords;
            this.pri = InnerGetPri(left) + InnerGetPri(right);
            this.highlighter = this.left?.GetHighlighter();
            if (this.highlighter == null)
            {
                this.highlighter = this.right?.GetHighlighter();
            }
        }

        #region protected methods
        protected int InnerGetPri(BoolExpr expr)
        {
            if (expr == null)
            {
                return KeywordFilter.GetMaxPri();
            }
            return expr.GetPri();
        }
        #endregion

        #region public methods
        public int GetPri() => this.pri;

        public Highlighter GetHighlighter() => this.highlighter;

        public BoolExpr GetHightPriExpr()
        {
            var pl = InnerGetPri(left);
            var pr = InnerGetPri(right);
            // pri = 0 means do not change evaluating order
            if (pl == 0 || pr == 0)
            {
                return left;
            }
            return pl > pr ? left : right;
        }

        public BoolExpr GetLowPriExpr()
        {
            var pl = InnerGetPri(left);
            var pr = InnerGetPri(right);
            if (pl == 0 || pr == 0)
            {
                return right;
            }
            return pl > pr ? right : left;
        }

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
            var or = l.Concat(patch).ToList();
            return or;
        }
    }

    internal class LeafExpr : BoolExpr
    {
        readonly ISimpleFilter filter;

        public LeafExpr(List<ExprToken> src)
            : base(null, null, src.Select(tk => tk.value).ToArray())
        {
            for (int i = 0; i < creators.Count; i++)
            {
                this.filter = creators[i].Invoke(this.keywords);
                if (this.filter != null)
                {
                    break;
                }
            }

            if (this.filter == null)
            {
                this.pri = VgcApis.Libs.Infr.KeywordFilter.GetMaxPri();
            }
            else
            {
                this.pri = filter.GetPri();
                this.highlighter = this.filter.GetHighlighter();
            }
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
