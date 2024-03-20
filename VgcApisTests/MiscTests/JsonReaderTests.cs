using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using VgcApisTests.Resources.Resx;
using static VgcApis.Misc.Utils;

namespace VgcApisTests.MiscTests
{
    [TestClass]
    public class JsonReaderTests
    {
        [TestMethod]
        public void GetAllOutboundTagsFromJsonTest()
        {
            var json = Encoding.UTF8.GetString(Jsons.Outbs3);
            var tags = GetAllOutboundTagsFromJson(json);
            Assert.AreEqual(3, tags.Count);
            Assert.AreEqual("agentout1", tags[0]);
            Assert.AreEqual("agentout1", tags[0]);
            Assert.AreEqual("agentout1", tags[0]);

            json = Encoding.UTF8.GetString(Jsons.RoutingRandom);
            tags = GetAllOutboundTagsFromJson(json);
            Assert.AreEqual(0, tags.Count);
        }

        [TestMethod]
        public void GetAllOutboundTagsFromJsonFailTest()
        {
            var tags = GetAllOutboundTagsFromJson("");
            Assert.AreEqual(0, tags.Count);

            tags = GetAllOutboundTagsFromJson(null);
            Assert.AreEqual(0, tags.Count);

            tags = GetAllOutboundTagsFromJson("123abc中文😀");
            Assert.AreEqual(0, tags.Count);
        }

        [TestMethod]
        public void ParseJsonIntoBasicConfigAndOutboundsTest()
        {
            var json = Encoding.UTF8.GetString(Jsons.Outbs3);
            var tp = ParseAndSplitOutboundsFromConfig(json, null);
            Assert.AreEqual("agentin", tp.Item1["inbounds"][0]["tag"]);
            Assert.AreEqual(false, tp.Item1.ContainsKey("outbounds"));
            Assert.AreEqual(3, tp.Item2.Count);
            var idx = tp.Item2[2].IndexOf("eab3ff21b3b5");
            Assert.IsTrue(idx > 0);
        }

        [TestMethod]
        public void ExtractSummaryTest()
        {
            var json = Encoding.UTF8.GetString(Jsons.Outbs3);
            var summary = ExtractSummaryFromJsonConfig(json);
            Assert.AreEqual("vless.ws@1.2.3.4", summary);
        }

        [TestMethod]
        public void GetMultipleInboundsInfoTest()
        {
            var json = Encoding.UTF8.GetString(Jsons.Inbs3);
            var inbsInfo = GetInboundsInfoFromJsonConfig(json);
            Assert.AreNotEqual(null, inbsInfo);
            Assert.AreEqual(3, inbsInfo.Count);
            var info = inbsInfo.Last();
            Assert.AreEqual("dokodemo-door", info.protocol);
            Assert.AreEqual("127.0.0.1", info.host);
            Assert.AreEqual(10085, info.port);

            info = inbsInfo.First();
            Assert.AreEqual("http", info.protocol);
            Assert.AreEqual("127.0.0.1", info.host);
            Assert.AreEqual(8080, info.port);
        }

        [TestMethod]
        public void GetOneInboundInfoTest()
        {
            var json = Encoding.UTF8.GetString(Jsons.Outbs3);
            var inbsInfo = GetInboundsInfoFromJsonConfig(json);
            Assert.AreNotEqual(null, inbsInfo);
            Assert.AreEqual(1, inbsInfo.Count);
            var first = inbsInfo[0];
            Assert.AreEqual("socks", first.protocol);
            Assert.AreEqual("127.0.0.1", first.host);
            Assert.AreEqual(1080, first.port);
        }

        [TestMethod]
        public void GetJsonPropertyTest()
        {
            var json = Encoding.UTF8.GetString(Jsons.Outbs3);
            var outbs = GetFirstJsonProperty<JArray>("outbounds", json);
            // 2 comments +  3 outbounds = 5
            Assert.AreEqual(5, outbs.Count);
        }
    }
}
