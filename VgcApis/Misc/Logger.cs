using System;
using System.Runtime.CompilerServices;

namespace VgcApis.Misc
{
    public static class Logger
    {
        static readonly int capacity = Models.Consts.Libs.MainLoggerCacheLines;
        static readonly Libs.Sys.QueueLogger qLogger = new Libs.Sys.QueueLogger(capacity);

        static Logger() { }

        #region public methods
        public static long GetTimestamp() => qLogger.GetTimestamp();

        public static string GetContent() => qLogger.GetLogAsString(true);

        public static string GetTrimedContent() => qLogger.GetLogAsString(false);

        public static void Log(params string[] contents) => qLogger.Log(string.Join(" ", contents));

        [System.Diagnostics.Conditional("DEBUG")]
        public static void Debug(
            string contents,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0
        )
        {
            qLogger.Log(
                $"(debug) {DateTime.Now} {sourceFilePath}(#{sourceLineNumber}) {memberName}()\n{contents}"
            );
        }
        #endregion
    }
}
