using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace VgcApisTests
{
    [TestClass]
    public class Base64Tests
    {
        string[] normalStrings = new string[] {
            @"1234",
            @"abcd",
            @"1中23 文",
            @"中123ac文"
        };

        string GenRandHex() => VgcApis.Misc.Utils.RandomHex(7);

        [DataTestMethod]
        [DataRow(@"", @"")]
        [DataRow(null, null)]
        public void EmptyStringEncodingTest(string src, string expect)
        {
            var encoded = VgcApis.Misc.Utils.Base64EncodeString(src);
            Assert.AreEqual(expect, encoded);
        }

        [TestMethod]
        public void NormalEncodingTest()
        {

            foreach (var str in normalStrings)
            {
                var encoded = VgcApis.Misc.Utils.Base64EncodeString(str);
                var decoded = VgcApis.Misc.Utils.Base64DecodeToString(encoded);
                Assert.AreEqual(str, decoded);
            }

            for (int i = 0; i < 100; i++)
            {
                var str = GenRandHex();
                var encoded = VgcApis.Misc.Utils.Base64EncodeString(str);
                var decoded = VgcApis.Misc.Utils.Base64DecodeToString(encoded);
                Assert.AreEqual(str, decoded);
            }
        }


    }
}
