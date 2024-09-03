using System;
using System.Collections.Generic;
using System.Linq;
using VgcApis.Interfaces.CoreCtrlComponents;

namespace VgcApis.Libs.Infr.KwFilterComps
{
    public class AdvStringFilter : IAdvanceFilter<StringTagNames, StringOperators, string>
    {
        public AdvStringFilter(HashSet<StringTagNames> tagNames, StringOperators op, string keyword)
        {
            this.parsedKeyword = keyword.ToLower();
            this.tagNames = tagNames;
            this.op = op;

            this.matcher = CreateMatcher(this.op, this.parsedKeyword);
            this.highlighter = CreateHighlighter();
        }

        #region properties

        readonly string parsedKeyword = "";
        readonly HashSet<StringTagNames> tagNames = new HashSet<StringTagNames>() { };
        readonly StringOperators op = StringOperators.LIKE;
        readonly Func<string, bool> matcher = null;
        readonly Highlighter highlighter = null;

        #endregion

        #region lookup tables
        static readonly Dictionary<string, StringTagNames> tagNameLookupTable =
            Helpers.CreateEnumLookupTable<StringTagNames>();

        static readonly Dictionary<string, StringOperators> operatorLookupTable =
            Helpers.CreateEnumLookupTable<StringOperators>();

        static readonly HashSet<StringTagNames> titleTagNames = new HashSet<StringTagNames>()
        {
            StringTagNames.Title,
            StringTagNames.Summary,
            StringTagNames.Name,
        };

        #endregion

        #region public methods
        public List<Interfaces.ICoreServCtrl> Filter(List<Interfaces.ICoreServCtrl> coreServs)
        {
            var r = new List<Interfaces.ICoreServCtrl>();
            foreach (var coreServ in coreServs)
            {
                var cst = coreServ.GetCoreStates();
                foreach (var cname in tagNames)
                {
                    if (
                        TryGetStrTagValue(coreServ, cst, cname, out var content)
                        && CachedMatchCore(cname, content)
                    )
                    {
                        r.Add(coreServ);
                        break;
                    }
                }
            }
            return r;
        }

        public Highlighter GetHighlighter() => this.highlighter;

        #endregion

        #region for unit tests
        internal string GetParsedKeyword() => parsedKeyword;

        public StringOperators GetOperator() => op;

        public HashSet<StringTagNames> GetTagNames() => new HashSet<StringTagNames>(tagNames);

        public bool MatchCore(string content)
        {
            var cname = GetTagNames().First();
            return CachedMatchCore(cname, content);
        }

        Dictionary<string, bool> matchCache = new Dictionary<string, bool>();

        bool CachedMatchCore(StringTagNames cname, string content)
        {
            content = content?.ToLower() ?? "";
            if (matchCache.TryGetValue(content, out var r))
            {
                return r;
            }
            r = matcher.Invoke(content);
            if (!titleTagNames.Contains(cname))
            {
                matchCache.Add(content, r);
            }
            return r;
        }

        #endregion

        #region creator

        Highlighter CreateHighlighter()
        {
            foreach (var tag in titleTagNames)
            {
                if (this.tagNames.Contains(StringTagNames.Title))
                {
                    return new Highlighter(this.parsedKeyword);
                }
            }
            return null;
        }

        Func<string, bool> CreateMatcher(StringOperators op, string parsedKeyword)
        {
            var k = parsedKeyword;

            switch (op)
            {
                case StringOperators.IS:
                    return (c) => c.Equals(k);
                case StringOperators.NOT:
                    return (c) => !c.Equals(k);
                case StringOperators.HAS:
                    return (c) => string.IsNullOrEmpty(k) || c.Contains(k);
                case StringOperators.HASNOT:
                    return (c) =>
                    {
                        if (string.IsNullOrEmpty(k))
                        {
                            return !string.IsNullOrEmpty(c);
                        }
                        return !c.Contains(k);
                    };
                case StringOperators.LIKE:
                    return (c) => string.IsNullOrEmpty(k) || Misc.Utils.PartialMatch(c, k);
                case StringOperators.UNLIKE:
                    return (c) =>
                    {
                        if (string.IsNullOrEmpty(k))
                        {
                            return !string.IsNullOrEmpty(c);
                        }

                        return !Misc.Utils.PartialMatch(c, k);
                    };
                default:
                    break;
            }
            return null;
        }

        bool TryGetStrTagValue(
            Interfaces.ICoreServCtrl coreServ,
            ICoreStates cs,
            StringTagNames tag,
            out string r
        )
        {
            switch (tag)
            {
                case StringTagNames.Tag1:
                    r = cs.GetTag1();
                    break;
                case StringTagNames.Tag2:
                    r = cs.GetTag2();
                    break;
                case StringTagNames.Tag3:
                    r = cs.GetTag3();
                    break;
                case StringTagNames.Name:
                    r = cs.GetName();
                    break;
                case StringTagNames.Summary:
                    r = cs.GetSummary();
                    break;
                case StringTagNames.Remark:
                    r = cs.GetRemark();
                    break;
                case StringTagNames.Mark:
                    r = cs.GetMark();
                    break;
                case StringTagNames.Title:
                    r = cs.GetTitle();
                    break;
                case StringTagNames.Core:
                    r = coreServ.GetCoreCtrl().GetCustomCoreName();
                    break;
                case StringTagNames.Selected:
                    r = cs.IsSelected().ToString();
                    break;
                case StringTagNames.Modify:
                    var tick = cs.GetLastModifiedUtcTicks();
                    r = new DateTime(tick).ToLocalTime().ToString("yyMMdd");
                    break;
                default:
                    r = "";
                    return false;
            }
            return true;
        }

        static bool TryParseTagName(string tagName, out HashSet<StringTagNames> r)
        {
            r = new HashSet<StringTagNames>();

            // try full match first
            if (tagNameLookupTable.TryGetValue(tagName, out var cname))
            {
                r.Add(cname);
                return true;
            }

            var ok = false;
            foreach (var kv in tagNameLookupTable)
            {
                if (Misc.Utils.PartialMatch(kv.Key, tagName))
                {
                    r.Add(kv.Value);
                    ok = true;
                }
            }
            return ok;
        }

        internal static AdvStringFilter CreateFilter(string kw)
        {
            if (string.IsNullOrEmpty(kw) || !kw.StartsWith("#") || "#".Equals(kw))
            {
                return null;
            }
            var kws = kw.ToLower()
                .Substring(1)
                ?.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (!TryParseTagName(kws[0], out var cnames))
            {
                return null;
            }

            string parsedKeyword;
            StringOperators op;
            if (kws.Length < 2)
            {
                // #tag1 like $""
                op = StringOperators.LIKE;
                parsedKeyword = "";
            }
            else if (!operatorLookupTable.TryGetValue(kws[1], out op))
            {
                // #tag1 like ...
                op = StringOperators.LIKE;
                parsedKeyword = string.Join("", kws.Skip(1));
            }
            else
            {
                // #tag1 <op> ...
                parsedKeyword = string.Join("", kws.Skip(2));
            }

            var parser = new AdvStringFilter(cnames, op, parsedKeyword);
            return parser;
        }

        #endregion
    }
}
