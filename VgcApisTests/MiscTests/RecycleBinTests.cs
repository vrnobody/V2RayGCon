using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
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
            var ok = RecycleBin.TryTake<JObject>(j, out var r);
            Assert.IsFalse(ok); // too small
            Assert.AreEqual(null, r);

            o["padding"] = Utils.RandomHex(RecycleBin.minKeySize);

            j = JsonConvert.SerializeObject(o);
            RecycleBin.Put(j, o);
            ok = RecycleBin.TryTake(j, out r);
            Assert.IsTrue(ok);
            Assert.IsTrue(r is JObject);

            ok = RecycleBin.TryTake<JObject>(j, out _);
            Assert.IsFalse(ok);

            var j2 = JsonConvert.SerializeObject(r);
            Assert.AreEqual(j, j2);

            RecycleBin.Put(j, o);
            Utils.Sleep(RecycleBin.timeout.Add(TimeSpan.FromSeconds(2)));
            ok = RecycleBin.TryTake<JObject>(j, out _);
            Assert.IsFalse(ok);
        }
    }
}
