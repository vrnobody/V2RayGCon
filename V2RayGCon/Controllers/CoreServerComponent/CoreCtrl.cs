using System;
using V2RayGCon.Resources.Resx;

namespace V2RayGCon.Controllers.CoreServerComponent
{
    sealed public class CoreCtrl :
        VgcApis.BaseClasses.ComponentOf<CoreServerCtrl>,
        VgcApis.Interfaces.CoreCtrlComponents.ICoreCtrl
    {
        Libs.V2Ray.Core v2rayCore;
        Services.Settings setting;
        Services.ConfigMgr configMgr;

        static long SpeedtestTimeout = VgcApis.Models.Consts.Core.SpeedtestTimeout;

        VgcApis.Libs.Tasks.Routine bookKeeper;

        VgcApis.Libs.Tasks.Bar isRecording = new VgcApis.Libs.Tasks.Bar();

        public CoreCtrl(
            Services.Settings setting,
            Services.ConfigMgr configMgr)
        {
            this.setting = setting;
            this.configMgr = configMgr;
        }

        CoreStates coreStates;
        Configer configer;
        Logger logger;

        public override void Prepare()
        {
            v2rayCore = new Libs.V2Ray.Core(setting);

            coreStates = GetSibling<CoreStates>();
            configer = GetSibling<Configer>();
            logger = GetSibling<Logger>();

            bookKeeper = new VgcApis.Libs.Tasks.Routine(RecordStatSample, 3000);
        }

        #region public mehtods
        // 非正常终止时调用 
        public void SetTitle(string title) => v2rayCore.title = title;

        public void BindEvents()
        {
            v2rayCore.OnLog += OnLogHandler;
            v2rayCore.OnCoreStatusChanged += OnCoreStateChangedHandler;
        }
        public void ReleaseEvents()
        {
            bookKeeper?.Dispose();
            v2rayCore.OnLog -= OnLogHandler;
            v2rayCore.OnCoreStatusChanged -= OnCoreStateChangedHandler;
        }

        public void StopCore() => StopCoreWorker(null);

        public void StopCoreThen() => StopCoreThen(null);

        public void StopCoreThen(Action next) =>
            VgcApis.Misc.Utils.RunInBackground(() => StopCoreWorker(next));

        public void RestartCore() => RestartCoreWorker(null);

        public void RestartCoreThen() => RestartCoreThen(null);
        public void RestartCoreThen(Action next) =>
            VgcApis.Misc.Utils.RunInBackground(() => RestartCoreWorker(next));

        public bool IsCoreRunning() => v2rayCore.isRunning;

        public void RunSpeedTest() => SpeedTestWorker(configer.GetConfig());
        #endregion

        #region private methods
        VgcApis.Models.Datas.StatsSample TakeStatisticsSample(int port)
        {
            if (!setting.isEnableStatistics || port <= 0)
            {
                return null;
            }
            return v2rayCore.QueryStatsApi(port);
        }

        void RecordStatSample()
        {
            if (!setting.isEnableStatistics
                || !IsCoreRunning()
                || setting.IsScreenLocked()
                || setting.IsClosing()
                || !isRecording.Install())
            {
                return;
            }

            try
            {
                var statsPort = coreStates.GetStatPort();
                if (statsPort > 0)
                {
                    var sample = v2rayCore.QueryStatsApi(statsPort);
                    coreStates.AddStatSample(sample);
                }
            }
            catch { }
            isRecording.Remove();
        }

        void OnCoreStateChangedHandler(object sender, EventArgs args)
        {
            VgcApis.Misc.Utils.RunInBackground(() =>
            {
                if (v2rayCore.isRunning)
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
            var cycles = Math.Max(1, setting.isUseCustomSpeedtestSettings ? setting.CustomSpeedtestCycles : 1);

            coreStates.SetSpeedTestResult(0);
            coreStates.SetStatus(I18N.Testing);

            logger.Log(I18N.Testing);
            for (int i = 0; i < cycles && !setting.isSpeedtestCancelled; i++)
            {
                var sr = configMgr.RunDefaultSpeedTest(rawConfig, coreStates.GetTitle(), (s, a) => logger.Log(a.Data));
                curDelay = sr.Item1;
                coreStates.AddStatSample(new VgcApis.Models.Datas.StatsSample(0, sr.Item2));
                ShowCurrentSpeedtestResult(I18N.CurSpeedtestResult, curDelay);
                if (curDelay == SpeedtestTimeout)
                {
                    continue;
                }

                avgDelay = VgcApis.Misc.Utils.SpeedtestMean(avgDelay, curDelay, VgcApis.Models.Consts.Config.CustomSpeedtestMeanWeight);
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

        void OnLogHandler(object sender, VgcApis.Models.Datas.StrEvent arg) =>
            logger.Log(arg.Data);

        void StopCoreWorker(Action next)
        {
            bookKeeper?.Pause();
            try
            {
                GetParent().InvokeEventOnCoreClosing();
                v2rayCore.StopCore();
            }
            finally
            {
                next?.Invoke();
            }
        }

        void RestartCoreWorker(Action next)
        {
            try
            {
                var finalConfig = configer.GetFinalConfig();
                if (finalConfig == null)
                {
                    StopCore();
                    return;
                }

                v2rayCore.title = coreStates.GetTitle();
                v2rayCore.RestartCore(finalConfig.ToString(), Misc.Utils.GetEnvVarsFromConfig(finalConfig));
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
