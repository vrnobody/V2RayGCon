using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace VgcApisTests
{
    [TestClass]
    public class BitStreamTests
    {
        VgcApis.Libs.Streams.BitStream bs;

        public BitStreamTests()
        {
            bs = new VgcApis.Libs.Streams.BitStream();
        }

        [TestMethod]
        public void Crc8ChecksumFailTest()
        {
            var bs1 = new VgcApis.Libs.Streams.BitStream();
            bs1.Write(12345);
            bs1.WriteAddress("abc.com");

            var b1 = bs1.ToBytes("2b");
            b1[0] = (byte)(b1[0]+1);
            try
            {
                var b2 = new VgcApis.Libs.Streams.BitStream(b1);
            }
            catch (ArgumentException)
            {
                return;
            }
            Assert.Fail();
        }

        [TestMethod]
        public void BsNormalTest()
        {
            var bs1 = new VgcApis.Libs.Streams.BitStream();
            var uuid = Guid.NewGuid();
            bs1.Write(true);
            bs1.Write(12345);
            bs1.Write(uuid);
            bs1.WriteAddress("abc.com");
            bs1.WriteAddress("::1");
            bs1.WriteAddress("1.2.3.4");
            bs1.Write("123");
            bs1.Write("1中23文");
            var b1 = bs1.ToBytes("1a");
            bs1.Dispose();
            var bs2 = new VgcApis.Libs.Streams.BitStream(b1);
            var b2 = bs2.ToBytes("1a");
            bs2.Dispose();

            for (int i = 0; i < b1.Length; i++)
            {
                Assert.AreEqual(b2[i], b1[i]);
            }

        }

        [TestMethod]
        public void ShortIntTest()
        {
            var rand = new Random();
            for (int i = 0; i < 10; i++)
            {
                bs.Clear();
                const int len = 7;
                var source = new List<int>();
                for (int j = 0; j < 3; j++)
                {
                    var val = rand.Next(127);
                    source.Add(val);
                    bs.WriteTinyInt(val, len);
                }

                for (int j = 0; j < 3; j++)
                {
                    var read = bs.ReadTinyInt(len);
                    Assert.AreEqual(source[j], read);
                }
            }
        }

        [DataTestMethod]
        [DataRow("abc.com")]
        [DataRow("1.2.3.4")]
        [DataRow("::1")]
        [DataRow("2001:4860:4860::8888")]
        public void AddressTest(string str)
        {
            bs.Clear();
            bs.WriteAddress(str);
            var result = bs.ReadAddress();
            Assert.AreEqual(str, result);
        }

        [DataTestMethod]
        [DataRow(@"")]
        [DataRow(@"123abcAbc")]
        [DataRow(@"abcd1234中文")]
        [DataRow(@"中文abc1{23+}-./")]
        [DataRow(@"a中文abc1{23+}-./")]
        public void StringTest(string str)
        {
            bs.Clear();
            bs.Write(str);
            var result = bs.Read<string>();
            Assert.AreEqual(str, result);
        }

        [TestMethod]
        public void GuidTest()
        {
            for (int i = 0; i < 10; i++)
            {
                var uuid = Guid.NewGuid();
                bs.Clear();
                bs.Write(uuid);
                var result = bs.Read<Guid>();
                Assert.AreEqual(uuid, result);
            }
        }

        [TestMethod]
        public void WriteIntTest()
        {
            for (int i = 0; i < 65535; i += 600)
            {
                bs.Clear();
                bs.Write(i);
                var result = bs.Read<int>();
                Assert.AreEqual(i, result);
            }
        }

        [TestMethod]
        public void WriteBoolTest()
        {
            bs.Clear();
            bs.Write(true);
            bs.Write(false);
            var r1 = bs.Read<bool>();
            var r2 = bs.Read<bool>();
            Assert.AreEqual(true, r1);
            Assert.AreEqual(false, r2);
        }
    }
}
