using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace VgcApisTests
{
    [TestClass]
    public class Crc8Tests
    {
        [TestMethod]
        public void Crc8NormalTest()
        {
            var b1 = new byte[]
            {
                0,1,2,3
            };

            var crc1 = VgcApis.Libs.Streams.Crc8.ComputeChecksum(b1, 1);

            var b2 = new byte[]
            {
                12,1,2,3,crc1
            };

            var crc2 = VgcApis.Libs.Streams.Crc8.ComputeChecksum(b2, 1);

            Assert.AreEqual(0, crc2);

            var crc3 = VgcApis.Libs.Streams.Crc8.ComputeChecksum(b2);

            Assert.AreNotEqual(crc2, crc3);
        }

    }
}
