using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace VgcApisTests
{
    [TestClass]
    public class StringReadWriterTests
    {
        public StringReadWriterTests() { }

        [DataTestMethod]
        [DataRow(@"")]
        [DataRow(@"123abcAbc")]
        [DataRow(@"abcd1🎈🎆🧨234中文")]
        [DataRow(@"中文abc1{23+}-./")]
        [DataRow(@"a中文abc1{23+}-./")]
        public void ReadWriteNormalTest(string content)
        {
            var stream = new MemoryStream();

            var writer = new VgcApis.Libs.Streams.StringWriter(stream);
            writer.Write(content);

            stream.Position = 0;

            var reader = new VgcApis.Libs.Streams.StringReader(stream);
            var s = reader.Read();

            stream.Dispose();

            Assert.AreEqual(content, s);
        }

        [TestMethod]
        public void ReadWriteSpecialCaseTest()
        {
            var stream = new MemoryStream();
            var reader = new VgcApis.Libs.Streams.StringReader(stream);
            var s = reader.Read();
            Assert.AreEqual(null, s);

            var writer = new VgcApis.Libs.Streams.StringWriter(stream);
            writer.Write(null);
            stream.Position = 0;
            s = reader.Read();
            Assert.AreEqual(@"", s);

            var strs = new List<string>() { null, "123", @"", @"abcd1🎈🎆🧨234中文", };
            stream.Position = 0;
            foreach (var str in strs)
            {
                writer.Write(str);
            }
            strs[0] = @"";
            stream.Position = 0;
            foreach (var str in strs)
            {
                s = reader.Read();
                Assert.AreEqual(str, s);
            }
        }
    }
}
