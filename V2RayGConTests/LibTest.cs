using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using V2RayGCon.Test.Resource.Resx;
using static V2RayGCon.Lib.Utils;

namespace V2RayGCon.Test
{
    [TestClass]
    public class LibTest
    {

        [DataTestMethod]
        [DataRow(@"http://abc.com", @"abc.com")]
        [DataRow(@"v://abc.com", @"abc.com")]
        [DataRow(@"vee://", @"")]
        [DataRow(@"vmess://abc", @"abc")]
        [DataRow(@"v2cfg://abc", @"abc")]
        [DataRow(@"http://abc", @"abc")]
        [DataRow(@"https://abc", @"abc")]
        [DataRow(@"any://://", @"://")]
        public void GetLinkBodyNormalTest(string link, string expect)
        {
            var body = GetLinkBody(link);
            Assert.AreEqual(expect, body);

        }

        [DataTestMethod]
        [DataRow(@"v/", @"")]
        [DataRow(@":v/v/", @"")]
        public void GetLinkBodyFailTest(string link, string expect)
        {
            try
            {
                var body = GetLinkBody(link);
            }
            catch { return; }
            Assert.Fail();
        }

        [DataTestMethod]
        [DataRow("https://www.baidu.com/")]
        public void FetchTest(string url)
        {
            var html = Lib.Utils.Fetch(url);
            Assert.AreEqual(false, string.IsNullOrEmpty(html));
        }

        [DataTestMethod]
        [DataRow("https://www.baidu.com/", 3)]
        public void FetchBatchTest(string url, int times)
        {
            var urls = new List<string>();
            for (int i = 0; i < times; i++)
            {
                urls.Add(url);
            }

            List<string> htmls = new List<string>();
            try
            {
                htmls = ExecuteInParallel(urls, (u) =>
                {
                    return Fetch(u);
                });
            }
            catch
            {
                Assert.Fail();
            }

            foreach (var html in htmls)
            {
                Assert.AreEqual(false, string.IsNullOrEmpty(html));
            }
        }

        [DataTestMethod]
        [DataRow("http://a.com/b/c/", "/d/e/abc.html", "http://a.com/d/e/abc.html")]
        [DataRow("vv/b/c/", "/e/abc.html", "/e/abc.html")]
        [DataRow("http://a.com/c", "/d/abc.html", "http://a.com/d/abc.html")]
        public void PatchUrlTest(string url, string relativeUrl, string expect)
        {
            var patchedUrl = Lib.Utils.PatchHref(url, relativeUrl);
            Assert.AreEqual(expect, patchedUrl);
        }

        [DataTestMethod]
        [DataRow("0.0.0.0.", "0.0.0.0.")]
        [DataRow("0.0.1.11", "0.0.1.11")]
        [DataRow("0.0.1.0", "0.0.1")]
        [DataRow("0.0.0.1", "0.0.0.1")]
        [DataRow("0.0.0.0", "0.0")]
        public void TrimVersionStringTest(string version, string expect)
        {
            var result = Lib.Utils.TrimVersionString(version);
            Assert.AreEqual(expect, result);
        }

        [DataTestMethod]
        [DataRow("", "e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855")]
        [DataRow(null, "e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855")]
        [DataRow("1234", "03ac674216f3e15c761ee1a5e255f067953623c8b388b4459e13f978d7c846f4")]
        public void Sha256Test(string source, string expect)
        {
            var sha = Lib.Utils.SHA256(source);
            Assert.AreEqual(expect, sha);
        }

        [DataTestMethod]
        [DataRow(0.1, 0.2, false)]
        [DataRow(0.000000001, 0.00000002, true)]
        [DataRow(0.001, 0.002, false)]
        [DataRow(-0.1, 0.1, false)]
        [DataRow(2, 2, true)]
        public void AreEqualTest(double a, double b, bool expect)
        {
            Assert.AreEqual(expect, Lib.Utils.AreEqual(a, b));
        }

        [DataTestMethod]
        [DataRow("http://www.baidu.com")]
        public void VisitWebPageSpeedTestTest(string url)
        {
            var time = VisitWebPageSpeedTest(url, -1, 1024 * 1024, -1);
            Assert.AreEqual(long.MaxValue, time);

            time = VisitWebPageSpeedTest(url, -1, -1, -1);
            Assert.AreEqual(true, time < long.MaxValue);
        }

        [DataTestMethod]
        [DataRow("aaaaaa", 0, "...")]
        [DataRow("aaaaaaaaa", 5, "aa...")]
        [DataRow("aaaaaa", 3, "...")]
        [DataRow("aaaaaa", -1, "...")]
        [DataRow("", 100, "")]
        public void CutStrTest(string org, int len, string expect)
        {
            var cut = Lib.Utils.CutStr(org, len);
            Assert.AreEqual(expect, cut);
        }

        [DataTestMethod]
        [DataRow(@"{}", "")]
        [DataRow(@"{v2raygcon:{env:['1','2']}}", "")]
        [DataRow(@"{v2raygcon:{env:{a:'1',b:2}}}", "a:1,b:2")]
        [DataRow(@"{v2raygcon:{env:{a:'1',b:'2'}}}", "a:1,b:2")]
        public void GetEnvVarsFromConfigTest(string json, string expect)
        {
            var j = JObject.Parse(json);
            var env = Lib.Utils.GetEnvVarsFromConfig(j);
            var strs = env.OrderBy(p => p.Key).Select(p => p.Key + ":" + p.Value);
            var r = string.Join(",", strs);

            Assert.AreEqual(expect, r);
        }


        [TestMethod]
        public void CreateDeleteAppFolderTest()
        {
            var appFolder = Lib.Utils.GetSysAppDataFolder();
            Assert.AreEqual(false, string.IsNullOrEmpty(appFolder));

            // do not run these tests 
            // Lib.Utils.CreateAppDataFolder();
            // Assert.AreEqual(true, Directory.Exists(appFolder));
            // Lib.Utils.DeleteAppDataFolder();
            // Assert.AreEqual(false, Directory.Exists(appFolder));
        }

        [DataTestMethod]
        [DataRow(@"{}", "a", "abc", @"{'a':'abc'}")]
        [DataRow(@"{'a':{'b':{'c':1234}}}", "a.b.c", "abc", @"{'a':{'b':{'c':'abc'}}}")]
        public void SetValueStringTest(string json, string path, string value, string expect)
        {
            var r = JObject.Parse(json);
            var e = JObject.Parse(expect);
            Lib.Utils.SetValue<string>(r, path, value);
            Assert.AreEqual(true, JObject.DeepEquals(e, r));
        }

        [DataTestMethod]
        [DataRow(@"{}", "a", 1, @"{'a':1}")]
        [DataRow(@"{'a':{'b':{'c':1234}}}", "a.b.c", 5678, @"{'a':{'b':{'c':5678}}}")]
        public void SetValueIntTest(string json, string path, int value, string expect)
        {
            var r = JObject.Parse(json);
            var e = JObject.Parse(expect);
            Lib.Utils.SetValue<int>(r, path, value);
            Assert.AreEqual(true, JObject.DeepEquals(e, r));
        }

        [DataTestMethod]
        [DataRow(@"{'a':{'c':null},'b':1}", "a.b.c")]
        [DataRow(@"{'a':[0,1,2],'b':1}", "a.0")]
        [DataRow(@"{}", "")]
        public void RemoveKeyFromJsonFailTest(string json, string key)
        {
            // outboundDetour inboundDetour
            var j = JObject.Parse(json);
            Assert.ThrowsException<KeyNotFoundException>(() =>
            {
                RemoveKeyFromJObject(j, key);
            });
        }

        [DataTestMethod]
        [DataRow(@"{'a':{'c':null,'a':2},'b':1}", "a.c", @"{'a':{'a':2},'b':1}")]
        [DataRow(@"{'a':{'c':1},'b':1}", "a.c", @"{'a':{},'b':1}")]
        [DataRow(@"{'a':{'c':1},'b':1}", "a.b", @"{'a':{'c':1},'b':1}")]
        [DataRow(@"{'a':1,'b':1}", "c", @"{'a':1,'b':1}")]
        [DataRow(@"{'a':1,'b':1}", "a", @"{'b':1}")]
        public void RemoveKeyFromJsonNormalTest(string json, string key, string expect)
        {
            // outboundDetour inboundDetour
            var j = JObject.Parse(json);
            RemoveKeyFromJObject(j, key);
            var e = JObject.Parse(expect);
            Assert.AreEqual(true, JObject.DeepEquals(e, j));
        }

        [DataTestMethod]
        [DataRow("", "")]
        [DataRow("1", "1")]
        [DataRow("1 , 2", "1,2")]
        [DataRow(",  ,  ,", "")]
        [DataRow(",,,  ,1  ,  ,2,  ,3,,,", "1,2,3")]
        public void Str2JArray2Str(string value, string expect)
        {
            var array = Lib.Utils.Str2JArray(value);
            var str = Lib.Utils.JArray2Str(array);
            Assert.AreEqual(expect, str);
        }

        [DataTestMethod]
        [DataRow("0", 0)]
        [DataRow("-1", -1)]
        [DataRow("str-1.234", 0)]
        [DataRow("-1.234str", 0)]
        [DataRow("-1.234", -1)]
        [DataRow("1.432", 1)]
        [DataRow("1.678", 2)]
        [DataRow("-1.678", -2)]
        public void Str2Int(string value, int expect)
        {
            Assert.AreEqual(expect, Lib.Utils.Str2Int(value));
        }

        [TestMethod]
        public void GetLocalCoreVersion()
        {
            var core = new V2RayGCon.Lib.V2Ray.Core(Service.Setting.Instance);
            var version = core.GetCoreVersion();

            if (core.IsExecutableExist())
            {
                Assert.AreNotEqual(string.Empty, version);
            }
            else
            {
                Assert.AreEqual(string.Empty, version);
            }
        }

        [TestMethod]
        public void GetValue_GetBoolFromString_ReturnDefault()
        {
            var json = Service.Cache.Instance.
                tpl.LoadMinConfig();
            Assert.AreEqual(default(bool), GetValue<bool>(json, "log.loglevel"));
        }

        [TestMethod]
        public void GetValue_GetStringNotExist_ReturnNull()
        {
            var json = Service.Cache.Instance.
                tpl.LoadMinConfig();
            Assert.AreEqual(string.Empty, GetValue<string>(json, "log.keyNotExist"));
        }

        [TestMethod]
        public void GetValue_KeyNotExist_ReturnDefault()
        {
            var json = Service.Cache.Instance.
                tpl.LoadMinConfig();
            var value = Lib.Utils.GetValue<int>(json, "log.key_not_exist");
            Assert.AreEqual(default(int), value);
        }

        [TestMethod]
        public void ConfigResource_Validate()
        {
            foreach (var config in Lib.Utils.TestingGetResourceConfigJson())
            {
                try
                {
                    JObject.Parse(config);
                }
                catch
                {
                    Assert.Fail();
                }
            }
        }

        [TestMethod]
        public void Str2ListStr()
        {
            var testData = new Dictionary<string, int> {
                // string serial, int expectLength
                {"",0 },
                {",,,,",0 },
                {"1.1,2.2,,3.3,,,4.4.4,", 4},
            };

            foreach (var item in testData)
            {
                var len = Lib.Utils.Str2ListStr(item.Key).Count;
                Assert.AreEqual(item.Value, len);
            }
        }

        [DataTestMethod]
        [DataRow("http://abc.com https://def.com", 0)]
        [DataRow("<a href='http://abc.com' ></a> https://def.com", 1)]
        [DataRow("<a href='a' ></a> https://def.com", 1)]
        public void FindAllHrefTest(string html, int count)
        {
            var links = Lib.Utils.FindAllHrefs(html);
            Assert.AreEqual(count, links.Count());
        }

        [TestMethod]
        public void ExtractLinks_HttpLink()
        {
            var html = "http://abc.com https://def.com";

            var httpLinks = Lib.Utils.ExtractLinks(html,
                VgcApis.Models.Datas.Enum.LinkTypes.http);

            var httpsLinks = Lib.Utils.ExtractLinks(html,
                VgcApis.Models.Datas.Enum.LinkTypes.https);

            Assert.AreEqual(2, httpLinks.Count());
            Assert.AreEqual(2, httpsLinks.Count());
        }


        [DataTestMethod]
        [DataRow("ss://ZHVtbXkwMA==", "ss://ZHVtbXkwMA==")]
        [DataRow("ss://ZHVtbXkwMA", "ss://ZHVtbXkwMA")]
        [DataRow("ss://ZHVtbXkwMA===============", "ss://ZHVtbXkwMA===")]
        [DataRow("ss://ZHVtbXkwMA==#abc.%20&_-中文", "ss://ZHVtbXkwMA==#abc.%20&_-")]
        [DataRow("ss://ZHVtbXkwMA==#", "ss://ZHVtbXkwMA==")]
        [DataRow("ss://ZHVtbXkwMA==#abc.%20&中_-文", "ss://ZHVtbXkwMA==#abc.%20&")]
        public void ExtractLinks_FromString(string source, string expect)
        {
            var result = ExtractLinks(source, VgcApis.Models.Datas.Enum.LinkTypes.ss);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(expect, result[0]);
        }

        [TestMethod]
        public void ExtractLinks_FromLinksTxt()
        {
            var content = TestConst.links;
            var links = Lib.Utils.ExtractLinks(content, VgcApis.Models.Datas.Enum.LinkTypes.vmess);
            Assert.AreEqual(2, links.Count);
        }

        [TestMethod]
        public void ExtractLink_FromEmptyString_Return_EmptyList()
        {
            var content = "";
            var links = Lib.Utils.ExtractLinks(content, VgcApis.Models.Datas.Enum.LinkTypes.vmess);
            Assert.AreEqual(0, links.Count);
        }

        [TestMethod]
        public void GetRemoteCoreVersions()
        {
            // skip this time consuming test 
            // List<string> versions = Lib.Utils.GetCoreVersions(-1);
            // Assert.AreEqual(true, versions.Count > 0);
        }

        [TestMethod]
        public void GetVGCVersions()
        {
            // skip this time consuming test 
            //var version = Lib.Utils.GetLatestVGCVersion();
            //Assert.AreNotEqual(string.Empty, version);
        }
    }
}
