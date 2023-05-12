using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using V2RayGCon.Test.Resource.Resx;
using static V2RayGCon.Misc.Utils;

namespace V2RayGCon.Test
{
    [TestClass]
    public class LibTest
    {
        static long SpeedtestTimeout = VgcApis.Models.Consts.Core.SpeedtestTimeout;

        [TestMethod]
        public void ParseStatApiResultTestXray()
        {
            var str =
@"{
    ""stat"":  [
        {
                ""name"":  ""inbound>>>agentin>>>traffic>>>uplink"",
            ""value"":  ""12345""
        },
        {
                ""name"":  ""inbound>>>agentin>>>traffic>>>downlink"",
            ""value"":  ""098765""
        }
    ]
}";
            var result = ParseStatApiResult(true, str);
            Assert.AreEqual(12345, result.statsUplink);
            Assert.AreEqual(98765, result.statsDownlink);
        }

        [TestMethod]
        public void ParseStatApiResultTestV2Ray()
        {
            var str =
@"stat: <
  name: ""inbound>>>agentin>>>traffic>>>uplink""
>
stat: <
  name: ""inbound>>>agentin>>>traffic>>>downlink""
>
stat: <
  name: ""outbound>>>StatsApiInb>>>traffic>>>uplink""
  value: 1
>
stat: <
  name: ""inbound1>>>StatsApiInb>>>traffic>>>uplink""
  value: 2
>
stat: <
  name: ""inbound>>>StatsApiInb>>>traffic>>>uplink""
  value: 1
>
stat: <
  name: ""inbound>>>StatsApiInb>>>traffic>>>downlink""
  value: 2
>
stat: <
  name: ""inbound>>>StatsApiInb>>>traffic>>>uplink""
  value: 3
>
stat: <
  name: ""inbound>>>StatsApiInb>>>traffic>>>uplink""
  value: 4
>
stat: <
  name: ""inbound>>>StatsApiInb>>>traffic>>>downlink""
  value: 5
>";

            var result = ParseStatApiResult(false, str);
            Assert.AreEqual(8, result.statsUplink);
            Assert.AreEqual(7, result.statsDownlink);
        }


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
        [DataRow(
            @"[#f00]vless://b0dd64e4-0fbd-4038-9139-d1f32a68a0dc@qv2ray.net:3279?security=xtls&flow=xtls-rprx-splice#VLESSTCPXTLSSplice [/#f00]",
            @"vless://b0dd64e4-0fbd-4038-9139-d1f32a68a0dc@qv2ray.net:3279?security=xtls&flow=xtls-rprx-splice#VLESSTCPXTLSSplice")]
        [DataRow(
            @"1.vless://399ce595-894d-4d40-add1-7d87f1a3bd10@qv2ray.net:50288?type=kcp&seed=69f04be3-d64e-45a3-8550-af3172c63055#VLESSmKCPSeed",
            @"vless://399ce595-894d-4d40-add1-7d87f1a3bd10@qv2ray.net:50288?type=kcp&seed=69f04be3-d64e-45a3-8550-af3172c63055#VLESSmKCPSeed")]
        [DataRow(
            @"<td>vless://399ce595-894d-4d40-add1-7d87f1a3bd10@qv2ray.net:41971?type=kcp&headerType=wireguard&seed=69f04be3-d64e-45a3-8550-af3172c63055#VLESSmKCPSeedWG</td>",
            @"vless://399ce595-894d-4d40-add1-7d87f1a3bd10@qv2ray.net:41971?type=kcp&headerType=wireguard&seed=69f04be3-d64e-45a3-8550-af3172c63055#VLESSmKCPSeedWG")]
        [DataRow(
            @"### vless://399ce595-894d-4d40-add1-7d87f1a3bd10@%5B%3A%3A1%5D:50288?type=kcp&seed=69f04be3-d64e-45a3-8550-af3172c63055#VLESSmKCPSeed ###",
            @"vless://399ce595-894d-4d40-add1-7d87f1a3bd10@%5B%3A%3A1%5D:50288?type=kcp&seed=69f04be3-d64e-45a3-8550-af3172c63055#VLESSmKCPSeed")]
        public void ExtractVless0ShareLinksTest(string text, string expected)
        {
            // var encoded = Uri.EscapeDataString(expected);
            var links = ExtractLinks(text, VgcApis.Models.Datas.Enums.LinkTypes.vless);
            Assert.AreEqual(1, links.Count);
            Assert.AreEqual(expected, links[0]);
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
            var html = Misc.Utils.Fetch(url);
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
            var patchedUrl = Misc.Utils.PatchHref(url, relativeUrl);
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
            var result = Misc.Utils.TrimVersionString(version);
            Assert.AreEqual(expect, result);
        }

        [DataTestMethod]
        [DataRow("", "e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855")]
        [DataRow(null, "e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855")]
        [DataRow("1234", "03ac674216f3e15c761ee1a5e255f067953623c8b388b4459e13f978d7c846f4")]
        public void Sha256Test(string source, string expect)
        {
            var sha = Misc.Utils.SHA256(source);
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
            Assert.AreEqual(expect, Misc.Utils.AreEqual(a, b));
        }

        [DataTestMethod]
        [DataRow("https://www.baidu.com")]
        public void VisitWebPageSpeedTestTest(string url)
        {
            var r = VgcApis.Misc.Utils.TimedDownloadTest(url, -1, 1024 * 1024, -1);
            var time = r.Item1;
            Assert.AreEqual(SpeedtestTimeout, time);

            r = VgcApis.Misc.Utils.TimedDownloadTest(url, -1, -1, -1);
            time = r.Item1;
            Assert.AreEqual(true, time < SpeedtestTimeout);
        }

        [DataTestMethod]
        [DataRow(@"{}", "")]
        [DataRow(@"{v2raygcon:{env:['1','2']}}", "")]
        [DataRow(@"{v2raygcon:{env:{a:'1',b:2}}}", "a:1,b:2")]
        [DataRow(@"{v2raygcon:{env:{a:'1',b:'2'}}}", "a:1,b:2")]
        public void GetEnvVarsFromConfigTest(string json, string expect)
        {
            var j = JObject.Parse(json);
            var env = Misc.Utils.GetEnvVarsFromConfig(j);
            var strs = env.OrderBy(p => p.Key).Select(p => p.Key + ":" + p.Value);
            var r = string.Join(",", strs);

            Assert.AreEqual(expect, r);
        }


        [TestMethod]
        public void CreateDeleteAppFolderTest()
        {
            var appFolder = Misc.Utils.GetSysAppDataFolder();
            Assert.AreEqual(false, string.IsNullOrEmpty(appFolder));

            // do not run these tests 
            // Misc.Utils.CreateAppDataFolder();
            // Assert.AreEqual(true, Directory.Exists(appFolder));
            // Misc.Utils.DeleteAppDataFolder();
            // Assert.AreEqual(false, Directory.Exists(appFolder));
        }

        [DataTestMethod]
        [DataRow(@"{}", "a", "abc", @"{'a':'abc'}")]
        [DataRow(@"{'a':{'b':{'c':1234}}}", "a.b.c", "abc", @"{'a':{'b':{'c':'abc'}}}")]
        public void SetValueStringTest(string json, string path, string value, string expect)
        {
            var r = JObject.Parse(json);
            var e = JObject.Parse(expect);
            Misc.Utils.TrySetValue<string>(r, path, value);
            Assert.AreEqual(true, JObject.DeepEquals(e, r));
        }

        [DataTestMethod]
        [DataRow(@"{}", "a", 1, @"{'a':1}")]
        [DataRow(@"{'a':{'b':{'c':1234}}}", "a.b.c", 5678, @"{'a':{'b':{'c':5678}}}")]
        public void SetValueIntTest(string json, string path, int value, string expect)
        {
            var r = JObject.Parse(json);
            var e = JObject.Parse(expect);
            Misc.Utils.TrySetValue<int>(r, path, value);
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
            var array = Misc.Utils.Str2JArray(value);
            var str = Misc.Utils.JArray2Str(array);
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
            Assert.AreEqual(expect, VgcApis.Misc.Utils.Str2Int(value));
        }

        [TestMethod]
        public void GetLocalCoreVersion()
        {
            var core = new V2RayGCon.Libs.V2Ray.Core(Services.Settings.Instance);
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
            var json = Services.Cache.Instance.
                tpl.LoadMinConfig();
            Assert.AreEqual(default(bool), GetValue<bool>(json, "log.loglevel"));
        }

        [TestMethod]
        public void GetValue_GetStringNotExist_ReturnNull()
        {
            var json = Services.Cache.Instance.
                tpl.LoadMinConfig();
            Assert.AreEqual(string.Empty, GetValue<string>(json, "log.keyNotExist"));
        }

        [TestMethod]
        public void GetValue_KeyNotExist_ReturnDefault()
        {
            var json = Services.Cache.Instance.
                tpl.LoadMinConfig();
            var value = Misc.Utils.GetValue<int>(json, "log.key_not_exist");
            Assert.AreEqual(default(int), value);
        }

        [TestMethod]
        public void ConfigResource_Validate()
        {
            foreach (var config in Misc.Utils.TestingGetResourceConfigJson())
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
        public void ExtractLinks_HttpLink()
        {
            var html = "http://abc.com https://def.com";

            var httpLinks = Misc.Utils.ExtractLinks(html,
                VgcApis.Models.Datas.Enums.LinkTypes.http);

            var httpsLinks = Misc.Utils.ExtractLinks(html,
                VgcApis.Models.Datas.Enums.LinkTypes.https);

            Assert.AreEqual(2, httpLinks.Count());
            Assert.AreEqual(2, httpsLinks.Count());
        }


        [DataTestMethod]
        [DataRow("ss://ZHVtbXkwMA==", "ss://ZHVtbXkwMA==")]
        [DataRow("ss://ZHVtbXkwMA", "ss://ZHVtbXkwMA")]
        [DataRow("ss://ZHVtbXkwMA===============", "ss://ZHVtbXkwMA===============")]
        [DataRow("ss://ZHVtbXkwMA==#abc.%20&_-中文", "ss://ZHVtbXkwMA==#abc.%20&_-")]
        [DataRow("ss://ZHVtbXkwMA==#", "ss://ZHVtbXkwMA==#")]
        [DataRow("ss://ZHVtbXkwMA==#abc.%20&中_-文", "ss://ZHVtbXkwMA==#abc.%20&")]
        public void ExtractLinks_FromString(string source, string expect)
        {
            var result = ExtractLinks(source, VgcApis.Models.Datas.Enums.LinkTypes.ss);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(expect, result[0]);
        }

        [TestMethod]
        public void ExtractLinks_FromLinksTxt()
        {
            var content = TestConst.links;
            var links = Misc.Utils.ExtractLinks(content, VgcApis.Models.Datas.Enums.LinkTypes.vmess);
            Assert.AreEqual(2, links.Count);
        }

        [TestMethod]
        public void ExtractLink_FromEmptyString_Return_EmptyList()
        {
            var content = "";
            var links = Misc.Utils.ExtractLinks(content, VgcApis.Models.Datas.Enums.LinkTypes.vmess);
            Assert.AreEqual(0, links.Count);
        }

        [TestMethod]
        public void GetRemoteCoreVersions()
        {
            // skip this time consuming test 
            // List<string> versions = Misc.Utils.GetCoreVersions(-1);
            // Assert.AreEqual(true, versions.Count > 0);
        }

        [TestMethod]
        public void GetVGCVersions()
        {
            // skip this time consuming test 
            //var version = Misc.Utils.GetLatestVGCVersion();
            //Assert.AreNotEqual(string.Empty, version);
        }
    }
}
