using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace VgcApisTests.LibsTests
{
    [TestClass]
    public class TicketPoolTests
    {
#if DEBUG
        [TestMethod]
        public void MultiThreadWaitEmptyTest()
        {
            var logs = new ConcurrentQueue<string>();

            var pool = new VgcApis.Libs.Tasks.TicketPool(5);

            Assert.IsTrue(pool.TryTakeOne());
            logs.Enqueue($"TryTakeOne() {pool}");
            var waiters = new List<Task>();
            for (int i = 0; i < 3; i++)
            {
                var id = i;
                var task = Task.Run(() =>
                {
                    logs.Enqueue($"Waiter[{id}] waiting");
                    pool.WaitUntilRecovery();
                    logs.Enqueue($"Waiter[{id}] done");
                });
                waiters.Add(task);
            }

            Assert.IsTrue(pool.TryTake(4));
            logs.Enqueue($"TryTake(4) {pool}");
            var customers = new List<Task>();
            for (int i = 0; i < 10; i++)
            {
                var id = i;
                var task = Task.Run(() =>
                {
                    logs.Enqueue($"Customer[{id}] come");
                    var count = 0;
                    while (count < 20)
                    {
                        Thread.Sleep(200);
                        if (pool.TryTakeOne())
                        {
                            count++;
                            pool.ReturnOne();
                        }
                    }
                    logs.Enqueue($"Customer[{id}] leave");
                });
                customers.Add(task);
            }

            Thread.Sleep(4000);
            foreach (var customer in customers)
            {
                Assert.IsFalse(customer.IsCompleted);
            }
            foreach (var waiter in waiters)
            {
                Assert.IsFalse(waiter.IsCompleted);
            }

            logs.Enqueue("begin to serve");

            pool.Return(4);
            logs.Enqueue($"Return(4) {pool}");

            Thread.Sleep(1000);
            foreach (var waiter in waiters)
            {
                Assert.IsFalse(waiter.IsCompleted);
            }

            Task.WaitAll(customers.ToArray());
            pool.ReturnOne();
            logs.Enqueue($"ReturnOne() {pool}");
            Thread.Sleep(1000);
            foreach (var waiter in waiters)
            {
                Assert.IsTrue(waiter.IsCompleted);
            }
            Task.WaitAll(waiters.ToArray());
            Assert.AreEqual(5, pool.GetHistoryMaxSize());

            // logs is complete now
            foreach (var log in logs)
            {
                Console.WriteLine(log);
            }
        }
#endif

#if DEBUG
        [TestMethod]
        public void MultiThreadTests()
        {
            var pool = new VgcApis.Libs.Tasks.TicketPool(5);

            var tasks = new List<Task>();
            for (int i = 0; i < 3; i++)
            {
                var task = Task.Run(() =>
                {
                    for (int j = 0; j < 5; j++)
                    {
                        if (pool.TryTake(2))
                        {
                            Console.WriteLine($"TryTake(2), {pool}");
                            Thread.Sleep(500);
                            pool.Return(2);
                            Console.WriteLine($"Return(2) {pool}");
                        }
                    }
                });
                tasks.Add(task);
            }

            for (int i = 0; i < 3; i++)
            {
                var task = Task.Run(() =>
                {
                    for (int j = 0; j < 5; j++)
                    {
                        if (pool.TryTakeOne())
                        {
                            Console.WriteLine($"TryTakeOne() {pool}");
                            Thread.Sleep(500);
                            pool.ReturnOne();
                            Console.WriteLine($"ReturnOne() {pool}");
                        }
                    }
                });
                tasks.Add(task);
            }

            for (int i = 0; i < 5; i++)
            {
                var task = Task.Run(() =>
                {
                    for (int j = 0; j < 15; j++)
                    {
                        if (pool.WaitOne(200))
                        {
                            Console.WriteLine($"WaitOne(200) {pool}");
                            Thread.Sleep(200);
                            pool.ReturnOne();
                            Console.WriteLine($"ReturnOne() {pool}");
                        }
                    }
                });
                tasks.Add(task);
            }

            for (int i = 0; i < 5; i++)
            {
                var task = Task.Run(() =>
                {
                    for (int j = 0; j < 15; j++)
                    {
                        pool.WaitOne();
                        Console.WriteLine($"WaitOne() {pool}");
                        Thread.Sleep(200);
                        pool.ReturnOne();
                        Console.WriteLine($"ReturnOne() {pool}");
                    }
                });
                tasks.Add(task);
            }

            Thread.Sleep(2000);
            Assert.IsTrue(pool.GetWaitQueueSize() > 0);
            Task.WaitAll(tasks.ToArray());
            Assert.IsTrue(pool.Count() == 0);
            Assert.AreEqual(0, pool.GetWaitQueueSize());
            Assert.AreEqual(5, pool.GetHistoryMaxSize());
        }
#endif

#if DEBUG
        [TestMethod]
        public void MultiThreadDisposeTests()
        {
            var pool = new VgcApis.Libs.Tasks.TicketPool(5);
            var tasks = new List<Task>();

            for (int i = 0; i < 10; i++)
            {
                var task = Task.Run(() =>
                {
                    for (int j = 0; j < 15; j++)
                    {
                        pool.WaitOne();
                    }
                });

                tasks.Add(task);
            }
            Thread.Sleep(5000);
            Assert.IsTrue(pool.Count() == 5);
            Assert.IsTrue(pool.GetWaitQueueSize() > 0);
            Assert.AreEqual(5, pool.GetHistoryMaxSize());
            pool.Dispose();
            Task.WaitAll(tasks.ToArray());
            Assert.IsTrue(pool.Count() > 0);
            Assert.IsTrue(pool.GetWaitQueueSize() == 0);
            Assert.AreEqual(5, pool.GetHistoryMaxSize());
        }
#endif

        [TestMethod]
        public void SingleThreadTests()
        {
            var pool = new VgcApis.Libs.Tasks.TicketPool(10);
            Assert.IsTrue(pool.TryTake(5));
            Assert.IsFalse(pool.IsDrained());
            Assert.IsTrue(pool.Count() == 5);
            Assert.AreEqual(5, pool.GetHistoryMaxSize());

            Assert.IsTrue(pool.TryTake(5));
            Assert.IsTrue(pool.Count() == 10);
            Assert.IsTrue(pool.IsDrained());
            Assert.AreEqual(10, pool.GetHistoryMaxSize());

            Assert.IsFalse(pool.TryTake(1));
            Assert.IsFalse(pool.TryTake(1));
            Assert.IsFalse(pool.TryTake(1));
            Assert.IsTrue(pool.Count() == 10);
            Assert.IsTrue(pool.IsDrained());
            Assert.AreEqual(10, pool.GetHistoryMaxSize());

            Assert.IsTrue(pool.GetPoolSize() == 10);
            pool.SetPoolSize(15);
            Assert.IsFalse(pool.IsDrained());
            Assert.IsTrue(pool.GetPoolSize() == 15);
            pool.WaitOne();
            Assert.IsTrue(pool.Count() == 11);
            pool.SetPoolSize(10);
            Assert.IsTrue(pool.IsDrained());
            Assert.IsFalse(pool.WaitOne(10));
            pool.ReturnOne();
            Assert.IsTrue(pool.GetPoolSize() == 10);
            Assert.IsTrue(pool.Count() == 10);

            Assert.IsFalse(pool.TryTakeOne());
            Assert.IsTrue(pool.IsDrained());

            Assert.IsFalse(pool.WaitOne(10));
            pool.ReturnOne();
            Assert.IsTrue(pool.WaitOne(0));
            pool.Return(10);
            Assert.IsTrue(pool.Count() == 0);

            Assert.AreEqual(11, pool.GetHistoryMaxSize());
        }
    }
}
