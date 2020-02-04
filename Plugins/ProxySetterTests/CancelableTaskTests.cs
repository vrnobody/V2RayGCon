using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace ProxySetterTests
{
    [TestClass]
    public class CancelableTaskTests
    {
        void Worker()
        {
            while (true)
            {
                //Console.WriteLine("hello");
                Wait(300);
            }
        }

        void Wait(int milSeconds)
        {
            Task.Delay(milSeconds).Wait();
        }

        [TestMethod]
        public void RunTaskTest()
        {
#if DEBUG
            var cancelableTask = new ProxySetter.Libs.Sys.CancelableTask(Worker);
            cancelableTask.Start();
            Wait(1000);
            cancelableTask.Stop();
            Wait(1000);
            cancelableTask.Start();
            Wait(1000);
            cancelableTask.Stop();
            Wait(1000);
#endif
        }
    }
}
