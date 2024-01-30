using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace VgcApisTests.MiscTests
{
    [TestClass]
    public class JsonRecycleBinTests
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
            VgcApis.Misc.JsonRecycleBin.Put(j, o);
            var ok = VgcApis.Misc.JsonRecycleBin.TryTake(j, out var r);
            Assert.IsTrue(ok);
            Assert.IsTrue(r is JObject);

            ok = VgcApis.Misc.JsonRecycleBin.TryTake(j, out _);
            Assert.IsFalse(ok);

            var j2 = JsonConvert.SerializeObject(r);
            Assert.AreEqual(j, j2);

            VgcApis.Misc.JsonRecycleBin.Put(j, o);
            VgcApis.Misc.Utils.Sleep(13000);
            ok = VgcApis.Misc.JsonRecycleBin.TryTake(j, out _);
            Assert.IsFalse(ok);
        }
    }
}
