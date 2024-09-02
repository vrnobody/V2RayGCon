using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VgcApis.Libs.Infr;
using VgcApis.Libs.Infr.KwParserComps;

namespace VgcApisTests.LibsTests
{
    [TestClass]
    public class KeywordFilterTests
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
        public void AdvNumberFilterMatchCoreTest(string keyword, long value, bool expResult)
        {
            var np = AdvNumberFilter.CreateFilter(keyword);
            Assert.IsNotNull(np);
            Assert.AreEqual(expResult, np.MatchCore(value));
        }

        [DataTestMethod]
        [DataRow("#tg not a", "A", false)]
        [DataRow("#tg unlike abc", "abcd", false)]
        [DataRow("#tg unlike abc", "bcd", true)]
        [DataRow("#tg unlike abc", "bc", true)]
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
        [DataRow("#ttl hasnot bc", "abc", false)]
        [DataRow("#tg hasnot abcd", "abc", true)]
        [DataRow("#tg hasnot ac", "abc", true)]
        [DataRow("#nm hasnot e", "abc", true)]
        [DataRow("#tg hasnot ", "a", true)]
        [DataRow("#tg hasnot ", "", false)]
        [DataRow("#tg not ", "a", true)]
        [DataRow("#tg not a", "a", false)]
        [DataRow("#tg not   ", "", false)]
        [DataRow("#tg is a", "a", true)]
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
        public void SimpleTitleFilterMatchTest(string keyword, string content, bool exp)
        {
            var f = SimpleTitleFilter.CreateFilter(keyword);
            Assert.IsNotNull(f);
            Assert.AreEqual(exp, f.MatchCore(content));
        }

        [DataTestMethod]
        [DataRow(
            "#upd = 123 456 aaa",
            KeywordFilter.NumberTagNames.Upload,
            KeywordFilter.NumberOperators.Is
        )]
        [DataRow(
            "#upd ! 123 456 aaa",
            KeywordFilter.NumberTagNames.Upload,
            KeywordFilter.NumberOperators.Not
        )]
        [DataRow(
            "#upd ~ 123 123 aaa",
            KeywordFilter.NumberTagNames.Upload,
            KeywordFilter.NumberOperators.Between
        )]
        [DataRow(
            "#upd ~ 123 456 aaa",
            KeywordFilter.NumberTagNames.Upload,
            KeywordFilter.NumberOperators.Between
        )]
        [DataRow(
            "#download > 0000123 bbbb",
            KeywordFilter.NumberTagNames.Download,
            KeywordFilter.NumberOperators.LargerThen
        )]
        [DataRow(
            "#lat < 123",
            KeywordFilter.NumberTagNames.Latency,
            KeywordFilter.NumberOperators.SmallerThen
        )]
        public void AdvNumberFilterParseTagNameAndOperatorTest(
            string kw,
            KeywordFilter.NumberTagNames expContentName,
            KeywordFilter.NumberOperators expOp
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
        [DataRow(
            "#name is is",
            "is",
            KeywordFilter.StringOperators.IS,
            KeywordFilter.StringTagNames.Name
        )]
        [DataRow(
            "#tg1 not a B ,;😁! cDDE 中文",
            "ab,;😁!cdde中文",
            KeywordFilter.StringOperators.NOT,
            KeywordFilter.StringTagNames.Tag1
        )]
        [DataRow(
            "#summary has",
            "",
            KeywordFilter.StringOperators.HAS,
            KeywordFilter.StringTagNames.Summary
        )]
        [DataRow(
            "#ttl hasnot 测 A 试 1 😊 ",
            "测a试1😊",
            KeywordFilter.StringOperators.HASNOT,
            KeywordFilter.StringTagNames.Title
        )]
        [DataRow("#tg2", "", KeywordFilter.StringOperators.LIKE, KeywordFilter.StringTagNames.Tag2)]
        [DataRow(
            "#summary has",
            "",
            KeywordFilter.StringOperators.HAS,
            KeywordFilter.StringTagNames.Summary
        )]
        [DataRow(
            "#tg",
            "",
            KeywordFilter.StringOperators.LIKE,
            KeywordFilter.StringTagNames.Tag1,
            KeywordFilter.StringTagNames.Tag2,
            KeywordFilter.StringTagNames.Tag3
        )]
        public void AdvStringFilterParseTagNameAndOpTest(
            string kw,
            string expKw,
            KeywordFilter.StringOperators expMatchType,
            params KeywordFilter.StringTagNames[] expContentNames
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

        [DataTestMethod]
        [DataRow("#tg")]
        [DataRow("#tag1")]
        [DataRow("#mk")]
        [DataRow("#rk")]
        [DataRow("#name")]
        [DataRow("#smm")]
        [DataRow("#ttl")]
        public void CreateAdvStringFilterTest(string kw)
        {
            var kwf = new KeywordFilter(kw);
            var f = kwf.GetFilter();
            Assert.IsTrue(f is AdvStringFilter);
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
        [DataRow("#down")]
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

        [DataTestMethod]
        [DataRow("#laty = 13")]
        [DataRow("#latency ! 145")]
        [DataRow("#upd > 123 aaa 456 ")]
        [DataRow("#latency < 100")]
        [DataRow("#upd > 100")]
        [DataRow("#down ~ 321 45")]
        [DataRow("#laty     =    13")]
        [DataRow("#latency    !   145   ")]
        [DataRow("#upd  >    123   aaa   456  ")]
        [DataRow("#latency    <    100")]
        [DataRow("#upd    >    100")]
        [DataRow("#down   ~    321   45")]
        public void CreateAdvNumberFilterTest(string kw)
        {
            var kwf = new KeywordFilter(kw);
            var f = kwf.GetFilter();
            Assert.IsTrue(f is AdvNumberFilter);
        }
    }
}
