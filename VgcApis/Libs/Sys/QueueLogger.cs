using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VgcApis.Libs.Sys
{
    sealed public class QueueLogger :
        BaseClasses.Disposable
    {
        long updateTimestamp = DateTime.Now.Ticks;
        Queue<string> logCache = new Queue<string>();
        object logWriteLocker = new object();

        public QueueLogger() { }

        #region public methods
        public void Reset()
        {
            lock (logWriteLocker)
            {
                logCache = new Queue<string>();
                updateTimestamp = DateTime.Now.Ticks;
            }
        }

        public long GetTimestamp() => updateTimestamp;

        public void Log(string message)
        {
            lock (logWriteLocker)
            {
                logCache.Enqueue(message ?? @"");
                updateTimestamp = DateTime.Now.Ticks;
                TrimLogCache();
            }
        }

        public string GetLogAsString(bool addNewLineAtTheEnd)
        {
            lock (logWriteLocker)
            {
                var sb = new StringBuilder();
                foreach (var line in logCache)
                {
                    sb.AppendLine(line);
                }
                if (addNewLineAtTheEnd)
                {
                    sb.AppendLine();
                }
                return sb.ToString();
            }
        }
        #endregion

        #region private methods
        void TrimLogCache()
        {
            var count = logCache.Count();
            if (count < Models.Consts.Libs.MaxCacheLoggerLineNumber)
            {
                return;
            }

            var len = count - Models.Consts.Libs.MinCacheLoggerLineNumber;
            for (int i = 0; i < len; i++)
            {
                logCache.Dequeue();
            }
        }

        #endregion

        #region protected methods
        protected override void Cleanup() { }
        #endregion

    }
}
