using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace VgcApisTests
{
    [TestClass]
    public class RawBitStreamTests
    {
        const int BitsPerByte = VgcApis.Models.Consts.BitStream.BitsPerByte;
        const int MaxStrLenInBits = VgcApis.Models.Consts.BitStream.MaxStringLenInBits;
        const int MaxStrLen = VgcApis.Models.Consts.BitStream.MaxStringLen;
        const int SubVersionIndexInBytes = VgcApis.Models.Consts.BitStream.SubVersionByteIndex;
        const int InfoAreaLenInBytes = VgcApis.Models.Consts.BitStream.InfoAreaLenInBytes;

        VgcApis.Libs.Streams.RawBitStream.RawBitStream bitStream;
        VgcApis.Libs.Streams.RawBitStream.Numbers numbers;
        VgcApis.Libs.Streams.RawBitStream.Uuids uuids;
        VgcApis.Libs.Streams.RawBitStream.Address address;
        VgcApis.Libs.Streams.RawBitStream.Bytes bytesWriter;


        public RawBitStreamTests()
        {
            bitStream = new VgcApis.Libs.Streams.RawBitStream.RawBitStream();
            bitStream.Run();

            numbers = bitStream.GetComponent<VgcApis.Libs.Streams.RawBitStream.Numbers>();
            bytesWriter = bitStream.GetComponent<VgcApis.Libs.Streams.RawBitStream.Bytes>();
            uuids = bitStream.GetComponent<VgcApis.Libs.Streams.RawBitStream.Uuids>();
            address = bitStream.GetComponent<VgcApis.Libs.Streams.RawBitStream.Address>();
        }

        [DataTestMethod]
        [DataRow(@"0a")]
        [DataRow(@"123Z")]
        [DataRow(@"255z")]
        [DataRow(@"200z")]
        public void VersionNormalTest(string version)
        {
            var rand = new Random();
            var lenMark = rand.Next(8);

            var bytes = new byte[InfoAreaLenInBytes + 1];
            bytes[SubVersionIndexInBytes] = (byte)lenMark;

            VgcApis.Libs.Streams.RawBitStream.Utils.WriteVersion(version, bytes);
            var read = VgcApis.Libs.Streams.RawBitStream.Utils.ReadVersion(bytes);
            Assert.AreEqual(version.ToLower(), read);

            int result = bytes[SubVersionIndexInBytes];
            Assert.AreEqual(lenMark, result % 8);
        }

        [DataTestMethod]
        [DataRow(@"00a")]
        [DataRow(@"023Z")]
        public void VersionFailTest2(string version)
        {
            var rand = new Random();
            var lenMark = rand.Next(8);

            var bytes = new byte[InfoAreaLenInBytes + 1];
            bytes[SubVersionIndexInBytes] = (byte)lenMark;

            VgcApis.Libs.Streams.RawBitStream.Utils.WriteVersion(version, bytes);
            var read = VgcApis.Libs.Streams.RawBitStream.Utils.ReadVersion(bytes);
            Assert.AreNotEqual(version.ToLower(), read);

            int result = bytes[SubVersionIndexInBytes];
            Assert.AreEqual(lenMark, result % 8);
        }

        [DataTestMethod]
        [DataRow(@"-1a", 3)]
        [DataRow(@"1a", 1)]
        [DataRow(@"a", 3)]
        [DataRow(@"1233z", 3)]
        [DataRow(@"123", 3)]
        public void VersionFailTest1(string version, int byteLen)
        {
            try
            {
                var bytes = new byte[byteLen];
                VgcApis.Libs.Streams.RawBitStream.Utils.WriteVersion(version, bytes);
                var read = VgcApis.Libs.Streams.RawBitStream.Utils.ReadVersion(bytes);
            }
            catch
            {
                return;
            }
            Assert.Fail();
        }

        [TestMethod]
        public void UtilsBoolListTest()
        {
            var rand = new Random();

            for (int i = 0; i < 60; i += 3)
            {
                var source = new List<bool>();
                for (var j = 0; j < i; j++)
                {
                    var v = rand.Next(2);
                    source.Add(v == 1);
                }

                var bytes = VgcApis.Libs.Streams.RawBitStream.Utils.BoolList2Bytes(source);

                var dest = VgcApis.Libs.Streams.RawBitStream.Utils.Bytes2BoolList(bytes);

                Assert.AreEqual(source.Count, dest.Count);
                for (int j = 0; j < source.Count; j++)
                {
                    Assert.AreEqual(source[j], dest[j]);
                }
            }
        }

        [DataTestMethod]
        [DataRow("abc.com")]
        [DataRow("1.2.3.4")]
        [DataRow("::1")]
        [DataRow("2001:4860:4860::8888")]
        public void AddressNormalTest(string val)
        {
            bitStream.Clear();
            address.Write(val);
            var read = address.Read();
            Assert.AreEqual(read, val);
        }

        [TestMethod]
        public void UuidNormalTest()
        {
            bitStream.Clear();
            for (int i = 0; i < 10; i++)
            {
                var gid = Guid.NewGuid();
                uuids.Write(gid);
                var read = uuids.Read();
                Assert.AreEqual(read.ToString(), gid.ToString());
            }
        }

        [DataTestMethod]
        [DataRow(@"")]
        [DataRow(@"abc123")]
        [DataRow(@"中文abc1{23+}-./")]
        [DataRow(@"a中文abc1{23+}-./")]
        public void BytesNormalTest(string val)
        {
            bitStream.Clear();
            var bytes = Encoding.UTF8.GetBytes(val);
            bytesWriter.Write(bytes);
            var read = bytesWriter.Read();
            var result = Encoding.UTF8.GetString(read);
            Assert.AreEqual(val.Length, result.Length);
            Assert.AreEqual(val, result);
        }

        [TestMethod]
        public void BitStreamReadWritTest()
        {
            bitStream.Write(true);
            bitStream.Clear();
            Assert.AreEqual(0, bitStream.Count());
            Assert.AreEqual(0, bitStream.GetIndex());
            bitStream.Write(true);
            Assert.AreEqual(1, bitStream.Count());
            Assert.AreEqual(0, bitStream.GetIndex());
            var bit = bitStream.Read();
            Assert.AreEqual(1, bitStream.Count());
            Assert.AreEqual(1, bitStream.GetIndex());
            Assert.AreEqual(true, bit);
            bit = bitStream.Read();
            Assert.AreEqual(null, bit);
            Assert.AreEqual(1, bitStream.GetIndex());
            bitStream.Rewind();
            Assert.AreEqual(0, bitStream.GetIndex());
            Assert.AreEqual(1, bitStream.Count());
        }

        [DataTestMethod]
        [DataRow(0, 1)]
        [DataRow(1, 1)]
        [DataRow(0, 3)]
        [DataRow(1, 3)]
        [DataRow(2048, 16)]
        [DataRow(65535, 16)]
        public void NumbersNormalTest(int val, int len)
        {
            bitStream.Clear();
            numbers.Write(val, len);
            var result = numbers.Read(len);
            Assert.AreEqual(result, val);
        }

        [DataTestMethod]
        [DataRow(-1, 3)]
        [DataRow(1, 0)]
        [DataRow(2048, 17)]
        public void NumbersFailTest(int val, int len)
        {
            bitStream.Clear();

            try
            {
                numbers.Write(val, len);
                var result = numbers.Read(len);
            }
            catch (ArgumentOutOfRangeException)
            {
                return;
            }

            Assert.Fail();
        }


    }
}
