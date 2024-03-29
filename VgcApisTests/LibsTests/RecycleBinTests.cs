﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VgcApis.Misc;

namespace VgcApisTests.LibsTests
{
    [TestClass]
    public class RecycleBinTests
    {
        [TestMethod]
        public void SingleThreadTests()
        {
            var o = JObject.Parse(
                @"{
                ""hello"": [1,2,3,4,5],
                ""world"": [""ab"",""中文"",""😀,😂""]
            }"
            );

            var recycleBin = new VgcApis.Libs.Infr.RecycleBin();

            var size = recycleBin.GetSize();
            Assert.AreEqual(0, size);
            Assert.AreEqual(0, recycleBin.GetRecycleQueueLength());
            var ok = recycleBin.TryTake<object>(null, out var r);
            Assert.IsFalse(ok);
            Assert.AreEqual(null, r);

            var j = JsonConvert.SerializeObject(o);
            recycleBin.Put(j, o);
            size = recycleBin.GetSize();
            Assert.AreEqual(1, size);
            Assert.AreEqual(1, recycleBin.GetRecycleQueueLength());

            ok = recycleBin.TryTake(j, out r);
            Assert.IsTrue(ok);
            Assert.IsTrue(r is JObject);
            Assert.AreEqual(o, r);
            size = recycleBin.GetSize();
            Assert.AreEqual(0, size);

            var j2 = JsonConvert.SerializeObject(r);
            Assert.AreEqual(j, j2);

            ok = recycleBin.TryTake<JObject>(j, out _);
            Assert.IsFalse(ok);

            r = recycleBin.Parse(j);
            Assert.AreNotEqual(o, r);

            recycleBin.Put(j, o);
            recycleBin.Put(j, o);
            Assert.AreEqual(3, recycleBin.GetRecycleQueueLength());
            size = recycleBin.GetSize();
            Assert.AreEqual(1, size);

            Utils.Sleep(3000);
            // 500ms take one
            Assert.AreEqual(2, recycleBin.GetRecycleQueueLength());
            r = recycleBin.Parse(j);
            Assert.AreEqual(o, r);
            size = recycleBin.GetSize();
            Assert.AreEqual(0, size);

            recycleBin.Put(j, o);
            Assert.AreEqual(3, recycleBin.GetRecycleQueueLength());
            Utils.Sleep(recycleBin.timeout);
            ok = recycleBin.TryTake<JObject>(j, out _);
            Assert.IsFalse(ok);
            size = recycleBin.GetSize();
            Assert.AreEqual(0, size);
            Assert.AreEqual(0, recycleBin.GetRecycleQueueLength());
        }
    }
}
