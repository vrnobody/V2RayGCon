using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text;

namespace V2RayGCon.Test
{
    [TestClass]
    public class Base64Tests
    {
        string[] normalStrings = new string[] {
            @"",
            @"1234",
            @"abcd",
            @"1中23 文",
            @"中123ac文"
        };

        string GenRandHex() => Lib.Utils.RandomHex(7);

       
        [TestMethod]
        public void NormalEncodingTest()
        {
            foreach (var str in normalStrings)
            {
                var encoded = Lib.Utils.Base64Encode(str);
                var decoded = Lib.Utils.Base64Decode(encoded);
                Assert.AreEqual(str, decoded);
            }

            for (int i = 0; i < 100; i++)
            {
                var str = GenRandHex();
                var encoded = Lib.Utils.Base64Encode(str);
                var decoded = Lib.Utils.Base64Decode(encoded);
                Assert.AreEqual(str, decoded);
            }
        }


    }
}
