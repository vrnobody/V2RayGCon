using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace V2RayGCon.Test.MiscCacheTests
{
    [TestClass]
    public class JsonTests
    {
        [DataTestMethod]
        [DataRow(@"tplLogWarn", @"{'log': {'loglevel': 'warning'}}")]
        public void LoadTplTest(string key, string expect)
        {
            var v = Misc.Caches.Jsons.LoadTemplate(key);
            var e = JObject.Parse(expect);
            Assert.AreEqual(true, JToken.DeepEquals(v, e));
        }
    }
}
