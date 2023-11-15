using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace VgcApis.Libs.Tasks
{
    public class TicketPool : IDisposable
    {
        List<ManualResetEventSlim> waitQ = new List<ManualResetEventSlim>();
        int count = 0;
        int size = 0;
        ManualResetEventSlim emptyWaiter = new ManualResetEventSlim(true);

        public TicketPool()
            : this(0) { }

        public TicketPool(int capacity)
        {
            if (capacity < 0)
            {
                throw new ArgumentOutOfRangeException("Capacity should be a positve number.");
            }
            this.size = capacity;
        }

        #region properties

        #endregion

        #region public methods

        public void WaitUntilEmpty()
        {
            emptyWaiter.Wait();
        }

        public void WaitOne()
        {
            if (TryTakeOne())
            {
                return;
            }

            if (isDisposed)
            {
                return;
            }

            var mev = new ManualResetEventSlim(false);
            lock (waitQ)
            {
                waitQ.Add(mev);
            }
            mev.Wait();
        }

        public bool WaitOne(int ms)
        {
            if (TryTakeOne())
            {
                return true;
            }

            if (ms == 0)
            {
                return false;
            }

            if (isDisposed)
            {
                return false;
            }

            var mev = new ManualResetEventSlim(false);
            lock (waitQ)
            {
                waitQ.Add(mev);
            }

            if (mev.Wait(ms))
            {
                return true;
            }

            lock (waitQ)
            {
                if (mev.Wait(0))
                {
                    return true;
                }
                waitQ.Remove(mev);
            }
            return false;
        }

        public bool TryTakeOne()
        {
            return TryTake(1);
        }

        public void Return(int num)
        {
            if (num < 1)
            {
                throw new ArgumentOutOfRangeException("Num should be a positve number.");
            }

            Interlocked.Add(ref count, -1 * num);

            Misc.Utils.RunInBgSlim(() =>
            {
                CheckWaitQ();
                if (count < 1)
                {
                    emptyWaiter.Set();
                }
            });
        }

        public void ReturnOne() => Return(1);

        public bool TryTake(int num)
        {
            if (num < 1)
            {
                throw new ArgumentOutOfRangeException("Num should be a positve number.");
            }

            var n = Interlocked.Add(ref count, num);
            if (n <= size)
            {
                emptyWaiter.Reset();
                return true;
            }
            Interlocked.Add(ref count, -1 * num);
            return false;
        }

        public bool IsEmpty() => count >= size;

        public int Count() => count;

        public int GetWaitQueueSize() => waitQ.Count();

        public int GetPoolSize() => size;

        public void SetPoolSize(int capacity)
        {
            if (capacity < 0)
            {
                throw new ArgumentOutOfRangeException("Capacity should be a positve number.");
            }
            this.size = capacity;
        }

        #endregion

        #region private methods
        void CheckWaitQ()
        {
            if (count > size)
            {
                return;
            }

            lock (waitQ)
            {
                while (waitQ.Count() > 0 && TryTakeOne())
                {
                    waitQ[0].Set();
                    waitQ.RemoveAt(0);
                }
            }
        }
        #endregion

        #region IDisposable

        private bool disposedValue;
        bool isDisposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    isDisposed = true;
                    // TODO: 释放托管状态(托管对象)
                    lock (waitQ)
                    {
                        foreach (var w in waitQ)
                        {
                            w.Set();
                        }
                        waitQ.Clear();
                    }
                }

                // TODO: 释放未托管的资源(未托管的对象)并重写终结器
                // TODO: 将大型字段设置为 null
                disposedValue = true;
            }
        }

        // // TODO: 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
        // ~TicketPool()
        // {
        //     // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion

        #region protected methods

        #endregion
    }
}
