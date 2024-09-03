using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace VgcApisTests.MiscTests
{
    [TestClass]
    public class LazyGuyTests
    {
#if DEBUG
        [TestMethod]
        public void LazyGuyChainedTaskTest()
        {
            var str = "";

            void secTask(Action done)
            {
                Task.Delay(200).Wait();
                str += "2";
                done();
            }

            void firstTask(Action done)
            {
                Task.Delay(200).Wait();
                str += "1";
                secTask(done);
            }

            var alex = new VgcApis.Libs.Tasks.LazyGuy(firstTask, 1000, 300);

            str = "";
            alex.Deadline();
            alex.Throttle();
            alex.Deadline();
            alex.Throttle();
            Assert.AreEqual("", str);
            Task.Delay(3000).Wait();
            Assert.AreEqual("12", str);

            str = "";
            alex.Postpone();
            alex.Deadline();
            alex.Postpone();
            alex.Throttle();
            Task.Delay(500).Wait();
            Assert.AreEqual("", str);
            Task.Delay(3000).Wait();
            Assert.AreEqual("12", str);

            str = "";
            alex.Postpone();
            Task.Delay(500).Wait();
            alex.Postpone();
            Task.Delay(500).Wait();
            alex.Postpone();
            Task.Delay(500).Wait();
            alex.Postpone();
            Task.Delay(500).Wait();
            alex.Postpone();
            Assert.AreEqual("", str);
            Task.Delay(3000).Wait();
            Assert.AreEqual("12", str);

            str = "";
            alex.Deadline();
            alex.Deadline();
            alex.Deadline();
            Assert.AreEqual("", str);
            Task.Delay(5000).Wait();
            Assert.AreEqual("12", str);

            str = "";
            alex.Deadline();
            alex.Deadline();
            alex.Deadline();
            alex.ForgetIt();
            Assert.AreEqual("", str);
            Task.Delay(3000).Wait();
            Assert.AreEqual("", str);
            alex.PickItUp();

            str = "";
            alex.Throttle();
            alex.Throttle();
            alex.Throttle();
            alex.ForgetIt();
            Assert.AreEqual("", str);
            Task.Delay(3000).Wait();
            var ok = str == "" || str == "12";
            Assert.IsTrue(ok);
            alex.PickItUp();

            str = "";
            alex.Throttle();
            Task.Delay(100).Wait();
            alex.Throttle();
            alex.Throttle();
            Assert.AreEqual("", str);
            Task.Delay(5000).Wait();
            Assert.AreEqual("1212", str);
        }
#endif

#if DEBUG
        [TestMethod]
        public void LazyGuySingleTaskTest()
        {
            // 这个测试有概率会失败。

            var str = "";

            void task()
            {
                Task.Delay(200).Wait();
                str += ".";
            }

            var adam = new VgcApis.Libs.Tasks.LazyGuy(task, 1000, 300);

            str = "";
            adam.Postpone();
            Task.Delay(500).Wait();
            Console.WriteLine("500 x 1");
            adam.Postpone();
            Task.Delay(500).Wait();
            Console.WriteLine("500 x 2");
            adam.Postpone();
            Task.Delay(500).Wait();
            Console.WriteLine("500 x 3");
            adam.Postpone();
            Task.Delay(500).Wait();
            Console.WriteLine("500 x 4");
            adam.Postpone();
            Assert.AreEqual("", str);
            Task.Delay(3000).Wait();
            Assert.AreEqual(".", str);

            str = "";
            adam.Deadline();
            Task.Delay(10).Wait();
            adam.Deadline();
            Task.Delay(10).Wait();
            adam.Deadline();
            Task.Delay(10).Wait();
            adam.Deadline();
            Task.Delay(10).Wait();
            adam.Deadline();
            Assert.AreEqual("", str);
            Task.Delay(3000).Wait();
            Assert.AreEqual(".", str);

            str = "";
            adam.Deadline();
            Task.Delay(3000).Wait();
            Assert.AreEqual(".", str);

            str = "";
            adam.Deadline();
            adam.ForgetIt();
            Task.Delay(3000).Wait();
            Assert.AreEqual("", str);
            adam.PickItUp();

            str = "";
            adam.Throttle();
            adam.ForgetIt();
            Task.Delay(1100).Wait();
            Assert.IsTrue(str.Length < 2);
            adam.PickItUp();

            str = "";
            adam.Throttle();
            Task.Delay(50).Wait(); // wait for task spin up
            adam.Throttle();
            adam.Throttle();
            adam.Throttle();
            adam.Throttle();
            Assert.IsTrue(str.Length < 2);
            Task.Delay(3000).Wait();
            Assert.AreEqual("..", str);

            str = "";
            adam.Throttle();
            Task.Delay(1000).Wait();
            Assert.AreEqual(".", str);
        }
#endif
    }
}
