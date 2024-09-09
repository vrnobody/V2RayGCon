using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace VgcApisTests.LibsTests
{
    [TestClass]
    public class MrePoolTest
    {
        [TestMethod]
        public void MrePoolNormalTest()
        {
            var mre = VgcApis.Libs.Tasks.MrePool.Rent(true);
            Assert.AreEqual(0, VgcApis.Libs.Tasks.MrePool.Count);
            Assert.IsNotNull(mre);
            mre.WaitOne();

            VgcApis.Libs.Tasks.MrePool.Return(mre);
            Assert.AreEqual(1, VgcApis.Libs.Tasks.MrePool.Count);
            var mre2 = VgcApis.Libs.Tasks.MrePool.Rent(false);
            Assert.AreEqual(0, VgcApis.Libs.Tasks.MrePool.Count);
            Assert.IsNotNull(mre2);
            Assert.IsFalse(mre.WaitOne(0));
            mre2.Set();
            mre2.WaitOne();
            Assert.AreEqual(mre, mre2);
        }
    }
}
