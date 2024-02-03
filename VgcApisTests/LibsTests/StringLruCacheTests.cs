using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace VgcApisTests.LibsTests
{
    [TestClass]
    public class StringLruCacheTests
    {
        [TestMethod]
        public void SingleThreadTimeoutTest()
        {
            var cap = 10;
            var timeout = TimeSpan.FromSeconds(3);
            var cache = new VgcApis.Libs.Infr.StringLruCache<int>(cap, timeout);
            Assert.AreEqual(0, cache.GetSize());
            for (int i = 0; i < cap; i++)
            {
                cache.Add(i.ToString(), i);
            }
            Assert.AreEqual(cap, cache.GetSize());
            Assert.AreEqual(cap, cache.GetRecycleQueueLength());

            VgcApis.Misc.Utils.Sleep((int)timeout.TotalMilliseconds / 2);
            int half = cap / 2;
            var ok = cache.TryGet(half.ToString(), out var v);
            Assert.IsTrue(ok);
            Assert.AreEqual(half, v);
            Assert.AreEqual(cap, cache.GetSize());

            // 500ms take one
            Assert.AreEqual(cap - 1, cache.GetRecycleQueueLength());

            VgcApis.Misc.Utils.Sleep((int)timeout.TotalMilliseconds);
            ok = cache.TryGet(half.ToString(), out v);
            Assert.IsFalse(ok);
            Assert.AreEqual(0, v);
            Assert.AreEqual(0, cache.GetSize());
            Assert.AreEqual(0, cache.GetRecycleQueueLength());
        }

        [DataTestMethod]
        [DataRow(-1)]
        [DataRow(0)]
        [DataRow(5)]
        public void SingleThreadTest(int seconds)
        {
            var cache = new VgcApis.Libs.Infr.StringLruCache<int>(
                20,
                TimeSpan.FromSeconds(seconds)
            );
            var ok = cache.TryGet(null, out var v);
            Assert.IsFalse(ok);
            Assert.AreEqual(0, v);

            ok = cache.TryGet("", out v);
            Assert.IsFalse(ok);
            Assert.AreEqual(0, v);

            Assert.AreEqual(0, cache.GetSize());
            Assert.AreEqual(0, cache.GetRecycleQueueLength());

            var max = cache.capacity;
            for (int i = 0; i < max * 2; i++)
            {
                cache.Add(i.ToString(), i);
            }
            Assert.AreEqual(max, cache.GetSize());

            var expQlen = seconds > 0 ? max * 2 : 0;
            Assert.AreEqual(expQlen, cache.GetRecycleQueueLength());

            var notExist = max / 2;
            ok = cache.TryGet(notExist.ToString(), out v);
            Assert.IsFalse(ok);
            Assert.AreEqual(0, v);

            int exist = (int)(max * 1.5);
            ok = cache.TryGet(exist.ToString(), out v);
            Assert.IsTrue(ok);
            Assert.AreEqual(exist, v);

            ok = cache.Remove(exist.ToString());
            Assert.IsTrue(ok);
            ok = cache.TryGet(exist.ToString(), out v);
            Assert.IsFalse(ok);
            Assert.AreEqual(0, v);
            Assert.AreEqual(max - 1, cache.GetSize());

            cache.Add(exist.ToString(), exist);
            ok = cache.TryGet(exist.ToString(), out v);
            Assert.IsTrue(ok);
            Assert.AreEqual(exist, v);

            expQlen = seconds > 0 ? expQlen + 1 : 0;
            Assert.AreEqual(expQlen, cache.GetRecycleQueueLength());

            notExist = notExist - 1;
            ok = cache.Remove(notExist.ToString());
            Assert.IsFalse(ok);
            Assert.AreEqual(max, cache.GetSize());

            ok = cache.TryGet(notExist.ToString(), out v);
            Assert.IsFalse(ok);
            Assert.AreEqual(0, v);

            cache.Add("", 123);
            ok = cache.TryGet("", out v);
            Assert.IsTrue(ok);
            Assert.AreEqual(123, v);

            expQlen = seconds > 0 ? expQlen + 1 : 0;
            Assert.AreEqual(expQlen, cache.GetRecycleQueueLength());

            cache.Add(null, 234);
            ok = cache.TryGet("", out v);
            Assert.IsTrue(ok);
            Assert.AreEqual(234, v);

            expQlen = seconds > 0 ? expQlen + 1 : 0;
            Assert.AreEqual(expQlen, cache.GetRecycleQueueLength());
        }

        [TestMethod]
        public void MultiThreadsTest()
        {
            var cache = new VgcApis.Libs.Infr.StringLruCache<int>(20, TimeSpan.MinValue);
            var tasks = new List<Task>();

            for (int i = 0; i < 10; i++)
            {
                tasks.Add(
                    VgcApis.Misc.Utils.RunInBackground(() =>
                    {
                        VgcApis.Misc.Utils.Sleep(100);
                        for (int j = 0; j < 100; j++)
                        {
                            VgcApis.Misc.Utils.Sleep(10);
                            try
                            {
                                var k = j.ToString();
                                cache.Add(k, j);
                                cache.TryGet(k, out var v);
                                cache.Remove(k);
                            }
                            catch
                            {
                                Assert.Fail();
                            }
                        }
                    })
                );
            }

            Task.WhenAll(tasks.ToArray()).Wait();
        }
    }
}
