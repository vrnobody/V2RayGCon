using System.IO;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace VgcApisTests.LibsTests
{
    [TestClass]
    public class AsciiRoStrStreamTests
    {
        [TestMethod]
        public void ReadToBufferTest()
        {
            var str = "123hello1";
            var len = str.Length;
            var size = 20;
            var buff = new byte[size];

            using (var stream = new VgcApis.Libs.Streams.ReadonlyStringStream(str, Encoding.ASCII))
            {
                var c = stream.Read(buff, 0, size);
                Assert.IsTrue(c > 0);
                var r = Encoding.ASCII.GetString(buff, 0, len);
                Assert.AreEqual(str, r);
                c = stream.Read(buff, 0, 10);
                Assert.AreEqual(0, c);
            }

            using (var stream = new VgcApis.Libs.Streams.ReadonlyStringStream(str, Encoding.ASCII))
            {
                var s1 = 3;
                var s2 = 2;
                var c = 6;
                var buf2 = new byte[size];
                stream.Position = s1;
                stream.Read(buf2, s2, c);
                for (int i = 0; i < c; i++)
                {
                    Assert.AreEqual(buff[i + s1], buf2[i + s2]);
                }
            }
        }

        [TestMethod]
        public void ReadToStreamTest()
        {
            var str = VgcApis.Misc.Utils.RandomHex(64 * 1024) + "123hellol";

            string s;
            using (var src = new VgcApis.Libs.Streams.ReadonlyStringStream(str, Encoding.ASCII))
            using (var des = new MemoryStream())
            using (var r = new StreamReader(des, Encoding.ASCII))
            {
                src.CopyTo(des);
                src.Flush();
                des.Position = 0;
                s = r.ReadToEnd();
            }

            Assert.AreEqual(str, s);
        }
    }
}
