using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VgcApis.Libs.Infr;
using VgcApis.Libs.Infr.KwFilterComps;

namespace VgcApisTests.LibsTests
{
    [TestClass]
    public class KeywordFilterTests
    {
        #region adv number filter
        [DataTestMethod]
        [DataRow("#MOd  <  02", 01, true)]
        [DataRow("#moD  >  02", 02, false)]
        [DataRow("#moD  <  02", 03, false)]
        [DataRow("#moD  =  02", 03, false)]
        [DataRow("#moD  =  02", 02, true)]
        [DataRow("#moD  ~  05 01", 03, true)]
        [DataRow("#moD  ~  05 01", 01, true)]
        [DataRow("#moD  ~  05 01", 06, false)]
        // not
        [DataRow("#MOd not <  02", 01, false)]
        [DataRow("#moD  not >  02", 02, true)]
        [DataRow("#moD not <  02", 03, true)]
        [DataRow("#moD not =  02", 03, true)]
        [DataRow("#moD  not  02", 02, false)]
        [DataRow("#moD  not  ~  05 01", 03, false)]
        [DataRow("#moD not ~  05 01", 01, false)]
        [DataRow("#moD not ~  05 01", 06, true)]
        public void AdvNumberFilterTagModifyDayTest(string keyword, long value, bool expResult)
        {
            var np = AdvNumberFilter.CreateFilter(keyword);
            var now = DateTime.UtcNow.ToLocalTime();

            Assert.IsNotNull(np);
            var d = VgcApis.Misc.Utils.Str2Int(now.ToString("yyyyMM")) * 100;
            var r = np.MatchCore(value + d);
            Assert.AreEqual(expResult, r);
        }

        [DataTestMethod]
        [DataRow("#MOd  <  0802", 0801, true)]
        [DataRow("#moD  >  0802", 0802, false)]
        [DataRow("#moD  <  0802", 0803, false)]
        [DataRow("#moD  =  0802", 0803, false)]
        [DataRow("#moD  =  0802", 0802, true)]
        [DataRow("#moD  ~  0805 0801", 0803, true)]
        [DataRow("#moD  ~  0805 0801", 0801, true)]
        [DataRow("#moD  ~  0805 0801", 0806, false)]
        // not
        [DataRow("#MOd not <  0802", 0801, false)]
        [DataRow("#moD  not >  0802", 0802, true)]
        [DataRow("#moD not <  0802", 0803, true)]
        [DataRow("#moD not =  0802", 0803, true)]
        [DataRow("#moD  not  0802", 0802, false)]
        [DataRow("#moD  not  ~  0805 0801", 0803, false)]
        [DataRow("#moD not ~  0805 0801", 0801, false)]
        [DataRow("#moD not ~  0805 0801", 0806, true)]
        public void AdvNumberFilterTagModifyMonthDayTest(string keyword, long value, bool expResult)
        {
            var np = AdvNumberFilter.CreateFilter(keyword);
            var d = DateTime.UtcNow.ToLocalTime().Year * 10000;

            Assert.IsNotNull(np);
            var r = np.MatchCore(value + d);
            Assert.AreEqual(expResult, r);
        }

        [DataTestMethod]
        [DataRow("#MOd  <  240802", 240801, true)]
        [DataRow("#moD  >  240802", 240802, false)]
        [DataRow("#moD  <  240802", 240803, false)]
        [DataRow("#moD  =  240802", 240803, false)]
        [DataRow("#moD  =  240802", 240802, true)]
        [DataRow("#moD  ~  240805 240801", 240803, true)]
        [DataRow("#moD  ~  240805 240801", 240801, true)]
        [DataRow("#moD  ~  240805 240801", 240806, false)]
        // not
        [DataRow("#MOd not <  240802", 240801, false)]
        [DataRow("#moD  not >  240802", 240802, true)]
        [DataRow("#moD not <  240802", 240803, true)]
        [DataRow("#moD not =  240802", 240803, true)]
        [DataRow("#moD  not  240802", 240802, false)]
        [DataRow("#moD  not  ~  240805 240801", 240803, false)]
        [DataRow("#moD not ~  240805 240801", 240801, false)]
        [DataRow("#moD not ~  240805 240801", 240806, true)]
        public void AdvNumberFilterTagModifyYMDTest(string keyword, long value, bool expResult)
        {
            string[] tokens = Helpers.ParseLiteral(keyword.ToLower());
            var np = AdvNumberFilter.CreateFilter(tokens);
            var d = (DateTime.UtcNow.ToLocalTime().Year / 100) * 1000000;

            Assert.IsNotNull(np);
            var r = np.MatchCore(value + d);
            Assert.AreEqual(expResult, r);
        }

        [DataTestMethod]
        // between
        [DataRow("#lten ~ -1 2", -3, false)]
        [DataRow("#lten ~ -1 2", 3, false)]
        [DataRow("#lten ~ -1 2", 1, true)]
        [DataRow("#lten ~ -1 2", 2, true)]
        [DataRow("#LTen not ~ -1 2", -3, true)]
        [DataRow("#lteNc not ~ -1 2", 3, true)]
        [DataRow("#lTeny not ~ -1 2", 1, false)]
        [DataRow("#lten NOt ~ -1 2", 2, false)]
        // equals
        [DataRow("#Upd = -1", -1, true)]
        [DataRow("#upd = -2", 1, false)]
        [DataRow("#Upd not = -1", -1, false)]
        [DataRow("#upd not -2", 1, true)]
        [DataRow("#upd not 1", 1, false)]
        // smaller then
        [DataRow("#lAtency < 200", 123, true)]
        [DataRow("#lAtency < 200", 234, false)]
        [DataRow("#lAtency < 200", 200, false)]
        [DataRow("#lAtency not < 200", 123, false)]
        [DataRow("#lAtency not < 200", 234, true)]
        [DataRow("#lAtency not < 200", 200, true)]
        // larget then
        [DataRow("#upd > -1", 456, true)]
        [DataRow("#down > -1", -456, false)]
        [DataRow("#prt > 1080 ", 1080, false)]
        [DataRow("#uPd nOt > -1", 456, false)]
        [DataRow("#doWn noT > -1", -456, true)]
        [DataRow("#pRt  not > 1080 ", 1080, true)]
        public void AdvNumberFilterMatchCoreTest(string keyword, long value, bool expResult)
        {
            var np = AdvNumberFilter.CreateFilter(keyword);
            Assert.IsNotNull(np);
            Assert.AreEqual(expResult, np.MatchCore(value));
        }

        [DataTestMethod]
        [DataRow("#upd 123 456 aaa", "#(upload) is 123 0")]
        [DataRow("#load = 123 456 aaa", "#(upload, download) is 123 0")]
        [DataRow("#upd = 123 456 aaa", "#(upload) is 123 0")]
        [DataRow("#upD ~ 123 123 aaa", "#(upload) between 123 123")]
        [DataRow("#upD Not ~ 123 123 aaa", "#(upload) not between 123 123")]
        [DataRow("#UPld ~ 123 456 aaa", "#(upload) between 123 456")]
        [DataRow("#doWnload > 0000123 bbbb", "#(download) largerthen 123 0")]
        [DataRow("#doWnload nOT > 0000123 bbbb", "#(download) not largerthen 123 0")]
        [DataRow("#LaT < 123", "#(latency) smallerthen 123 0")]
        [DataRow("#prt = 1080", "#(port) is 1080 0")]
        [DataRow("#prt not 1080", "#(port) not is 1080 0")]
        public void AdvNumberFilterParseTagNameAndOperatorTest(string kw, string exp)
        {
            var parser1 = AdvNumberFilter.CreateFilter(kw);
            Assert.IsNotNull(parser1);
            Assert.AreEqual(exp, parser1.ToString());

            string[] tokens = Helpers.ParseLiteral(kw.ToLower());
            var parser2 = AdvNumberFilter.CreateFilter(tokens);
            Assert.IsNotNull(parser2);
            Assert.AreEqual(exp, parser2.ToString());
        }

        [DataTestMethod]
        [DataRow("#upd")]
        [DataRow("#upd ! 123 456 aaa")]
        [DataRow("#upD @ not 123 123 aaa")]
        [DataRow("#UPld ~ not 123 456 aaa")]
        [DataRow("#doWnload not")]
        public void AdvNumberFilterParserFailTest(string kw)
        {
            var parser1 = AdvNumberFilter.CreateFilter(kw);
            Assert.IsNull(parser1);

            string[] tokens = Helpers.ParseLiteral(kw.ToLower());
            var parser2 = AdvNumberFilter.CreateFilter(tokens);
            Assert.IsNull(parser2);
        }

        #endregion


        #region adv string filter
        [DataTestMethod]
        [DataRow("#tag1 match", "#(tag1) match \"\"")]
        [DataRow("#mArK nOt mAtCh   ", "#(mark) not match \"\"")]
        [DataRow("#mArK nOt mAtCh  aB cD 中文 ", "#(mark) not match \"aBcD中文\"")]
        [DataRow("#tG", "#(tag1, tag2, tag3) like \"\"")]
        [DataRow("#tG nOT", "#(tag1, tag2, tag3) not like \"\"")]
        [DataRow("#tG nOT is", "#(tag1, tag2, tag3) not is \"\"")]
        [DataRow("#TG nOT has 😀 \"a 😀b c\"", "#(tag1, tag2, tag3) not has \"😀a 😀b c\"")]
        [DataRow("#tag1 not", "#(tag1) not like \"\"")]
        [DataRow("#mk", "#(mark, remark) like \"\"")]
        [DataRow("#Mk has ", "#(mark, remark) has \"\"")]
        [DataRow(
            "#Mk not has a😀bc \"1 23 \" 中文 \"\"\"",
            "#(mark, remark) not has \"a😀bc1 23 中文\"\""
        )]
        [DataRow("#rk like a 😀b c d e ", "#(mark, remark) like \"a😀bcde\"")]
        [DataRow("#rk not ends a 😀b c d e ", "#(mark, remark) not ends \"a😀bcde\"")]
        [DataRow("#remark not ends a 😀b c d e ", "#(remark) not ends \"a😀bcde\"")]
        [DataRow("#nAe not em😀pty", "#(name) not like \"em😀pty\"")]
        [DataRow("#smMY starts empt😀y", "#(summary) starts \"empt😀y\"")]
        public void CreateAdvStringFilterSuccessTest(string kw, string exp)
        {
            var parser1 = AdvStringFilter.CreateFilter(kw);
            Assert.IsNotNull(parser1);
            var s1 = parser1.ToString();
            Assert.AreEqual(exp, s1);

            var tokens = Helpers.ParseLiteral(kw.ToLower());
            var parser2 = AdvStringFilter.CreateFilter(tokens);
            Assert.IsNotNull(parser2);
            var s2 = parser1.ToString();
            Assert.AreEqual(exp, s2);
        }

        [DataTestMethod]
        [DataRow("#nMe mAtch \\w+", "abcde", true)]
        [DataRow("#nMe mAtch ^\\d+", "abcde123", false)]
        [DataRow("#nMe mAtch \\d+$", "abcde123", true)]
        [DataRow("#Mark", "", true)]
        [DataRow("#mArk", "abc", true)]
        [DataRow("#mark like a b c", "abcde", true)]
        [DataRow("#tAg like a b e", "abcde", true)]
        [DataRow("#mark like a b f", "abcde", false)]
        [DataRow("#mArk has a b c", "ab", false)]
        [DataRow("#maRk is", "", true)]
        [DataRow("#mark is", "abc", false)]
        [DataRow("#nMe is a b c", "abcde", false)]
        [DataRow("#mar is a b f", "abcde", false)]
        [DataRow("#mark has a b c", "abc", true)]
        [DataRow("#maRk has", "", true)]
        [DataRow("#mark has", "abc", true)]
        [DataRow("#mark has a b c", "abcde", true)]
        [DataRow("#smM has a b f", "abcde", false)]
        [DataRow("#mark has a b c", "ab", false)]
        [DataRow("#rk starts", "", true)]
        [DataRow("#mark starts", "abc", true)]
        [DataRow("#mark starts a b c", "abcde", true)]
        [DataRow("#ttl starts a b f", "abcde", false)]
        [DataRow("#mark starts a b c", "ab", false)]
        [DataRow("#mark ends", "", true)]
        [DataRow("#mark ends", "abc", true)]
        [DataRow("#mark ends a b c", "abcde", false)]
        [DataRow("#mark ends cde", "abcde", true)]
        [DataRow("#mark ends a ", "ab", false)]
        public void CreateAdvStringFilterMatchCoreTest(string kw, string content, bool exp)
        {
            var parser1 = AdvStringFilter.CreateFilter(kw);
            Assert.IsNotNull(parser1);
            Assert.AreEqual(exp, parser1.MatchCore(content));

            var tokens = Helpers.ParseLiteral(kw.ToLower());
            var parser2 = AdvStringFilter.CreateFilter(tokens);
            Assert.IsNotNull(parser2);
            Assert.AreEqual(exp, parser2.MatchCore(content));
        }

        [DataTestMethod]
        [DataRow("#nMe not mAtch \\w+", "abcde", true)]
        [DataRow("#nMe not mAtch ^\\d+", "abcde123", false)]
        [DataRow("#nMe not mAtch \\d+$", "abcde123", true)]
        [DataRow("#Mark not", "", true)]
        [DataRow("#mArk not", "abc", false)]
        [DataRow("#mark not like a b c", "abcde", true)]
        [DataRow("#tAg not like a b e", "abcde", true)]
        [DataRow("#mark not like a b f", "abcde", false)]
        [DataRow("#mArk not has a b c", "ab", false)]
        [DataRow("#maRk not is", "", true)]
        [DataRow("#mark not is", "abc", false)]
        [DataRow("#NamE not is a b c", "abcde", false)]
        [DataRow("#mar not is a b f", "abcde", false)]
        [DataRow("#mark not has a b c", "abc", true)]
        [DataRow("#maRk not has", "", true)]
        [DataRow("#mark not has", "abc", false)]
        [DataRow("#mark not has a b c", "abcde", true)]
        [DataRow("#smM not has a b f", "abcde", false)]
        [DataRow("#mark not has a b c", "ab", false)]
        [DataRow("#rk not starts", "", true)]
        [DataRow("#mark not starts", "abc", true)]
        [DataRow("#mark not starts a b c", "abcde", true)]
        [DataRow("#ttl not starts a b f", "abcde", false)]
        [DataRow("#mark not starts a b c", "ab", false)]
        [DataRow("#mark not ends", "", true)]
        [DataRow("#mark not ends", "abc", true)]
        [DataRow("#mark not ends a b c", "abcde", false)]
        [DataRow("#mark not ends cde", "abcde", true)]
        [DataRow("#mark not ends a ", "ab", false)]
        public void CreateAdvStringFilterMatchCoreNotTest(string kw, string content, bool expNot)
        {
            var exp = !expNot;
            var parser1 = AdvStringFilter.CreateFilter(kw);
            Assert.IsNotNull(parser1);
            Assert.AreEqual(exp, parser1.MatchCore(content));

            var tokens = Helpers.ParseLiteral(kw.ToLower());
            var parser2 = AdvStringFilter.CreateFilter(tokens);
            Assert.IsNotNull(parser2);
            Assert.AreEqual(exp, parser2.MatchCore(content));
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
        public void CreateSimpleIndexFilterFailTest(string kw)
        {
            var kwf = new KeywordFilter(kw);
            var f = kwf.GetFilter();
            Assert.IsNull(f);
        }

        #endregion

        #region tokenizer test

        [DataTestMethod]
        [DataRow("    ")]
        [DataRow("")]
        [DataRow(null)]
        public void TokenizeEmptyStringTest(string src)
        {
            var tks = Helpers.ParseLiteral(src);
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
        public void TokenizeStringTest(string src, params string[] tokens)
        {
            var tks = Helpers.ParseLiteral(src);
            Assert.AreEqual(tokens.Length, tks.Length);
            for (int i = 0; i < tokens.Length; i++)
            {
                Assert.AreEqual(tokens[i], tks[i]);
            }
        }
        #endregion
    }
}
