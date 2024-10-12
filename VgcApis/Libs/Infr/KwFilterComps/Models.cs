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

        // priority not works well with "orderby" and "take" operators
        // protected int pri;

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
            this.highlighter = this.left?.GetHighlighter();
            if (this.highlighter == null)
            {
                this.highlighter = this.right?.GetHighlighter();
            }
        }

        #region public methods


        public Highlighter GetHighlighter() => this.highlighter;

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
                return $"{type}({s})";
            }
            var sl = left?.ToString() ?? "Null";
            var sr = right?.ToString() ?? "Null";
            return $"{type}({sl}, {sr})";
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
            var l = left?.Filter(servs) ?? new List<ICoreServCtrl>();
            if (l.Count < 1)
            {
                return l;
            }
            var r = right?.Filter(l) ?? new List<ICoreServCtrl>();
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

            if (this.filter != null)
            {
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
