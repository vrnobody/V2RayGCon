﻿using System.Collections.Concurrent;
using System.Threading;

namespace VgcApis.Libs.Tasks
{
    internal static class MrePool
    {
        static readonly int maxSize = 80;
        static readonly ConcurrentQueue<ManualResetEvent> pool =
            new ConcurrentQueue<ManualResetEvent>();

        static MrePool() { }

        #region public methods
        public static int Count
        {
            get => pool.Count;
        }

        public static ManualResetEvent Rent(bool signaled)
        {
            if (pool.TryDequeue(out var mre))
            {
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
                    return mre;
                }
                catch { }
            }
            return new ManualResetEvent(signaled);
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