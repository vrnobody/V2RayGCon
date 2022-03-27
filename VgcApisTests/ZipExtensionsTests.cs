using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VgcApis.Libs.Infr;

namespace VgcApisTests
{
    [TestClass]
    public class ZipExtensionsTests
    {
        public ZipExtensionsTests()
        { }

        [TestMethod]
        public void ZipSerdeTest()
        {
            var s1 = new List<int> { 1, 2, 3 };
            var b1 = ZipExtensions.SerializeObjectToCompressedBase64(s1);
            var t1 = ZipExtensions.DeserializeObjectFromCompressedBase64<List<int>>(b1);
            Assert.IsTrue(s1.SequenceEqual(t1));

            var s2 = new List<string> { "123", "abc", "123中文AbcZ", "", null };
            var b2 = ZipExtensions.SerializeObjectToCompressedBase64(s2);
            var t2 = ZipExtensions.DeserializeObjectFromCompressedBase64<List<string>>(b2);
            Assert.IsTrue(s2.SequenceEqual(t2));

            object es1 = null;
            var eb1 = ZipExtensions.SerializeObjectToCompressedBase64(es1);
            var et1 = ZipExtensions.DeserializeObjectFromCompressedBase64<object>(eb1);
            Assert.AreEqual(es1, et1);
        }

        [DataTestMethod]
        [DataRow("hello, world!")]
        [DataRow("he中llo780, wo文rld!123")]
        [DataRow("")]
        public void ZipBase64StringTest(string s)
        {
            var cs = VgcApis.Libs.Infr.ZipExtensions.CompressToBase64(s);
            var de = VgcApis.Libs.Infr.ZipExtensions.DecompressFromBase64(cs);

            Assert.AreEqual(s, de);
        }

        [DataTestMethod]
        [DataRow("hello, world!")]
        [DataRow("he中llo780, wo文rld!123")]
        [DataRow("")]
        public void ZipByteTest(string s)
        {
            var bytes = Encoding.UTF8.GetBytes(s);
            var cb = VgcApis.Libs.Infr.ZipExtensions.Compress(bytes);
            var de = VgcApis.Libs.Infr.ZipExtensions.Decompress(cb);

            Assert.IsTrue(bytes.SequenceEqual(de));

        }

    }
}
