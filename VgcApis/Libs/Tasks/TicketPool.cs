using System;
using System.Threading;
using System.Threading.Tasks;

namespace VgcApis.Libs.Tasks
{
    public class TicketPool : IDisposable
    {
        int historyMaxTaken = 0;

        int taken = 0;
        int poolSize = 0;
        int queueSize = 0;

        readonly object queueLock = new object();
        ManualResetEvent queueWaiter = new ManualResetEvent(false);
        readonly ManualResetEvent freeWaiter = new ManualResetEvent(true);

        public TicketPool()
            : this(0) { }

        public TicketPool(int capacity)
        {
            if (capacity < 0)
            {
                throw new ArgumentOutOfRangeException("Capacity should be a positve number.");
            }
            this.poolSize = capacity;
        }

        #region public methods

        public override string ToString()
        {
            return $"Total: {poolSize} Remain: {poolSize - taken} Full: {IsFull()} Drained: {IsDrained()} Waiting: {queueSize} ";
        }

        public void WaitUntilPoolIsFull()
        {
            freeWaiter.WaitOne();
        }

        public void WaitOne()
        {
            Interlocked.Add(ref queueSize, 1);
            while (!TryTakeOne() && !isDisposed)
            {
                GetTicketWaiter().WaitOne();
            }
            Interlocked.Add(ref queueSize, -1);
        }

        public bool WaitOne(int ms)
        {
            Interlocked.Add(ref queueSize, 1);
            var now = DateTime.Now;
            while (!TryTakeOne() && !isDisposed)
            {
                var remain = ms - (int)(DateTime.Now - now).TotalMilliseconds;
                if (remain < 1)
                {
                    Interlocked.Add(ref queueSize, -1);
                    return false;
                }
                GetTicketWaiter().WaitOne(remain);
            }
            Interlocked.Add(ref queueSize, -1);
            return !isDisposed;
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

            var n = Interlocked.Add(ref taken, -1 * num);
            if (n < 1)
            {
                freeWaiter.Set();
            }

            ManualResetEvent w;
            lock (queueLock)
            {
                w = this.queueWaiter;
                this.queueWaiter = new ManualResetEvent(false);
            }
            w.Set();
        }

        public void ReturnOne() => Return(1);

        public bool TryTake(int num)
        {
            if (num < 1)
            {
                throw new ArgumentOutOfRangeException("Num should be a positve number.");
            }

            if (taken >= poolSize)
            {
                return false;
            }

            var n = Interlocked.Add(ref taken, num);
            if (n <= poolSize)
            {
                freeWaiter.Reset();
                MarkDownMaxSize(n);
                return true;
            }
            Interlocked.Add(ref taken, -1 * num);
            return false;
        }

        public bool IsFull() => taken < 1;

        public bool IsDrained() => taken >= poolSize;

        public int GetOccupiedTicketCount() => taken;

        public int GetMaxOccupiedTicketCount() => historyMaxTaken;

        public int GetWaitingQueueSize() => queueSize;

        public int GetPoolSize() => poolSize;

        public void SetPoolSize(int capacity)
        {
            if (capacity < 0)
            {
                throw new ArgumentOutOfRangeException("Capacity should be a positve number.");
            }
            this.poolSize = capacity;
        }

        #endregion

        #region private methods
        ManualResetEvent GetTicketWaiter()
        {
            lock (queueLock)
            {
                return this.queueWaiter;
            }
        }

        void MarkDownMaxSize(int n)
        {
            var v = historyMaxTaken;
            while (n > v)
            {
                v = Interlocked.CompareExchange(ref historyMaxTaken, n, v);
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
                    GetTicketWaiter().Set();
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
