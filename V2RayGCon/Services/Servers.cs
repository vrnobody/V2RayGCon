using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
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
        Notifier notifier;

        ServersComponents.QueryHandler queryHandler;
        ServersComponents.IndexHandler indexHandler;

        public event EventHandler
            OnCoreStart, // ICoreServCtrl sender
            OnCoreClosing, // ICoreServCtrl sender
            OnCoreStop,

            OnServerPropertyChange,
            OnServerCountChange,

            // special events
            OnRequireFlyPanelReload;

        List<Controllers.CoreServerCtrl> coreServList =
            new List<Controllers.CoreServerCtrl>();

        ConcurrentDictionary<string, bool> markList = new ConcurrentDictionary<string, bool>();

        VgcApis.Libs.Tasks.LazyGuy lazyServerSettingsRecorder;
        readonly object serverListWriteLock = new object();
        VgcApis.Libs.Tasks.Bar speedTestingBar = new VgcApis.Libs.Tasks.Bar();

        Servers()
        {
            lazyServerSettingsRecorder = new VgcApis.Libs.Tasks.LazyGuy(
                SaveServersSettingsWorker,
                VgcApis.Models.Consts.Intervals.LazySaveServerListIntreval,
                1000)
            {
                Name = "Servers.SaveSettings()",
            };
        }

        public void Run(
           Settings setting,
           Cache cache,
           ConfigMgr configMgr,
           Notifier notifier)
        {
            this.notifier = notifier;
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
        public void ResetIndex() => indexHandler.ResetIndex();

        public void ResetIndexQuiet() => indexHandler.ResetIndexQuiet();

        public void ReverseSelectedByIndex()
        {
            SortSelectedServers((list) => indexHandler.ReverseCoreservCtrlListByIndex(ref list));
        }

        public void SortSelectedBySpeedTest()
        {
            SortSelectedServers((list) => indexHandler.SortCoreServCtrlListBySpeedTestResult(ref list));
        }
        public void SortSelectedByDownloadTotal()
        {
            SortSelectedServers((list) => indexHandler.SortCoreServerCtrlListByDownloadTotal(ref list));
        }
        public void SortSelectedByUploadTotal()
        {
            SortSelectedServers((list) => indexHandler.SortCoreServerCtrlListByUploadTotal(ref list));
        }


        public void SortSelectedByLastModifiedDate()
        {
            SortSelectedServers((list) => indexHandler.SortCoreServerCtrlListByLastModifyDate(ref list));
        }

        public void SortSelectedBySummary()
        {
            SortSelectedServers((list) => indexHandler.SortCoreServCtrlListBySummary(ref list));
        }

        #endregion

        #region querys
        public ReadOnlyCollection<ICoreServCtrl> GetRunningServers() =>
            queryHandler.GetRunningServers();

        public ReadOnlyCollection<ICoreServCtrl> GetAllServersOrderByIndex() =>
            queryHandler.GetAllServersOrderByIndex();

        public ReadOnlyCollection<ICoreServCtrl> GetTrackableServerList() =>
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
            lazyServerSettingsRecorder.Deadline();
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

        public void RequireFormMainReload() =>
            InvokeEventHandlerIgnoreError(OnRequireFlyPanelReload, this, EventArgs.Empty);

        /// <summary>
        /// return -1 when fail
        /// </summary>
        /// <returns></returns>
        public int GetAvailableHttpProxyPort()
        {
            List<ICoreServCtrl> list;

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

        public string[] GetMarkList() =>
            markList.Keys.OrderBy(x => x).ToArray();

        public void AddNewMark(string newMark)
        {
            if (string.IsNullOrEmpty(newMark) || markList.ContainsKey(newMark))
            {
                return;
            }
            markList.TryAdd(newMark, true);
        }

        public void UpdateMarkList()
        {
            List<string> marks = null;
            lock (serverListWriteLock)
            {
                marks = coreServList.Select(s => s.GetCoreStates().GetMark()).ToList();
            }

            markList.Clear();
            if (marks == null)
            {
                return;
            }

            var filtered = marks.Distinct().ToList();
            foreach (var mark in filtered)
            {
                AddNewMark(mark);
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

        public string PackSelectedServersV4(
            string orgUid, string pkgName,
             string interval, string url,
            VgcApis.Models.Datas.Enums.BalancerStrategies strategy,
            VgcApis.Models.Datas.Enums.PackageTypes packageType)
        {
            var servList = new List<VgcApis.Interfaces.ICoreServCtrl>();
            lock (serverListWriteLock)
            {
                servList = queryHandler.GetSelectedServers().ToList();
            }
            return PackServersIntoV4PackageWorker(
                servList, orgUid, pkgName, interval, url, strategy, packageType);
        }

        /// <summary>
        /// packageName is Null or empty ? "PackageV4" : packageName
        /// </summary>
        /// <param name="packageName"></param>
        /// <param name="servList"></param>
        public string PackServersV4Ui(
            List<VgcApis.Interfaces.ICoreServCtrl> servList,
            string orgUid,
            string packageName,
            string interval,
            string url,
            VgcApis.Models.Datas.Enums.BalancerStrategies strategy,
            VgcApis.Models.Datas.Enums.PackageTypes packageType)
        {
            if (servList == null || servList.Count < 1)
            {
                VgcApis.Misc.UI.MsgBoxAsync(I18N.ListIsEmpty);
                return "";
            }

            var uid = PackServersIntoV4PackageWorker(
                servList, orgUid, packageName, interval, url, strategy, packageType);
            Misc.UI.ShowMessageBoxDoneAsync();
            return uid;
        }

        public bool RunSpeedTestOnSelectedServers()
        {
            var evDone = new AutoResetEvent(false);
            var success = BatchSpeedTestWorkerThen(GetSelectedServer(), () => evDone.Set());
            notifier.BlockingWaitOne(evDone);
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

        public void StopAllServers()
        {
            List<Controllers.CoreServerCtrl> list;

            lock (serverListWriteLock)
            {
                list = coreServList.Where(c => c.GetCoreCtrl().IsCoreRunning()).ToList();
            }

            foreach (var serv in list)
            {
                serv.GetCoreCtrl().StopCore();
            }
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
                lazyServerSettingsRecorder.Deadline();
                UpdateMarkList();
                RequireFormMainReload();
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
                lazyServerSettingsRecorder.Deadline();
                UpdateMarkList();
                RequireFormMainReload();
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

        public void UpdateAllServersSummary()
        {
            var list = coreServList.ToList(); // clone
            foreach (var core in list)
            {
                try
                {
                    core.GetConfiger().UpdateSummaryQuiet();
                    if (core.GetCoreStates().GetLastModifiedUtcTicks() == 0)
                    {
                        var utcTicks = DateTime.UtcNow.Ticks;
                        core.GetCoreStates().SetLastModifiedUtcTicks(utcTicks);
                    }
                }
                catch { }
            }

            RequireFormMainReload();
            setting.LazyGC();
            lazyServerSettingsRecorder.Deadline();
            InvokeEventOnServerPropertyChange(this, EventArgs.Empty);
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
                lazyServerSettingsRecorder.Deadline();
                UpdateMarkList();
                RequireFormMainReload();
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
                newServer.GetConfiger().UpdateSummary();
                // UpdateSummaryThen will invoke OnServerPropertyChange.
                InvokeEventOnServerCountChange(this, EventArgs.Empty);
                RequireFormMainReload();
            }
            setting.LazyGC();
            lazyServerSettingsRecorder.Deadline();
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

        public string ReplaceOrAddNewServer(string orgUid, string newConfig) =>
            ReplaceOrAddNewServer(orgUid, newConfig, @"");

        public string ReplaceOrAddNewServer(string orgUid, string newConfig, string mark)
        {
            string orgConfig = null;

            lock (serverListWriteLock)
            {
                var orgServ = coreServList.FirstOrDefault(s => s.GetCoreStates().GetUid() == orgUid);
                if (orgServ != null)
                {
                    orgConfig = orgServ.GetConfiger().GetConfig();
                }
            }

            if (orgConfig != null)
            {
                ReplaceServerConfig(orgConfig, newConfig);
                return orgUid;
            }

            AddServer(newConfig, mark);
            lock (serverListWriteLock)
            {
                var newServ = coreServList.FirstOrDefault(s => s.GetConfiger().GetConfig() == newConfig);
                if (newServ != null)
                {
                    return newServ.GetCoreStates().GetUid();
                }
            }

            return string.Empty;
        }
        #endregion

        #region private methods
        void SaveServersSettingsWorker()
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

        void SortSelectedServers(Action<List<ICoreServCtrl>> sorter)
        {
            lock (serverListWriteLock)
            {
                var selectedServers = queryHandler.GetSelectedServers().ToList();
                sorter?.Invoke(selectedServers);
            }
            RequireFormMainReload();
            InvokeEventOnServerPropertyChange(this, EventArgs.Empty);
        }

        private List<ICoreServCtrl> GetSelectedServer()
        {
            lock (serverListWriteLock)
            {
                return queryHandler.GetSelectedServers(false).ToList();
            }
        }

        void InjectBalacerStrategy(
            ref JObject config,
            string interval,
            string url,
            VgcApis.Models.Datas.Enums.BalancerStrategies strategy)
        {
            switch (strategy)
            {
                case VgcApis.Models.Datas.Enums.BalancerStrategies.LeastPing:
                    try
                    {
                        config["observatory"] = JObject.Parse("{subjectSelector:['agentout']}");
                        if (!string.IsNullOrWhiteSpace(interval))
                        {
                            config["observatory"]["probeInterval"] = interval;
                        }
                        if (!string.IsNullOrWhiteSpace(url))
                        {
                            config["observatory"]["probeURL"] = url;
                        }
                        config["routing"]["balancers"][0]["strategy"] = JObject.Parse("{type:'leastPing'}");
                    }
                    catch { }
                    break;
                default:
                    break;
            }
        }

        string PackServersIntoV4PackageWorker(
           List<ICoreServCtrl> servList,
           string orgUid,
           string packageName,
           string interval,
           string url,
           VgcApis.Models.Datas.Enums.BalancerStrategies strategy,
           VgcApis.Models.Datas.Enums.PackageTypes packageType)
        {
            if (servList == null || servList.Count < 1)
            {
                return "";
            }

            JObject package = configMgr.GenV4ServersPackageConfig(
                servList, packageName, packageType);
            string mark;

            switch (packageType)
            {
                case VgcApis.Models.Datas.Enums.PackageTypes.Balancer:
                    InjectBalacerStrategy(ref package, interval, url, strategy);
                    mark = @"PackageV4";
                    break;
                case VgcApis.Models.Datas.Enums.PackageTypes.Chain:
                default:
                    mark = @"ChainV4";
                    break;
            }

            var newConfig = package.ToString(Formatting.None);
            string newUid = ReplaceOrAddNewServer(orgUid, newConfig, mark);

            UpdateMarkList();
            setting.SendLog(I18N.PackageDone);
            return newUid;
        }

        bool BatchSpeedTestWorkerThen(IEnumerable<ICoreServCtrl> servList, Action next)
        {
            if (!speedTestingBar.Install())
            {
                return false;
            }

            setting.isSpeedtestCancelled = false;

            var randList = VgcApis.Misc.Utils.Shuffle(servList);

            VgcApis.Misc.Utils.RunInBackground(() =>
            {
                Misc.Utils.ExecuteInParallel(
                    randList,
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
            VgcApis.Libs.Sys.FileLogger.Info("Servers.Cleanup() begin");

            setting.isServerTrackerOn = false;
            if (setting.GetShutdownReason() == VgcApis.Models.Datas.Enums.ShutdownReasons.Abort)
            {
                VgcApis.Libs.Sys.FileLogger.Info("Servers.Cleanup() abort");
                return;
            }


            VgcApis.Libs.Sys.FileLogger.Info("Servers.Cleanup() stop tracking");
            lazyServerTrackingTimer?.Timeout();
            lazyServerTrackingTimer?.Release();

            VgcApis.Libs.Sys.FileLogger.Info("Servers.Cleanup() save data");
            lazyServerSettingsRecorder?.Dispose();
            SaveServersSettingsWorker();
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
                        notifier.BlockingWaitOne(sayGoodbye);
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
