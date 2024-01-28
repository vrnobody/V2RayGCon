using System.IO;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace VgcApisTests.LibsTests
{
    [TestClass]
    public class UnicodeStreamTests
    {
        [TestMethod]
        public void ReadToBufferTest()
        {
            var str = "123h😀😁ello中文1";
            var len = str.Length * 2;
            var buff = new byte[1024];

            using (var stream = new VgcApis.Libs.Streams.UnicodeStringStream(str))
            {
                var c = stream.Read(buff, 0, len + 10);
                Assert.IsTrue(c > 0);
                var r = Encoding.Unicode.GetString(buff, 0, len);
                Assert.AreEqual(str, r);
                c = stream.Read(buff, 0, 10);
                Assert.AreEqual(0, c);
            }

            using (var stream = new VgcApis.Libs.Streams.UnicodeStringStream(str))
            {
                var s1 = 5;
                var s2 = 3;
                var c = 8;
                var buf2 = new byte[1024];
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
            var str = VgcApis.Misc.Utils.RandomHex(64 * 1024) + "123h😀😁ello中文1";

            string s;
            using (var src = new VgcApis.Libs.Streams.UnicodeStringStream(str))
            using (var des = new MemoryStream())
            using (var r = new StreamReader(des, Encoding.Unicode))
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
