using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using V2RayGCon.Resources.Resx;

namespace V2RayGCon.Libs.V2Ray
{
    public class Core
    {
        #region support ctrl+c
        // https://stackoverflow.com/questions/283128/how-do-i-send-ctrlc-to-a-process-in-c
        internal const int CTRL_C_EVENT = 0;
        #endregion

        readonly Encoding ioEncoding = Encoding.UTF8;

        public event EventHandler<VgcApis.Models.Datas.StrEvent> OnLog;
        public event EventHandler OnCoreStatusChanged;

        Services.Settings setting;

        Process v2rayCore;
        static object coreLock = new object();
        static int curConcurrentV2RayCoreNum = 0;
        bool isForcedExit = false;
        AutoResetEvent isCoreReadyEvent = new AutoResetEvent(false);

        public Core(Services.Settings setting)
        {
            isRunning = false;
            isWatchCoreReadyLog = false;
            v2rayCore = null;
            this.setting = setting;
        }

        #region property
        string _v2ctl = "";
        string v2ctl
        {
            get
            {
                if (string.IsNullOrEmpty(_v2ctl))
                {
                    _v2ctl = GetExecutablePath(StrConst.ExecutableV2ctl);
                }
                return _v2ctl;
            }
        }

        string _title;
        public string title
        {
            get
            {
                return string.IsNullOrEmpty(_title) ?
                    string.Empty :
                    VgcApis.Misc.Utils.AutoEllipsis(_title, VgcApis.Models.Consts.AutoEllipsis.V2rayCoreTitleMaxLength);
            }
            set
            {
                _title = value;
            }
        }

        public bool isRunning
        {
            get;
            private set;
        }

        bool isWatchCoreReadyLog
        {
            get;
            set;
        }
        #endregion

        #region public method
        public int QueryStatsApi(int port, bool isUplink)
        {
            if (string.IsNullOrEmpty(v2ctl))
            {
                return 0;
            }

            var queryParam = string.Format(
                StrConst.StatsQueryParamTpl,
                port.ToString(),
                isUplink ? "uplink" : "downlink");

            try
            {
                var output = Misc.Utils.GetOutputFromExecutable(
                    v2ctl,
                    queryParam,
                    VgcApis.Models.Consts.Core.GetStatisticsTimeout);

                // Regex pattern = new Regex(@"(?<value>(\d+))");
                var value = VgcApis.Misc.Utils.ExtractStringWithPattern(
                    "value", @"(\d+)", output);

                return VgcApis.Misc.Utils.Str2Int(value);
            }
            catch { }
            return 0;
        }

        public string GetCoreVersion()
        {
            if (!IsExecutableExist())
            {
                return string.Empty;
            }

            var output = Misc.Utils.GetOutputFromExecutable(
                GetExecutablePath(),
                "-version",
                VgcApis.Models.Consts.Core.GetVersionTimeout);

            // since 3.46.* v is deleted
            // Regex pattern = new Regex(@"(?<version>(\d+\.)+\d+)");
            // Regex pattern = new Regex(@"v(?<version>[\d\.]+)");
            return VgcApis.Misc.Utils.ExtractStringWithPattern(
                "version", @"(\d+\.)+\d+", output);
        }

        public bool IsExecutableExist()
        {
            return !string.IsNullOrEmpty(GetExecutablePath());
        }

        public string GetExecutablePath(string fileName = null)
        {
            List<string> folders = GenV2RayCoreSearchPaths(setting.isPortable);
            for (var i = 0; i < folders.Count; i++)
            {
                var file = Path.Combine(folders[i], fileName ?? StrConst.ExecutableV2ray);
                if (File.Exists(file))
                {
                    return file;
                }
            }
            return string.Empty;
        }

        private static List<string> GenV2RayCoreSearchPaths(bool isPortable)
        {
            var folders = new List<string>{
                Misc.Utils.GetSysAppDataFolder(), // %appdata%
                VgcApis.Misc.Utils.GetAppDir(),
                VgcApis.Misc.Utils.GetCoreFolderFullPath(),
            };

            if (isPortable)
            {
                folders.Reverse();
            }

            return folders;
        }

        // blocking
        public void RestartCore(
            string config,
            Dictionary<string, string> env = null)
        {
            lock (coreLock)
            {
                if (isRunning)
                {
                    StopCoreWorker();
                }

                if (IsExecutableExist())
                {
                    StartCore(config, env);
                }
                else
                {
                    VgcApis.Misc.Utils.RunInBackground(
                        () => MessageBox.Show(I18N.ExeNotFound));
                }
            }
            VgcApis.Misc.Utils.RunInBackground(() => InvokeEventOnCoreStatusChanged());
        }

        // non-blocking 
        public void RestartCoreThen(
            string config,
            Action next = null,
            Dictionary<string, string> env = null)
        {
            VgcApis.Misc.Utils.RunInBackground(() =>
            {
                RestartCore(config, env);
                InvokeActionIgnoreError(next);
            });
        }

        // blocking
        public void StopCore()
        {
            lock (coreLock)
            {
                StopCoreWorker();
            }
        }

        // non-blocking
        public void StopCoreThen(Action next = null)
        {
            VgcApis.Misc.Utils.RunInBackground(() =>
            {
                StopCore();
                InvokeActionIgnoreError(next);
            });
        }
        #endregion

        #region private method



        void InvokeEventOnCoreStatusChanged()
        {
            try
            {
                OnCoreStatusChanged?.Invoke(this, EventArgs.Empty);
            }
            catch { }
        }

        void StopCoreWorker()
        {
            if (v2rayCore == null)
            {
                isRunning = false;
            }

            if (!isRunning)
            {
                return;
            }

            var success = SendCtrlCSignalToV2RayCore();
            if (!success)
            {
                try
                {
                    // kill if send ctrl+c fail
                    KillCore();
                }
                catch { }
            }
            isRunning = false;
        }

        bool SendCtrlCSignalToV2RayCore()
        {
            var success = false;
            try
            {
                if (Libs.Sys.SafeNativeMethods.AttachConsole((uint)v2rayCore.Id))
                {
                    AutoResetEvent done = new AutoResetEvent(false);
                    v2rayCore.Exited += (s, a) =>
                    {
                        v2rayCore.Close();
                        done.Set();
                    };

                    Libs.Sys.SafeNativeMethods.SetConsoleCtrlHandler(null, true);
                    Libs.Sys.SafeNativeMethods.GenerateConsoleCtrlEvent(CTRL_C_EVENT, 0);

                    if (done.WaitOne(VgcApis.Models.Consts.Core.SendCtrlCTimeout))
                    {
                        success = true;
                    }

                    Libs.Sys.SafeNativeMethods.FreeConsole();
                    Libs.Sys.SafeNativeMethods.SetConsoleCtrlHandler(null, false);
                }
            }
            catch { }

            return success;
        }

        void KillCore()
        {
            Debug.WriteLine("Kill core!");

            isForcedExit = true;
            AutoResetEvent finished = new AutoResetEvent(false);

            SendLog(I18N.AttachToV2rayCoreProcessFail);

            v2rayCore.Exited += (s, a) =>
            {
                finished.Set();
            };

            Misc.Utils.KillProcessAndChildrens(v2rayCore.Id);
            finished.WaitOne(VgcApis.Models.Consts.Core.KillCoreTimeout);
        }

        static void InvokeActionIgnoreError(Action lambda)
        {
            try
            {
                lambda?.Invoke();
            }
            catch { }
        }

        Process CreateV2RayCoreProcess()
        {
            var p = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = GetExecutablePath(),
                    Arguments = "-config=stdin: -format=json",
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    RedirectStandardInput = true,
                    StandardOutputEncoding = ioEncoding,
                    StandardErrorEncoding = ioEncoding,
                }
            };
            p.EnableRaisingEvents = true;
            return p;
        }

        void InjectEnv(Process proc, Dictionary<string, string> envs)
        {
            if (envs == null || envs.Count <= 0)
            {
                return;
            }

            var procEnv = proc.StartInfo.EnvironmentVariables;
            foreach (var env in envs)
            {
                procEnv[env.Key] = env.Value;
            }
        }

        void ShowExitErrorMessage(int exitCode)
        {
            /*
            * https://stackoverflow.com/questions/4344923/process-exit-code-when-process-is-killed-forcibly
            * 1: Killed forcibly.
            */

            // ctrl + c not working
            if (isForcedExit && exitCode == 1)
            {
                return;
            }

            /*
             * v2ray-core/main/main.go
             * 23: Configuration error.
             * -1: Failed to start.
             */
            string msg = string.Format(I18N.V2rayCoreExitAbnormally, title, exitCode);
            switch (exitCode)
            {
                case 1:
                    msg = title + @" " + I18N.KilledByUserOrOtherApp;
                    break;
                case 23:
                    msg = title + @" " + I18N.HasFaultyConfig;
                    break;
                case -1:
                    msg = title + @" " + I18N.CanNotStartPlsCheckLogs;
                    break;
                default:
                    break;
            }

            MessageBox.Show(msg);
        }

        void OnCoreExited(object sender, EventArgs args)
        {
            Interlocked.Decrement(ref curConcurrentV2RayCoreNum);
            isCoreReadyEvent.Set();
            SendLog($"{I18N.ConcurrentV2RayCoreNum}{curConcurrentV2RayCoreNum}");

            SendLog(I18N.CoreExit);
            ReleaseEvents(v2rayCore);

            var err = v2rayCore.ExitCode;
            if (err != 0)
            {
                v2rayCore.Close();
                VgcApis.Misc.Utils.RunInBackground(() => ShowExitErrorMessage(err));
            }

            // SendLog("Exit code: " + err);
            isRunning = false;
            VgcApis.Misc.Utils.RunInBackground(() => InvokeEventOnCoreStatusChanged());
        }

        void BindEvents(Process proc)
        {
            proc.Exited += OnCoreExited;
            proc.ErrorDataReceived += SendLogHandler;
            proc.OutputDataReceived += SendLogHandler;
        }

        void ReleaseEvents(Process proc)
        {
            proc.Exited -= OnCoreExited;
            proc.ErrorDataReceived -= SendLogHandler;
            proc.OutputDataReceived -= SendLogHandler;
        }

        void StartCore(string config, Dictionary<string, string> envs = null)
        {
            isForcedExit = false;
            isCoreReadyEvent.Reset();
            isWatchCoreReadyLog = IsConfigWaitable(config);

            v2rayCore = CreateV2RayCoreProcess();
            InjectEnv(v2rayCore, envs);
            BindEvents(v2rayCore);

            isRunning = true;
            v2rayCore.Start();
            Interlocked.Increment(ref curConcurrentV2RayCoreNum);

            // Add to JOB object require win8+.
            Sys.ChildProcessTracker.AddProcess(v2rayCore);

            WriteConfigToStandardInput(config);

            v2rayCore.PriorityClass = ProcessPriorityClass.AboveNormal;
            v2rayCore.BeginErrorReadLine();
            v2rayCore.BeginOutputReadLine();

            if (isWatchCoreReadyLog)
            {
                // Assume core ready after 5 seconds, in case log set to none.
                isCoreReadyEvent.WaitOne(VgcApis.Models.Consts.Core.WaitUntilReadyTimeout);
            }
            isWatchCoreReadyLog = false;

            SendLog($"{I18N.ConcurrentV2RayCoreNum}{curConcurrentV2RayCoreNum}");
        }

        private void WriteConfigToStandardInput(string config)
        {
            var input = v2rayCore.StandardInput;
            var buff = ioEncoding.GetBytes(config);
            input.BaseStream.Write(buff, 0, buff.Length);
            input.WriteLine();
            input.Close();
        }

        bool IsConfigWaitable(string config)
        {
            List<string> levels = new List<string> { "none", "error" };
            try
            {
                var json = JObject.Parse(config);
                var loglevel = Misc.Utils.GetValue<string>(json, "log.loglevel")?.ToLower();
                return !levels.Contains(loglevel);
            }
            catch { }
            return true;
        }

        void SendLogHandler(object sender, DataReceivedEventArgs args)
        {
            var msg = args.Data;

            if (msg == null)
            {
                return;
            }

            if (isWatchCoreReadyLog && MatchAllReadyMarks(msg))
            {
                isCoreReadyEvent.Set();
            }

            SendLog(msg);
        }

        bool MatchAllReadyMarks(string message)
        {
            foreach (var mark in VgcApis.Models.Consts.Core.ReadyLogMarks)
            {
                if (!message.Contains(mark))
                {
                    return false;
                }
            }
            return true;
        }

        void SendLog(string log)
        {
            var arg = new VgcApis.Models.Datas.StrEvent(log);
            try
            {
                OnLog?.Invoke(this, arg);
            }
            catch { }
        }

        #endregion
    }
}
