using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace VgcApisTests
{
    [TestClass]
    public class Experiments
    {
        public Experiments() { }

        [TestMethod]
        public void GzipStreamTest()
        {
            try
            {
                var data = "hello";
                using (var dest = new MemoryStream())
                using (var gzip = new GZipStream(dest, CompressionMode.Decompress))
                using (var w = new StreamWriter(gzip))
                {
                    w.Write(data); // Decompress stream only support read method;
                    w.Flush();
                }
            }
            catch (ArgumentException)
            {
                return;
            }
            Assert.Fail();
        }

        [TestMethod]
        public void EncodingTest()
        {
            // unicode = utf16
            var a1 = "a";
            var a2 = "字";
            var a3 = "😀";

            Assert.AreEqual(1, a1.Length);
            Assert.AreEqual(1, a2.Length);
            Assert.AreEqual(2, a3.Length);

            var b1 = Encoding.Unicode.GetBytes(a1);
            var b2 = Encoding.Unicode.GetBytes(a2);
            var b3 = Encoding.Unicode.GetBytes(a3);

            Assert.AreEqual(2, b1.Length);
            Assert.AreEqual(2, b2.Length);
            Assert.AreEqual(4, b3.Length);
        }

        [TestMethod]
        public void CancellationTokenSourceDisposeTest()
        {
            var cts = new CancellationTokenSource();
            var token = cts.Token;
            var mre = new ManualResetEventSlim(false);
            var task = Task.Run(() =>
            {
                Assert.IsFalse(token.IsCancellationRequested);
                mre.Wait();
                Assert.IsTrue(token.IsCancellationRequested);
            });

            Thread.Sleep(2000);
            cts.Cancel();
            cts.Dispose();
            mre.Set();
            task.Wait();
            Assert.IsTrue(token.IsCancellationRequested);
        }

        [TestMethod]
        public void TimedCancellationTokenSourceDisposeTest()
        {
            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(2));
            var mre = new ManualResetEventSlim(false);
            var token = cts.Token;
            token.Register(() =>
            {
                cts.Dispose();
                mre.Set();
            });

            var task = Task.Run(() =>
            {
                Assert.IsFalse(token.IsCancellationRequested);
                mre.Wait();
                Assert.IsTrue(token.IsCancellationRequested);
            });
            task.Wait();
            Assert.IsTrue(token.IsCancellationRequested);
        }
    }
}
