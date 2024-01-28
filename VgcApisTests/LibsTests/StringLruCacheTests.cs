using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace VgcApisTests.LibsTests
{
    [TestClass]
    public class StringLruCacheTests
    {
        [TestMethod]
        public void SingleThreadTest()
        {
            var cache = new VgcApis.Libs.Infr.StringLruCache<int>();
            var ok = cache.TryGet(null, out var v);
            Assert.IsFalse(ok);
            Assert.AreEqual(0, v);

            ok = cache.TryGet("", out v);
            Assert.IsFalse(ok);
            Assert.AreEqual(0, v);

            for (int i = 0; i < 100; i++)
            {
                cache.Add(i.ToString(), i);
            }

            ok = cache.TryGet("1", out v);
            Assert.IsFalse(ok);
            Assert.AreEqual(0, v);

            ok = cache.TryGet("88", out v);
            Assert.IsTrue(ok);
            Assert.AreEqual(88, v);

            ok = cache.Remove("88");
            Assert.IsTrue(ok);
            ok = cache.TryGet("88", out v);
            Assert.IsFalse(ok);
            Assert.AreEqual(0, v);

            cache.Add("88", 88);
            ok = cache.TryGet("88", out v);
            Assert.IsTrue(ok);
            Assert.AreEqual(88, v);

            ok = cache.Remove("22");
            Assert.IsFalse(ok);

            ok = cache.TryGet("22", out v);
            Assert.IsFalse(ok);
            Assert.AreEqual(0, v);

            cache.Add("", 123);
            ok = cache.TryGet("", out v);
            Assert.IsTrue(ok);
            Assert.AreEqual(123, v);

            cache.Add(null, 234);
            ok = cache.TryGet("", out v);
            Assert.IsTrue(ok);
            Assert.AreEqual(234, v);
        }

        [TestMethod]
        public void MultiThreadsTest()
        {
            var cache = new VgcApis.Libs.Infr.StringLruCache<int>();
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
