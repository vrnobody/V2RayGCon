using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace VgcApisTests.LibsTests
{
    [TestClass]
    public class BarTests
    {
        [TestMethod]
        public void SingleThreadTest()
        {
            var bar = new VgcApis.Libs.Tasks.Bar();
            Assert.IsTrue(bar.Install());
            Assert.IsFalse(bar.Install());
            bar.Remove();
            Assert.IsTrue(bar.Install());
        }

        [TestMethod]
        public void MultiThreadsTest()
        {
            var bar = new VgcApis.Libs.Tasks.Bar();

            var num = 10;
            var counter = 0;
            var tasks = new List<Task>();
            for (int i = 0; i < num; i++)
            {
                tasks.Add(
                    VgcApis.Misc.Utils.RunInBackground(() =>
                    {
                        VgcApis.Misc.Utils.Sleep(500);
                        for (int j = 0; j < 20; j++)
                        {
                            VgcApis.Misc.Utils.Sleep(10);
                            if (!bar.Install())
                            {
                                continue;
                            }
                            counter++;
                            bar.Remove();
                        }
                    })
                );
            }

            Task.WhenAll(tasks.ToArray()).Wait();
            Assert.IsTrue(counter > 20);
        }
    }
}
