using System;
using System.Threading;

namespace VgcApis.Libs.Tasks
{
    public class Bar : IDisposable
    {
        readonly SemaphoreSlim mlocker = new SemaphoreSlim(1);
        private bool disposedValue;

        public Bar() { }

        public void Remove()
        {
            try
            {
                mlocker.Release();
            }
            catch { }
        }

        public bool Install()
        {
            try
            {
                return mlocker.Wait(0);
            }
            catch { }
            return false;
        }

        #region IDisposable
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)
                    mlocker.Dispose();
                }

                // TODO: 释放未托管的资源(未托管的对象)并重写终结器
                // TODO: 将大型字段设置为 null
                disposedValue = true;
            }
        }

        // // TODO: 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
        // ~Bar()
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
