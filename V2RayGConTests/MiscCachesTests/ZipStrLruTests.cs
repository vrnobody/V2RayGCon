using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using V2RayGCon.Misc.Caches;
using VgcApis.Misc;

namespace V2RayGCon.Test.MiscCacheTests
{
    [TestClass]
    public class ZipStrLruTests
    {
        [TestMethod]
        public void SingleThreadTest()
        {
            var key = Guid.NewGuid().ToString();
            var s = "hello中文😀😂1";

            var ok = ZipStrLru.Put(key, s);
            Assert.IsFalse(ok); // to small
            ok = ZipStrLru.TryGet(key, out var c);
            Assert.IsFalse(ok);
            Assert.AreEqual("", c);

            s = s + Utils.RandomHex(ZipStrLru.minSize);
            ok = ZipStrLru.Put(key, s);
            Assert.IsTrue(ok);
            ok = ZipStrLru.TryGet(key, out c);
            Assert.IsTrue(ok);
            Assert.AreEqual(s, c);
            ok = ZipStrLru.TryGet(key, out c);
            Assert.IsTrue(ok);
            Assert.AreEqual(s, c);

            for (var i = 0; i < ZipStrLru.capacity / 2; i++)
            {
                var k = Guid.NewGuid().ToString();
                ZipStrLru.Put(k, s);
            }
            ok = ZipStrLru.TryGet(key, out c); // bring key to head
            Assert.IsTrue(ok);
            Assert.AreEqual(s, c);

            for (var i = 0; i < ZipStrLru.capacity + 1; i++)
            {
                var k = Guid.NewGuid().ToString();
                ZipStrLru.Put(k, s);
            }
            ok = ZipStrLru.TryGet(key, out c);
            Assert.IsFalse(ok);
            Assert.AreEqual("", c);
            ok = ZipStrLru.TryRemove(key);
            Assert.IsFalse(ok);

            ok = ZipStrLru.Put(key, s);
            Assert.IsTrue(ok);
            ok = ZipStrLru.TryGet(key, out c);
            Assert.IsTrue(ok);
            Assert.AreEqual(s, c);

            ok = ZipStrLru.TryRemove(key);
            Assert.IsTrue(ok);

            ok = ZipStrLru.TryGet(key, out c);
            Assert.IsFalse(ok);
            Assert.AreEqual("", c);
        }
    }
}
