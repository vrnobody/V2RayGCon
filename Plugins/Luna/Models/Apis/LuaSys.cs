using NLua;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Luna.Models.Apis
{
    public class LuaSys :
        VgcApis.BaseClasses.Disposable,
        VgcApis.Interfaces.Lua.ILuaSys
    {
        readonly object procLocker = new object();
        private readonly LuaApis luaApis;

        List<Process> processes = new List<Process>();
        List<VgcApis.Interfaces.Lua.ILuaMailBox> mailboxs = new List<VgcApis.Interfaces.Lua.ILuaMailBox>();

        static readonly SysCmpos.PostOffice postOffice = new SysCmpos.PostOffice();

        public LuaSys(LuaApis luaApis)
        {
            this.luaApis = luaApis;
        }

        #region private methods
        void SendLogHandler(object sender, DataReceivedEventArgs args)
        {
            string msg = null;
            try
            {
                msg = args.Data;
            }
            catch { }

            if (msg == null)
            {
                return;
            }

            luaApis.SendLog(msg);
        }

        void TrackdownProcess(Process proc)
        {
            lock (procLocker)
            {
                if (processes.Contains(proc))
                {
                    return;
                }
                processes.Add(proc);
            }
            VgcApis.Libs.Sys.ChildProcessTracker.AddProcess(proc);
        }
        #endregion

        #region ILuaSys.PostOffice
        public VgcApis.Interfaces.Lua.ILuaMailBox CreateMailBox(string name)
        {
            var mailbox = postOffice.CreateMailBox(name);
            if (mailbox != null)
            {
                lock (procLocker)
                {
                    if (!mailboxs.Contains(mailbox))
                    {
                        mailboxs.Add(mailbox);
                    }
                }
            }
            return mailbox;
        }


        public bool DestoryMailBox(VgcApis.Interfaces.Lua.ILuaMailBox mailbox) =>
            postOffice.DestoryMailBox(mailbox);
        #endregion

        #region ILuaSys.Process
        public void WaitForExit(Process proc) => proc?.WaitForExit();

        public void Cleanup(Process proc) => proc?.Close();

        public bool CloseMainWindow(Process proc)
        {
            if (proc == null)
            {
                return false;
            }
            return proc.CloseMainWindow();
        }

        public bool HasExited(Process proc)
        {
            if (proc == null)
            {
                return false;
            }

            try
            {
                return proc.HasExited;
            }
            catch { }
            return true;
        }

        public bool SendStopSignal(Process proc) => VgcApis.Misc.Utils.SendStopSignal(proc);

        public Process Run(string exePath) =>
            Run(exePath, null);

        public Process Run(string exePath, string args) =>
            Run(exePath, args, null);

        public Process Run(string exePath, string args, string stdin) =>
            Run(exePath, args, stdin, null, true, false);


        public Process Run(string exePath, string args, string stdin,
            LuaTable envs, bool hasWindow, bool redirectOutput)
        {
            var useStdIn = !string.IsNullOrEmpty(stdin);
            var p = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = exePath,
                    Arguments = args,
                    CreateNoWindow = !hasWindow,
                    UseShellExecute = false,
                    RedirectStandardInput = useStdIn,
                    RedirectStandardError = redirectOutput,
                    RedirectStandardOutput = redirectOutput,
                }
            };

            if (redirectOutput)
            {
                p.Exited += (s, a) =>
                {
                    p.ErrorDataReceived -= SendLogHandler;
                    p.OutputDataReceived -= SendLogHandler;
                };

                p.ErrorDataReceived += SendLogHandler;
                p.OutputDataReceived += SendLogHandler;
            }

            if (envs != null)
            {
                VgcApis.Misc.Utils.SetProcessEnvs(p, Misc.Utils.LuaTableToDictionary(envs));
            }

            p.Start();
            TrackdownProcess(p);

            if (redirectOutput)
            {
                p.BeginErrorReadLine();
                p.BeginOutputReadLine();
            }

            if (useStdIn)
            {
                var input = p.StandardInput;
                var buff = Encoding.UTF8.GetBytes(stdin);
                input.BaseStream.Write(buff, 0, buff.Length);
                input.WriteLine();
                input.Close();
            }

            return p;
        }
        #endregion

        #region ILuaSys.System
        static string osReleaseId;
        public string GetOsReleaseInfo()
        {
            if (string.IsNullOrEmpty(osReleaseId))
            {
                // https://stackoverflow.com/questions/39778525/how-to-get-windows-version-as-in-windows-10-version-1607/39778770
                var root = @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion";
                var name = Microsoft.Win32.Registry.GetValue(root, @"ProductName", @"")?.ToString();
                var arch = Environment.Is64BitOperatingSystem ? @"x64" : @"x86";
                var id = Microsoft.Win32.Registry.GetValue(root, @"ReleaseId", "")?.ToString();
                var build = Microsoft.Win32.Registry.GetValue(root, @"CurrentBuildNumber", @"")?.ToString();

                osReleaseId = $"{name} {arch} {id} build {build}";
            }
            return osReleaseId;
        }

        public string GetOsVersion() => Environment.OSVersion.VersionString;

        public int SetWallpaper(string filename) => Libs.Sys.WinApis.SetWallpaper(filename);
        #endregion

        #region ILuaSys.File
        public string GetImageResolution(string filename) =>
           VgcApis.Misc.Utils.GetImageResolution(filename);

        public string PickRandomLine(string filename) =>
            VgcApis.Misc.Utils.PickRandomLine(filename);

        public bool IsFileExists(string path) => File.Exists(path);
        public bool IsDirExists(string path) => Directory.Exists(path);

        public bool CreateFolder(string path)
        {
            try
            {
                Directory.CreateDirectory(path);
                return true;
            }
            catch { }
            return false;
        }
        #endregion

        #region protected methods
        protected override void Cleanup()
        {
            var boxes = new List<VgcApis.Interfaces.Lua.ILuaMailBox>(mailboxs);
            foreach (var box in boxes)
            {
                DestoryMailBox(box);
            }

            var ps = new List<Process>(processes);
            foreach (var p in ps)
            {
                try
                {
                    if (!p.HasExited)
                    {
                        VgcApis.Misc.Utils.KillProcessAndChildrens(p.Id);
                    }
                }
                catch { }
            }
        }


        #endregion
    }
}
