using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace VgcApisTests.LibsTests
{
    [TestClass]
    public class WaiterTest
    {
        [TestMethod]
        public void WaiterNormalTest()
        {
            var w = new VgcApis.Libs.Tasks.Waiter();
            Assert.IsFalse(w.IsWaiting());
            w.Start();
            Assert.IsTrue(w.IsWaiting());
            Assert.IsFalse(w.Wait(100));
            Assert.IsTrue(w.IsWaiting());
            w.Stop();
            Assert.IsTrue(w.Wait());
            Assert.IsFalse(w.IsWaiting());
            w.Start();
            Assert.IsTrue(w.IsWaiting());
            Assert.IsFalse(w.Wait(100));
            Assert.IsTrue(w.IsWaiting());
            w.Dispose();
            Assert.IsTrue(w.Wait());
            Assert.IsFalse(w.IsWaiting());
        }
    }
}
