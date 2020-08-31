using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using V2RayGCon.Resources.Resx;

namespace V2RayGCon.Libs.V2Ray
{
    public class Core
    {
        readonly Encoding ioEncoding = Encoding.UTF8;

        public event EventHandler<VgcApis.Models.Datas.StrEvent> OnLog;
        public event EventHandler OnCoreStatusChanged;

        Services.Settings setting;

        Process v2rayCore;
        static object coreLock = new object();
        static int curConcurrentV2RayCoreNum = 0;
        bool isForcedExit = false;

        public Core(Services.Settings setting)
        {
            isReady = false;
            isRunning = false;
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
                    _v2ctl = GetExecutablePath(VgcApis.Models.Consts.Core.V2RayCtlExeFileName);
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

        public bool isReady
        {
            get;
            private set;
        }

        public bool isRunning
        {
            get;
            private set;
        }

        #endregion

        #region public method

        public VgcApis.Models.Datas.StatsSample QueryStatsApi(int port)
        {
            if (string.IsNullOrEmpty(v2ctl))
            {
                return null;
            }

            var queryParam = string.Format(VgcApis.Models.Consts.Core.StatsQueryParamTpl, port.ToString());
            try
            {
                var output = Misc.Utils.GetOutputFromExecutable(
                    v2ctl,
                    queryParam,
                    VgcApis.Models.Consts.Core.GetStatisticsTimeout);

                return Misc.Utils.ParseStatApiResult(output);
            }
            catch { }
            return null;
        }

        public string GetCoreVersion()
        {
            if (!IsExecutableExist())
            {
                return string.Empty;
            }

            var timeout = VgcApis.Models.Consts.Core.GetVersionTimeout;
            var output = Misc.Utils.GetOutputFromExecutable(GetExecutablePath(), "-version", timeout);

            // since 3.46.* v is deleted
            // Regex pattern = new Regex(@"(?<version>(\d+\.)+\d+)");
            // Regex pattern = new Regex(@"v(?<version>[\d\.]+)");
            var ver = VgcApis.Misc.Utils.ExtractStringWithPattern("version", @"(\d+\.)+\d+", output);
            return ver;
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
                var file = Path.Combine(folders[i], fileName ?? VgcApis.Models.Consts.Core.V2RayCoreExeFileName);
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
                    StartCoreWorker(config, env);
                }
                else
                {
                    VgcApis.Misc.UI.MsgBoxAsync(I18N.ExeNotFound);
                }
            }
            // do not run in background
            // VgcApis.Misc.Utils.RunInBackground(() => InvokeEventOnCoreStatusChanged());
            InvokeEventOnCoreStatusChanged();
        }

        // blocking
        public void StopCore()
        {
            lock (coreLock)
            {
                StopCoreWorker();
            }
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

            // Ctrl + c is buggy
            // var success = VgcApis.Misc.Utils.SendStopSignal(v2rayCore);
            // if (!success)
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

        void KillCore()
        {
            Debug.WriteLine("Kill core!");

            isForcedExit = true;
            SendLogBg(I18N.AttachToV2rayCoreProcessFail);
            try
            {
                VgcApis.Misc.Utils.KillProcessAndChildrens(v2rayCore.Id);
                v2rayCore.WaitForExit(VgcApis.Models.Consts.Core.KillCoreTimeout);
            }
            catch { }
        }

        Process CreateV2RayCoreProcess(string config)
        {
            var args = Misc.Utils.GenCmdArgFromConfig(config);

            var p = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = GetExecutablePath(),
                    Arguments = args,
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    RedirectStandardInput = true,

                    // 定时炸弹
                    StandardOutputEncoding = ioEncoding,
                    StandardErrorEncoding = ioEncoding,
                }
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

            // exitCode = 1 means Killed forcibly.
            // src https://stackoverflow.com/questions/4344923/process-exit-code-when-process-is-killed-forcibly

            // ctrl + c not working
            if (isForcedExit)
            {
                return null;
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

            return msg;
        }

        void OnCoreExited(object sender, EventArgs args)
        {
            isReady = false;

            Interlocked.Decrement(ref curConcurrentV2RayCoreNum);
            SendLogBg($"{I18N.ConcurrentV2RayCoreNum}{curConcurrentV2RayCoreNum}");

            SendLogBg(I18N.CoreExit);
            ReleaseEvents(v2rayCore);

            try
            {
                var msg = TranslateErrorCode(v2rayCore.ExitCode);
                if (!string.IsNullOrEmpty(msg))
                {
                    VgcApis.Misc.UI.MsgBoxAsync(msg);
                }
            }
            catch { }

            try
            {
                v2rayCore.Close();
            }
            catch { }

            // SendLog("Exit code: " + err);
            isRunning = false;

            // do not run in background
            // VgcApis.Misc.Utils.RunInBackground(() => InvokeEventOnCoreStatusChanged());
            InvokeEventOnCoreStatusChanged();
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

        void StartCoreWorker(string config, Dictionary<string, string> envs = null)
        {
            isReady = false;
            isForcedExit = false;

            v2rayCore = CreateV2RayCoreProcess(config);
            VgcApis.Misc.Utils.SetProcessEnvs(v2rayCore, envs);

            BindEvents(v2rayCore);

            v2rayCore.Start();
            Interlocked.Increment(ref curConcurrentV2RayCoreNum);

            // Add to JOB object require win8+.
            VgcApis.Libs.Sys.ChildProcessTracker.AddProcess(v2rayCore);
            isRunning = true;

            WriteConfigToStandardInput(config);

            v2rayCore.PriorityClass = ProcessPriorityClass.AboveNormal;
            v2rayCore.BeginErrorReadLine();
            v2rayCore.BeginOutputReadLine();

            SendLogBg($"{I18N.ConcurrentV2RayCoreNum}{curConcurrentV2RayCoreNum}");
        }

        private void WriteConfigToStandardInput(string config)
        {
            var input = v2rayCore.StandardInput;
            var buff = ioEncoding.GetBytes(config);
            input.BaseStream.Write(buff, 0, buff.Length);
            input.WriteLine();
            input.Close();
        }

        void SendLogHandler(object sender, DataReceivedEventArgs args)
        {
            var msg = args.Data;

            if (string.IsNullOrEmpty(msg))
            {
                return;
            }

            if (!isReady && MatchAllReadyMarks(msg))
            {
                isReady = true;
            }

            SendLogBg(msg);
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

        void SendLogBg(string log)
        {
            var arg = new VgcApis.Models.Datas.StrEvent(log);
            VgcApis.Misc.Utils.RunInBackground(() =>
            {
                try
                {
                    OnLog?.Invoke(this, arg);
                }
                catch { }
            });
        }

        #endregion
    }
}
