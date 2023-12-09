using System;
using System.Threading;
using V2RayGCon.Resources.Resx;
using VgcApis.Models.Datas;

namespace V2RayGCon.Controllers.CoreServerComponent
{
    public sealed class CoreCtrl
        : VgcApis.BaseClasses.ComponentOf<CoreServerCtrl>,
            VgcApis.Interfaces.CoreCtrlComponents.ICoreCtrl
    {
        Libs.V2Ray.Core core;
        readonly Services.Settings setting;
        readonly Services.ConfigMgr configMgr;
        readonly CoreInfo coreInfo;

        VgcApis.Libs.Tasks.Routine bookKeeper;
        readonly VgcApis.Libs.Tasks.Bar isRecording = new VgcApis.Libs.Tasks.Bar();

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
        public void DisposeCore()
        {
            StopCore();
            ReleaseEvents();
            core.Dispose();
        }

        public bool IsSpeedTesting() => !speedTestingEvt.WaitOne(0);

        public void ReleaseSpeedTestLock()
        {
            speedTestingEvt.Set();
        }

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
            var coreName = GetCustomCoreName();
            var text =
                configMgr.FetchWithCustomConfig(config, coreName, title, url, timeout)
                ?? string.Empty;
            coreStates.AddStatSample(new StatsSample(0, text.Length));
            return text;
        }

        public void StopCore() => StopCoreWorker(null);

        public void StopCoreThen() => StopCoreThen(null);

        public void StopCoreThen(Action next) =>
            VgcApis.Misc.Utils.RunInBackground(() => StopCoreWorker(next));

        public void RestartCore() => RestartCoreWorker(null, false);

        public void RestartCoreIgnoreError() => RestartCoreWorker(null, true);

        public void RestartCoreThen() => RestartCoreThen(null);

        public void RestartCoreThen(Action next) =>
            VgcApis.Misc.Utils.RunInBackground(() => RestartCoreWorker(next, false));

        public bool IsCoreRunning() => core.isRunning;

        public void RunSpeedTest()
        {
            AddToSpeedTestQueue();
            speedTestingEvt.WaitOne();
        }

        public void RunSpeedTestThen()
        {
            AddToSpeedTestQueue();
        }

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
            VgcApis.Misc.Utils.RunInBackground(() =>
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

        readonly ManualResetEvent speedTestingEvt = new ManualResetEvent(true);

        void AddToSpeedTestQueue()
        {
            speedTestingEvt.Reset();
            coreStates.SetSpeedTestResult(0);
            coreStates.SetStatus(I18N.Testing);
            configMgr.AddToSpeedTestQueue(GetParent());
        }

        void OnLogHandler(string msg) => logger.Log(msg);

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
                string cfg = configer.GetFinalConfig();
                core.title = coreStates.GetTitle();
                if (isQuiet)
                {
                    core.RestartCoreIgnoreError(cfg);
                }
                else
                {
                    core.RestartCore(cfg);
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
