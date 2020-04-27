using System;
using System.Collections.Generic;
using System.Linq;

namespace VgcApis.Libs.Sys
{
    sealed public class QueueLogger :
        BaseClasses.Disposable
    {
        long updateTimestamp = DateTime.Now.Ticks;
        Queue<string> logCache = new Queue<string>();
        const int maxLogLineNumber = Models.Consts.Libs.MaxCacheLoggerLineNumber;
        object logWriteLocker = new object();

        public QueueLogger() { }

        #region public methods
        public void Reset()
        {
            lock (logWriteLocker)
            {
                logCache = new Queue<string>();
                listCacheUpdateTimestamp = -1;
                stringCacheUpdateTimestamp = -1;
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

        long stringCacheUpdateTimestamp = -1;
        string stringCache = "";
        public string GetLogAsString(bool addNewLineAtTheEnd)
        {
            lock (logWriteLocker)
            {
                if (stringCacheUpdateTimestamp != updateTimestamp)
                {
                    stringCache = string.Join(Environment.NewLine, logCache);
                    if (addNewLineAtTheEnd)
                    {
                        stringCache += Environment.NewLine;
                    }
                    stringCacheUpdateTimestamp = updateTimestamp;
                }
                return stringCache;
            }
        }

        long listCacheUpdateTimestamp = -1;
        List<string> listCache = new List<string>();
        public IReadOnlyCollection<string> GetLogAsList()
        {
            lock (logWriteLocker)
            {
                if (listCacheUpdateTimestamp != updateTimestamp)
                {
                    listCache = logCache.ToList();
                    listCacheUpdateTimestamp = updateTimestamp;
                }

                return listCache.AsReadOnly();
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
