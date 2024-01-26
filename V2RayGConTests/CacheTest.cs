using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace V2RayGCon.Test
{
    [TestClass]
    public class CacheTest
    {
        readonly Services.Cache cache;

        public CacheTest()
        {
            cache = Services.Cache.Instance;
        }

        [TestMethod]
        public void GeneralCacheNormalTest() { }

        [DataTestMethod]
        [DataRow(@"tplLogWarn", @"{'log': {'loglevel': 'warning'}}")]
        public void LoadTplTest(string key, string expect)
        {
            var v = cache.tpl.LoadTemplate(key);
            var e = JObject.Parse(expect);
            Assert.AreEqual(true, JToken.DeepEquals(v, e));
        }
    }
}
