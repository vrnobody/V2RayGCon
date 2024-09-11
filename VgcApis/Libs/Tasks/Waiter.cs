using System;
using System.Threading;

namespace VgcApis.Libs.Tasks
{
    public class Waiter : IDisposable
    {
        readonly object mreLocker = new object();
        ManualResetEvent mre = null;

        public Waiter() { }

        #region public methods
        public void Start()
        {
            // should not start after disposed
            if (disposedValue)
            {
                return;
            }

            lock (mreLocker)
            {
                if (mre == null)
                {
                    mre = MrePool.Rent(false);
                }
            }
        }

        public bool IsWaiting()
        {
            var mre = this.mre;
            try
            {
                if (mre != null)
                {
                    return !mre.WaitOne(0);
                }
            }
            catch { }
            return false;
        }

        public bool Wait(int ms)
        {
            var mre = this.mre;
            try
            {
                if (mre != null)
                {
                    return mre.WaitOne(ms);
                }
            }
            catch { }
            return true;
        }

        public void Wait()
        {
            var mre = this.mre;
            try
            {
                mre?.WaitOne();
            }
            catch { }
        }

        public void Stop()
        {
            ManualResetEvent mre = null;
            lock (mreLocker)
            {
                mre = this.mre;
                this.mre = null;
            }
            try
            {
                mre?.Set();
                MrePool.Return(mre);
            }
            catch { }
        }
        #endregion

        #region private methods

        #endregion

        #region IDisposable
        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)
                    Stop();
                }

                // TODO: 释放未托管的资源(未托管的对象)并重写终结器
                // TODO: 将大型字段设置为 null
                disposedValue = true;
            }
        }

        // // TODO: 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
        // ~ManualResetEventWrapper()
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
    }
}
