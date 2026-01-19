using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using VgcApis.Interfaces.CoreCtrlComponents;

namespace VgcApis.Libs.Infr.KwFilterComps
{
    internal class AdvNumberFilter : IAdvanceFilter<NumberTagNames, NumberOperators, long>
    {
        public AdvNumberFilter(
            HashSet<NumberTagNames> contentNames,
            bool not,
            NumberOperators op,
            long firstNumber,
            long secondNumber
        )
        {
            this.contentNames = contentNames;
            this.not = not;
            this.op = op;
            this.first = firstNumber;
            this.second = secondNumber;

            if (this.contentNames.Contains(NumberTagNames.Modify))
            {
                var now = DateTime.Now.ToLocalTime();
                this.first = Misc.Utils.ToShortDateInt(now, this.first);
                this.second = Misc.Utils.ToShortDateInt(now, this.second);
            }

            if (this.first > this.second && this.op == NumberOperators.Between)
            {
                var tmp = this.first;
                this.first = this.second;
                this.second = tmp;
            }

            this.matcher = CreateMatcher(this.op, this.first, this.second);
        }

        #region properties
        readonly bool not = false;
        readonly long first = 0;
        readonly long second = 0;

        readonly HashSet<NumberTagNames> contentNames;
        readonly NumberOperators op;
        readonly Func<long, bool> matcher;

        readonly Highlighter highlighter = null;

        #endregion

        #region lookup tables

        static readonly Dictionary<string, NumberTagNames> contentNameLookupTable =
            Helpers.CreateEnumLookupTable<NumberTagNames>();

        internal static readonly Dictionary<string, NumberOperators> operatorLookupTable =
            new Dictionary<string, NumberOperators>()
            {
                { ">", NumberOperators.LargerThen },
                { "<", NumberOperators.SmallerThen },
                { "=", NumberOperators.Is },
                { "~", NumberOperators.Between },
            };

        static List<string> tips = CreateTipsCache(operatorLookupTable.Keys);

        #endregion

        #region public methods

        public override string ToString()
        {
            var n = this.not ? $" {Helpers.NOT}" : "";
            var o = this.op.ToString().ToLower();
            var ts = string.Join(", ", this.contentNames).ToString().ToLower();
            var r = $"#({ts}){n} {o} {this.first} {this.second}";
            return r;
        }

        internal static ReadOnlyCollection<string> GetTips() => tips.AsReadOnly();

        internal static bool TryGetContent(
            Interfaces.ICoreServCtrl coreServ,
            ICoreStates cs,
            NumberTagNames cname,
            out long r
        )
        {
            switch (cname)
            {
                case NumberTagNames.Index:
                    r = (long)cs.GetIndex();
                    return true;
                case NumberTagNames.Latency:
                    r = cs.GetSpeedTestResult();
                    return true;
                case NumberTagNames.Upload:
                    r = cs.GetUplinkTotalInBytes() / Helpers.MiB;
                    return true;
                case NumberTagNames.Download:
                    r = cs.GetDownlinkTotalInBytes() / Helpers.MiB;
                    return true;
                case NumberTagNames.Port:
                    r = cs.GetInboundPort();
                    return true;
                case NumberTagNames.Modify:
                    var utick = cs.GetLastModifiedUtcTicks();
                    var d = new DateTime(utick, DateTimeKind.Utc).ToLocalTime();
                    var str = d.ToString("yyyyMMdd");
                    r = (long)Misc.Utils.Str2Int(str);
                    return true;
            }
            r = 0;
            return false;
        }

        public IReadOnlyCollection<Interfaces.ICoreServCtrl> Filter(
            IReadOnlyCollection<Interfaces.ICoreServCtrl> coreServs
        )
        {
            var r = new List<Interfaces.ICoreServCtrl>();
            foreach (var coreServ in coreServs)
            {
                var cs = coreServ.GetCoreStates();
                foreach (var cname in contentNames)
                {
                    if (TryGetContent(coreServ, cs, cname, out var number) && MatchCore(number))
                    {
                        r.Add(coreServ);
                        break;
                    }
                }
            }
            return r;
        }

        public Highlighter GetHighlighter() => highlighter;

        #endregion

        #region for unit tests

        public bool MatchCore(long num)
        {
            return matcher.Invoke(num);
        }

        #endregion

        #region private methods
        static List<string> CreateTipsCache(IEnumerable<string> keys)
        {
            var r = new List<string>() { };
            foreach (var tag in Enum.GetNames(typeof(NumberTagNames)))
            {
                r.Add($"#{tag.ToLower()}");
                r.Add($"#{tag.ToLower()} {Helpers.NOT}");
                foreach (var op in keys)
                {
                    r.Add($"#{tag.ToLower()} {op}");
                    r.Add($"#{tag.ToLower()} {Helpers.NOT} {op}");
                }
            }
            return r;
        }

        Func<long, bool> CreateMatcher(NumberOperators numMatchingType, long first, long second)
        {
            switch (numMatchingType)
            {
                case NumberOperators.SmallerThen:
                    return (n) => not ? n >= first : n < first;
                case NumberOperators.LargerThen:
                    return (n) => not ? n <= first : n > first;
                case NumberOperators.Is:
                    return (n) => not ? n != first : n == first;
                case NumberOperators.Between:
                    return (n) => not ? (n < first || n > second) : (n >= first && n <= second);
                default:
                    break;
            }
            return null;
        }

        #endregion

        #region creator

        internal static bool TryParseContenName(string name, out HashSet<NumberTagNames> r)
        {
            r = new HashSet<NumberTagNames>();

            // try full match first
            if (contentNameLookupTable.TryGetValue(name, out var ctype))
            {
                r.Add(ctype);
                return true;
            }

            var ok = false;
            foreach (var kv in contentNameLookupTable)
            {
                if (Misc.Utils.PartialMatch(kv.Key, name))
                {
                    r.Add(kv.Value);
                    ok = true;
                }
            }
            return ok;
        }

        internal static AdvNumberFilter CreateFilter(string kw)
        {
            if (string.IsNullOrEmpty(kw) || !kw.StartsWith("#") || "#".Equals(kw))
            {
                return null;
            }
            var kws = Helpers.ParseLiteral(kw);
            return CreateFilter(kws);
        }

        // remove later
        internal static AdvNumberFilter CreateFilter(string[] kws)
        {
            if (kws.Length < 2)
            {
                // #latency < 100
                return null;
            }

            var tag = kws[0];
            if (string.IsNullOrEmpty(tag) || !tag.StartsWith("#") || tag.Length < 2)
            {
                return null;
            }

            if (!TryParseContenName(tag.Substring(1).ToLower(), out var cnames))
            {
                return null;
            }

            var idx = 1;
            var not = false;
            if (kws[1].ToLower() == Helpers.NOT)
            {
                idx++;
                not = true;
                if (kws.Length < idx + 1)
                {
                    return null;
                }
            }

            var op = NumberOperators.Is;
            if (operatorLookupTable.TryGetValue(kws[idx].ToLower(), out var tmpOp))
            {
                op = tmpOp;
                idx++;
                if (kws.Length < idx + 1)
                {
                    return null;
                }
            }

            if (!TryParseParams(kws, idx, op, out var firstNumber, out var secondNumber))
            {
                return null;
            }

            var parser = new AdvNumberFilter(cnames, not, op, firstNumber, secondNumber);
            return parser;
        }

        private static bool TryParseParams(
            string[] kws,
            int idx,
            NumberOperators op,
            out long firstNumber,
            out long secondNumber
        )
        {
            // not idx = 2
            // op idx = 3
            // 0    1   2   3     4
            // #tag not op first second
            secondNumber = 0;
            if (!long.TryParse(kws[idx], out firstNumber))
            {
                return false;
            }

            if (op == NumberOperators.Between)
            {
                // #latency ~ 100 200
                if (kws.Length < idx + 2)
                {
                    return false;
                }

                var ok = long.TryParse(kws[idx + 1], out secondNumber);
                if (!ok)
                {
                    return false;
                }
            }
            return true;
        }

        #endregion
    }
}
