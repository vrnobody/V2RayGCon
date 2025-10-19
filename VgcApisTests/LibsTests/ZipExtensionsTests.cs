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
        public void IsZipBase64StringTest()
        {
            var c =
                "中文测试😀😁😂🤣😃😄😅😆"
                + "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789"
                + "/!&#^?,(*'<[.~$:_\"{`%;)|+>=-\\}@]";

            for (int i = 0; i < 30; i++)
            {
                var s = VgcApis.Misc.Utils.PickRandomChars(c, 20);
                var z = ZipExtensions.ZstdToBase64(s);
                var g = ZipExtensions.CompressToBase64(s);
                Assert.IsTrue(ZipExtensions.IsZstdBase64(z));
                Assert.IsFalse(ZipExtensions.IsCompressedBase64(z));
                Assert.IsFalse(ZipExtensions.IsZstdBase64(g));
                Assert.IsTrue(ZipExtensions.IsCompressedBase64(g));
            }
        }

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

            // GZip
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

            // ZSTD
            dest = new VgcApis.Libs.Streams.ArrayPoolMemoryStream(Encoding.ASCII);
            using (var w = new StreamWriter(dest))
            {
                ZipExtensions.SerializeObjectAsZstdBase64ToStream(dest, o1);
                w.Write(",");
                w.Flush();
                ZipExtensions.SerializeObjectAsZstdBase64ToStream(dest, o2);
            }
            dest.Dispose();
            str = dest.GetString();
            Assert.IsFalse(string.IsNullOrEmpty(str));
            parts = str.Split(',');
            Assert.AreEqual(2, parts.Length);

            r1 = ZipExtensions.DeserializeObjectFromZstdBase64<Dictionary<string, List<string>>>(
                parts[0]
            );
            CompareDicts(o1, r1);

            r2 = ZipExtensions.DeserializeObjectFromZstdBase64<Dictionary<string, List<string>>>(
                parts[1]
            );
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

        [TestMethod]
        public void NullStringTest()
        {
            var rnull = ZipExtensions.CompressToBase64(null);
            Assert.AreEqual("", rnull);

            rnull = ZipExtensions.ZstdToBase64(null);
            Assert.AreEqual("", rnull);

            rnull = ZipExtensions.DecompressFromBase64(null);
            Assert.AreEqual("", rnull);

            rnull = ZipExtensions.ZstdFromBase64(null);
            Assert.AreEqual("", rnull);

            rnull = ZipExtensions.SerializeObjectToCompressedUnicodeBase64(null);
            Assert.AreEqual("", rnull);

            rnull = ZipExtensions.SerializeObjectToZstdBase64(null);
            Assert.AreEqual("", rnull);

            var onull = ZipExtensions.DeserializeObjectFromCompressedUnicodeBase64<object>(null);
            Assert.AreEqual(null, onull);

            onull = ZipExtensions.DeserializeObjectFromZstdBase64<object>(null);
            Assert.AreEqual(null, onull);

            using (var s = new MemoryStream())
            {
                ZipExtensions.SerializeObjectAsCompressedUnicodeBase64ToStream(s, null);
                Assert.AreEqual(0, s.Length);
            }

            using (var s = new MemoryStream())
            {
                ZipExtensions.SerializeObjectAsZstdBase64ToStream(s, null);
                Assert.AreEqual(0, s.Length);
            }
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

            ser = ZipExtensions.SerializeObjectToZstdBase64(s);
            de = ZipExtensions.DeserializeObjectFromZstdBase64<string>(ser);
            Assert.AreEqual(s, de);
        }

        [TestMethod]
        public void ZipSerdeTest()
        {
            var s1 = new List<int> { 1, 2, 3 };
            var b1 = ZipExtensions.SerializeObjectToCompressedUnicodeBase64(s1);
            var t1 = ZipExtensions.DeserializeObjectFromCompressedUnicodeBase64<List<int>>(b1);
            Assert.IsTrue(s1.SequenceEqual(t1));

            b1 = ZipExtensions.SerializeObjectToZstdBase64(s1);
            t1 = ZipExtensions.DeserializeObjectFromZstdBase64<List<int>>(b1);
            Assert.IsTrue(s1.SequenceEqual(t1));

            var s2 = new List<string>
            {
                "123",
                "abc",
                "123中文AbcZ",
                "😀😀😣👨‍🦰🎗🥙🛴❣",
                "",
                null,
            };
            var b2 = ZipExtensions.SerializeObjectToCompressedUnicodeBase64(s2);
            var t2 = ZipExtensions.DeserializeObjectFromCompressedUnicodeBase64<List<string>>(b2);
            Assert.IsTrue(s2.SequenceEqual(t2));

            b2 = ZipExtensions.SerializeObjectToZstdBase64(s2);
            t2 = ZipExtensions.DeserializeObjectFromZstdBase64<List<string>>(b2);
            Assert.IsTrue(s2.SequenceEqual(t2));

            object es1 = null;
            var eb1 = ZipExtensions.SerializeObjectToCompressedUnicodeBase64(es1);
            var et1 = ZipExtensions.DeserializeObjectFromCompressedUnicodeBase64<object>(eb1);
            Assert.AreEqual(es1, et1);

            eb1 = ZipExtensions.SerializeObjectToZstdBase64(es1);
            et1 = ZipExtensions.DeserializeObjectFromZstdBase64<object>(eb1);
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

            cs = ZipExtensions.ZstdToBase64(s);
            de = ZipExtensions.ZstdFromBase64(cs);
            Assert.AreEqual(s, de);
        }
    }
}
