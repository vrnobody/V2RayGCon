using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace VgcApis.Libs.Infr.KwFilterComps
{
    internal class AdvOrderByFilter : ISimpleFilter
    {
        static readonly string FILTER_NAME = @"orderby";

        public AdvOrderByFilter(List<TagName> tagNames)
        {
            this.tagNames = tagNames;
        }

        #region properties

        readonly List<TagName> tagNames;

        readonly Highlighter highlighter = null;

        #endregion

        #region lookup tables

        readonly static List<string> tips = CreateTipsCache();

        #endregion

        #region public methods
        public int GetPri() => 1;

        public override string ToString()
        {
            var ts = string.Join(", ", this.tagNames).ToLower();
            var r = $"#{FILTER_NAME} {ts}";
            return r;
        }

        public static ReadOnlyCollection<string> GetTips() => tips.AsReadOnly();

        public IReadOnlyCollection<Interfaces.ICoreServCtrl> Filter(
            IReadOnlyCollection<Interfaces.ICoreServCtrl> coreServs
        )
        {
            // clone
            var r = coreServs.ToList();
            r.Sort(Compare);
            return r;
        }

        public Highlighter GetHighlighter() => highlighter;

        #endregion

        #region private methods

        int Compare(Interfaces.ICoreServCtrl x, Interfaces.ICoreServCtrl y)
        {
            var csx = x.GetCoreStates();
            var csy = y.GetCoreStates();
            var r = 0;
            foreach (var tagName in tagNames)
            {
                switch (tagName.tag)
                {
                    case StringTagNames stag:
                        if (
                            AdvStringFilter.TryGetStrTagValue(x, csx, stag, out var sx)
                            && AdvStringFilter.TryGetStrTagValue(y, csy, stag, out var sy)
                        )
                        {
                            r = sx.CompareTo(sy);
                        }
                        break;
                    case NumberTagNames ntag:
                        if (
                            AdvNumberFilter.TryGetContent(x, csx, ntag, out var nx)
                            && AdvNumberFilter.TryGetContent(y, csy, ntag, out var ny)
                        )
                        {
                            r = nx.CompareTo(ny);
                        }
                        break;
                }
                if (r != 0)
                {
                    return tagName.not ? -1 * r : r;
                }
            }
            return r;
        }

        static List<string> CreateTipsCache()
        {
            var prefix = $"#{FILTER_NAME}";
            var tags = Enum.GetNames(typeof(NumberTagNames))
                .Concat(Enum.GetNames(typeof(StringTagNames)))
                .Select(t => t.ToLower())
                .ToList();

            var r = new List<string>() { prefix };
            foreach (var tag1 in tags)
            {
                r.Add($"{prefix} {tag1}");
                r.Add($"{prefix} -{tag1}");
                foreach (var tag2 in tags)
                {
                    if (tag1 != tag2)
                    {
                        r.Add($"{prefix} {tag1} {tag2}");
                        r.Add($"{prefix} {tag1} -{tag2}");
                        r.Add($"{prefix} -{tag1} {tag2}");
                        r.Add($"{prefix} -{tag1} -{tag2}");
                    }
                }
            }
            return r;
        }

        #endregion

        #region creator

        internal static AdvOrderByFilter CreateFilter(string kw)
        {
            if (string.IsNullOrEmpty(kw) || !kw.StartsWith("#") || "#".Equals(kw))
            {
                return null;
            }
            var kws = Helpers.ParseLiteral(kw);
            return CreateFilter(kws);
        }

        // remove later
        internal static AdvOrderByFilter CreateFilter(string[] kws)
        {
            if (kws.Length < 2)
            {
                // #latency < 100
                return null;
            }

            var name = kws[0];
            if (
                string.IsNullOrEmpty(name)
                || !name.StartsWith("#")
                || name.Length < 2
                || !Misc.Utils.PartialMatchCi($"#{FILTER_NAME}", name)
            )
            {
                return null;
            }

            var tags = new List<TagName>();
            var remainKws = kws.Skip(1);
            foreach (var kw in remainKws)
            {
                TryAddTag(ref tags, kw);
            }

            if (tags.Count < 1)
            {
                return null;
            }

            var filter = new AdvOrderByFilter(tags);
            return filter;
        }

        static bool TryAddTag(ref List<TagName> tags, string tag)
        {
            tag = tag?.ToLower() ?? "";

            var not = tag.StartsWith("-");

            if (not)
            {
                tag = tag.Substring(1);
            }

            if (AdvNumberFilter.TryParseContenName(tag, out var ntags))
            {
                foreach (var ntag in ntags)
                {
                    tags.Add(new TagName(ntag, not));
                }
                return true;
            }

            if (AdvStringFilter.TryParseTagName(tag, out var stags))
            {
                foreach (var stag in stags)
                {
                    tags.Add(new TagName(stag, not));
                }
                return true;
            }
            return false;
        }

        #endregion
    }
}
