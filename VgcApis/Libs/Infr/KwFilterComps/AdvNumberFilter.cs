using System;
using System.Collections.Generic;
using VgcApis.Interfaces.CoreCtrlComponents;

namespace VgcApis.Libs.Infr.KwFilterComps
{
    internal class AdvNumberFilter : IAdvanceFilter<NumberTagNames, NumberOperators, long>
    {
        public AdvNumberFilter(
            HashSet<NumberTagNames> contentNames,
            NumberOperators op,
            long firstNumber,
            long secondNumber
        )
        {
            this.contentNames = contentNames;
            this.op = op;
            this.first = firstNumber;
            this.second = secondNumber;

            this.matcher = CreateMatcher(this.op, this.first, this.second);
        }

        #region properties

        readonly long first = 0;
        readonly long second = 0;

        readonly HashSet<NumberTagNames> contentNames;
        readonly NumberOperators op;
        readonly Func<long, bool> matcher;

        readonly Highlighter highlighter = new Highlighter();

        #endregion

        #region lookup tables

        static readonly Dictionary<string, NumberTagNames> contentNameLookupTable =
            Helpers.CreateEnumLookupTable<NumberTagNames>();

        static readonly Dictionary<string, NumberOperators> operatorLookupTable = new Dictionary<
            string,
            NumberOperators
        >()
        {
            { ">", NumberOperators.LargerThen },
            { "<", NumberOperators.SmallerThen },
            { "=", NumberOperators.Is },
            { "!", NumberOperators.Not },
            { "~", NumberOperators.Between },
        };

        #endregion

        #region public methods
        public List<Interfaces.ICoreServCtrl> Filter(List<Interfaces.ICoreServCtrl> coreServs)
        {
            var r = new List<Interfaces.ICoreServCtrl>();
            foreach (var coreServ in coreServs)
            {
                var cs = coreServ.GetCoreStates();
                foreach (var cname in contentNames)
                {
                    if (TryGetContent(cs, cname, out var number) && MatchCore(number))
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

        public NumberOperators GetOperator() => op;

        public HashSet<NumberTagNames> GetTagNames() => new HashSet<NumberTagNames>(contentNames);

        public bool MatchCore(long num)
        {
            return matcher.Invoke(num);
        }

        #endregion

        #region private methods
        Func<long, bool> CreateMatcher(NumberOperators numMatchingType, long first, long second)
        {
            switch (numMatchingType)
            {
                case NumberOperators.SmallerThen:
                    return (n) => n <= first;
                case NumberOperators.LargerThen:
                    return (n) => n >= first;
                case NumberOperators.Is:
                    return (n) => n == first;
                case NumberOperators.Not:
                    return (n) => n != first;
                case NumberOperators.Between:
                    return (n) => n >= first && n <= second;
                default:
                    break;
            }
            return null;
        }

        bool TryGetContent(ICoreStates cs, NumberTagNames cname, out long r)
        {
            switch (cname)
            {
                case NumberTagNames.Latency:
                    r = cs.GetSpeedTestResult();
                    return true;
                case NumberTagNames.Upload:
                    // MiB
                    r = cs.GetUplinkTotalInBytes() / Helpers.MiB;
                    return true;
                case NumberTagNames.Download:
                    // MiB
                    r = cs.GetDownlinkTotalInBytes() / Helpers.MiB;
                    return true;
                case NumberTagNames.Port:
                    r = cs.GetInboundPort();
                    return true;
            }
            r = 0;
            return false;
        }

        #endregion

        #region creator

        static bool TryParseContenName(string name, out HashSet<NumberTagNames> r)
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

        // remove later
        internal static AdvNumberFilter CreateFilter(string kw)
        {
            if (string.IsNullOrEmpty(kw) || !kw.StartsWith("#") || "#".Equals(kw))
            {
                return null;
            }
            var kws = kw.ToLower()
                .Substring(1)
                ?.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (kws.Length < 3)
            {
                // #latency < 100
                return null;
            }

            if (!TryParseContenName(kws[0], out var cnames))
            {
                return null;
            }

            if (!operatorLookupTable.TryGetValue(kws[1], out var op))
            {
                return null;
            }

            if (!TryParseParams(kws, op, out var firstNumber, out var secondNumber))
            {
                return null;
            }

            var parser = new AdvNumberFilter(cnames, op, firstNumber, secondNumber);
            return parser;
        }

        private static bool TryParseParams(
            string[] kws,
            NumberOperators op,
            out long firstNumber,
            out long secondNumber
        )
        {
            secondNumber = 0;
            if (!long.TryParse(kws[2], out firstNumber))
            {
                return false;
            }

            if (op == NumberOperators.Between)
            {
                // #latency ~ 100 200
                if (kws.Length < 4)
                {
                    return false;
                }

                var ok = long.TryParse(kws[3], out secondNumber);
                if (!ok)
                {
                    return false;
                }

                if (firstNumber > secondNumber)
                {
                    var tmp = firstNumber;
                    firstNumber = secondNumber;
                    secondNumber = tmp;
                }
            }
            return true;
        }

        #endregion
    }
}
