using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace VgcApisTests.LibsTests
{
    [TestClass]
    public class RoutineTests
    {
#if DEBUG
        // Sleep() is not reliable in GitHub action runner.
        [TestMethod]
        public void SingleThreadRoutinTest()
        {
            var counter = 0;
            void add()
            {
                counter++;
            }
            var routin = new VgcApis.Libs.Tasks.Routine(add, 500);
            Assert.AreEqual(0, counter);
            routin.Stop();
            VgcApis.Misc.Utils.Sleep(600);
            Assert.AreEqual(0, counter);
            routin.Restart();
            VgcApis.Misc.Utils.Sleep(600);
            Assert.AreEqual(1, counter);
            VgcApis.Misc.Utils.Sleep(500);
            Assert.AreEqual(2, counter);
            routin.Stop();
            VgcApis.Misc.Utils.Sleep(600);
            Assert.AreEqual(2, counter);
            routin.Restart();
            VgcApis.Misc.Utils.Sleep(600);
            Assert.AreEqual(3, counter);
            VgcApis.Misc.Utils.Sleep(500);
            Assert.AreEqual(4, counter);
            routin.Dispose();
            routin.Restart();
            VgcApis.Misc.Utils.Sleep(600);
            Assert.AreEqual(4, counter);
        }
#endif
    }
}
