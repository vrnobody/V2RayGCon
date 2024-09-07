using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VgcApis.Libs.Infr.KwFilterComps;

namespace VgcApisTests.LibsTests
{
    [TestClass]
    public class BoolExprFilterTests
    {
        #region filter test
        [DataTestMethod]
        [DataRow(
            "#@#smm Ends    .cOM    !     #laTenCy    >     300    &  #sUm     StArtS VLess",
            "AndExpr(NotExpr(LeafExpr(#smm ends .com), LeafExpr(#latency > 300)), LeafExpr(#sum starts vless))"
        )]
        [DataRow(
            "#@ (#smm ends .com) ! (#latency > 300) & (#sum starts vless)",
            "AndExpr(NotExpr(LeafExpr(#smm ends .com), LeafExpr(#latency > 300)), LeafExpr(#sum starts vless))"
        )]
        [DataRow(
            "#@ (   #mK Is a   ) &(   (   #sMm noT b)   |   (#ttl liKe C  ))",
            "AndExpr(LeafExpr(#mk is a), OrExpr(LeafExpr(#smm not b), LeafExpr(#ttl like c)))"
        )]
        [DataRow(
            "#@ (#mk is \"\")&(#smm not \"a b c\")",
            "AndExpr(LeafExpr(#mk is), LeafExpr(#smm not a b c))"
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
            " (   #mk is a   ) !(      #smm not b)   &   (#ttl like c  ) ",
            "AndExpr(NotExpr(LeafExpr(#mk is a), LeafExpr(#smm not b)), LeafExpr(#ttl like c))"
        )]
        [DataRow(
            "(   #mk is a   ) &(   (   #smm not b)   |   (#ttl like c  ))",
            "AndExpr(LeafExpr(#mk is a), OrExpr(LeafExpr(#smm not b), LeafExpr(#ttl like c)))"
        )]
        [DataRow(
            "(#mk is \"\")&(#smm not \"a b c\")",
            "AndExpr(LeafExpr(#mk is), LeafExpr(#smm not a b c))"
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

        #region tokenizer test
        [DataTestMethod]
        [DataRow(
            "#@ #mk is \"(\" \")\" \"|\" \"&\" \" \"",
            " ",
            "&",
            "|",
            ")",
            "(",
            "is",
            "#mk",
            "#@"
        )]
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
            "#@ !(#mk is \" \")|!()&()",
            "#@",
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
            "a ) (b c"
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
