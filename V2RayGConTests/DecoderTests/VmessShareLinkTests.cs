using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace V2RayGCon.Test.DecoderTests
{
    [TestClass]
    public class VmessShareLinkTests
    {
        List<Tuple<Model.Data.Vmess, string>> GenVmessTestData()
        {
            var data = new List<Tuple<Model.Data.Vmess, string>>();

            var vmessStandard = new Model.Data.Vmess
            {
                v = "2",
                add = "1.2.3.4",
                port = "1234",
                id = Guid.NewGuid().ToString(),
                ps = "standard",
            };

            var linkStandard = vmessStandard.ToVmessLink();

            var vmessNonStandard = new Model.Data.Vmess
            {
                v = "2",
                add = "1.2.3.4",
                port = "1234",
                id = Guid.NewGuid().ToString(),
                ps = "non-standard",
            };

            var linkNonStandard = vmessNonStandard.ToVmessLink().Replace(@"=", @"");

            data.Add(new Tuple<Model.Data.Vmess, string>(vmessStandard, linkStandard));
            data.Add(new Tuple<Model.Data.Vmess, string>(vmessNonStandard, linkNonStandard));
            return data;
        }

        [TestMethod]
        public void VmessDecoderTest()
        {
            var testData = GenVmessTestData();
            var testLinks = testData.Select(e => e.Item2).ToList();
            var subText = string.Join(Environment.NewLine, testLinks);

            var cache = V2RayGCon.Service.Cache.Instance;
            var vmessDecoder = new V2RayGCon.Service.ShareLinkComponents.VmessDecoder(cache);

            var vmessLinks = vmessDecoder.ExtractLinksFromText(subText);
            foreach (var vmessLink in vmessLinks)
            {
                Assert.IsTrue(testLinks.Contains(vmessLink));
            }
        }

        [TestMethod]
        public void ExtractVmessLinkTest()
        {
            var testData = GenVmessTestData();
            var vmessLinks = testData.Select(e => e.Item2).ToList();
            var subText = string.Join(Environment.NewLine, vmessLinks);

            var extracted = Lib.Utils.ExtractLinks(
                subText, VgcApis.Models.Datas.Enum.LinkTypes.vmess);

            Assert.AreEqual(testData.Count, extracted.Count);
            foreach (var link in extracted)
            {
                Assert.IsTrue(vmessLinks.Contains(link));
            }
        }

        [TestMethod]
        public void DecodeVmessTest()
        {
            var testData = GenVmessTestData();

            foreach (var data in testData)
            {
                var vmess = Lib.Utils.VmessLink2Vmess(data.Item2);
                Assert.IsTrue(vmess.Equals(data.Item1));
            }
        }

        [TestMethod]
        public void DecodeVmessFailTest()
        {
            var errorVmess = new V2RayGCon.Model.Data.Vmess
            {
                id = "1234",  // invalid GUID
                port = "1234",
                add = "1.2.3.4",
            };

            var vmessLink = errorVmess.ToVmessLink();
            var vmessDecode = Lib.Utils.VmessLink2Vmess(vmessLink);
            Assert.IsNull(vmessDecode);
        }
    }
}
