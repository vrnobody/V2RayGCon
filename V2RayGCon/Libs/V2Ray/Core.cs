using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using V2RayGCon.Resources.Resx;

namespace V2RayGCon.Libs.V2Ray
{
    public class Core : IDisposable
    {
        public event Action<string> OnLog;
        public event EventHandler OnCoreStatusChanged;

        readonly Services.Settings setting;
        readonly object coreStartStopLocker = new object();
        readonly VgcApis.Libs.Tasks.Waiter coreReadyWaiter = new VgcApis.Libs.Tasks.Waiter();
        Process coreProc;
        static int curConcurrentV2RayCoreNum = 0;
        bool isForcedExit = false;
        string customCoreName = string.Empty;

        public Core(Services.Settings setting)
        {
            coreProc = null;
            this.setting = setting;
        }

        #region property
        string statExe
        {
            get
            {
                var exe = GetV2RayExecutablePath(VgcApis.Models.Consts.Core.XrayCoreExeFileName);
                if (string.IsNullOrEmpty(exe))
                {
                    return GetV2RayExecutablePath(VgcApis.Models.Consts.Core.V2RayCtlExeFileName);
                }
                return exe;
            }
        }

        string _title;
        public string title
        {
            get
            {
                return string.IsNullOrEmpty(_title)
                    ? string.Empty
                    : VgcApis.Misc.Utils.AutoEllipsis(
                        _title,
                        VgcApis.Models.Consts.AutoEllipsis.V2rayCoreTitleMaxLength
                    );
            }
            set { _title = value; }
        }

        public bool isRunning
        {
            get => IsProcRunning(coreProc);
        }

        #endregion

        #region public method
        public bool WaitUntilReady()
        {
            var r = coreReadyWaiter.Wait(30 * 1000);
            coreReadyWaiter.Stop();
            return r;
        }

        public void SetCustomCoreName(string name)
        {
            this.customCoreName = name;
        }

        public VgcApis.Models.Datas.StatsSample QueryV2RayStatsApi(int port)
        {
            var exe = statExe;
            if (IsCustomCore() || string.IsNullOrEmpty(exe) || setting.IsClosing())
            {
                return null;
            }

            var isXray = exe.EndsWith(VgcApis.Models.Consts.Core.XrayCoreExeFileName);
            var queryTpl = isXray
                ? VgcApis.Models.Consts.Core.XrayStatsQueryParamTpl
                : VgcApis.Models.Consts.Core.V2RayStatsQueryParamTpl;
            var queryParam = string.Format(queryTpl, port.ToString());
            try
            {
                var output = VgcApis.Misc.Utils.ExecuteAndGetStdOut(
                    exe,
                    queryParam,
                    VgcApis.Models.Consts.Core.GetStatisticsTimeout,
                    null
                );

                return Misc.Utils.ParseStatApiResult(isXray, output);
            }
            catch { }
            return null;
        }

        public string GetV2RayCoreVersion()
        {
            if (!IsV2RayExecutableExist() || IsCustomCore())
            {
                return string.Empty;
            }

            var exe = GetV2RayExecutablePath(VgcApis.Models.Consts.Core.XrayCoreExeFileName);
            if (string.IsNullOrEmpty(exe))
            {
                exe = GetV2RayExecutablePath(VgcApis.Models.Consts.Core.V2RayCoreExeFileName);
            }

            var timeout = VgcApis.Models.Consts.Core.GetVersionTimeout;
            var output = VgcApis.Misc.Utils.ExecuteAndGetStdOut(exe, "-version", timeout, null);

            // since 3.46.* v is deleted
            // Regex pattern = new Regex(@"(?<version>(\d+\.)+\d+)");
            // Regex pattern = new Regex(@"v(?<version>[\d\.]+)");
            var ver = VgcApis.Misc.Utils.ExtractStringWithPattern(
                "version",
                @"(\d+\.)+\d+",
                output
            );
            return ver;
        }

        public bool IsV2RayExecutableExist()
        {
            var cores = new string[]
            {
                VgcApis.Models.Consts.Core.XrayCoreExeFileName,
                VgcApis.Models.Consts.Core.V2RayCoreExeFileName,
            };

            foreach (var core in cores)
            {
                if (!string.IsNullOrEmpty(GetV2RayExecutablePath(core)))
                {
                    return true;
                }
            }
            return false;
        }

        // blocking
        public void RestartCore(string config) => RestartCoreWorker(config, false);

        public void RestartCoreIgnoreError(string config) => RestartCoreWorker(config, true);

        // blocking
        public void StopCore()
        {
            lock (coreStartStopLocker)
            {
                StopCoreIgnoreError(coreProc);
            }
        }

        string envs = "";

        public void SetEnvs(string envs)
        {
            this.envs = envs;
        }

        #endregion

        #region private method
        Models.Datas.CustomCoreSettings GetCustomCoreSettings()
        {
            return setting.GetCustomCoresSetting().FirstOrDefault(cs => cs.name == customCoreName);
        }

        bool IsCustomCore()
        {
            return !string.IsNullOrEmpty(customCoreName);
        }

        string GetV2RayExecutablePath(string fileName)
        {
            List<string> folders = GenV2RayCoreSearchPaths(setting.isPortable);
            for (var i = 0; i < folders.Count; i++)
            {
                var p = Path.Combine(folders[i], fileName);
                if (File.Exists(p))
                {
                    return p;
                }
            }
            return string.Empty;
        }

        string GetCustomCoreExePath(Models.Datas.CustomCoreSettings cs)
        {
            if (cs == null)
            {
                return null;
            }

            if (string.IsNullOrEmpty(cs.dir))
            {
                return string.IsNullOrEmpty(cs.exe) ? null : cs.exe;
            }

            var f = Path.Combine(cs.dir, cs.exe);
            return File.Exists(f) ? f : null;
        }

        bool IsCoreExcutableExist()
        {
            if (!IsCustomCore())
            {
                return IsV2RayExecutableExist();
            }
            var cs = GetCustomCoreSettings();
            var exe = GetCustomCoreExePath(cs);
            return !string.IsNullOrEmpty(exe);
        }

        void RestartCoreWorker(string config, bool quiet)
        {
            if (!IsCoreExcutableExist())
            {
                if (quiet)
                {
                    SendLog(I18N.ExeNotFound);
                }
                else
                {
                    VgcApis.Misc.UI.MsgBoxAsync(I18N.ExeNotFound);
                }
                InvokeEventOnCoreStatusChanged();
                return;
            }

            lock (coreStartStopLocker)
            {
                StopCoreIgnoreError(this.coreProc);

                try
                {
                    if (!setting.IsClosing())
                    {
                        StartCore(config, quiet);
                    }
                }
                catch
                {
                    StopCoreIgnoreError(this.coreProc);
                }
            }

            // do not run in background
            InvokeEventOnCoreStatusChanged();
        }

        void StopCoreIgnoreError(Process core)
        {
            this.coreProc = null;
            if (IsProcRunning(core))
            {
                isForcedExit = true;
                try
                {
                    core?.Kill();
                    core?.WaitForExit();
                }
                catch { }
            }
            VgcApis.Misc.Utils.Sleep(500);
        }

        bool IsProcRunning(Process proc)
        {
            try
            {
                if (proc != null && !proc.HasExited)
                {
                    return true;
                }
            }
            catch { }
            return false;
        }

        static List<string> GenV2RayCoreSearchPaths(bool isPortable)
        {
            var appRoot = VgcApis.Misc.Utils.GetAppDir();

            var folders = new List<string>
            {
                Misc.Utils.GetSysAppDataFolder(), // %appdata%
                // 兼容
                appRoot,
                Path.Combine(appRoot, VgcApis.Models.Consts.Files.CoreFolderName),
                // 整合
                VgcApis.Misc.Utils.GetCoreFolderFullPath(),
            };

            if (isPortable)
            {
                folders.Reverse();
            }

            return folders;
        }

        void InvokeEventOnCoreStatusChanged()
        {
            try
            {
                OnCoreStatusChanged?.Invoke(this, EventArgs.Empty);
            }
            catch { }
        }

        Process CreateCustomCoreProcess(Models.Datas.CustomCoreSettings customSettings)
        {
            var ec = customSettings.GetStdOutEncoding();
            var p = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = GetCustomCoreExePath(customSettings),
                    Arguments = customSettings.args,
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    RedirectStandardInput = customSettings.useStdin,
                    StandardOutputEncoding = ec,
                    StandardErrorEncoding = ec,
                },
            };
            if (customSettings.setWorkingDir && !string.IsNullOrEmpty(customSettings.dir))
            {
                p.StartInfo.WorkingDirectory = customSettings.dir;
            }
            p.EnableRaisingEvents = true;

            VgcApis.Misc.Utils.SetProcessEnvs(p, customSettings.envs);
            return p;
        }

        Process CreateV2RayCoreProcess()
        {
            var exe = GetV2RayExecutablePath(VgcApis.Models.Consts.Core.XrayCoreExeFileName);
            var args = "-config=stdin: -format=json";
            if (string.IsNullOrEmpty(exe))
            {
                exe = GetV2RayExecutablePath(VgcApis.Models.Consts.Core.V2RayCoreExeFileName);
            }

            var p = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = exe,
                    Arguments = args,
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    RedirectStandardInput = true,
                    // 定时炸弹
                    StandardOutputEncoding = Encoding.UTF8,
                    StandardErrorEncoding = Encoding.UTF8,
                },
            };
            p.EnableRaisingEvents = true;
            return p;
        }

        string TranslateErrorCode(int exitCode)
        {
            if (exitCode == 0)
            {
                return null;
            }

            // ctrl + c not working
            if (isForcedExit)
            {
                isForcedExit = false;
                return null;
            }

            // exitCode = 1 means Killed forcibly.
            // src https://stackoverflow.com/questions/4344923/process-exit-code-when-process-is-killed-forcibly
            if (exitCode == 1)
            {
                return title + @" " + I18N.KilledByUserOrOtherApp;
            }

            string msg = string.Format(I18N.V2rayCoreExitAbnormally, title, exitCode);
            if (IsCustomCore())
            {
                return msg;
            }

            /*
             * v2ray-core/main/main.go
             * 23: Configuration error.
             * -1: Failed to start.
             */
            switch (exitCode)
            {
                case 23:
                    msg = title + @" " + I18N.HasFaultyConfig;
                    break;
                case -1:
                    msg = title + @" " + I18N.CanNotStartPlsCheckLogs;
                    break;
                default:
                    break;
            }

            return msg;
        }

        void OnCoreExitedQuiet(object sender, EventArgs args) => OnCoreExitedHandler(sender, true);

        void OnCoreExited(object sender, EventArgs args) => OnCoreExitedHandler(sender, false);

        void OnCoreExitedHandler(object sender, bool quiet)
        {
            coreReadyWaiter.Stop();

            var core = sender as Process;

            Interlocked.Decrement(ref curConcurrentV2RayCoreNum);

            try
            {
                if (quiet)
                {
                    core.Exited -= OnCoreExitedQuiet;
                }
                else
                {
                    core.Exited -= OnCoreExited;
                }
            }
            catch { }

            string msg = null;
            try
            {
                // Process.ExitCode may throw exceptions
                msg = TranslateErrorCode(core.ExitCode);
            }
            catch { }
            core.Dispose();

            SendLog($"{I18N.ConcurrentV2RayCoreNum}{curConcurrentV2RayCoreNum}");
            SendLog(I18N.CoreExit);

            // do not run in background
            InvokeEventOnCoreStatusChanged();

            if (!quiet && !string.IsNullOrEmpty(msg))
            {
                VgcApis.Misc.UI.MsgBoxAsync(msg);
            }
        }

        void BindEvents(Process proc, bool quiet)
        {
            try
            {
                if (quiet)
                {
                    proc.Exited += OnCoreExitedQuiet;
                }
                else
                {
                    proc.Exited += OnCoreExited;
                }

                proc.Exited += (s, a) =>
                {
                    proc.ErrorDataReceived -= SendLogHandler;
                    proc.OutputDataReceived -= SendLogHandler;
                };

                proc.ErrorDataReceived += SendLogHandler;
                proc.OutputDataReceived += SendLogHandler;
            }
            catch { }
        }

        void WaitForFile(string filename)
        {
            var md5 = VgcApis.Misc.Utils.Md5Hex(filename);
            var name = "V2RayGCon-custom-core-config-file-" + md5;

            var mre = new ManualResetEventSlim(false);
            VgcApis.Misc.Utils.RunInBackground(() =>
            {
#if DEBUG
                SendLog($"waitting for file {title}");
#endif
                using (var mutex = new Mutex(false, name))
                {
                    mutex.WaitOne();
                    try
                    {
                        mre.Set();
                    }
                    catch { }
                    VgcApis.Misc.Utils.Sleep(TimeSpan.FromSeconds(3));
                    mutex.ReleaseMutex();
                }
#if DEBUG
                SendLog($"release file: {title}");
#endif
            });
            mre.Wait();
            mre.Dispose();
#if DEBUG
            SendLog($"write config file {title}");
#endif
        }

        void StartCore(string config, bool quiet)
        {
            var isCustomCore = IsCustomCore();
            coreReadyWaiter.Start();
            var cs = isCustomCore ? GetCustomCoreSettings() : null;
            var core = isCustomCore ? CreateCustomCoreProcess(cs) : CreateV2RayCoreProcess();
            VgcApis.Misc.Utils.SetProcessEnvs(core, envs);

            BindEvents(core, quiet);
            Interlocked.Increment(ref curConcurrentV2RayCoreNum);

            if (isCustomCore && cs.useFile)
            {
                var fn = Path.GetFullPath(cs.configFile);
                WaitForFile(fn);
                File.WriteAllText(fn, config);
            }

            core.Start();
            this.coreProc = core;

            // Add to JOB object require win8+.
            VgcApis.Libs.Sys.ChildProcessTracker.AddProcess(core);

            if (!isCustomCore || cs.useStdin)
            {
                var ec = isCustomCore ? cs.GetStdInEncoding() : Encoding.Default;
                WriteConfigToStandardInput(core, config, ec);
            }

            core.PriorityClass = ProcessPriorityClass.AboveNormal;
            core.BeginErrorReadLine();
            core.BeginOutputReadLine();

            SendLog($"{I18N.ConcurrentV2RayCoreNum}{curConcurrentV2RayCoreNum}");
        }

        private void WriteConfigToStandardInput(Process proc, string config, Encoding encoding)
        {
            using (var s = proc.StandardInput.BaseStream)
            using (var w = new StreamWriter(s, encoding))
            {
                w.Write(config);
            }
        }

        void SendLogHandler(object sender, DataReceivedEventArgs args)
        {
            var msg = args.Data;

            if (string.IsNullOrEmpty(msg))
            {
                return;
            }

            if (!coreReadyWaiter.Wait(0) && MatchAllReadyMarks(msg))
            {
                coreReadyWaiter.Stop();
            }

            SendLog(msg);
        }

        bool MatchAllReadyMarks(string message)
        {
            if (IsCustomCore())
            {
                return true;
            }

            var lower = message.ToLower();
            foreach (var mark in VgcApis.Models.Consts.Core.ReadyLogMarks)
            {
                if (!lower.Contains(mark))
                {
                    return false;
                }
            }
            return true;
        }

        void SendLog(string log)
        {
            try
            {
                OnLog?.Invoke(log);
            }
            catch { }
        }
        #endregion

        #region IDisposable
        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)
                    coreReadyWaiter.Dispose();
                }

                // TODO: 释放未托管的资源(未托管的对象)并重写终结器
                // TODO: 将大型字段设置为 null
                disposedValue = true;
            }
        }

        // // TODO: 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
        // ~Core()
        // {
        //     // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
