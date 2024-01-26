using System;
using System.Collections.Generic;
using System.Text;

namespace VgcApis.Libs.Sys
{
    public sealed class QueueLogger : BaseClasses.Disposable
    {
        long logTimestamp = DateTime.Now.Ticks;

        readonly int maxLineNumber;
        readonly int minLineNumber;

        readonly object logLock = new object();
        Queue<string> logs = new Queue<string>();

        public QueueLogger()
            : this(
                Models.Consts.Libs.MinCacheLoggerLineNumber,
                Models.Consts.Libs.MaxCacheLoggerLineNumber
            ) { }

        public QueueLogger(int minLineNumber, int maxLineNumber)
        {
            this.minLineNumber = minLineNumber;
            this.maxLineNumber = maxLineNumber;
        }

        #region public methods
        public int Count() => logs.Count;

        public void Clear()
        {
            lock (logLock)
            {
                logs = new Queue<string>();
            }
            UpdateLogTimestamp();
        }

        public long GetTimestamp() => logTimestamp;

        public void Log(string message)
        {
            lock (logLock)
            {
                logs.Enqueue(message ?? @"");
            }
            UpdateLogTimestamp();
            TrimLogCache();
        }

        public string GetLogAsString(bool hasNewLineAtTheEnd)
        {
            return CachedGetLogAsString(hasNewLineAtTheEnd);
        }

        #endregion

        #region private methods
        void UpdateLogTimestamp()
        {
            lock (logStrCacheLock)
            {
                logTimestamp = DateTime.Now.Ticks;
                if (logTimestamp == logStrCacheTimestamp)
                {
                    logStrCacheTimestamp = -1;
                }
                if (logStrCache != string.Empty)
                {
                    logStrCache = string.Empty;
                }
                if (trimedLogStrCache != string.Empty)
                {
                    trimedLogStrCache = string.Empty;
                }
            }
        }

        string logStrCache = string.Empty;
        string trimedLogStrCache = string.Empty;
        long logStrCacheTimestamp = -1;
        readonly object logStrCacheLock = new object();

        string CachedGetLogAsString(bool hasNewLineAtTheEnd)
        {
            lock (logStrCacheLock)
            {
                if (logTimestamp != logStrCacheTimestamp)
                {
                    var sb = new StringBuilder();
                    lock (logLock)
                    {
                        foreach (var line in logs)
                        {
                            sb.AppendLine(line);
                        }
                    }
                    sb.AppendLine();
                    logStrCache = sb.ToString();
                    trimedLogStrCache = Misc.Utils.TrimTrailingNewLine(logStrCache);
                    logStrCacheTimestamp = logTimestamp;
                }
            }
            return hasNewLineAtTheEnd ? logStrCache : trimedLogStrCache;
        }

        readonly Tasks.Bar bar = new Tasks.Bar();

        void TrimLogCache()
        {
            if (!bar.Install())
            {
                return;
            }

            var count = logs.Count;
            if (count < maxLineNumber)
            {
                bar.Remove();
                return;
            }

            lock (logLock)
            {
                var num = logs.Count - minLineNumber;
                for (int i = 0; i < num; i++)
                {
                    logs.Dequeue();
                }
            }
            bar.Remove();
        }

        #endregion

        #region protected methods
        protected override void Cleanup() { }
        #endregion
    }
}
