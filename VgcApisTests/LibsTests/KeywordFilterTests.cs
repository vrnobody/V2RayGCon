using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VgcApis.Libs.Infr;
using VgcApis.Libs.Infr.KwFilterComps;

namespace VgcApisTests.LibsTests
{
    [TestClass]
    public class KeywordFilterTests
    {
        #region adv number filter
        [TestMethod]
        [DataRow("#MOd  <  02", 1, true)]
        [DataRow("#moD  <  2", 2, true)]
        [DataRow("#moD  <  02", 3, false)]
        [DataRow("#MOd  >  2", 1, false)]
        [DataRow("#moD  >  02", 2, true)]
        [DataRow("#moD  >  2", 3, true)]
        public void AdvNumberFilterTagModifyDayTest(string keyword, long value, bool expResult)
        {
            var np = AdvNumberFilter.CreateFilter(keyword);
            var now = DateTime.UtcNow.ToLocalTime();

            Assert.IsNotNull(np);
            var d = VgcApis.Misc.Utils.Str2Int(now.ToString("yyyyMM")) * 100;
            var r = np.MatchCore(value + d);
            Assert.AreEqual(expResult, r);
        }

        [TestMethod]
        [DataRow("#MOd  <  802", 801, true)]
        [DataRow("#moD  <  0802", 802, true)]
        [DataRow("#moD  <  802", 803, false)]
        [DataRow("#MOd  >  0802", 801, false)]
        [DataRow("#moD  >  802", 802, true)]
        [DataRow("#moD  >  0802", 803, true)]
        public void AdvNumberFilterTagModifyMonthDayTest(string keyword, long value, bool expResult)
        {
            var np = AdvNumberFilter.CreateFilter(keyword);
            var d = DateTime.UtcNow.ToLocalTime().Year * 10000;

            Assert.IsNotNull(np);
            var r = np.MatchCore(value + d);
            Assert.AreEqual(expResult, r);
        }

        [DataRow("#MOd  <  240802", 240801, true)]
        [DataRow("#moD  >  240802", 240802, true)]
        [DataRow("#moD  <  240802", 240803, false)]
        public void AdvNumberFilterTagModifyYMDTest(string keyword, long value, bool expResult)
        {
            var np = AdvNumberFilter.CreateFilter(keyword);
            var d = (DateTime.UtcNow.ToLocalTime().Year / 100) * 1000000;

            Assert.IsNotNull(np);
            var r = np.MatchCore(value + d);
            Assert.AreEqual(expResult, r);
        }

        [DataTestMethod]
        [DataRow("#lten ~ -1 2", -3, false)]
        [DataRow("#lten ~ -1 2", 3, false)]
        [DataRow("#lten ~ -1 2", 1, true)]
        [DataRow("#Upd = -1", -1, true)]
        [DataRow("#upd = -2", 1, false)]
        [DataRow("#uPd ! -1", 456, true)]
        [DataRow("#upd ! -1", -1, false)]
        [DataRow("#lTeN ~ -1 2", -1, true)]
        [DataRow("#lAtency < 200", 123, true)]
        [DataRow("#upd > -1", 456, true)]
        [DataRow("#down > -1", -456, false)]
        [DataRow("#prt > 1080 ", 1080, true)]
        [DataRow("#MOd  <  20240802", 20240801, true)]
        [DataRow("#moD  >  20240802", 20240802, true)]
        [DataRow("#moD  <  20240802", 20240803, false)]
        [DataRow("#moD  <  120240802", 120240701, true)]
        [DataRow("#moD  <  120240802", 120240803, false)]
        public void AdvNumberFilterMatchCoreTest(string keyword, long value, bool expResult)
        {
            var np = AdvNumberFilter.CreateFilter(keyword);
            Assert.IsNotNull(np);
            Assert.AreEqual(expResult, np.MatchCore(value));
        }

        [DataTestMethod]
        [DataRow("#upd = 123 456 aaa", NumberTagNames.Upload, NumberOperators.Is)]
        [DataRow("#upd ! 123 456 aaa", NumberTagNames.Upload, NumberOperators.Not)]
        [DataRow("#upd ~ 123 123 aaa", NumberTagNames.Upload, NumberOperators.Between)]
        [DataRow("#upd ~ 123 456 aaa", NumberTagNames.Upload, NumberOperators.Between)]
        [DataRow("#download > 0000123 bbbb", NumberTagNames.Download, NumberOperators.LargerThen)]
        [DataRow("#lat < 123", NumberTagNames.Latency, NumberOperators.SmallerThen)]
        [DataRow("#prt = 1080", NumberTagNames.Port, NumberOperators.Is)]
        [DataRow("#mod ! 123", NumberTagNames.Modify, NumberOperators.Not)]
        public void AdvNumberFilterParseTagNameAndOperatorTest(
            string kw,
            NumberTagNames expContentName,
            NumberOperators expOp
        )
        {
            var parser = AdvNumberFilter.CreateFilter(kw);
            Assert.IsNotNull(parser);

            var cnames = parser.GetTagNames();
            Assert.AreEqual(1, cnames.Count);
            Assert.IsTrue(cnames.Contains(expContentName));

            Assert.AreEqual(expOp, parser.GetOperator());
        }

        [DataTestMethod]
        [DataRow("#laty = 13")]
        [DataRow("#latency ! 145")]
        [DataRow("#upd > 123 aaa 456 ")]
        [DataRow("#latency < 100")]
        [DataRow("#upd > 100")]
        [DataRow("#down ~ 321 45")]
        [DataRow("#laty     =    13")]
        [DataRow("#lAtEncy    !   145   ")]
        [DataRow("#upd  >    123   aaa   456  ")]
        [DataRow("#laTency    <    100")]
        [DataRow("#upD    >    100")]
        [DataRow("#dOwN   ~    321   45")]
        [DataRow("#port  =    1080")]
        [DataRow("#modify  =   240230")]
        public void CreateAdvNumberFilterTest(string kw)
        {
            var kwf = new KeywordFilter(kw);
            var f = kwf.GetFilter();
            Assert.IsTrue(f is AdvNumberFilter);
        }

        #endregion

        #region adv string filter
        [DataTestMethod]
        [DataRow("#tg")]
        [DataRow("#tag1")]
        [DataRow("#mk")]
        [DataRow("#rk    ")]
        [DataRow("#nAe")]
        [DataRow("#smMY")]
        [DataRow("#tTl    ")]
        public void CreateAdvStringFilterTest(string kw)
        {
            var kwf = new KeywordFilter(kw);
            var f = kwf.GetFilter();
            Assert.IsTrue(f is AdvStringFilter);
        }

        [DataTestMethod]
        [DataRow("#TiLE \"\"", "", StringOperators.LIKE, StringTagNames.Title)]
        [DataRow("#TiLE  hAs  \"\"", "", StringOperators.HAS, StringTagNames.Title)]
        [DataRow("#TiLE", "", StringOperators.LIKE, StringTagNames.Title)]
        [DataRow(
            "#tITle     \" is \" \"\"\" not  \"\"\"  ",
            " is \"not\"",
            StringOperators.LIKE,
            StringTagNames.Title
        )]
        [DataRow("#TiLE \" is \"", " is ", StringOperators.LIKE, StringTagNames.Title)]
        [DataRow(
            "#tg1 not a B ,;😁! cDDE 中文",
            "ab,;😁!cdde中文",
            StringOperators.NOT,
            StringTagNames.Tag1
        )]
        [DataRow("#NaMe  IS Is", "is", StringOperators.IS, StringTagNames.Name)]
        [DataRow(
            "#tg1 not a B ,;😁! cDDE 中文",
            "ab,;😁!cdde中文",
            StringOperators.NOT,
            StringTagNames.Tag1
        )]
        [DataRow("#suMmaRy  hAs", "", StringOperators.HAS, StringTagNames.Summary)]
        [DataRow(
            "#ttl hasnot 测 A 试 1 😊 ",
            "测a试1😊",
            StringOperators.HASNOT,
            StringTagNames.Title
        )]
        [DataRow("#tg2", "", StringOperators.LIKE, StringTagNames.Tag2)]
        [DataRow("#summary has", "", StringOperators.HAS, StringTagNames.Summary)]
        [DataRow(
            "#tg",
            "",
            StringOperators.LIKE,
            StringTagNames.Tag1,
            StringTagNames.Tag2,
            StringTagNames.Tag3
        )]
        [DataRow("#coRe hAs raY", "ray", StringOperators.HAS, StringTagNames.Core)]
        [DataRow("#selt uNlike  fAl", "fal", StringOperators.UNLIKE, StringTagNames.Selected)]
        public void AdvStringFilterParseTagNameAndOpTest(
            string kw,
            string expKw,
            StringOperators expMatchType,
            params StringTagNames[] expContentNames
        )
        {
            var parser = AdvStringFilter.CreateFilter(kw);
            Assert.IsNotNull(parser);
            Assert.AreEqual(expKw, parser.GetParsedKeyword());
            Assert.AreEqual(expMatchType, parser.GetOperator());
            var cnames = parser.GetTagNames();
            Assert.AreEqual(expContentNames.Count(), cnames.Count());
            foreach (var cname in cnames)
            {
                Assert.IsTrue(expContentNames.Contains(cname));
            }
        }

        [DataTestMethod]
        [DataRow("#tITlE   Is   \"\" ", "", true)]
        [DataRow("#tItlE   Is   \"\" ", "   ", false)]
        [DataRow("#tItlE   not   \" \" \" \" \" \" ", "   ", false)]
        [DataRow("#tItlE   not   \"  \" ", "   ", true)]
        [DataRow("#tg not a", "A", false)]
        [DataRow("#tg unlike abc", "abcd", false)]
        [DataRow("#tG  uNlike abc", "bcd", true)]
        [DataRow("#tg unlike  aBc", "bc", true)]
        [DataRow("#tg unlike A cd,😀 文", ";-,a C  D中,😀文,Bc", false)]
        [DataRow("#tg unlike a# ,😀 文", ";-,a 中,😀文,bc", true)]
        [DataRow("#tg unlike a", "", true)]
        [DataRow("#tg unlike ", "aa", true)]
        [DataRow("#tg unlike ", "", false)]
        [DataRow("#tg like a ,BcD😀 文", ";-,a 中, bv C d😀文,bc", true)]
        [DataRow("#tg like a", "", false)]
        [DataRow("#tg has ab", "abc", true)]
        [DataRow("#tg has bc", "abc", true)]
        [DataRow("#tg has abcd", "abc", false)]
        [DataRow("#tg has ac", "abc", false)]
        [DataRow("#tg has e", "abc", false)]
        [DataRow("#tg has ", "a", true)]
        [DataRow("#tg has ", "", true)]
        [DataRow("#tg hasnot ab", "abc", false)]
        [DataRow("#tTl haSnot bc", "abc", false)]
        [DataRow("#tg hasnot abcd", "abc", true)]
        [DataRow("#tg hasnot ac", "abc", true)]
        [DataRow("#nm hasnot e", "abc", true)]
        [DataRow("#tg hasnot ", "a", true)]
        [DataRow("#tg hasnot ", "", false)]
        [DataRow("#tg nOt ", "a", true)]
        [DataRow("#tg not a", "a", false)]
        [DataRow("#tg not   ", "", false)]
        [DataRow("#tg Is a", "a", true)]
        [DataRow("#tg is a", "aa", false)]
        [DataRow("#tg is a", "A", true)]
        [DataRow("#tg is A", "a", true)]
        [DataRow("#tg is a", "", false)]
        [DataRow("#tg is is", "is", true)]
        [DataRow("#tg is", "", true)]
        [DataRow("#tg is", "aaa", false)]
        [DataRow("#smm  a ,😀 文", ";-,a 中,😀文,bc", true)]
        [DataRow("#tg a", "", false)]
        [DataRow("#tg ", "aa", true)]
        [DataRow("#tg ", "", true)]
        [DataRow("#tg like a", "a", true)]
        [DataRow("#tg a", "a", true)]
        [DataRow("#tg like ", "aa", true)]
        [DataRow("#tg like ", "", true)]
        [DataRow("#tg", "", true)]
        public void AdvStringFilterMatchCoreTest(string keyword, string content, bool exp)
        {
            var kws = AdvStringFilter.CreateFilter(keyword);
            Assert.IsNotNull(kws);
            Assert.AreEqual(exp, kws.MatchCore(content));
        }

        #endregion

        #region simple title filter
        [DataTestMethod]
        [DataRow(" a ,😀 文", ";-,a 中,😀文,bc", true)]
        [DataRow("a", "", false)]
        [DataRow("", "aa", true)]
        [DataRow("    ", "", true)]
        [DataRow("a", "a", true)]
        [DataRow("AaaAa", "aaaaa", true)]
        [DataRow("", "", true)]
        [DataRow("", "aaa", true)]
        [DataRow("##23", "#1234", true)]
        [DataRow("##123", "#12", false)]
        [DataRow("##1A2b3", "#1a2b3", true)]
        [DataRow("##123", "#123", true)]
        [DataRow("####", "#1#2##3##", true)]
        [DataRow("###", "#", false)]
        [DataRow("##", "#", true)]
        [DataRow("##", "aaa", false)]
        public void SimpleTitleFilterMatchTest(string keyword, string content, bool exp)
        {
            var f = SimpleTitleFilter.CreateFilter(keyword);
            Assert.IsNotNull(f);
            Assert.AreEqual(exp, f.MatchCore(content));
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
        public void SimpleTitleFilterParseKeywordTest(string kw, string expKw)
        {
            var kws = SimpleTitleFilter.CreateFilter(kw);
            Assert.IsNotNull(kws);
            Assert.AreEqual(expKw, kws.GetParsedKeyword());
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("1")]
        [DataRow("##")]
        [DataRow("a B 中 文 1😀😀")]
        [DataRow("##a B 中 文 1😀😀")]
        public void CreateSimpleTitleFilterTest(string kw)
        {
            var kwf = new KeywordFilter(kw);
            var f = kwf.GetFilter();
            Assert.IsTrue(f is SimpleTitleFilter);
        }

        #endregion

        #region simple index filter

        [DataTestMethod]
        [DataRow("#0", 0)]
        [DataRow("#", 0)]
        [DataRow("#-1", -1)]
        [DataRow("#-123", -123)]
        [DataRow("#123", 123)]
        [DataRow("#00000123", 123)]
        [DataRow("#00000", 0)]
        public void SimpleIndexFilterTest(string kw, int expIndex)
        {
            var f = SimpleIndexFilter.CreateFilter(kw);
            Assert.IsNotNull(f);
            var index = f.GetIndex();
            Assert.AreEqual(expIndex, index);
        }

        [DataTestMethod]
        [DataRow("#", 0)]
        [DataRow("#0", 0)]
        [DataRow("#1", 1)]
        [DataRow("#001", 1)]
        [DataRow("#-11", -11)]
        [DataRow("#-011", -11)]
        public void CreateSimpleIndexFilterSuccessTest(string kw, int expIndex)
        {
            var kwf = new KeywordFilter(kw);
            var f = kwf.GetFilter();
            var sif = f as SimpleIndexFilter;
            Assert.IsNotNull(sif);
            Assert.AreEqual(expIndex, sif.GetIndex());
        }

        [DataTestMethod]
        [DataRow("#1a")]
        [DataRow("#1.0")]
        [DataRow("#rkz")]
        [DataRow("#latency")]
        [DataRow("#upd")]
        [DataRow("#doWn")]
        [DataRow("#ltny < ")]
        [DataRow("#ltny ~ 123")]
        [DataRow("#ltny # 123")]
        [DataRow("#upd > aaa")]
        [DataRow("#down ~ 12")]
        public void CreateSimpleIndexFilterFailTest(string kw)
        {
            var kwf = new KeywordFilter(kw);
            var f = kwf.GetFilter();
            Assert.IsNull(f);
        }

        #endregion

        #region text parser
        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        public void TextParserCoreEmptyStringTest(string src)
        {
            var r = Helpers.ParseTextCore(src, ' ', '"').ToList();
            Assert.AreEqual(0, r.Count);
        }

        [DataTestMethod]
        [DataRow("    ", "", "", "", "", "")]
        [DataRow(" \"  \" ", "", "  ", "")]
        [DataRow(" \"\"\" ", "", "\"", "")]
        public void TextParserCoreTest(string src, params string[] tokens)
        {
            var r = Helpers.ParseTextCore(src, ' ', '"').ToList();
            Assert.AreEqual(tokens.Length, r.Count);
            for (int i = 0; i < tokens.Length; i++)
            {
                Assert.AreEqual(tokens[i], r[i]);
            }
        }

        [DataTestMethod]
        [DataRow("    ")]
        [DataRow("")]
        [DataRow(null)]
        public void TokenizerEmptyStringTest(string src)
        {
            var tks = Helpers.Tokenize(src);
            Assert.AreEqual(0, tks.Length);
        }

        [DataTestMethod]
        [DataRow("#1 \"引号\"粘连测试\"  ", "#1", "引号\"粘连测试")]
        [DataRow(
            "   \"tag\"   \"not\"  \"test 测试 abc\"   def  ",
            "tag",
            "not",
            "test 测试 abc",
            "def"
        )]
        [DataRow(" \"tag\" \"not\" \"test\"测试 abc\" def ", "tag", "not", "test\"测试 abc", "def")]
        [DataRow(
            "tag is \" t  est 测试\"中文\" 测试   😊   a  b c   \"测试 😊 a b c\" ",
            "tag",
            "is",
            " t  est 测试\"中文",
            "测试",
            "😊",
            "a",
            "b",
            "c",
            "测试 😊 a b c"
        )]
        public void TokenizerTest(string src, params string[] tokens)
        {
            var tks = Helpers.Tokenize(src);
            Assert.AreEqual(tokens.Length, tks.Length);
            for (int i = 0; i < tokens.Length; i++)
            {
                Assert.AreEqual(tokens[i], tks[i]);
            }
        }
        #endregion
    }
}
