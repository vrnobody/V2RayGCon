using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

namespace VgcApis.Libs.Sys
{
    // https://docs.microsoft.com/en-us/dotnet/standard/io/how-to-open-and-append-to-a-log-file
    public class FileLogger
    {
        static string logFilename = @"";

        static readonly object writeLogLocker = new object();
        static readonly List<string> earlyLogs = new List<string>();
        static bool isReady = false;

        #region public method
        static public void Ready()
        {
            lock (writeLogLocker)
            {
                isReady = true;
                if (!Disabled())
                {
                    using (StreamWriter w = File.AppendText(logFilename))
                    {
                        foreach (var line in earlyLogs)
                        {
                            w.WriteLine(line);
                        }
                    }
                }
                earlyLogs.Clear();
            }
        }

        public static void SetLogFilename(string filename)
        {
            logFilename = filename;
        }

        public static void Raw(string message)
        {
            if (isReady && Disabled())
            {
                return;
            }

            lock (writeLogLocker)
            {
                if (!isReady)
                {
                    earlyLogs.Add(message);
                }
                else
                {
                    using (StreamWriter w = File.AppendText(logFilename))
                    {
                        w.WriteLine(message);
                    }
                }
            }
        }

        public static void Debug(string message)
        {
            AppendLog("Debug", message);
        }

        public static void Info(string message)
        {
            AppendLog("Info", message);
        }

        public static void Warn(string message)
        {
            AppendLog("Warn", message);
        }

        public static void Error(string message)
        {
            AppendLog("Error", message);
        }

        public static void DumpCallStack(string message)
        {
            var details = Misc.Utils.GetCurCallStack();
            Debug(message);
            Debug(details);
        }

        #endregion

        #region private method

        static bool Disabled()
        {
            return string.IsNullOrEmpty(logFilename);
        }

        static void AppendLog(string tag, string message)
        {
            var text = string.Format(
                "{0} [{1}] {2}",
                DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"),
                tag,
                message
            );

            Raw(text);
        }
        #endregion
    }
}
