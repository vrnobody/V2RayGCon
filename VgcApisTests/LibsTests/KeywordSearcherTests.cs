using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VgcApis.Libs.Infr;

namespace VgcApisTests.LibsTests
{
    [TestClass]
    public class KeywordSearcherTests
    {
        [DataTestMethod]
        [DataRow("#tg unlike abc", "abcd", false)]
        [DataRow("#tg unlike abc", "bcd", true)]
        [DataRow("#tg unlike abc", "bc", true)]
        [DataRow("#tg unlike a ,😀 文", ";-,a 中,😀文,bc", false)]
        [DataRow("#tg unlike a# ,😀 文", ";-,a 中,😀文,bc", true)]
        [DataRow("#tg unlike a", "", true)]
        [DataRow("#tg unlike ", "aa", true)]
        [DataRow("#tg unlike ", "", false)]
        [DataRow("#tg like a ,😀 文", ";-,a 中,😀文,bc", true)]
        [DataRow("#tg like a", "", false)]
        [DataRow("#tg like ", "aa", true)]
        [DataRow("#tg like ", "", true)]
        [DataRow("#tg has ab", "abc", true)]
        [DataRow("#tg has bc", "abc", true)]
        [DataRow("#tg has abcd", "abc", false)]
        [DataRow("#tg has ac", "abc", false)]
        [DataRow("#tg has e", "abc", false)]
        [DataRow("#tg has ", "a", true)]
        [DataRow("#tg has ", "", true)]
        [DataRow("#tg hasnot ab", "abc", false)]
        [DataRow("#tg hasnot bc", "abc", false)]
        [DataRow("#tg hasnot abcd", "abc", true)]
        [DataRow("#tg hasnot ac", "abc", true)]
        [DataRow("#tg hasnot e", "abc", true)]
        [DataRow("#tg hasnot ", "a", true)]
        [DataRow("#tg hasnot ", "", false)]
        [DataRow("#tg not ", "a", true)]
        [DataRow("#tg not a", "a", false)]
        [DataRow("#tg not   ", "", false)]
        [DataRow("#tg is a", "a", true)]
        [DataRow("#tg is a", "aa", false)]
        [DataRow("#tg is a", "", false)]
        [DataRow("#tg is is", "is", true)]
        [DataRow("#tg is", "", true)]
        [DataRow("#tg is", "aaa", false)]
        [DataRow("#tg  a ,😀 文", ";-,a 中,😀文,bc", true)]
        [DataRow("#tg a", "", false)]
        [DataRow("#tg ", "aa", true)]
        [DataRow("#tg ", "", true)]
        [DataRow("#tg like a", "a", true)]
        [DataRow("#tg a", "a", true)]
        [DataRow("#tg", "", true)]
        [DataRow(" a ,😀 文", ";-,a 中,😀文,bc", true)]
        [DataRow("a", "", false)]
        [DataRow("", "aa", true)]
        [DataRow("    ", "", true)]
        [DataRow("a", "a", true)]
        [DataRow("", "", true)]
        [DataRow("", "aaa", true)]
        [DataRow("##23", "#1234", true)]
        [DataRow("##123", "#12", false)]
        [DataRow("##123", "#123", true)]
        [DataRow("####", "#1#2##3##", true)]
        [DataRow("###", "#", false)]
        [DataRow("##", "#", true)]
        [DataRow("##", "aaa", false)]
        public void SearchContentTests(string keyword, string content, bool exp)
        {
            var kws = new KeywordSearcher(keyword);
            Assert.IsFalse(kws.IsIndex());
            var cache = new Dictionary<string, bool>();
            var r = kws.MatchString(cache, content);
            Assert.AreEqual(exp, r);
        }

        [DataTestMethod]
        [DataRow(
            "#name is is",
            "is",
            KeywordSearcher.MatchTypes.IS,
            KeywordSearcher.ContentNames.Name
        )]
        [DataRow(
            "#tg1 not a B ,;😁! cDDE 中文",
            "ab,;😁!cdde中文",
            KeywordSearcher.MatchTypes.NOT,
            KeywordSearcher.ContentNames.Tag1
        )]
        [DataRow(
            "#summary",
            "",
            KeywordSearcher.MatchTypes.LIKE,
            KeywordSearcher.ContentNames.Summary
        )]
        [DataRow("#tg2", "", KeywordSearcher.MatchTypes.LIKE, KeywordSearcher.ContentNames.Tag2)]
        [DataRow(
            "#tg",
            "",
            KeywordSearcher.MatchTypes.LIKE,
            KeywordSearcher.ContentNames.Tag1,
            KeywordSearcher.ContentNames.Tag2,
            KeywordSearcher.ContentNames.Tag3
        )]
        public void ContentNameParserTests(
            string kw,
            string expKw,
            KeywordSearcher.MatchTypes expMatchType,
            params KeywordSearcher.ContentNames[] expContentNames
        )
        {
            var kws = new KeywordSearcher(kw);
            Assert.IsFalse(kws.IsIndex());
            Assert.AreEqual(expKw, kws.GetKeyword());
            Assert.AreEqual(expMatchType, kws.GetMatchType());
            var cnames = kws.GetContentNames();
            Assert.AreEqual(expContentNames.Count(), cnames.Count());
            foreach (var cname in cnames)
            {
                Assert.IsTrue(expContentNames.Contains(cname));
            }
        }

        [DataTestMethod]
        [DataRow("#namenotexist", 0)]
        [DataRow("#index", 0)]
        [DataRow("#idx", 0)]
        [DataRow("#0", 0)]
        [DataRow("#", 0)]
        [DataRow("#12.345", 0)]
        [DataRow("#012.345", 0)]
        [DataRow("#-12.345", 0)]
        [DataRow("#-1", -1)]
        [DataRow("#123", 123)]
        [DataRow("#00000123", 123)]
        [DataRow("#00000", 0)]
        [DataRow("#abcdefg", 0)]
        [DataRow("#1abcdefg", 0)]
        public void SearchIndexTests(string kw, int expIndex)
        {
            var kws = new KeywordSearcher(kw);
            Assert.IsTrue(kws.IsIndex());
            Assert.AreEqual(expIndex, kws.GetIndex());
            Assert.AreEqual(KeywordSearcher.MatchTypes.LIKE, kws.GetMatchType());
            var cnames = kws.GetContentNames();
            Assert.AreEqual(1, cnames.Count());
        }

        [DataTestMethod]
        [DataRow(null, "")]
        [DataRow("", "")]
        [DataRow("1", "1")]
        [DataRow("##", "#")]
        [DataRow("a B 中 文 1😀😀", "ab中文1😀😀")]
        [DataRow("##a B 中 文 1😀😀", "#ab中文1😀😀")]
        public void SearchAllContentsTests(string kw, string expKw)
        {
            var kws = new KeywordSearcher(kw);
            Assert.IsFalse(kws.IsIndex());
            var cnames = kws.GetContentNames();
            Assert.IsFalse(cnames.Contains(KeywordSearcher.ContentNames.Index));
            Assert.AreEqual(
                Enum.GetNames(typeof(KeywordSearcher.ContentNames)).Count() - 1,
                cnames.Count()
            );
            Assert.AreEqual(KeywordSearcher.MatchTypes.LIKE, kws.GetMatchType());
            Assert.AreEqual(expKw, kws.GetKeyword());
        }
    }
}
