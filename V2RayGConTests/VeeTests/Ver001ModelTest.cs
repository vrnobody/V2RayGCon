using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace V2RayGCon.Test.VeeTests
{
    [TestClass]
    public class Ver001ModelTest
    {
        [TestMethod]
        public void Ver001NormalTest()
        {
            for (int i = 0; i < 10; i++)
            {
                var v1 = new Models.VeeShareLinks.Vmess0b
                {
                    address = "::1",
                    alias = "中文abc 123",
                    description = "描述abc123",
                    tlsType = "tls",
                    tlsServName = "baidu.com",
                    streamParam1 = "/v2ray?#abc",
                    streamParam2 = VgcApis.Misc.Utils.RandomHex(7),
                    streamParam3 = VgcApis.Misc.Utils.RandomHex(7),
                    streamType = VgcApis.Misc.Utils.RandomHex(7),
                    port = 123,
                    uuid = Guid.NewGuid(),
                };
                var bytes = v1.ToBytes();
                var v2 = new Models.VeeShareLinks.Vmess0b(bytes);
                Assert.AreEqual(true, v1.EqTo(v2));
            }
        }

    }
}
