using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VgcApis.Libs.Infr.KwFilterComps;

namespace VgcApisTests.LibsTests
{
    [TestClass]
    public class BoolExprFilterTests
    {
        #region helper funcs
        [DataTestMethod]
        [DataRow(null, false)]
        [DataRow("", false)]
        [DataRow("#maRk", true)]
        [DataRow("   ", false)]
        [DataRow("#", false)]
        [DataRow("##abcde", false)]
        [DataRow("#0123456", false)]
        [DataRow("(#TaG ", true)]
        [DataRow("       #TaG ", true)]
        [DataRow("  (     #TaG ", true)]
        [DataRow("(((((#lat", true)]
        [DataRow(" (((( ( # lat", false)]
        [DataRow(" ( ( (  ( ( #lat", true)]
        public void IsAdvSearchKeywordTest(string kw, bool exp)
        {
            var r = Helpers.IsAdvSearchKeyword(kw);
            Assert.AreEqual(exp, r);
        }
        #endregion

        #region filter test
        [DataTestMethod]
        [DataRow(
            "#smm Ends    .cOM   &    #oRdB -idx  &     #laTenCy    >     300  ",
            "AndExpr@7(AndExpr@3(LeafExpr@2(#smm Ends .cOM), LeafExpr@1(#oRdB -idx)), LeafExpr@4(#laTenCy > 300))"
        )]
        [DataRow(
            " (#smm mAtCh \"#!&|.cOM$\") ! (#lAt > 300) & (#sum staRts vLeSS)",
            "AndExpr@8(NotExpr@6(LeafExpr@2(#smm mAtCh #!&|.cOM$), LeafExpr@4(#lAt > 300)), LeafExpr@2(#sum staRts vLeSS))"
        )]
        [DataRow(
            "#smm Ends    .cOM    !     #laTenCy    >     300    &  #sUm     StArtS VLess",
            "AndExpr@8(NotExpr@6(LeafExpr@2(#smm Ends .cOM), LeafExpr@4(#laTenCy > 300)), LeafExpr@2(#sUm StArtS VLess))"
        )]
        [DataRow(
            " (#smm ends .com) ! (#latency > 300) & (#sum starts vless)",
            "AndExpr@8(NotExpr@6(LeafExpr@2(#smm ends .com), LeafExpr@4(#latency > 300)), LeafExpr@2(#sum starts vless))"
        )]
        [DataRow(
            " (   #mK Is a   ) &(   (   #sMm noT b)   |   (#ttl liKe C  ))",
            "AndExpr@6(LeafExpr@2(#mK Is a), OrExpr@4(LeafExpr@2(#sMm noT b), LeafExpr@2(#ttl liKe C)))"
        )]
        [DataRow(
            " (#mk is \"\")&(#smm not \"a b c\")",
            "AndExpr@4(LeafExpr@2(#mk is), LeafExpr@2(#smm not a b c))"
        )]
        public void CreateFilterTest(string src, string exp)
        {
            var filter = BoolExprFilter.CreateFilter(src);
            Assert.IsNotNull(filter);
            var expr = filter.ToString();
            Assert.AreEqual(exp, expr);
        }
        #endregion

        #region make expression test
        [DataTestMethod]
        [DataRow(
            " (      #smm not b)!|& ",
            "AndExpr@26(OrExpr@18(NotExpr@10(LeafExpr@2(#smm not b), Null), Null), Null)"
        )]
        [DataRow(
            " &|!(      #smm not b) ",
            "NotExpr@26(OrExpr@18(AndExpr@10(LeafExpr@2(#smm not b), Null), Null), Null)"
        )]
        [DataRow(
            " (   #mk is a   ) !(      #smm not b)   &   (#ttl like c  ) ",
            "AndExpr@6(NotExpr@4(LeafExpr@2(#mk is a), LeafExpr@2(#smm not b)), LeafExpr@2(#ttl like c))"
        )]
        [DataRow(
            "(   #mk is a   ) &(   (   #smm not b)   |   (#ttl like c  ))",
            "AndExpr@6(LeafExpr@2(#mk is a), OrExpr@4(LeafExpr@2(#smm not b), LeafExpr@2(#ttl like c)))"
        )]
        [DataRow(
            "(#mk is \"\")&(#smm not \"a b c\")",
            "AndExpr@4(LeafExpr@2(#mk is), LeafExpr@2(#smm not a b c))"
        )]
        public void MakeExpressionTest(string src, string exp)
        {
            var tokens = Helpers.ParseExprToken(src);
            var pe = Helpers.TransformToPolishNotation(tokens);
            var em = pe.GetEnumerator();
            em.MoveNext();
            var expr = Helpers.MakeExpr(ref em);
            Assert.IsNotNull(expr);
            var r = expr.ToString();
            Assert.AreEqual(exp, r);
        }
        #endregion

        #region transform tokens test
        [DataTestMethod]
        [DataRow(
            "(   #mk is a   ) &(!   (   #smm not b)   |   (#ttl like c  ))",
            "&",
            "#mk is a",
            "|",
            "!",
            "#smm not b",
            "#ttl like c"
        )]
        [DataRow("(#MK iS \"\")&(#sMm noT \"a b c\")", "&", "#MK iS", "#sMm noT a b c")]
        public void TransformToPolishNotationTest(string src, params string[] tokens)
        {
            var tmp = Helpers.ParseExprToken(src);
            var pe = Helpers.TransformToPolishNotation(tmp);
            var tks = pe.Select(list => string.Join(" ", list.Select(el => el.value))).ToList();
            Assert.AreEqual(tokens.Length, tks.Count);
            for (int i = 0; i < tokens.Length; i++)
            {
                Assert.AreEqual(tokens[i], tks[i]);
            }
        }
        #endregion

        #region PatchParentheses test
        [DataTestMethod]
        [DataRow(")()()(((", "()()()((()))")]
        [DataRow("()))", "((()))")]
        [DataRow("()", "()")]
        [DataRow(null, null)]
        [DataRow("", "")]
        public void PatchParenthesesTest(string src, string exp)
        {
            var exprSrc = Helpers.ParseExprToken(src);
            var exprExp = Helpers.ParseExprToken(exp);

            Assert.AreEqual(exprExp.Count, exprSrc.Count);
            Assert.AreEqual(ExprTokenTypes.EXPR_END, exprSrc.Last().type);
            Assert.AreEqual(ExprTokenTypes.EXPR_END, exprExp.Last().type);

            for (int i = 0; i < exprExp.Count; i++)
            {
                Assert.AreEqual(exprExp[i].type, exprSrc[i].type);
                Assert.AreEqual(exprExp[i].value, exprSrc[i].value);
            }
        }
        #endregion

        #region tokenizer test
        [DataTestMethod]
        [DataRow(" #mk is \"(\" \")\" \"|\" \"&\" \" \"", " ", "&", "|", ")", "(", "is", "#mk")]
        public void TokenizeStringTest(string str, params string[] tokens)
        {
            var tks = Helpers.ParseExprToken(str);

            Assert.AreEqual(tokens.Length + 1, tks.Count);
            for (int i = 0; i < tokens.Length; i++)
            {
                var exp = tokens[i];
                var v = tks[i];
                Assert.AreEqual(exp, v.value);
                Assert.AreEqual(ExprTokenTypes.LITERAL, v.type);
            }
            Assert.AreEqual(ExprTokenTypes.EXPR_END, tks.Last().type);
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("    ")]
        public void TokenizeEmptyStringTest(string str)
        {
            var tokens = Helpers.ParseExprToken(str);
            Assert.AreEqual(1, tokens.Count);
            Assert.AreEqual(ExprTokenTypes.EXPR_END, tokens[0].type);
        }

        [DataTestMethod]
        [DataRow(
            " !(#mk is \" \")|!()&()",
            "!",
            "(",
            "#mk",
            "is",
            " ",
            ")",
            "|",
            "!",
            "(",
            ")",
            "&",
            "(",
            ")"
        )]
        [DataRow(
            " ( #mk     is     \" \"     )    &   (    #smm   not    \"a ) (b c\"",
            "(",
            "#mk",
            "is",
            " ",
            ")",
            "&",
            "(",
            "#smm",
            "not",
            "a ) (b c",
            ")"
        )]
        [DataRow(
            "(#mk is \"\")&(#smm not \"a b c\")",
            "(",
            "#mk",
            "is",
            // "", empty string is ignored!
            ")",
            "&",
            "(",
            "#smm",
            "not",
            "a b c",
            ")"
        )]
        public void TokenizeExpressionTest(string src, params string[] tokens)
        {
            var len = tokens.Length;
            var table = Helpers.exprOperators;
            var tks = Helpers.ParseExprToken(src);
            Assert.AreEqual(len + 1, tks.Count);
            Assert.AreEqual(ExprTokenTypes.EXPR_END, tks.Last().type);
            for (int i = 0; i < tokens.Length; i++)
            {
                var exp = tokens[len - i - 1]; // reverse order
                Assert.AreEqual(exp, tks[i].value);
                if (exp.Length > 0 && table.ContainsKey(exp[0]))
                {
                    Assert.AreEqual(table[exp[0]].Key, tks[i].type);
                }
            }
        }
        #endregion
    }
}
