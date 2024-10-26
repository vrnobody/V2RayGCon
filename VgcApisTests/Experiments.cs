using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
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
        public void ShiftLeftOperandTest()
        {
            var s0 = 1 << 0;
            Assert.AreEqual(1, s0);
            var s1 = 1 << 1;
            Assert.AreEqual(2, s1);
            var s2 = 1 << 2;
            Assert.AreEqual(4, s2);
        }

#if DEBUG
        [TestMethod]
        public void ManualResetEventTest()
        {
            var r = false;

            // false 阻塞
            var mre = new ManualResetEvent(false);
            r = mre.WaitOne(0);
            Assert.IsFalse(r);

            VgcApis.Misc.Utils.RunInBackground(() =>
            {
                try
                {
                    r = mre.WaitOne();
                }
                catch { }
            });
            VgcApis.Misc.Utils.Sleep(1000);
            mre.Set();
            mre.Dispose();
            VgcApis.Misc.Utils.Sleep(500);

            Assert.IsTrue(r);
        }
#endif

        [TestMethod]
        public void DateTimeTest()
        {
            var now = DateTime.Now;
            var utcnow = DateTime.UtcNow;
            var localnow = now.ToLocalTime();
            var localutcnow = utcnow.ToLocalTime();
            var locallocal = now.ToLocalTime().ToLocalTime();

            var tick = utcnow.Ticks;
            var d = new DateTime(tick, DateTimeKind.Utc);
            var dlocal = d.ToLocalTime();

            // break here
            // mouse hover to see each value
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void EmptyStringTest()
        {
            Assert.IsFalse("" == null);
            Assert.IsTrue("" == "");
            Assert.IsFalse("" == "a");
            Assert.IsTrue("" == string.Empty);
            Assert.IsTrue("a".Contains(""));
            Assert.IsFalse("".Equals("a"));
            Assert.IsFalse("a".Equals(""));
            Assert.IsTrue("".Equals(""));
        }

        [TestMethod]
        public void TakeMoreThanListLengthTest()
        {
            var list = new int[] { 1, 2, 3 };
            var r = list.Take(10).ToList();
            Assert.AreEqual(3, r.Count);
        }

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
