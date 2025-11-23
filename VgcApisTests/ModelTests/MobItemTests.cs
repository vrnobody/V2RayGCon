using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VgcApis.Models.Datas;

namespace VgcApisTests.ModelTests
{
    [TestClass]
    public class MobItemTests
    {
        [TestMethod]
        public void MobItemToShareLinkMetaDataTest()
        {
            var mob = new MobItem()
            {
                ver = "1",
                server = new List<string>() { "serv1", "1.2.3.4", "1234" },
                protocol = new List<string>() { "vless", "b961142d-f200-47d1-9f08-d358933a77be" },
                stream = new List<string>() { "ws", "/ws", "bing.com" },
                enc = new List<string>() { "tls", "baidu.com", "", "h2" },
            };

            var meta = mob.ToShareLinkMetaData();
            Assert.IsNotNull(meta);
            var mob2 = new MobItem(meta);
            Assert.IsNotNull(mob2);

            Assert.AreEqual(mob.ver, mob2.ver);

            void compare(List<string> src, List<string> dest)
            {
                Assert.AreEqual(src.Count, dest.Count);
                for (int i = 0; i < src.Count; i++)
                {
                    Assert.AreEqual(src[i], dest[i]);
                }
            }

            compare(mob.server, mob2.server);
            compare(mob.protocol, mob2.protocol);
            compare(mob.stream, mob2.stream);
            compare(mob.enc, mob2.enc);
        }
    }
}
