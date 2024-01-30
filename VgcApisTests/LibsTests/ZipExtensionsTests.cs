using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using VgcApis.Libs.Infr;

namespace VgcApisTests.LibsTests
{
    [TestClass]
    public class ZipExtensionsTests
    {
        public ZipExtensionsTests() { }

        [TestMethod]
        public void SerializeToStreamTest()
        {
            var o1 = new Dictionary<string, List<string>>()
            {
                { "hello", "1,2,3,4,5".Split(',').ToList() },
                { "world", "ab,中文,😀,😂".Split(',').ToList() },
            };
            var o2 = new Dictionary<string, List<string>>()
            {
                { "中文1", "1,2,3,4,5".Split(',').ToList() },
                { "😀,😂", "ab,中文,😀,😂".Split(',').ToList() },
            };

            var dest = new VgcApis.Libs.Streams.ArrayPoolMemoryStream(Encoding.ASCII);
            using (var w = new StreamWriter(dest))
            {
                ZipExtensions.SerializeObjectAsCompressedUnicodeBase64ToStream(dest, o1);
                w.Write(",");
                w.Flush();
                ZipExtensions.SerializeObjectAsCompressedUnicodeBase64ToStream(dest, o2);
            }
            dest.Dispose();
            var str = dest.GetString();
            Assert.IsFalse(string.IsNullOrEmpty(str));
            var parts = str.Split(',');
            Assert.AreEqual(2, parts.Length);

            var r1 = ZipExtensions.DeserializeObjectFromCompressedUnicodeBase64<
                Dictionary<string, List<string>>
            >(parts[0]);
            CompareDicts(o1, r1);

            var r2 = ZipExtensions.DeserializeObjectFromCompressedUnicodeBase64<
                Dictionary<string, List<string>>
            >(parts[1]);
            CompareDicts(o2, r2);
        }

        void CompareDicts(
            Dictionary<string, List<string>> src,
            Dictionary<string, List<string>> dest
        )
        {
            Assert.IsFalse(src == null);
            Assert.IsFalse(dest == null);
            foreach (var kv in src)
            {
                var key = kv.Key;
                Assert.IsTrue(dest.ContainsKey(key));
                var vs = kv.Value;
                var ds = dest[key];
                for (int i = 0; i < vs.Count; i++)
                {
                    Assert.AreEqual(vs[i], ds[i]);
                }
            }
        }

        [DataTestMethod]
        [DataRow(
            "heloworld😀😂中文1",
            "H4sIAAAAAAAEAPv/L4MhlSGHIZ+hHIiLgKwUBtsbDPdsbzDd0/VrTzVkAAAgk1AFIgAAAA=="
        )]
        public void BaseLineTest(string src, string compressed)
        {
            var cr = ZipExtensions.CompressToBase64(src);
            Assert.AreEqual(compressed, cr);

            var dr = ZipExtensions.DecompressFromBase64(compressed);
            Assert.AreEqual(src, dr);
        }

        [DataTestMethod]
        [DataRow("hello, world!")]
        [DataRow("he中llo780, wo文rld!123")]
        [DataRow("😀😀😣👨‍🦰🎗🥙🛴❣")]
        [DataRow("\uD83C")]
        [DataRow("")]
        public void JsonSerdeUnicodeTest(string s)
        {
            var ser = JsonConvert.SerializeObject(s);
            var de = JsonConvert.DeserializeObject(ser);

            Assert.AreEqual(s, de);
        }

        [DataTestMethod]
        [DataRow("\uD83C")]
        public void SerializeUnicodeTest(string s)
        {
            var ser = ZipExtensions.SerializeObjectToCompressedUnicodeBase64(s);
            var de = ZipExtensions.DeserializeObjectFromCompressedUnicodeBase64<string>(ser);
            Assert.AreEqual(s, de);
        }

        [TestMethod]
        public void ZipSerdeTest()
        {
            var s1 = new List<int> { 1, 2, 3 };
            var b1 = ZipExtensions.SerializeObjectToCompressedUnicodeBase64(s1);
            var t1 = ZipExtensions.DeserializeObjectFromCompressedUnicodeBase64<List<int>>(b1);
            Assert.IsTrue(s1.SequenceEqual(t1));

            var s2 = new List<string> { "123", "abc", "123中文AbcZ", "😀😀😣👨‍🦰🎗🥙🛴❣", "", null };
            var b2 = ZipExtensions.SerializeObjectToCompressedUnicodeBase64(s2);
            var t2 = ZipExtensions.DeserializeObjectFromCompressedUnicodeBase64<List<string>>(b2);
            Assert.IsTrue(s2.SequenceEqual(t2));

            object es1 = null;
            var eb1 = ZipExtensions.SerializeObjectToCompressedUnicodeBase64(es1);
            var et1 = ZipExtensions.DeserializeObjectFromCompressedUnicodeBase64<object>(eb1);
            Assert.AreEqual(es1, et1);
        }

        [DataTestMethod]
        [DataRow("hello, world!")]
        [DataRow("he中llo780, wo文rld!123")]
        [DataRow("😀😀😣👨‍🦰🎗🥙🛴❣")]
        [DataRow("\uD83C")]
        [DataRow("")]
        public void ZipBase64StringTest(string s)
        {
            var cs = ZipExtensions.CompressToBase64(s);
            var de = ZipExtensions.DecompressFromBase64(cs);

            Assert.AreEqual(s, de);
        }
    }
}
