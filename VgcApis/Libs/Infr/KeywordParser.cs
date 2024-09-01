using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using VgcApis.Interfaces.CoreCtrlComponents;
using static ScintillaNET.Style;

namespace VgcApis.Libs.Infr
{
    public class KeywordParser
    {
        #region properties

        int parsedIndex = 0;
        long firstNumber = 0;
        long secondNumber = 0;
        string parsedKeyword = "";

        SearchTypes searchType = SearchTypes.String;

        HashSet<StringContentNames> strContentNames = new HashSet<StringContentNames>() { };
        StringMatchingTypes strMatchingType = StringMatchingTypes.LIKE;
        Func<string, bool> strMatcher = null;

        NumberContentNames numContentName = NumberContentNames.Latency;
        NumberMatchingTypes numMatchingType = NumberMatchingTypes.SmallerThen;
        Func<long, bool> numMatcher = null;

        #endregion

        public KeywordParser(string keyword)
        {
            ParseSearchKeyword(keyword);
            if (searchType == SearchTypes.String)
            {
                this.strMatcher = CreateStrMatcher(this.strMatchingType, this.parsedKeyword);
            }
            if (searchType == SearchTypes.Number)
            {
                this.numMatcher = CreateNumMatcher(
                    this.numMatchingType,
                    this.firstNumber,
                    this.secondNumber
                );
            }
        }

        #region lookup tables

        static readonly HashSet<StringContentNames> allStrContentNames = InitAllStrContentNames();

        static readonly Dictionary<string, StringContentNames> strContentNameLookupTable =
            allStrContentNames.ToDictionary(n => n.ToString().ToLower(), n => n);

        static readonly HashSet<StringMatchingTypes> allStrMatchingTypes =
            InitAllStrMatchingTypes();

        static readonly Dictionary<string, StringMatchingTypes> strMatchingTypeLookupTable =
            allStrMatchingTypes.ToDictionary(t => t.ToString().ToLower(), t => t);

        static readonly HashSet<StringContentNames> largeStrContentNames =
            new HashSet<StringContentNames>()
            {
                StringContentNames.Title,
                StringContentNames.Summary,
                StringContentNames.Name,
            };

        static readonly Dictionary<string, NumberMatchingTypes> signToNumMatchingTypeLookupTable =
            new Dictionary<string, NumberMatchingTypes>()
            {
                { ">", NumberMatchingTypes.LargerThen },
                { "<", NumberMatchingTypes.SmallerThen },
                { "=", NumberMatchingTypes.Is },
                { "!", NumberMatchingTypes.Not },
                { "~", NumberMatchingTypes.Between },
            };

        static readonly List<string> tips = CreateTipsCache();
        #endregion

        #region public methods
        public bool Match(Interfaces.ICoreServCtrl coreServ)
        {
            // assume matching type is string or number

            var cs = coreServ.GetCoreStates();
            switch (searchType)
            {
                case SearchTypes.String:
                    if (this.strMatcher == null)
                    {
                        return false;
                    }
                    foreach (var cname in strContentNames)
                    {
                        if (
                            TryGetStringContent(cs, cname, out var content)
                            && MatchString(cname, content)
                        )
                        {
                            return true;
                        }
                    }
                    break;
                case SearchTypes.Number:
                    if (this.numMatcher == null)
                    {
                        return false;
                    }
                    if (TryGetNumContent(cs, numContentName, out var number) && MatchNumber(number))
                    {
                        return true;
                    }
                    break;
            }
            return false;
        }

        public SearchTypes GetSearchType() => searchType;

        public bool RequireTitleHighlighting()
        {
            if (searchType != SearchTypes.String)
            {
                return false;
            }
            foreach (var sname in largeStrContentNames)
            {
                if (strContentNames.Contains(sname))
                {
                    return true;
                }
            }
            return false;
        }

        public int GetIndex() => parsedIndex;

        public string GetKeyword() => parsedKeyword;

        public static ReadOnlyCollection<string> GetTips() => tips.AsReadOnly();

        // lazy
        public HashSet<StringContentNames> GetAllStrContentNames() =>
            new HashSet<StringContentNames>(allStrContentNames);
        #endregion

        #region for unit tests

        internal StringMatchingTypes GetStrMatchType() => strMatchingType;

        internal HashSet<StringContentNames> GetStrContentNames() =>
            new HashSet<StringContentNames>(strContentNames);

        Dictionary<string, bool> strMatchCache = new Dictionary<string, bool>();

        internal bool MatchString(StringContentNames cname, string content)
        {
            content = content?.ToLower() ?? "";
            if (strMatchCache.TryGetValue(content, out var r))
            {
                return r;
            }
            r = strMatcher.Invoke(content);
            if (!largeStrContentNames.Contains(cname))
            {
                strMatchCache.Add(content, r);
            }
            return r;
        }

        internal NumberContentNames GetNumContentName() => numContentName;

        internal NumberMatchingTypes GetNumMatchType() => numMatchingType;

        internal bool MatchNumber(long num)
        {
            return numMatcher.Invoke(num);
        }

        #endregion

        #region private methods

        static HashSet<StringContentNames> InitAllStrContentNames()
        {
            var r = new HashSet<StringContentNames>();
            foreach (StringContentNames cname in Enum.GetValues(typeof(StringContentNames)))
            {
                r.Add(cname);
            }
            return r;
        }

        static string LookupNumberMatchingSign(NumberMatchingTypes numMatchingType)
        {
            foreach (var kv in signToNumMatchingTypeLookupTable)
            {
                if (kv.Value == numMatchingType)
                {
                    return kv.Key;
                }
            }
            return null;
        }

        public static List<string> CreateTipsCache()
        {
            var r = new List<string>() { "#" };

            var snames = Enum.GetNames(typeof(StringContentNames)).Select(n => $"#{n.ToLower()}");
            var first = snames.First();
            r.AddRange(snames);
            var smts = Enum.GetNames(typeof(StringMatchingTypes))
                .Select(n => $"{first} {n.ToString().ToLower()}");
            r.AddRange(smts);

            var numNames = Enum.GetNames(typeof(NumberContentNames)).Select(n => $"#{n.ToLower()}");
            first = numNames.First();
            r.AddRange(numNames);

            var between = signToNumMatchingTypeLookupTable
                .Where(kv => kv.Value == NumberMatchingTypes.Between)
                .Select(kv => kv.Key)
                .First();
            foreach (string sign in signToNumMatchingTypeLookupTable.Keys)
            {
                var str = $"{first} {sign} 123";
                if (sign == between)
                {
                    str = $"{str} 456";
                }
                r.Add(str);
            }
            return r;
        }

        Func<long, bool> CreateNumMatcher(
            NumberMatchingTypes numMatchingType,
            long first,
            long second
        )
        {
            switch (numMatchingType)
            {
                case NumberMatchingTypes.SmallerThen:
                    return (n) => n <= first;
                case NumberMatchingTypes.LargerThen:
                    return (n) => n >= first;
                case NumberMatchingTypes.Is:
                    return (n) => n == first;
                case NumberMatchingTypes.Not:
                    return (n) => n != first;
                case NumberMatchingTypes.Between:
                    return (n) => n >= first && n <= second;
                default:
                    break;
            }
            return null;
        }

        static readonly long MiB = 1024 * 1024;

        bool TryGetNumContent(ICoreStates cs, NumberContentNames cname, out long r)
        {
            switch (cname)
            {
                case NumberContentNames.Latency:
                    r = cs.GetSpeedTestResult();
                    return true;
                case NumberContentNames.Upload:
                    // MiB
                    r = cs.GetUplinkTotalInBytes() / MiB;
                    return true;
                case NumberContentNames.Download:
                    // MiB
                    r = cs.GetDownlinkTotalInBytes() / MiB;
                    return true;
            }
            r = 0;
            return false;
        }

        Func<string, bool> CreateStrMatcher(StringMatchingTypes strMatchingType, string keyword)
        {
            var k = keyword;

            switch (strMatchingType)
            {
                case StringMatchingTypes.IS:
                    return (c) => c.Equals(k);
                case StringMatchingTypes.NOT:
                    return (c) => !c.Equals(k);
                case StringMatchingTypes.HAS:
                    return (c) => string.IsNullOrEmpty(k) || c.Contains(this.parsedKeyword);
                case StringMatchingTypes.HASNOT:
                    return (c) =>
                    {
                        if (string.IsNullOrEmpty(k))
                        {
                            return !string.IsNullOrEmpty(c);
                        }
                        return !c.Contains(k);
                    };
                case StringMatchingTypes.LIKE:
                    return (c) => string.IsNullOrEmpty(k) || Misc.Utils.PartialMatch(c, k);
                case StringMatchingTypes.UNLIKE:
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

        bool TryGetStringContent(ICoreStates cs, StringContentNames cname, out string content)
        {
            content = "";
            switch (cname)
            {
                case StringContentNames.Tag1:
                    content = cs.GetTag1();
                    return true;
                case StringContentNames.Tag2:
                    content = cs.GetTag2();
                    return true;
                case StringContentNames.Tag3:
                    content = cs.GetTag3();
                    return true;
                case StringContentNames.Name:
                    content = cs.GetName();
                    return true;
                case StringContentNames.Summary:
                    content = cs.GetSummary();
                    return true;
                case StringContentNames.Remark:
                    content = cs.GetRemark();
                    return true;
                case StringContentNames.Mark:
                    content = cs.GetMark();
                    return true;
                case StringContentNames.Title:
                    content = cs.GetTitle();
                    return true;
                default:
                    return false;
            }
        }

        void ParseSearchKeyword(string keyword)
        {
            var kw = keyword?.ToLower() ?? "";
            this.searchType = SearchTypes.String;

            if (kw.StartsWith("##"))
            {
                this.parsedKeyword = kw.Substring(1).Replace(" ", "");
                this.strContentNames = GetAllStrContentNames();
                return;
            }

            if (kw.StartsWith("#"))
            {
                this.searchType = SearchTypes.Index;
                if (kw == "#")
                {
                    return;
                }

                kw = kw.Substring(1);
                if (!int.TryParse(kw, out this.parsedIndex) && !TryParseComplexSearchKeyword(kw))
                {
                    searchType = SearchTypes.Error;
                }
                return;
            }

            this.parsedKeyword = kw.Replace(" ", "");
            this.strContentNames = GetAllStrContentNames();
        }

        bool TryParseStrContenKeyword(string name, out HashSet<StringContentNames> r)
        {
            r = new HashSet<StringContentNames>();

            // try full match first
            if (strContentNameLookupTable.TryGetValue(name, out var ctype))
            {
                r.Add(ctype);
                return true;
            }

            var ok = false;
            foreach (var kv in strContentNameLookupTable)
            {
                if (Misc.Utils.PartialMatch(kv.Key, name))
                {
                    r.Add(kv.Value);
                    ok = true;
                }
            }
            return ok;
        }

        static HashSet<StringMatchingTypes> InitAllStrMatchingTypes()
        {
            var r = new HashSet<StringMatchingTypes>();
            foreach (StringMatchingTypes t in Enum.GetValues(typeof(StringMatchingTypes)))
            {
                r.Add(t);
            }

            return r;
        }

        bool TryParseStringSearchKeywords(string[] kws)
        {
            searchType = SearchTypes.String;

            if (!TryParseStrContenKeyword(kws[0], out var ctypes))
            {
                return false;
            }

            this.strContentNames = ctypes;
            if (kws.Length < 2)
            {
                // #tag1 like $""
                return true;
            }

            if (strMatchingTypeLookupTable.TryGetValue(kws[1], out var smt))
            {
                // #tag1 is ...
                strMatchingType = smt;
                this.parsedKeyword = string.Join("", kws.Skip(2));
                return true;
            }

            // #tag1 like ...
            this.parsedKeyword = string.Join("", kws.Skip(1));
            return true;
        }

        bool TryParseNumContenKeyword(string name, out NumberContentNames r)
        {
            foreach (NumberContentNames ty in Enum.GetValues(typeof(NumberContentNames)))
            {
                if (Misc.Utils.PartialMatchCi(ty.ToString(), name))
                {
                    r = ty;
                    return true;
                }
            }

            r = NumberContentNames.Latency;
            return false;
        }

        bool TryParseNumberSearchKeywords(string[] kws)
        {
            searchType = SearchTypes.Number;
            if (kws.Length < 3)
            {
                // #latency < 100
                return false;
            }

            if (!TryParseNumContenKeyword(kws[0], out var ctype))
            {
                return false;
            }
            this.numContentName = ctype;

            if (!signToNumMatchingTypeLookupTable.TryGetValue(kws[1], out var mtype))
            {
                return false;
            }
            numMatchingType = mtype;
            if (!long.TryParse(kws[2], out firstNumber))
            {
                return false;
            }

            if (mtype == NumberMatchingTypes.Between)
            {
                // #latency ~ 100 200
                if (kws.Length < 4)
                {
                    return false;
                }

                var ok = long.TryParse(kws[3], out this.secondNumber);
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

        bool TryParseComplexSearchKeyword(string kw)
        {
            var kws = kw.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (kws.Length < 1)
            {
                return false;
            }

            if (TryParseStringSearchKeywords(kws))
            {
                return true;
            }

            if (TryParseNumberSearchKeywords(kws))
            {
                return true;
            }

            return false;
        }

        #endregion


        #region enums
        public enum SearchTypes
        {
            Error, // parse keyword failed
            Index,
            String,
            Number,
        }

        public enum StringContentNames
        {
            Title,
            Name,
            Summary,
            Mark,
            Remark,
            Tag1,
            Tag2,
            Tag3,
        }

        public enum NumberContentNames
        {
            Latency,
            Upload,
            Download,
        }

        public enum NumberMatchingTypes
        {
            LargerThen,
            SmallerThen,
            Between,
            Is,
            Not,
        }

        public enum StringMatchingTypes
        {
            IS,
            NOT,
            HAS,
            HASNOT,
            LIKE,
            UNLIKE,
        }

        #endregion
    }
}
