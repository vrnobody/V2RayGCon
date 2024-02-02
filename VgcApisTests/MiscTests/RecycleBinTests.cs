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

            var j = JsonConvert.SerializeObject(o);
            RecycleBin.Put(j, o);
            var ok = RecycleBin.TryTake<object>(j, out var r);
            Assert.IsFalse(ok); // too small
            Assert.AreEqual(null, r);

            o["padding"] = Utils.RandomHex(RecycleBin.minSize);

            j = JsonConvert.SerializeObject(o);
            RecycleBin.Put(j, o);
            ok = RecycleBin.TryTake(j, out r);
            Assert.IsTrue(ok);
            Assert.IsTrue(r is JObject);
            Assert.AreEqual(o, r);
            var j2 = JsonConvert.SerializeObject(r);
            Assert.AreEqual(j, j2);

            ok = RecycleBin.TryTake<JObject>(j, out _);
            Assert.IsFalse(ok);

            r = RecycleBin.Parse(j);
            Assert.AreNotEqual(o, r);

            RecycleBin.Put(j, o);
            Utils.Sleep(3000);
            r = RecycleBin.Parse(j);
            Assert.AreEqual(o, r);

            RecycleBin.Put(j, o);
            Utils.Sleep(RecycleBin.timeout);
            ok = RecycleBin.TryTake<JObject>(j, out _);
            Assert.IsFalse(ok);
        }
    }
}
