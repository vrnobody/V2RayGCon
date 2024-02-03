using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VgcApis.Misc;

namespace VgcApisTests.MiscTests
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

            var size = RecycleBin.GetSize();
            Assert.AreEqual(0, size);
            Assert.AreEqual(0, RecycleBin.GetRecycleQueueLength());
            var ok = RecycleBin.TryTake<object>(null, out var r);
            Assert.IsFalse(ok);
            Assert.AreEqual(null, r);

            var j = JsonConvert.SerializeObject(o);
            RecycleBin.Put(j, o);
            size = RecycleBin.GetSize();
            Assert.AreEqual(1, size);
            Assert.AreEqual(1, RecycleBin.GetRecycleQueueLength());

            ok = RecycleBin.TryTake(j, out r);
            Assert.IsTrue(ok);
            Assert.IsTrue(r is JObject);
            Assert.AreEqual(o, r);
            size = RecycleBin.GetSize();
            Assert.AreEqual(0, size);

            var j2 = JsonConvert.SerializeObject(r);
            Assert.AreEqual(j, j2);

            ok = RecycleBin.TryTake<JObject>(j, out _);
            Assert.IsFalse(ok);

            r = RecycleBin.Parse(j);
            Assert.AreNotEqual(o, r);

            RecycleBin.Put(j, o);
            RecycleBin.Put(j, o);
            Assert.AreEqual(3, RecycleBin.GetRecycleQueueLength());
            size = RecycleBin.GetSize();
            Assert.AreEqual(1, size);

            Utils.Sleep(3000);
            // 500ms take one
            Assert.AreEqual(2, RecycleBin.GetRecycleQueueLength());
            r = RecycleBin.Parse(j);
            Assert.AreEqual(o, r);
            size = RecycleBin.GetSize();
            Assert.AreEqual(0, size);

            RecycleBin.Put(j, o);
            Assert.AreEqual(3, RecycleBin.GetRecycleQueueLength());
            Utils.Sleep(RecycleBin.timeout);
            ok = RecycleBin.TryTake<JObject>(j, out _);
            Assert.IsFalse(ok);
            size = RecycleBin.GetSize();
            Assert.AreEqual(0, size);
            Assert.AreEqual(0, RecycleBin.GetRecycleQueueLength());
        }
    }
}
