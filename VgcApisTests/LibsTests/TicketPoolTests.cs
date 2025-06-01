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
                    pool.WaitUntilPoolIsFull();
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
            Assert.AreEqual(5, pool.GetMaxOccupiedTicketCount());

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
                            // Console.WriteLine($"Return(2) {pool}");
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
                            // Console.WriteLine($"ReturnOne() {pool}");
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
                            // Console.WriteLine($"ReturnOne() {pool}");
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
                        // Console.WriteLine($"ReturnOne() {pool}");
                    }
                });
                tasks.Add(task);
            }

            Thread.Sleep(2000);
            Task.WaitAll(tasks.ToArray());
            Assert.IsTrue(pool.GetOccupiedTicketCount() == 0);
            Assert.AreEqual(0, pool.GetWaitingQueueSize());
            Assert.AreEqual(5, pool.GetMaxOccupiedTicketCount());
        }
#endif

#if DEBUG
        [TestMethod]
        public void HighConcurrencyTests()
        {
            var pool = new VgcApis.Libs.Tasks.TicketPool(5);
            Console.WriteLine($"begin: {pool}");

            var threadNum = 20;
            var success = new int[threadNum];

            var cts = new CancellationTokenSource();

            Task.Run(() =>
            {
                var c = 0;
                while (!cts.Token.IsCancellationRequested)
                {
                    Console.WriteLine($"at {c++} sec: {pool}");
                    Thread.Sleep(1000);
                }
            });

            var tasks = new Task[threadNum];

            for (int i = 0; i < threadNum; i++)
            {
                var idx = i;
                tasks[idx] = Task.Run(() =>
                {
                    Thread.Sleep(1000);
                    while (!cts.Token.IsCancellationRequested)
                    {
                        if (pool.WaitOne(200))
                        {
                            success[idx]++;
                            Thread.Sleep(100);
                            pool.ReturnOne();
                        }
                        Thread.Sleep(100);
                    }
                });
            }

            Thread.Sleep(8000);
            Assert.AreEqual(5, pool.GetMaxOccupiedTicketCount());
            Assert.IsTrue(pool.GetOccupiedTicketCount() > 0);

            cts.Cancel();
            Task.WaitAll(tasks);
            Console.WriteLine($"finished: {pool}");
            Assert.AreEqual(0, pool.GetOccupiedTicketCount());
            Assert.AreEqual(0, pool.GetWaitingQueueSize());

            var total = 0;
            for (int i = 0; i < threadNum; i++)
            {
                var idx = i;
                var c = success[idx];
                total += c;
                Console.WriteLine($"task[{idx}]: {c}");
            }
            Assert.IsTrue(total > 0);
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
            Assert.IsTrue(pool.GetOccupiedTicketCount() == 5);
            Assert.IsTrue(pool.GetWaitingQueueSize() > 0);
            Assert.AreEqual(5, pool.GetMaxOccupiedTicketCount());
            pool.Dispose();
            Task.WaitAll(tasks.ToArray());
            Assert.IsTrue(pool.GetOccupiedTicketCount() > 0);
            Assert.IsTrue(pool.GetWaitingQueueSize() == 0);
            Assert.AreEqual(5, pool.GetMaxOccupiedTicketCount());
        }
#endif

        [TestMethod]
        public void SingleThreadTests()
        {
            var pool = new VgcApis.Libs.Tasks.TicketPool(10);
            Assert.IsTrue(pool.TryTake(5));
            Assert.IsFalse(pool.IsDrained());
            Assert.IsTrue(pool.GetOccupiedTicketCount() == 5);
            Assert.AreEqual(5, pool.GetMaxOccupiedTicketCount());

            Assert.IsTrue(pool.TryTake(5));
            Assert.IsTrue(pool.GetOccupiedTicketCount() == 10);
            Assert.IsTrue(pool.IsDrained());
            Assert.AreEqual(10, pool.GetMaxOccupiedTicketCount());

            Assert.IsFalse(pool.TryTake(1));
            Assert.IsFalse(pool.TryTake(1));
            Assert.IsFalse(pool.TryTake(1));
            Assert.IsTrue(pool.GetOccupiedTicketCount() == 10);
            Assert.IsTrue(pool.IsDrained());
            Assert.AreEqual(10, pool.GetMaxOccupiedTicketCount());

            Assert.IsTrue(pool.GetPoolSize() == 10);
            pool.SetPoolSize(15);
            Assert.IsFalse(pool.IsDrained());
            Assert.IsTrue(pool.GetPoolSize() == 15);
            pool.WaitOne();
            Assert.IsTrue(pool.GetOccupiedTicketCount() == 11);
            pool.SetPoolSize(10);
            Assert.IsTrue(pool.IsDrained());
            Assert.IsFalse(pool.WaitOne(10));
            pool.ReturnOne();
            Assert.IsTrue(pool.GetPoolSize() == 10);
            Assert.IsTrue(pool.GetOccupiedTicketCount() == 10);

            Assert.IsFalse(pool.TryTakeOne());
            Assert.IsTrue(pool.IsDrained());

            Assert.IsFalse(pool.WaitOne(10));
            pool.ReturnOne();
            Assert.IsTrue(pool.WaitOne(0));
            pool.Return(10);
            Assert.IsTrue(pool.GetOccupiedTicketCount() == 0);

            Assert.AreEqual(11, pool.GetMaxOccupiedTicketCount());
        }
    }
}
