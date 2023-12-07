using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace VgcApisTests
{
    [TestClass]
    public class Experiments
    {
        public Experiments() { }

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
