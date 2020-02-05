using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using V2RayGCon.Resources.Resx;
using VgcApis.Interfaces;

namespace V2RayGCon.Services
{
    public class Servers :
        BaseClasses.SingletonService<Servers>,
        VgcApis.Interfaces.Services.IServersService
    {
        Settings setting = null;
        Cache cache = null;
        ConfigMgr configMgr;

        ServersComponents.QueryHandler queryHandler;
        ServersComponents.IndexHandler indexHandler;

        public event EventHandler
            OnCoreStart, // ICoreServCtrl sender
            OnCoreClosing, // ICoreServCtrl sender
            OnCoreStop,

            OnServerPropertyChange,
            OnServerCountChange,

            // special events 
            OnRequireFlyPanelUpdate,
            OnRequireFlyPanelReload;

        List<Controllers.CoreServerCtrl> coreServList =
            new List<Controllers.CoreServerCtrl>();

        List<string> markList = new List<string>();

        VgcApis.Libs.Tasks.LazyGuy serverSaver;
        readonly object serverListWriteLock = new object();
        VgcApis.Libs.Tasks.Bar speedTestingBar = new VgcApis.Libs.Tasks.Bar();

        Servers()
        {
            serverSaver = new VgcApis.Libs.Tasks.LazyGuy(
                SaveServersSettingsNow,
                VgcApis.Models.Consts.Intervals.LazySaveServerListIntreval);
        }

        public void Run(
           Settings setting,
           Cache cache,
           ConfigMgr configMgr)
        {
            this.configMgr = configMgr;
            this.cache = cache;
            this.setting = setting;
            InitServerCtrlList();
            UpdateMarkList();

            queryHandler = new ServersComponents.QueryHandler(
                serverListWriteLock,
                coreServList);

            indexHandler = new ServersComponents.IndexHandler(
                serverListWriteLock,
                coreServList);
        }

        #region sort
        public void ResetIndex() =>
            indexHandler.ResetIndex();

        public void ResetIndexQuiet() =>
            indexHandler.ResetIndexQuiet();

        public void SortSelectedBySpeedTest()
        {
            lock (serverListWriteLock)
            {
                var selectedServers = queryHandler.GetSelectedServers().ToList();
                indexHandler.SortCoreServCtrlListBySpeedTestResult(ref selectedServers);
            }
            RequireFormMainReload();
        }

        public void SortSelectedByLastModifiedDate()
        {
            lock (serverListWriteLock)
            {
                var selectedServers = queryHandler.GetSelectedServers().ToList();
                indexHandler.SortCoreServerCtrlListByLastModifyDate(ref selectedServers);
            }
            RequireFormMainReload();
        }

        public void SortSelectedBySummary()
        {
            lock (serverListWriteLock)
            {
                var selectedServers = queryHandler.GetSelectedServers().ToList();
                indexHandler.SortCoreServCtrlListBySummary(ref selectedServers);
            }
            RequireFormMainReload();
        }

        #endregion

        #region querys
        public ReadOnlyCollection<VgcApis.Interfaces.ICoreServCtrl>
          GetRunningServers() =>
          queryHandler.GetRunningServers();

        public ReadOnlyCollection<VgcApis.Interfaces.ICoreServCtrl>
           GetAllServersOrderByIndex() =>
           queryHandler.GetAllServersOrderByIndex();

        public ReadOnlyCollection<VgcApis.Interfaces.ICoreServCtrl>
            GetTrackableServerList() =>
            queryHandler.GetTrackableServerList();

        #endregion

        #region event relay

        void InvokeEventOnServerCountChange(object sender, EventArgs args) =>
            InvokeEventHandlerIgnoreError(OnServerCountChange, sender, EventArgs.Empty);

        void InvokeEventHandlerIgnoreError(EventHandler handler, object sender, EventArgs args)
        {
            try
            {
                handler?.Invoke(sender, args);
            }
            catch { }
        }

        // must transfer sender!
        void InvokeEventOnCoreStartIgnoreError(object sender, EventArgs args) =>
            InvokeEventHandlerIgnoreError(OnCoreStart, sender, EventArgs.Empty);

        // must transfer sender!
        void InvokeEventOnCoreClosingIgnoreError(object sender, EventArgs args) =>
            InvokeEventHandlerIgnoreError(OnCoreClosing, sender, EventArgs.Empty);

        // must transfer sender!
        void InvokeEventOnCoreStopIgnoreError(object sender, EventArgs args) =>
            InvokeEventHandlerIgnoreError(OnCoreStop, sender, EventArgs.Empty);

        void InvokeEventOnServerPropertyChange(object sender, EventArgs arg)
        {
            serverSaver.DoItLater();
            InvokeEventHandlerIgnoreError(OnServerPropertyChange, null, EventArgs.Empty);
        }

        void OnTrackCoreStartHandler(object sender, EventArgs args) =>
            TrackCoreRunningStateHandler(sender, true);

        void OnTrackCoreStopHandler(object sender, EventArgs args) =>
            TrackCoreRunningStateHandler(sender, false);

        void BindEventsTo(Controllers.CoreServerCtrl server)
        {
            server.OnCoreClosing += InvokeEventOnCoreClosingIgnoreError;
            server.OnCoreStart += OnTrackCoreStartHandler;
            server.OnCoreStop += OnTrackCoreStopHandler;

            server.OnPropertyChanged += InvokeEventOnServerPropertyChange;
        }

        void ReleaseEventsFrom(Controllers.CoreServerCtrl server)
        {
            server.OnCoreClosing -= InvokeEventOnCoreClosingIgnoreError;
            server.OnCoreStart -= OnTrackCoreStartHandler;
            server.OnCoreStop -= OnTrackCoreStopHandler;

            server.OnPropertyChanged -= InvokeEventOnServerPropertyChange;
        }
        #endregion

        #region server tracking

        void ServerTrackingUpdateWorker(
            Controllers.CoreServerCtrl coreServCtrl,
            bool isStart)
        {
            var config = coreServCtrl?.GetConfiger()?.GetConfig();

            var curTrackerSetting =
                configMgr.GenCurTrackerSetting(
                    coreServList.AsReadOnly(),
                    config ?? string.Empty,
                    isStart);

            setting.SaveServerTrackerSetting(curTrackerSetting);
            return;
        }

        Libs.Sys.CancelableTimeout lazyServerTrackingTimer = null;
        void DoServerTrackingLater(Action onTimeout)
        {
            lazyServerTrackingTimer?.Release();
            lazyServerTrackingTimer = null;
            lazyServerTrackingTimer = new Libs.Sys.CancelableTimeout(onTimeout, 2000);
            lazyServerTrackingTimer.Start();
        }

        void TrackCoreRunningStateHandler(object sender, bool isCoreStart)
        {
            // for plugins
            if (isCoreStart)
            {
                InvokeEventOnCoreStartIgnoreError(sender, EventArgs.Empty);
            }
            else
            {
                InvokeEventOnCoreStopIgnoreError(sender, EventArgs.Empty);
            }

            if (!setting.isServerTrackerOn)
            {
                return;
            }

            var server = sender as Controllers.CoreServerCtrl;
            if (server.GetCoreStates().IsUntrack())
            {
                return;
            }

            DoServerTrackingLater(
                () => ServerTrackingUpdateWorker(
                    server, isCoreStart));
        }
        #endregion

        #region public method

        // expose to launcher for shutdown
        public void SaveServersSettingsNow()
        {
            List<VgcApis.Models.Datas.CoreInfo> coreInfoList;
            lock (serverListWriteLock)
            {
                coreInfoList = coreServList
                   .Select(s => s.GetCoreStates().GetAllRawCoreInfo())
                   .ToList();
            }
            setting.SaveServerList(coreInfoList);
        }

        /// <summary>
        /// Add new only.
        /// </summary>
        public void RequireFormMainUpdate() =>
            InvokeEventHandlerIgnoreError(OnRequireFlyPanelUpdate, this, EventArgs.Empty);

        /// <summary>
        /// Remove all then add new.
        /// </summary>
        public void RequireFormMainReload() =>
            InvokeEventHandlerIgnoreError(OnRequireFlyPanelReload, this, EventArgs.Empty);

        /// <summary>
        /// return -1 when fail
        /// </summary>
        /// <returns></returns>
        public int GetAvailableHttpProxyPort()
        {
            List<VgcApis.Interfaces.ICoreServCtrl> list;

            lock (serverListWriteLock)
            {
                list = GetAllServersOrderByIndex()
                    .Where(s => s.GetCoreCtrl().IsCoreRunning())
                    .ToList();
            }

            foreach (var serv in list)
            {
                if (serv.GetConfiger().IsSuitableToBeUsedAsSysProxy(
                    true, out bool isSocks, out int port))
                {
                    return port;
                }
            }
            return -1;
        }

        public void OnAutoTrackingOptionChanged() =>
            ServerTrackingUpdateWorker(null, false);

        public int CountSelectedServers()
        {
            lock (serverListWriteLock)
            {
                return coreServList.Count(s => s.GetCoreStates().IsSelected());
            }
        }

        public int CountAllServers() => coreServList.Count;

        public void SetAllServerIsSelected(bool isSelected)
        {
            List<Controllers.CoreServerCtrl> cache;

            lock (serverListWriteLock)
            {
                cache = coreServList.ToList();
            }

            foreach (var c in cache)
            {
                try
                {
                    c.GetCoreStates().SetIsSelected(isSelected);
                }
                catch { }
            }
        }

        public ReadOnlyCollection<string> GetMarkList() =>
             markList.AsReadOnly();

        public void AddNewMark(string newMark)
        {
            if (!markList.Contains(newMark))
            {
                UpdateMarkList();
            }
        }

        public void UpdateMarkList()
        {
            lock (serverListWriteLock)
            {
                markList = coreServList
                    .Select(s => s.GetCoreStates().GetMark())
                    .Distinct()
                    .Where(s => !string.IsNullOrEmpty(s))
                    .OrderBy(s => s)
                    .ToList();
            }
        }

        public void RestartServersWithImportMark()
        {
            var list = new List<Controllers.CoreServerCtrl>();
            lock (serverListWriteLock)
            {
                list = coreServList
                    .Where(s => s.GetCoreStates().IsInjectGlobalImport() && s.GetCoreCtrl().IsCoreRunning())
                    .OrderBy(s => s.GetCoreStates().GetIndex())
                    .ToList();
            }

            RestartServersThen(list);
        }

        public bool IsEmpty()
        {
            lock (serverListWriteLock)
            {
                return !(this.coreServList.Any());
            }
        }

        public bool IsSelecteAnyServer()
        {
            lock (serverListWriteLock)
            {
                return coreServList.Any(s => s.GetCoreStates().IsSelected());
            }
        }

        public string PackSelectedServersIntoV4Package(string orgUid, string pkgName)
        {
            var servList = new List<VgcApis.Interfaces.ICoreServCtrl>();
            lock (serverListWriteLock)
            {
                servList = queryHandler.GetSelectedServers().ToList();
            }
            return PackServersIntoV4PackageWorker(servList, orgUid, pkgName, true);
        }

        /// <summary>
        /// packageName is Null or empty ? "PackageV4" : packageName
        /// </summary>
        /// <param name="packageName"></param>
        /// <param name="servList"></param>
        public string PackServersIntoV4Package(
            List<VgcApis.Interfaces.ICoreServCtrl> servList,
            string orgUid,
            string packageName) =>
            PackServersIntoV4PackageWorker(
                servList, orgUid, packageName, false);

        public bool RunSpeedTestOnSelectedServers()
        {
            var evDone = new AutoResetEvent(false);
            var success = BatchSpeedTestWorkerThen(GetSelectedServer(), () => evDone.Set());
            evDone.WaitOne();
            return success;
        }

        public void RunSpeedTestOnSelectedServersBg()
        {
            var success = BatchSpeedTestWorkerThen(
                GetSelectedServer(),
                () => MessageBox.Show(I18N.SpeedTestFinished));
            if (!success)
            {
                MessageBox.Show(I18N.LastTestNoFinishYet);
            }
        }

        public void RestartServersThen(
            IEnumerable<VgcApis.Interfaces.ICoreServCtrl> servers,
            Action done = null)
        {
            var list = new List<VgcApis.Interfaces.ICoreServCtrl>();
            lock (serverListWriteLock)
            {
                list = servers.ToList();
            }
            void worker(int index, Action next)
            {
                list[index].GetCoreCtrl().RestartCoreThen(next);
            }

            Misc.Utils.ChainActionHelperAsync(list.Count, worker, done);
        }

        public void WakeupServersInBootList()
        {
            List<Controllers.CoreServerCtrl> bootList = configMgr.GenServersBootList(coreServList);

            void worker(int index, Action next)
            {
                bootList[index].GetCoreCtrl().RestartCoreThen(next);
            }

            Misc.Utils.ChainActionHelperAsync(bootList.Count, worker);
        }

        public void RestartSelectedServersThen(Action done = null)
        {
            var list = coreServList;

            void worker(int index, Action next)
            {
                if (list[index].GetCoreStates().IsSelected())
                {
                    list[index].GetCoreCtrl().RestartCoreThen(next);
                }
                else
                {
                    next();
                }
            }

            Misc.Utils.ChainActionHelperAsync(list.Count, worker, done);
        }

        public void StopSelectedServersThen(Action lambda = null)
        {
            var list = coreServList;

            void worker(int index, Action next)
            {
                if (list[index].GetCoreStates().IsSelected())
                {
                    list[index].GetCoreCtrl().StopCoreThen(next);
                }
                else
                {
                    next();
                }
            }

            Misc.Utils.ChainActionHelperAsync(list.Count, worker, lambda);
        }

        public void StopAllServersThen(Action lambda = null)
        {

            List<Controllers.CoreServerCtrl> list;
            lock (serverListWriteLock)
            {
                list = coreServList.Where(c => c.GetCoreCtrl().IsCoreRunning()).ToList();
            }

            void worker(int index, Action next)
            {
                list[index].GetCoreCtrl().StopCoreThen(next);
            }

            Misc.Utils.ChainActionHelperAsync(list.Count, worker, lambda);
        }

        public void DeleteSelectedServersThen(Action done = null)
        {
            if (!speedTestingBar.Install())
            {
                MessageBox.Show(I18N.LastTestNoFinishYet);
                return;
            }

            List<Controllers.CoreServerCtrl> coreServs;
            lock (serverListWriteLock)
            {
                coreServs = coreServList.Where(cs => cs.GetCoreStates().IsSelected()).ToList();
                foreach (var cs in coreServs)
                {
                    coreServList.Remove(cs);
                }
            }

            void worker(int index, Action next)
            {
                var cs = coreServs[index];
                DisposeCoreServThen(cs, next);
            }

            void finish()
            {
                serverSaver.DoItLater();
                UpdateMarkList();
                RequireFormMainUpdate();
                InvokeEventOnServerCountChange(this, EventArgs.Empty);
                speedTestingBar.Remove();

                done?.Invoke();
            }

            Misc.Utils.ChainActionHelperAsync(coreServs.Count, worker, finish);
        }

        public void DeleteAllServersThen(Action done = null)
        {
            if (!speedTestingBar.Install())
            {
                MessageBox.Show(I18N.LastTestNoFinishYet);
                return;
            }

            void finish()
            {
                serverSaver.DoItLater();
                UpdateMarkList();
                RequireFormMainUpdate();
                InvokeEventOnServerCountChange(this, EventArgs.Empty);
                speedTestingBar.Remove();
                done?.Invoke();
            }

            List<Controllers.CoreServerCtrl> servs;
            lock (serverListWriteLock)
            {
                servs = coreServList.ToList();
                coreServList.Clear();
            }

            void worker(int index, Action next)
            {
                var cs = servs[index];
                DisposeCoreServThen(cs, next);
            }

            Misc.Utils.ChainActionHelperAsync(servs.Count, worker, finish);
        }

        public void UpdateAllServersSummarySync()
        {
            var list = coreServList;
            AutoResetEvent isFinished = new AutoResetEvent(false);


            void worker(int index, Action next)
            {
                var core = list[index];

                try
                {
                    if (core.GetCoreStates().GetLastModifiedUtcTicks() == 0)
                    {
                        var utcTicks = DateTime.UtcNow.Ticks;
                        core.GetCoreStates().SetLastModifiedUtcTicks(utcTicks);
                    }
                }
                catch { }

                try
                {
                    core.GetConfiger().UpdateSummaryThen(next);
                }
                catch
                {
                    // skip if something goes wrong
                    next();
                }
            }

            void done()
            {
                setting.LazyGC();
                serverSaver.DoItLater();
                RequireFormMainUpdate();
                InvokeEventOnServerPropertyChange(this, EventArgs.Empty);
                isFinished.Set();
            }

            Misc.Utils.ChainActionHelper(list.Count, worker, done);
            isFinished.WaitOne();
        }

        public void UpdateAllServersSummaryBg()
        {
            VgcApis.Misc.Utils.RunInBackground(UpdateAllServersSummarySync);
        }

        public void DeleteServerByConfig(string config)
        {
            if (!speedTestingBar.Install())
            {
                MessageBox.Show(I18N.LastTestNoFinishYet);
                return;
            }

            Controllers.CoreServerCtrl coreServ;
            lock (serverListWriteLock)
            {
                coreServ = coreServList.FirstOrDefault(cs => cs.GetConfiger().GetConfig() == config);
                if (coreServ != null)
                {
                    coreServList.Remove(coreServ);
                }
            }

            if (coreServ == null)
            {
                MessageBox.Show(I18N.CantFindOrgServDelFail);
                speedTestingBar.Remove();
                return;
            }

            DisposeCoreServThen(coreServ, () =>
            {
                InvokeEventOnServerCountChange(this, EventArgs.Empty);
                serverSaver.DoItLater();
                UpdateMarkList();
                RequireFormMainUpdate();
                speedTestingBar.Remove();
            });
        }

        public bool IsServerExist(string config)
        {
            lock (serverListWriteLock)
            {
                return coreServList
                    .Any(s => s.GetConfiger().GetConfig() == config);
            }
        }

        public bool AddServer(string config, string mark, bool quiet = false)
        {
            // first check
            if (IsServerExist(config))
            {
                return false;
            }

            var coreInfo = new VgcApis.Models.Datas.CoreInfo
            {
                foldingLevel = setting.CustomDefImportIsFold ? 1 : 0,
                isInjectImport = setting.CustomDefImportGlobalImport,
                isInjectSkipCNSite = setting.CustomDefImportBypassCnSite,
                customInbType = setting.CustomDefImportMode,
                inbIp = setting.CustomDefImportIp,
                inbPort = setting.CustomDefImportPort,
                config = config,
                customMark = mark,
            };

            var newServer = new Controllers.CoreServerCtrl(coreInfo);
            newServer.Run(cache, setting, configMgr, this);

            bool duplicated = true;
            lock (serverListWriteLock)
            {
                // double check
                if (!IsServerExist(config))
                {
                    coreServList.Add(newServer);
                    var idx = coreServList.Count();
                    newServer.GetCoreStates().SetIndexQuiet(idx);
                    AddNewMark(mark);
                    duplicated = false;
                }
            }

            if (duplicated)
            {
                newServer.Dispose();
                return false;
            }

            BindEventsTo(newServer);
            if (!quiet)
            {
                newServer.GetConfiger().UpdateSummaryThen(() =>
                {
                    // UpdateSummaryThen will invoke OnServerPropertyChange.
                    InvokeEventOnServerCountChange(this, EventArgs.Empty);
                    RequireFormMainUpdate();
                });
            }
            setting.LazyGC();
            serverSaver.DoItLater();
            return true;
        }

        public bool ReplaceServerConfig(string orgConfig, string newConfig)
        {
            Controllers.CoreServerCtrl coreCtrl;

            lock (serverListWriteLock)
            {
                coreCtrl = coreServList.FirstOrDefault(cs => cs.GetConfiger().GetConfig() == orgConfig);
            }

            if (coreCtrl == null)
            {
                return false;
            }

            coreCtrl.GetConfiger().SetConfig(newConfig);
            coreCtrl.GetCoreStates().SetLastModifiedUtcTicks(DateTime.UtcNow.Ticks);
            return true;
        }

        public string ReplaceOrAddNewServer(string orgUid, string newConfig)
        {
            lock (serverListWriteLock)
            {
                var servUid = "";

                var orgServ = coreServList.FirstOrDefault(s => s.GetCoreStates().GetUid() == orgUid);
                if (orgServ != null)
                {
                    ReplaceServerConfig(orgServ.GetConfiger().GetConfig(), newConfig);
                    servUid = orgUid;
                }
                else
                {
                    AddServer(newConfig, "PackageV4");
                    var newServ = coreServList.FirstOrDefault(s => s.GetConfiger().GetConfig() == newConfig);
                    if (newServ != null)
                    {
                        servUid = newServ.GetCoreStates().GetUid();
                    }
                }

                return servUid;
            }
        }
        #endregion

        #region private methods

        private List<ICoreServCtrl> GetSelectedServer()
        {
            lock (serverListWriteLock)
            {
                return queryHandler.GetSelectedServers(false).ToList();
            }
        }

        string PackServersIntoV4PackageWorker(
           List<VgcApis.Interfaces.ICoreServCtrl> servList,
           string orgUid,
           string packageName,
           bool quiet)
        {
            if (servList == null || servList.Count <= 0)
            {
                if (!quiet)
                {
                    VgcApis.Misc.UI.MsgBoxAsync(I18N.ListIsEmpty);
                }
                return "";
            }

            JObject package = configMgr.GenV4ServersPackage(servList, packageName);

            var newConfig = package.ToString(Formatting.None);
            string newUid = ReplaceOrAddNewServer(orgUid, newConfig);

            UpdateMarkList();
            setting.SendLog(I18N.PackageDone);
            if (!quiet)
            {
                Misc.UI.ShowMessageBoxDoneAsync();
            }
            return newUid;
        }

        bool BatchSpeedTestWorkerThen(IEnumerable<ICoreServCtrl> servList, Action next)
        {
            if (!speedTestingBar.Install())
            {
                return false;
            }

            setting.isSpeedtestCancelled = false;

            VgcApis.Misc.Utils.RunInBackground(() =>
            {
                Misc.Utils.ExecuteInParallel(
                    servList,
                    serv => serv.GetCoreCtrl().RunSpeedTest());
                speedTestingBar.Remove();
                setting.SendLog(I18N.SpeedTestFinished);
                next?.Invoke();
            });
            return true;
        }

        void InitServerCtrlList()
        {
            lock (serverListWriteLock)
            {
                var coreInfoList = setting.LoadCoreInfoList();
                foreach (var coreInfo in coreInfoList)
                {
                    var server = new Controllers.CoreServerCtrl(coreInfo);
                    coreServList.Add(server);
                }
            }

            foreach (var server in coreServList)
            {
                server.Run(cache, setting, configMgr, this);
                BindEventsTo(server);
            }
        }

        void DisposeCoreServThen(Controllers.CoreServerCtrl coreServ, Action next = null)
        {
            var copy = coreServ;

            VgcApis.Misc.Utils.RunInBackground(() =>
            {
                ReleaseEventsFrom(copy);
                copy.Dispose();
                next?.Invoke();
            });
        }

        #endregion

        #region protected methods
        protected override void Cleanup()
        {
            setting.isServerTrackerOn = false;

            if (setting.ShutdownReason == VgcApis.Models.Datas.Enums.ShutdownReasons.Abort)
            {
                return;
            }

            VgcApis.Libs.Sys.FileLogger.Info("Services.Servers.Cleanup");
            VgcApis.Libs.Sys.FileLogger.Info("Services.StopTracking");

            lazyServerTrackingTimer?.Timeout();
            lazyServerTrackingTimer?.Release();

            VgcApis.Libs.Sys.FileLogger.Info("Services.SaveSettings");
            serverSaver.DoItNow();
            serverSaver.Quit();

            // let it go
            var cores = coreServList;
            VgcApis.Misc.Utils.RunInBackground(() =>
            {
                VgcApis.Libs.Sys.FileLogger.Info("Stop cores quiet begin.");
                foreach (var core in cores)
                {
                    core.GetCoreCtrl().StopCoreQuiet();
                }
                VgcApis.Libs.Sys.FileLogger.Info("Stop cores quiet done.");
            });
        }

        #endregion

        #region debug
#if DEBUG
        public void DbgFastRestartTest(int round)
        {
            var list = coreServList.ToList();
            var rnd = new Random();

            var count = list.Count;
            VgcApis.Misc.Utils.RunInBackground(() =>
            {
                var taskList = new List<Task>();
                for (int i = 0; i < round; i++)
                {
                    var index = rnd.Next(0, count);
                    var isStopCore = rnd.Next(0, 2) == 0;
                    var server = list[index];

                    var task = new Task(() =>
                    {
                        AutoResetEvent sayGoodbye = new AutoResetEvent(false);
                        if (isStopCore)
                        {
                            server.GetCoreCtrl().StopCoreThen(() => sayGoodbye.Set());
                        }
                        else
                        {
                            server.GetCoreCtrl().RestartCoreThen(() => sayGoodbye.Set());
                        }
                        sayGoodbye.WaitOne();
                    }, TaskCreationOptions.LongRunning);

                    taskList.Add(task);
                    task.Start();
                }

                Task.WaitAll(taskList.ToArray());
                MessageBox.Show(I18N.Done);
            });
        }
#endif
        #endregion
    }
}
