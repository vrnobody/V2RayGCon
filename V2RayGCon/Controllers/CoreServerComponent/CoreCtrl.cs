using System;
using System.Collections.Generic;
using V2RayGCon.Resources.Resx;
using VgcApis.Models.Datas;

namespace V2RayGCon.Controllers.CoreServerComponent
{
    public sealed class CoreCtrl
        : VgcApis.BaseClasses.ComponentOf<CoreServerCtrl>,
            VgcApis.Interfaces.CoreCtrlComponents.ICoreCtrl
    {
        Libs.V2Ray.Core core;
        Services.Settings setting;
        Services.ConfigMgr configMgr;
        CoreInfo coreInfo;

        static long SpeedtestTimeout = VgcApis.Models.Consts.Core.SpeedtestTimeout;

        VgcApis.Libs.Tasks.Routine bookKeeper;

        VgcApis.Libs.Tasks.Bar isRecording = new VgcApis.Libs.Tasks.Bar();

        public CoreCtrl(Services.Settings setting, CoreInfo coreInfo, Services.ConfigMgr configMgr)
        {
            this.coreInfo = coreInfo;
            this.setting = setting;
            this.configMgr = configMgr;
        }

        CoreStates coreStates;
        Configer configer;
        Logger logger;

        public override void Prepare()
        {
            core = new Libs.V2Ray.Core(setting);
            core.SetCustomCoreName(coreInfo.customCoreName);

            coreStates = GetSibling<CoreStates>();
            configer = GetSibling<Configer>();
            logger = GetSibling<Logger>();

            bookKeeper = new VgcApis.Libs.Tasks.Routine(RecordStatSample, 3000);
        }

        #region public mehtods
        public string GetCustomCoreName() => coreInfo.customCoreName;

        public bool SetCustomCoreName(string name)
        {
            name = name ?? string.Empty;
            if (name == coreInfo.customCoreName)
            {
                return false;
            }

            coreInfo.customCoreName = name;
            core.SetCustomCoreName(name);
            GetParent().InvokeEventOnPropertyChange();
            return true;
        }

        // 非正常终止时调用
        public void SetTitle(string title) => core.title = title;

        public void BindEvents()
        {
            core.OnLog += OnLogHandler;
            core.OnCoreStatusChanged += OnCoreStateChangedHandler;
        }

        public void ReleaseEvents()
        {
            bookKeeper?.Dispose();
            core.OnLog -= OnLogHandler;
            core.OnCoreStatusChanged -= OnCoreStateChangedHandler;
        }

        public string Fetch(string url) => Fetch(url, -1);

        public string Fetch(string url, int timeout)
        {
            var config = configer.GetConfig();
            var title = coreStates.GetTitle();
            var text = configMgr.FetchWithCustomConfig(config, title, url, timeout) ?? string.Empty;
            coreStates.AddStatSample(new VgcApis.Models.Datas.StatsSample(0, text.Length));
            return text;
        }

        public void StopCore() => StopCoreWorker(null);

        public void StopCoreThen() => StopCoreThen(null);

        public void StopCoreThen(Action next) =>
            VgcApis.Misc.Utils.RunInBgSlim(() => StopCoreWorker(next));

        public void RestartCore() => RestartCoreWorker(null, false);

        public void RestartCoreIgnoreError() => RestartCoreWorker(null, true);

        public void RestartCoreThen() => RestartCoreThen(null);

        public void RestartCoreThen(Action next) =>
            VgcApis.Misc.Utils.RunInBgSlim(() => RestartCoreWorker(next, false));

        public bool IsCoreRunning() => core.isRunning;

        public void RunSpeedTest() => SpeedTestWorker(configer.GetConfig());

        public void RunSpeedTestThen() =>
            VgcApis.Misc.Utils.RunInBackground(() =>
            {
                SpeedTestWorker(configer.GetConfig());
            });
        #endregion

        #region private methods
        void RecordStatSample()
        {
            if (
                !setting.isEnableStatistics
                || !IsCoreRunning()
                || setting.IsScreenLocked()
                || setting.IsClosing()
                || !isRecording.Install()
            )
            {
                return;
            }

            try
            {
                var statsPort = coreStates.GetStatPort();
                if (statsPort > 0)
                {
                    var sample = core.QueryV2RayStatsApi(statsPort);
                    coreStates.AddStatSample(sample);
                }
            }
            catch { }
            isRecording.Remove();
        }

        void OnCoreStateChangedHandler(object sender, EventArgs args)
        {
            VgcApis.Misc.Utils.RunInBgSlim(() =>
            {
                if (core.isRunning)
                {
                    GetParent().InvokeEventOnCoreStart();
                }
                else
                {
                    GetParent().InvokeEventOnCoreStop();
                }
            });
        }

        void SpeedTestWorker(string rawConfig)
        {
            long avgDelay = -1;
            long curDelay = SpeedtestTimeout;
            var cycles = Math.Max(
                1,
                setting.isUseCustomSpeedtestSettings ? setting.CustomSpeedtestCycles : 1
            );

            coreStates.SetSpeedTestResult(0);
            coreStates.SetStatus(I18N.Testing);

            logger.Log(I18N.Testing);
            for (int i = 0; i < cycles && !setting.isSpeedtestCancelled; i++)
            {
                var sr = configMgr.RunDefaultSpeedTest(
                    rawConfig,
                    coreStates.GetTitle(),
                    (s, a) => logger.Log(a.Data)
                );
                curDelay = sr.Item1;
                coreStates.AddStatSample(new VgcApis.Models.Datas.StatsSample(0, sr.Item2));
                ShowCurrentSpeedtestResult(I18N.CurSpeedtestResult, curDelay);
                if (curDelay == SpeedtestTimeout)
                {
                    continue;
                }

                avgDelay = VgcApis.Misc.Utils.SpeedtestMean(
                    avgDelay,
                    curDelay,
                    VgcApis.Models.Consts.Config.CustomSpeedtestMeanWeight
                );
            }

            // all speedtest timeout
            if (avgDelay <= 0)
            {
                avgDelay = SpeedtestTimeout;
            }
            ShowCurrentSpeedtestResult(I18N.AvgSpeedtestResult, avgDelay);
        }

        void ShowCurrentSpeedtestResult(string prefix, long delay)
        {
            if (delay <= 0)
            {
                delay = SpeedtestTimeout;
            }
            var text = delay == SpeedtestTimeout ? I18N.Timeout : $"{delay}ms";
            coreStates.SetStatus(text);
            coreStates.SetSpeedTestResult(delay);
            logger.Log($"{prefix}{text}");
        }

        void OnLogHandler(object sender, VgcApis.Models.Datas.StrEvent arg) => logger.Log(arg.Data);

        void StopCoreWorker(Action next)
        {
            bookKeeper?.Pause();
            try
            {
                GetParent().InvokeEventOnCoreClosing();
                core.StopCore();
            }
            finally
            {
                next?.Invoke();
            }
        }

        void RestartCoreWorker(Action next, bool isQuiet)
        {
            try
            {
                string cfg;
                Dictionary<string, string> envs;
                var finalConfig = configer.GetFinalConfig();
                if (finalConfig != null)
                {
                    cfg = VgcApis.Misc.Utils.FormatConfig(finalConfig);
                    envs = Misc.Utils.GetEnvVarsFromConfig(finalConfig);
                }
                else
                {
                    cfg = configer.GetConfig();
                    envs = Misc.Utils.GetEnvVarsFromConfig(cfg);
                }

                core.title = coreStates.GetTitle();
                if (isQuiet)
                {
                    core.RestartCoreIgnoreError(cfg, envs);
                }
                else
                {
                    core.RestartCore(cfg, envs);
                }
                bookKeeper?.Run();
            }
            finally
            {
                next?.Invoke();
            }
        }
        #endregion
    }
}
