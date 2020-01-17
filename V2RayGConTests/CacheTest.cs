using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace V2RayGCon.Test
{
    [TestClass]
    public class CacheTest
    {
        V2RayGCon.Service.Cache cache;

        public CacheTest()
        {
            cache = V2RayGCon.Service.Cache.Instance;
        }

        [TestMethod]
        public void GeneralCacheNormalTest()
        {

        }

        [TestMethod]
        public void HTMLFailTest()
        {
            Assert.ThrowsException<WebException>(() =>
            {
                var t = cache.html[""];
            });
        }

        [DataTestMethod]
        [DataRow("https://www.baidu.com/")]
        [DataRow("https://www.sogou.com/,https://www.baidu.com/")]
        public void HTMLNormalTest(string rawData)
        {
            var data = rawData.Split(',');
            var urls = new List<string>();
            var len = data.Length;
            for (var i = 0; i < 1000; i++)
            {
                urls.Add(data[i % len]);
            }
            var html = cache.html;
            html.Clear();

            try
            {
                Lib.Utils.ExecuteInParallel(urls, (url) =>
                {
                    return html[url];
                });
            }
            catch
            {
                Assert.Fail();
            }

            Assert.AreEqual<int>(data.Length, html.Count);
            html.Clear();
            Assert.AreEqual<int>(0, html.Count);
        }

        [DataTestMethod]
        [DataRow(@"inTpl.sniffing", @"{'enabled':false,'destOverride':['http','tls']}")]
        public void LoadExampleTest(string key, string expect)
        {
            var v = cache.tpl.LoadExample(key);
            var e = JToken.Parse(expect);
            Assert.AreEqual(true, JToken.DeepEquals(v, e));
        }

        [DataTestMethod]
        [DataRow(@"vgc", @"{'alias': '','description': ''}")]
        public void LoadTplTest(string key, string expect)
        {
            var v = cache.tpl.LoadTemplate(key);
            var e = JObject.Parse(expect);
            Assert.AreEqual(true, JObject.DeepEquals(v, e));
        }

        [TestMethod]
        public void LoadMinConfigTest()
        {
            var min = cache.tpl.LoadMinConfig();
            var v = Lib.Utils.GetValue<string>(min, "log.loglevel");
            Assert.AreEqual<string>("warning", v);
        }
    }
}
