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
            var waiters = new List<Task>();
            for (int i = 0; i < 3; i++)
            {
                var id = i;
                var task = Task.Run(() =>
                {
                    logs.Enqueue($"Waiter[{id}] waiting");
                    pool.WaitUntilEmpty();
                    logs.Enqueue($"Waiter[{id}] done");
                });
                waiters.Add(task);
            }

            Assert.IsTrue(pool.TryTake(4));
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

            Thread.Sleep(1000);
            foreach (var waiter in waiters)
            {
                Assert.IsFalse(waiter.IsCompleted);
            }

            Task.WaitAll(customers.ToArray());
            pool.ReturnOne();
            Thread.Sleep(1000);
            foreach (var waiter in waiters)
            {
                Assert.IsTrue(waiter.IsCompleted);
            }
            Task.WaitAll(waiters.ToArray());

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
                            Console.WriteLine("take 2");
                            Thread.Sleep(500);
                            pool.Return(2);
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
                            Console.WriteLine("take one");
                            Thread.Sleep(500);
                            pool.ReturnOne();
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
                            Console.WriteLine("wait 200");
                            Thread.Sleep(200);
                            pool.ReturnOne();
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
                        Console.WriteLine("wait()");
                        Thread.Sleep(200);
                        pool.ReturnOne();
                    }
                });
                tasks.Add(task);
            }

            Thread.Sleep(2000);
            Assert.IsTrue(pool.GetWaitQueueSize() > 0);

            Task.WaitAll(tasks.ToArray());
            Assert.IsTrue(pool.Count() == 0);
            Assert.IsTrue(pool.GetWaitQueueSize() == 0);
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
            pool.Dispose();
            Task.WaitAll(tasks.ToArray());
            Assert.IsTrue(pool.Count() > 0);
            Assert.IsTrue(pool.GetWaitQueueSize() == 0);
        }
#endif

        [TestMethod]
        public void SingleThreadTests()
        {
            var pool = new VgcApis.Libs.Tasks.TicketPool(10);
            Assert.IsTrue(pool.TryTake(5));
            Assert.IsFalse(pool.IsEmpty());
            Assert.IsTrue(pool.Count() == 5);

            Assert.IsTrue(pool.TryTake(5));
            Assert.IsTrue(pool.Count() == 10);
            Assert.IsTrue(pool.IsEmpty());

            Assert.IsTrue(pool.GetPoolSize() == 10);
            pool.SetPoolSize(15);
            Assert.IsFalse(pool.IsEmpty());
            Assert.IsTrue(pool.GetPoolSize() == 15);
            pool.WaitOne();
            Assert.IsTrue(pool.Count() == 11);
            pool.SetPoolSize(10);
            Assert.IsTrue(pool.IsEmpty());
            Assert.IsFalse(pool.WaitOne(10));
            pool.ReturnOne();
            Assert.IsTrue(pool.GetPoolSize() == 10);
            Assert.IsTrue(pool.Count() == 10);

            Assert.IsFalse(pool.TryTakeOne());
            Assert.IsTrue(pool.IsEmpty());

            Assert.IsFalse(pool.WaitOne(10));
            pool.ReturnOne();
            Assert.IsTrue(pool.WaitOne(0));
            pool.Return(10);
            Assert.IsTrue(pool.Count() == 0);
        }
    }
}
