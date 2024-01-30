using System.IO;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace VgcApisTests.LibsTests
{
    [TestClass]
    public class ArrayPoolMemoryStreamTests
    {
        [TestMethod]
        public void ArrayPoolMemoryStreamWithoutEncodingTests()
        {
            var str = "123hello1";
            var stream = new VgcApis.Libs.Streams.ArrayPoolMemoryStream();
            using (var w = new StreamWriter(stream))
            {
                w.Write(str);
                w.Flush();
                Assert.AreEqual(str.Length, stream.Length);
            }
            Assert.AreEqual(0, stream.Length);
            stream.Dispose();
            Assert.AreEqual("", stream.GetString());

            str = VgcApis.Misc.Utils.RandomHex(16 * 1024) + "😀hello中文1";
            stream = new VgcApis.Libs.Streams.ArrayPoolMemoryStream();
            using (var w = new StreamWriter(stream))
            {
                w.Write(str);
                w.Flush();
                Assert.AreEqual(str.Length + 2 + 4, stream.Length);
            }
            Assert.AreEqual(0, stream.Length);
            stream.Dispose();
            Assert.AreEqual("", stream.GetString());
        }

        [TestMethod]
        public void ArrayPoolMemoryStreamWithEncodingTests()
        {
            var str = "123hello1";
            var encoding = Encoding.ASCII;
            var stream = new VgcApis.Libs.Streams.ArrayPoolMemoryStream(encoding);
            using (var w = new StreamWriter(stream, encoding))
            {
                w.Write(str);
                w.Flush();
                Assert.AreEqual(str.Length, stream.Length);
            }
            Assert.AreEqual(0, stream.Length);
            stream.Dispose();
            Assert.AreEqual(str, stream.GetString());

            var len = 10;
            str = VgcApis.Misc.Utils.RandomHex(len) + "😀hello中文1";
            encoding = Encoding.Unicode;
            stream = new VgcApis.Libs.Streams.ArrayPoolMemoryStream(encoding);
            using (var w = new StreamWriter(stream, encoding))
            {
                w.Write(str);
                w.Flush();
                Assert.AreEqual((str.Length * 2) + 2, stream.Length);
            }
            Assert.AreEqual(0, stream.Length);
            stream.Dispose();

            var r = stream.GetString();
            if (r.Length != str.Length)
            {
                var bom = r[0];
                Assert.AreEqual(bom + str, r);
            }
            else
            {
                Assert.AreEqual(str, r);
            }
        }
    }
}
