using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using VgcApis.Interfaces.CoreCtrlComponents;

namespace VgcApis.Libs.Infr.KwFilterComps
{
    public class AdvStringFilter : IAdvanceFilter<StringTagNames, StringOperators, string>
    {
        public AdvStringFilter(
            HashSet<StringTagNames> tagNames,
            bool not,
            StringOperators op,
            string keyword
        )
        {
            this.parsedKeyword = (op == StringOperators.MATCH) ? keyword : keyword.ToLower();
            this.tagNames = tagNames;
            this.op = op;
            this.not = not;

            this.matcher = CreateMatcher(this.op, this.parsedKeyword);
            this.highlighter = CreateHighlighter();
        }

        #region properties

        readonly bool not = false;
        readonly string parsedKeyword = "";
        readonly HashSet<StringTagNames> tagNames = new HashSet<StringTagNames>() { };
        readonly StringOperators op = StringOperators.LIKE;
        readonly Func<string, bool> matcher = null;
        readonly Highlighter highlighter = null;

        #endregion

        #region lookup tables

        static List<string> tips = CreateTipsCache();

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
        public int GetPri() => 1 << 1;

        internal static ReadOnlyCollection<string> GetTips() => tips.AsReadOnly();

        internal static bool TryGetStrTagValue(
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
                default:
                    r = "";
                    return false;
            }
            return true;
        }

        public IReadOnlyCollection<Interfaces.ICoreServCtrl> Filter(
            IReadOnlyCollection<Interfaces.ICoreServCtrl> coreServs
        )
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

        public override string ToString()
        {
            var n = this.not ? $" {Helpers.NOT}" : "";
            var o = this.op.ToString().ToLower();
            var ts = string.Join(", ", this.tagNames).ToString().ToLower();
            var r = $"#({ts}){n} {o} \"{this.parsedKeyword}\"";
            return r;
        }

        public Highlighter GetHighlighter() => this.highlighter;

        #endregion

        #region for unit tests
        public bool MatchCore(string content)
        {
            var cname = tagNames.First();
            return CachedMatchCore(cname, content);
        }

        Dictionary<string, bool> matchCache = new Dictionary<string, bool>();

        bool CachedMatchCore(StringTagNames cname, string content)
        {
            content = content ?? "";
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

        #region private methods
        static List<string> CreateTipsCache()
        {
            var r = new List<string>() { };
            foreach (var tag in Enum.GetNames(typeof(StringTagNames)))
            {
                r.Add($"#{tag.ToLower()}");
                r.Add($"#{tag.ToLower()} {Helpers.NOT}");
                foreach (var op in Enum.GetNames(typeof(StringOperators)))
                {
                    r.Add($"#{tag.ToLower()} {op.ToLower()}");
                    r.Add($"#{tag.ToLower()} {Helpers.NOT} {op.ToLower()}");
                }
            }
            return r;
        }
        #endregion

        #region creator

        Highlighter CreateHighlighter()
        {
            if (this.tagNames.Contains(StringTagNames.Title))
            {
                return new Highlighter(this.parsedKeyword);
            }
            return null;
        }

        Func<string, bool> CreateMatcher(StringOperators op, string parsedKeyword)
        {
            var k = parsedKeyword;

            switch (op)
            {
                case StringOperators.IS:
                    return (c) => this.not ? !c.ToLower().Equals(k) : c.ToLower().Equals(k);
                case StringOperators.HAS:
                    return (c) =>
                    {
                        if (string.IsNullOrEmpty(k))
                        {
                            if (not)
                            {
                                return !string.IsNullOrEmpty(c);
                            }
                            return true;
                        }
                        var has = c.ToLower().Contains(k);
                        return not ? !has : has;
                    };
                case StringOperators.LIKE:
                    return (c) =>
                    {
                        if (string.IsNullOrEmpty(k))
                        {
                            if (not)
                            {
                                return !string.IsNullOrEmpty(c);
                            }
                            return true;
                        }
                        var like = Misc.Utils.PartialMatch(c.ToLower(), k);
                        return not ? !like : like;
                    };
                case StringOperators.STARTS:
                    return (c) =>
                    {
                        var ok = c.ToLower().StartsWith(k);
                        return not ? !ok : ok;
                    };
                case StringOperators.ENDS:
                    return (c) =>
                    {
                        var ok = c.ToLower().EndsWith(k);
                        return not ? !ok : ok;
                    };
                case StringOperators.MATCH:
                    return (c) =>
                    {
                        var ok = false;
                        try
                        {
                            ok = Regex.IsMatch(c, k);
                        }
                        catch { }
                        return not ? !ok : ok;
                    };
                default:
                    break;
            }
            return null;
        }

        internal static bool TryParseTagName(string tagName, out HashSet<StringTagNames> r)
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
            var kws = Helpers.ParseLiteral(kw);
            return CreateFilter(kws);
        }

        internal static AdvStringFilter CreateFilter(string[] kws)
        {
            if (kws.Length < 1)
            {
                return null;
            }

            var tag = kws[0];
            if (string.IsNullOrEmpty(tag) || !tag.StartsWith("#") || tag.Length < 2)
            {
                return null;
            }

            if (!TryParseTagName(tag.Substring(1).ToLower(), out var cnames))
            {
                return null;
            }

            var idx = 1;
            var not = false;
            if (kws.Length > idx && kws[idx].ToLower() == Helpers.NOT)
            {
                not = true;
                idx++;
            }

            var op = StringOperators.LIKE;
            if (
                kws.Length > idx
                && operatorLookupTable.TryGetValue(kws[idx].ToLower(), out var tmpOp)
            )
            {
                op = tmpOp;
                idx++;
            }

            var parsedKeyword = string.Join("", kws.Skip(idx));
            var parser = new AdvStringFilter(cnames, not, op, parsedKeyword);
            return parser;
        }

        #endregion
    }
}
