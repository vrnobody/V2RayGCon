using System;
using System.Collections.Generic;
using System.Linq;
using VgcApis.Interfaces;

namespace VgcApis.Libs.Infr.KwFilterComps.BoolExprComps
{
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

    internal class BoolExpr : IFilter
    {
        protected readonly string[] keywords;
        protected readonly BoolExpr left;
        protected readonly BoolExpr right;

        public BoolExpr(BoolExpr left, BoolExpr right, string[] keywords)
        {
            this.left = left;
            this.right = right;
            this.keywords = keywords;
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
                return $"{type}({s})";
            }
            var sl = left?.ToString() ?? "Null";
            var sr = right?.ToString() ?? "Null";
            return $"{type}({sl}, {sr})";
        }
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
            var r = right?.Filter(l) ?? new List<ICoreServCtrl>();
            if (r.Count < 1 || l.Count < 1)
            {
                return l;
            }
            var uids = new HashSet<string>(r.Select(s => s.GetCoreStates().GetUid()));
            var n = l.Where(s => !uids.Contains(s.GetCoreStates().GetUid())).ToList();
            return n;
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
            var uids = new HashSet<string>(l.Select(s => s.GetCoreStates().GetUid()));
            var patch = r.Where(s => !uids.Contains(s.GetCoreStates().GetUid()));
            var or = l.Concat(patch).OrderBy(s => s).ToList();
            return or;
        }
    }

    internal class LeafExpr : BoolExpr
    {
        public LeafExpr(List<ExprToken> src)
            : base(null, null, src.Select(tk => tk.value).ToArray()) { }

        public override IReadOnlyCollection<ICoreServCtrl> Filter(
            IReadOnlyCollection<ICoreServCtrl> servs
        )
        {
            var sf = AdvStringFilter.CreateFilter(this.keywords);
            if (sf != null)
            {
                return sf.Filter(servs);
            }
            var nf = AdvNumberFilter.CreateFilter(this.keywords);
            if (nf != null)
            {
                return nf.Filter(servs);
            }
            return new List<ICoreServCtrl>();
        }
    }
}
