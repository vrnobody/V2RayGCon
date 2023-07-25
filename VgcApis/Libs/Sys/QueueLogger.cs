using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VgcApis.Libs.Sys
{
    sealed public class QueueLogger :
        BaseClasses.Disposable
    {
        long updateTimestamp = DateTime.Now.Ticks;

        ConcurrentQueue<string> logCache = new ConcurrentQueue<string>();
        object logCacheWLock = new object();

        public QueueLogger() { }

        #region public methods
        public int Count() => logCache.Count;

        public void Clear()
        {
            lock (logCacheWLock)
            {
                logCache = new ConcurrentQueue<string>();
                updateTimestamp = DateTime.Now.Ticks;
            }
        }

        public long GetTimestamp() => updateTimestamp;

        public void Log(string message)
        {
            logCache.Enqueue(message ?? @"");
            updateTimestamp = DateTime.Now.Ticks;
            TrimLogCache();
        }

        public string GetLogAsString(bool hasNewLineAtTheEnd)
        {
            return CachedGetLogAsString(hasNewLineAtTheEnd);
        }

        #endregion

        #region private methods
        string strCache = string.Empty;
        string trimedStrCache = string.Empty;
        long strCacheTimestamp = -1;
        object strCacheLock = new object();

        string CachedGetLogAsString(bool hasNewLineAtTheEnd)
        {
            lock (strCacheLock)
            {
                if (updateTimestamp != strCacheTimestamp)
                {

                    var sb = new StringBuilder();
                    lock (logCacheWLock)
                    {
                        foreach (var line in logCache)
                        {
                            sb.AppendLine(line);
                        }
                    }
                    sb.AppendLine();
                    strCache = sb.ToString();
                    trimedStrCache = Misc.Utils.TrimTrailingNewLine(strCache);
                    strCacheTimestamp = updateTimestamp;
                }
            }
            return hasNewLineAtTheEnd ? strCache : trimedStrCache;
        }

        Tasks.Bar bar = new Tasks.Bar();
        void TrimLogCache()
        {
            if (!bar.Install())
            {
                return;
            }

            var count = logCache.Count;
            if (count < Models.Consts.Libs.MaxCacheLoggerLineNumber)
            {
                bar.Remove();
                return;
            }

            var len = count - Models.Consts.Libs.MinCacheLoggerLineNumber;
            for (int i = 0; i < len; i++)
            {
                logCache.TryDequeue(out _);
            }
            bar.Remove();
        }

        #endregion

        #region protected methods
        protected override void Cleanup() { }
        #endregion

    }
}
