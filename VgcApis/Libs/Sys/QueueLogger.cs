using System;
using System.Collections.Generic;
using System.Text;

namespace VgcApis.Libs.Sys
{
    public sealed class QueueLogger : BaseClasses.Disposable
    {
        long logTimestamp = DateTime.Now.Ticks;

        readonly int capacity;
        int tail = 0;

        readonly object logLock = new object();
        List<string> logs = new List<string>();

        public QueueLogger()
            : this(Models.Consts.Libs.DefaultLoggerCacheLines) { }

        public QueueLogger(int capacity)
        {
            this.capacity = capacity;
        }

        #region public methods
        public int Count() => logs.Count;

        public void Clear()
        {
            lock (logLock)
            {
                logs = new List<string>();
                tail = 0;
            }
            UpdateLogTimestamp();
        }

        public long GetTimestamp() => logTimestamp;

        public void Log(string message)
        {
            if (isDisposed)
            {
                return;
            }

            lock (logLock)
            {
                if (logs.Count < capacity)
                {
                    logs.Add(message ?? @"");
                }
                else
                {
                    logs[tail] = message ?? "";
                    tail = (tail + 1) % logs.Count;
                }
            }
            UpdateLogTimestamp();
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
                logStrCache = "";
                trimedLogStrCache = "";
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
                        for (int i = tail; i < logs.Count; i++)
                        {
                            sb.AppendLine(logs[i]);
                        }

                        for (int i = 0; i < tail; i++)
                        {
                            sb.AppendLine(logs[i]);
                        }
                    }
                    logStrCache = sb.ToString();
                    trimedLogStrCache = Misc.Utils.TrimTrailingNewLine(logStrCache);
                    logStrCacheTimestamp = logTimestamp;
                }
            }
            return hasNewLineAtTheEnd ? logStrCache : trimedLogStrCache;
        }

        #endregion
    }
}
