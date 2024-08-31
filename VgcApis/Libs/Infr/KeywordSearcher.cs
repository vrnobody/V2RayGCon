using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using VgcApis.Interfaces.CoreCtrlComponents;
using static ScintillaNET.Style;

namespace VgcApis.Libs.Infr
{
    public class KeywordSearcher
    {
        #region enums
        public enum ContentNames
        {
            Index,
            Title,
            Name,
            Summary,
            Mark,
            Remark,
            Tag1,
            Tag2,
            Tag3,
        }

        public enum MatchTypes
        {
            IS,
            NOT,
            HAS,
            HASNOT,
            LIKE,
            UNLIKE,
        }

        #endregion

        HashSet<ContentNames> contentNames = new HashSet<ContentNames>() { };
        int index = 0;
        string keyword = "";
        MatchTypes matchType = MatchTypes.LIKE;
        Func<string, bool> matcher = null;

        HashSet<ContentNames> contentNamesWithoutIndex = GetContentNamesWithoutIndex();

        public KeywordSearcher(string keyword)
        {
            ParseSearchKeyword(keyword);
            this.matcher = CreateMatcher(this.matchType, this.keyword);
        }

        #region static methods
        static public HashSet<MatchTypes> GetMatchTypes()
        {
            var r = new HashSet<MatchTypes>();
            foreach (MatchTypes ty in Enum.GetValues(typeof(MatchTypes)))
            {
                r.Add(ty);
            }
            return r;
        }

        public static HashSet<ContentNames> GetContentNamesWithoutIndex()
        {
            var r = new HashSet<ContentNames>();
            foreach (ContentNames cname in Enum.GetValues(typeof(ContentNames)))
            {
                if (cname != ContentNames.Index)
                {
                    r.Add(cname);
                }
            }
            return r;
        }
        #endregion

        #region public methods


        public bool Match(Interfaces.ICoreServCtrl coreServ)
        {
            if (this.matcher == null)
            {
                return false;
            }

            var cache = new Dictionary<string, bool>() { };
            var cs = coreServ.GetCoreStates();
            foreach (var cname in contentNames)
            {
                if (TryGetContentByName(cs, cname, out var content) && MatchString(cache, content))
                {
                    return true;
                }
            }
            return false;
        }

        // for unit tests
        internal bool MatchString(Dictionary<string, bool> cache, string content)
        {
            content = content.ToLower();
            if (cache.TryGetValue(content, out var ok) && ok)
            {
                return true;
            }
            var r = matcher.Invoke(content);
            cache[content] = r;
            return r;
        }

        public bool IsIndex() => contentNames.Contains(ContentNames.Index);

        public int GetIndex() => index;

        public string GetKeyword() => keyword;

        public MatchTypes GetMatchType() => matchType;

        public bool HasContentName(ContentNames ctype) => contentNames.Contains(ctype);

        public HashSet<ContentNames> GetContentNames() => new HashSet<ContentNames>(contentNames);

        #endregion

        #region private methods
        Func<string, bool> CreateMatcher(MatchTypes matchType, string kw)
        {
            var k = kw;

            switch (matchType)
            {
                case MatchTypes.IS:
                    return (c) => c.Equals(k);
                case MatchTypes.NOT:
                    return (c) => !c.Equals(k);
                case MatchTypes.HAS:
                    return (c) => string.IsNullOrEmpty(k) || c.Contains(keyword);
                case MatchTypes.HASNOT:
                    return (c) =>
                    {
                        if (string.IsNullOrEmpty(k))
                        {
                            return !string.IsNullOrEmpty(c);
                        }
                        return !c.Contains(k);
                    };
                case MatchTypes.LIKE:
                    return (c) => string.IsNullOrEmpty(k) || Misc.Utils.PartialMatch(c, k);
                case MatchTypes.UNLIKE:
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

        bool TryGetContentByName(ICoreStates cs, ContentNames cname, out string content)
        {
            content = "";
            switch (cname)
            {
                case ContentNames.Tag1:
                    content = cs.GetTag1();
                    return true;
                case ContentNames.Tag2:
                    content = cs.GetTag2();
                    return true;
                case ContentNames.Tag3:
                    content = cs.GetTag3();
                    return true;
                case ContentNames.Name:
                    content = cs.GetName();
                    return true;
                case ContentNames.Summary:
                    content = cs.GetSummary();
                    return true;
                case ContentNames.Remark:
                    content = cs.GetRemark();
                    return true;
                case ContentNames.Mark:
                    content = cs.GetMark();
                    return true;
                case ContentNames.Title:
                    content = cs.GetTitle();
                    return true;
                default:
                    return false;
            }
        }

        void ParseSearchKeyword(string kw)
        {
            kw = kw?.ToLower() ?? "";

            if (kw.StartsWith("##"))
            {
                this.contentNames = contentNamesWithoutIndex;
                this.keyword = kw.Substring(1).Replace(" ", "");
                return;
            }

            if (kw.StartsWith("#"))
            {
                kw = kw.Substring(1);
                this.contentNames = new HashSet<ContentNames>() { ContentNames.Index };
                if (!int.TryParse(kw, out this.index))
                {
                    index = 0;
                    ParseConditionalKeyword(kw);
                }
                return;
            }

            this.contentNames = contentNamesWithoutIndex;
            this.keyword = kw.Replace(" ", "");
        }

        HashSet<ContentNames> ParseContenName(string name)
        {
            var cnames = contentNamesWithoutIndex;
            var r = new HashSet<ContentNames>();

            foreach (var cname in cnames)
            {
                if (cname.ToString().ToLower() == name)
                {
                    r.Add(cname);
                    return r;
                }
            }

            foreach (var cname in cnames)
            {
                if (Misc.Utils.PartialMatchCi(cname.ToString(), name))
                {
                    r.Add(cname);
                }
            }
            return r;
        }

        void ParseConditionalKeyword(string kw)
        {
            var kws = kw.Split(' ');
            if (kws.Length < 1)
            {
                return;
            }
            var contentName = kws[0];
            if (string.IsNullOrEmpty(contentName))
            {
                return;
            }

            var ctypes = ParseContenName(contentName);
            if (ctypes.Count < 1)
            {
                return;
            }
            this.contentNames = ctypes;

            if (kws.Length < 2)
            {
                return;
            }

            foreach (MatchTypes ty in Enum.GetValues(typeof(MatchTypes)))
            {
                if (ty.ToString().ToLower() == kws[1])
                {
                    this.matchType = ty;
                    this.keyword = string.Join("", kws.Skip(2));
                    return;
                }
            }

            this.keyword = string.Join("", kws.Skip(1));
        }

        #endregion
    }
}
