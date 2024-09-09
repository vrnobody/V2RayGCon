using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace VgcApis.Libs.Tasks
{
    internal static class MrePool
    {
        static int maxSize = 50;
        static ConcurrentQueue<ManualResetEvent> pool = new ConcurrentQueue<ManualResetEvent>();

        static MrePool() { }

        #region public methods
        public static int Count
        {
            get => pool.Count;
        }

        public static ManualResetEvent Rent()
        {
            if (pool.TryDequeue(out var mre))
            {
                return mre;
            }
            return new ManualResetEvent(true);
        }

        public static ManualResetEvent Rent(bool signaled)
        {
            var mre = Rent();
            try
            {
                if (signaled)
                {
                    mre.Set();
                }
                else
                {
                    mre.Reset();
                }
            }
            catch { }
            return mre;
        }

        public static void Return(ManualResetEvent mre)
        {
            if (mre == null)
            {
                return;
            }

            if (pool.Count > maxSize)
            {
                try
                {
                    mre.Dispose();
                }
                catch { }
            }
            else
            {
                pool.Enqueue(mre);
            }
        }
        #endregion
    }
}
