using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VgcApis.Libs.Infr;

namespace VgcApisTests.LibsTests
{
    [TestClass]
    public class KeywordParserTests
    {
        [DataTestMethod]
        [DataRow("#lten ~ -1 2", -3, false)]
        [DataRow("#lten ~ -1 2", 3, false)]
        [DataRow("#lten ~ -1 2", 1, true)]
        [DataRow("#upd = -1", -1, true)]
        [DataRow("#upd = -2", 1, false)]
        [DataRow("#upd ! -1", 456, true)]
        [DataRow("#upd ! -1", -1, false)]
        [DataRow("#lten ~ -1 2", -1, true)]
        [DataRow("#latency < 200", 123, true)]
        [DataRow("#upd > -1", 456, true)]
        [DataRow("#down > -1", -456, false)]
        public void SearchNumContentTest(string keyword, long value, bool expResult)
        {
            var kws = new KeywordParser(keyword);
            Assert.AreEqual(expResult, kws.MatchNumber(value));
        }

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
        public void SearchStrContentTest(string keyword, string content, bool exp)
        {
            var kws = new KeywordParser(keyword);
            Assert.AreEqual(kws.GetSearchType(), KeywordParser.SearchTypes.String);
            var r = kws.MatchString(KeywordParser.StringContentNames.Tag1, content);
            Assert.AreEqual(exp, r);
        }

        [DataTestMethod]
        [DataRow(
            "#upd = 123 456 aaa",
            KeywordParser.NumberContentNames.Upload,
            KeywordParser.NumberMatchingTypes.Is
        )]
        [DataRow(
            "#upd ! 123 456 aaa",
            KeywordParser.NumberContentNames.Upload,
            KeywordParser.NumberMatchingTypes.Not
        )]
        [DataRow(
            "#upd ~ 123 123 aaa",
            KeywordParser.NumberContentNames.Upload,
            KeywordParser.NumberMatchingTypes.Between
        )]
        [DataRow(
            "#upd ~ 123 456 aaa",
            KeywordParser.NumberContentNames.Upload,
            KeywordParser.NumberMatchingTypes.Between
        )]
        [DataRow(
            "#download > 0000123 bbbb",
            KeywordParser.NumberContentNames.Download,
            KeywordParser.NumberMatchingTypes.LargerThen
        )]
        [DataRow(
            "#lat < 123",
            KeywordParser.NumberContentNames.Latency,
            KeywordParser.NumberMatchingTypes.SmallerThen
        )]
        public void ParseNumberMatchingTypeTest(
            string kw,
            KeywordParser.NumberContentNames expContentName,
            KeywordParser.NumberMatchingTypes expMatchType
        )
        {
            var kws = new KeywordParser(kw);
            Assert.AreEqual(KeywordParser.SearchTypes.Number, kws.GetSearchType());
            Assert.AreEqual(expMatchType, kws.GetNumMatchType());

            var cname = kws.GetNumContentName();
            Assert.AreEqual(expContentName, cname);
        }

        [DataTestMethod]
        [DataRow(
            "#name is is",
            "is",
            KeywordParser.StringMatchingTypes.IS,
            KeywordParser.StringContentNames.Name
        )]
        [DataRow(
            "#tg1 not a B ,;😁! cDDE 中文",
            "ab,;😁!cdde中文",
            KeywordParser.StringMatchingTypes.NOT,
            KeywordParser.StringContentNames.Tag1
        )]
        [DataRow(
            "#summary has",
            "",
            KeywordParser.StringMatchingTypes.HAS,
            KeywordParser.StringContentNames.Summary
        )]
        [DataRow(
            "#ttl hasnot 测 A 试 1 😊 ",
            "测a试1😊",
            KeywordParser.StringMatchingTypes.HASNOT,
            KeywordParser.StringContentNames.Title
        )]
        [DataRow(
            "#tg2",
            "",
            KeywordParser.StringMatchingTypes.LIKE,
            KeywordParser.StringContentNames.Tag2
        )]
        [DataRow(
            "#summary has",
            "",
            KeywordParser.StringMatchingTypes.HAS,
            KeywordParser.StringContentNames.Summary
        )]
        [DataRow(
            "#tg",
            "",
            KeywordParser.StringMatchingTypes.LIKE,
            KeywordParser.StringContentNames.Tag1,
            KeywordParser.StringContentNames.Tag2,
            KeywordParser.StringContentNames.Tag3
        )]
        public void ParseStringMatchingTypeTest(
            string kw,
            string expKw,
            KeywordParser.StringMatchingTypes expMatchType,
            params KeywordParser.StringContentNames[] expContentNames
        )
        {
            var kws = new KeywordParser(kw);
            Assert.AreEqual(kws.GetSearchType(), KeywordParser.SearchTypes.String);
            Assert.AreEqual(expKw, kws.GetKeyword());
            Assert.AreEqual(expMatchType, kws.GetStrMatchType());
            var cnames = kws.GetStrContentNames();
            Assert.AreEqual(expContentNames.Count(), cnames.Count());
            foreach (var cname in cnames)
            {
                Assert.IsTrue(expContentNames.Contains(cname));
            }
        }

        [DataTestMethod]
        [DataRow("#0", 0)]
        [DataRow("#", 0)]
        [DataRow("#-1", -1)]
        [DataRow("#-123", -123)]
        [DataRow("#123", 123)]
        [DataRow("#00000123", 123)]
        [DataRow("#00000", 0)]
        public void ParseIndexTest(string kw, int expIndex)
        {
            var kws = new KeywordParser(kw);
            Assert.AreEqual(kws.GetSearchType(), KeywordParser.SearchTypes.Index);
            Assert.AreEqual(expIndex, kws.GetIndex());
        }

        [DataTestMethod]
        [DataRow(null, "")]
        [DataRow("", "")]
        [DataRow("1", "1")]
        [DataRow("-1234", "-1234")]
        [DataRow("##", "#")]
        [DataRow("######", "#####")]
        [DataRow("a B 中 文 1😀😀", "ab中文1😀😀")]
        [DataRow("##a B 中 文;# 1😀😀", "#ab中文;#1😀😀")]
        public void ParseSimpleStringTest(string kw, string expKw)
        {
            var kws = new KeywordParser(kw);
            Assert.AreEqual(kws.GetSearchType(), KeywordParser.SearchTypes.String);
            Assert.AreEqual(expKw, kws.GetKeyword());
        }

        [DataTestMethod]
        [DataRow(null, KeywordParser.SearchTypes.String)]
        [DataRow("", KeywordParser.SearchTypes.String)]
        [DataRow("#1a", KeywordParser.SearchTypes.Error)]
        [DataRow("#1.0", KeywordParser.SearchTypes.Error)]
        [DataRow("1", KeywordParser.SearchTypes.String)]
        [DataRow("##", KeywordParser.SearchTypes.String)]
        [DataRow("a B 中 文 1😀😀", KeywordParser.SearchTypes.String)]
        [DataRow("##a B 中 文 1😀😀", KeywordParser.SearchTypes.String)]
        [DataRow("#", KeywordParser.SearchTypes.Index)]
        [DataRow("#0", KeywordParser.SearchTypes.Index)]
        [DataRow("#1", KeywordParser.SearchTypes.Index)]
        [DataRow("#001", KeywordParser.SearchTypes.Index)]
        [DataRow("#-11", KeywordParser.SearchTypes.Index)]
        [DataRow("#tg", KeywordParser.SearchTypes.String)]
        [DataRow("#tag1", KeywordParser.SearchTypes.String)]
        [DataRow("#mk", KeywordParser.SearchTypes.String)]
        [DataRow("#rk", KeywordParser.SearchTypes.String)]
        [DataRow("#rkz", KeywordParser.SearchTypes.Error)]
        [DataRow("#latency", KeywordParser.SearchTypes.Error)]
        [DataRow("#upd", KeywordParser.SearchTypes.Error)]
        [DataRow("#down", KeywordParser.SearchTypes.Error)]
        [DataRow("#ltny < ", KeywordParser.SearchTypes.Error)]
        [DataRow("#ltny ~ 123", KeywordParser.SearchTypes.Error)]
        [DataRow("#ltny # 123", KeywordParser.SearchTypes.Error)]
        [DataRow("#laty = 13", KeywordParser.SearchTypes.Number)]
        [DataRow("#latency ! 145", KeywordParser.SearchTypes.Number)]
        [DataRow("#upd > 123 aaa 456 ", KeywordParser.SearchTypes.Number)]
        [DataRow("#upd > aaa", KeywordParser.SearchTypes.Error)]
        [DataRow("#down ~ 12", KeywordParser.SearchTypes.Error)]
        [DataRow("#latency < 100", KeywordParser.SearchTypes.Number)]
        [DataRow("#upd > 100", KeywordParser.SearchTypes.Number)]
        [DataRow("#down ~ 321 45", KeywordParser.SearchTypes.Number)]
        public void ParseSearchTypeTest(string kw, KeywordParser.SearchTypes expType)
        {
            var kws = new KeywordParser(kw);
            Assert.AreEqual(expType, kws.GetSearchType());
        }
    }
}
