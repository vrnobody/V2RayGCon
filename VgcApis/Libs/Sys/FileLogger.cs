﻿using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace VgcApis.Libs.Sys
{
    // https://docs.microsoft.com/en-us/dotnet/standard/io/how-to-open-and-append-to-a-log-file
    public class FileLogger
    {
        public static string LogFilename = @"";

        static readonly object writeLogLocker = new object();

        #region public method
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

        public static void Dump()
        {

            using (StreamReader r = File.OpenText(LogFilename))
            {
                string line;
                while ((line = r.ReadLine()) != null)
                {
                    Console.WriteLine(line);
                }
            }

        }

        static readonly object dumpCsLocker = new object();
        static public void DumpCallStack(string message)
        {
            Debug(message);
            // frame 1, true for source info
            lock (dumpCsLocker)
            {
                StackTrace stack = new StackTrace();
                foreach (var frame in stack.GetFrames())
                {
                    var method = frame.GetMethod();
                    var mn = Misc.Utils.GetFriendlyMethodDeclareInfo(method as MethodInfo);
                    Debug($" -> {mn}");
                }
            }
        }
        #endregion

        #region private method


        static void AppendLog(string prefix, string message)
        {
            if (string.IsNullOrEmpty(LogFilename))
            {
                return;
            }

            lock (writeLogLocker)
            {
                using (StreamWriter w = File.AppendText(LogFilename))
                {
                    w.WriteLine("[{0}] {1} {2}",
                        prefix,
                        DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"),
                        message);
                }
            }
        }
        #endregion
    }
}
