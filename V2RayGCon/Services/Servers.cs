using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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

        // uid => CoreServ
        Dictionary<string, Controllers.CoreServerCtrl> coreServCache =
            new Dictionary<string, Controllers.CoreServerCtrl>();

        ConcurrentDictionary<string, bool> markList = new ConcurrentDictionary<string, bool>();
        ConcurrentDictionary<string, string> configCache = new ConcurrentDictionary<string, string>();

        VgcApis.Libs.Tasks.LazyGuy lazyServerSettingsRecorder;
        ReaderWriterLockSlim locker = new ReaderWriterLockSlim();
        VgcApis.Libs.Tasks.Bar speedTestingBar = new VgcApis.Libs.Tasks.Bar();

        Servers()
        {
            lazyServerSettingsRecorder = new VgcApis.Libs.Tasks.LazyGuy(
                SaveServersSettingsWorker,
                VgcApis.Models.Consts.Intervals.LazySaveServerListIntreval,
                2 * 1000)
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
                locker,
                coreServCache);

            indexHandler = new ServersComponents.IndexHandler(
                locker,
                coreServCache);
        }

        #region sort
        public void ResetIndex() => indexHandler.ResetIndex(false);

        public void ResetIndexQuiet() => indexHandler.ResetIndex(true);

        public void ReverseSelectedByIndex()
        {
            SortSelectedServers((list) => indexHandler.ReverseCoreservCtrlListByIndex(ref list));
        }

        public void ReverseServersByIndex(List<string> uids)
        {
            var coreServs = queryHandler.GetServersByUids(uids);
            SortServers(coreServs, (list) => indexHandler.ReverseCoreservCtrlListByIndex(ref list));
        }

        public void SortSelectedBySpeedTest()
        {
            SortSelectedServers((list) => indexHandler.SortCoreServCtrlListBySpeedTestResult(ref list));
        }

        public void SortServersBySpeedTest(List<string> uids)
        {
            var coreServs = queryHandler.GetServersByUids(uids);
            SortServers(coreServs, (list) => indexHandler.SortCoreServCtrlListBySpeedTestResult(ref list));
        }

        public void SortSelectedByDownloadTotal()
        {
            SortSelectedServers((list) => indexHandler.SortCoreServerCtrlListByDownloadTotal(ref list));
        }

        public void SortServersByDownloadTotal(List<string> uids)
        {
            var coreServs = queryHandler.GetServersByUids(uids);
            SortServers(coreServs, (list) => indexHandler.SortCoreServerCtrlListByDownloadTotal(ref list));
        }

        public void SortSelectedByUploadTotal()
        {
            SortSelectedServers((list) => indexHandler.SortCoreServerCtrlListByUploadTotal(ref list));
        }

        public void SortServersByUploadTotal(List<string> uids)
        {
            var coreServs = queryHandler.GetServersByUids(uids);
            SortServers(coreServs, (list) => indexHandler.SortCoreServerCtrlListByUploadTotal(ref list));
        }

        public void SortSelectedByLastModifiedDate()
        {
            SortSelectedServers((list) => indexHandler.SortCoreServerCtrlListByLastModifyDate(ref list));
        }

        public void SortServersByLastModifiedDate(List<string> uids)
        {
            var coreServs = queryHandler.GetServersByUids(uids);
            SortServers(coreServs, (list) => indexHandler.SortCoreServerCtrlListByLastModifyDate(ref list));
        }

        public void SortSelectedBySummary()
        {
            SortSelectedServers((list) => indexHandler.SortCoreServCtrlListBySummary(ref list));
        }

        public void SortServersBySummary(List<string> uids)
        {
            var coreServs = queryHandler.GetServersByUids(uids);
            SortServers(coreServs, (list) => indexHandler.SortCoreServCtrlListBySummary(ref list));
        }


        #endregion

        #region querys
        public List<ICoreServCtrl> GetSelectedServers() =>
            queryHandler.GetSelectedServers();

        public List<ICoreServCtrl> GetRunningServers() =>
            queryHandler.GetRunningServers();

        public List<ICoreServCtrl> GetAllServersOrderByIndex() =>
            queryHandler.GetAllServers(false);

        public List<ICoreServCtrl> GetServersByUidsOrderByIndex(IEnumerable<string> uids) =>
            queryHandler.GetServersByUids(uids)
                .OrderBy(cs => cs.GetCoreStates().GetIndex())
                .ToList();

        public List<ICoreServCtrl> GetTrackableServerList() =>
            queryHandler.GetTrackableServerList();

        #endregion

        #region event relay

        void InvokeEventOnServerCountChange(object sender, EventArgs args)
        {
            if (selectedServersCountCache != -1)
            {
                selectedServersCountCache = -1;
            }
            InvokeEventHandlerIgnoreError(OnServerCountChange, sender, EventArgs.Empty);
        }

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
            if (selectedServersCountCache != -1)
            {
                selectedServersCountCache = -1;
            }
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

            var isTrackerOn = setting.GetServerTrackerSetting().isTrackerOn;
            DoServerTrackingLater(() => UpdateServerTrackerSettings(isTrackerOn));
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
            List<ICoreServCtrl> list = GetRunningServers();

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

        public void UpdateServerTrackerSettings(bool isTrackerOn)
        {
            var tracker = new Models.Datas.ServerTracker();

            tracker.isTrackerOn = isTrackerOn;

            tracker.curServer = string.Empty; // obsolete
            tracker.serverList = new List<string>(); //obsolete

            tracker.uids = tracker.isTrackerOn ? GetRunningServers()
                    .Where(s => !s.GetCoreStates().IsUntrack())
                    .Select(s => s.GetCoreStates().GetUid())
                    .ToList()
                    : new List<string>();

            setting.SaveServerTrackerSetting(tracker);
            return;
        }


        int selectedServersCountCache = -1;
        public int CountSelected()
        {
            if (selectedServersCountCache < 0)
            {
                locker.EnterReadLock();
                try
                {
                    selectedServersCountCache = coreServCache.Count(kv => kv.Value.GetCoreStates().IsSelected());
                }
                finally
                {
                    locker.ExitReadLock();
                }
            }

            return selectedServersCountCache;
        }

        public int Count() => coreServCache.Count;

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
            locker.EnterReadLock();
            try
            {
                markList.Clear();
                foreach (var kv in coreServCache)
                {
                    var mark = kv.Value.GetCoreStates().GetMark();
                    AddNewMark(mark);
                }
            }
            finally
            {
                locker.ExitReadLock();
            }
        }

        public void RestartServersWithImportMark()
        {
            var list = queryHandler.GetServers(
                kv => kv.Value.GetCoreStates().IsInjectGlobalImport() && kv.Value.GetCoreCtrl().IsCoreRunning());
            RestartServersThen(list);
        }

        public bool IsEmpty()
        {
            locker.EnterReadLock();
            try
            {
                return !(this.coreServCache.Any());
            }
            finally
            {
                locker.ExitReadLock();
            }
        }

        public bool IsSelecteAnyServer()
        {
            locker.EnterReadLock();
            try
            {
                return coreServCache.Any(kv => kv.Value.GetCoreStates().IsSelected());
            }
            finally
            {
                locker.ExitReadLock();
            }
        }

        public string PackServersWithUidsV4(
            List<string> uids,
            string orgUid, string pkgName,
            string interval, string url,
            VgcApis.Models.Datas.Enums.BalancerStrategies strategy,
            VgcApis.Models.Datas.Enums.PackageTypes packageType)
        {
            var coreServs = queryHandler.GetServersByUids(uids);
            return PackServersIntoV4PackageWorker(
                coreServs, orgUid, pkgName, interval, url, strategy, packageType);
        }

        public string PackSelectedServersV4(
            string orgUid, string pkgName,
            string interval, string url,
            VgcApis.Models.Datas.Enums.BalancerStrategies strategy,
            VgcApis.Models.Datas.Enums.PackageTypes packageType)
        {
            var servList = GetSelectedServers();
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
            var success = BatchSpeedTestWorkerThen(GetSelectedServers(), () => evDone.Set());
            notifier.BlockingWaitOne(evDone);
            return success;
        }

        public bool RunSpeedTestBgQuiet(List<string> uids)
        {
            var coreServs = queryHandler.GetServersByUids(uids);
            if (coreServs == null || coreServs.Count < 1)
            {
                return false;
            }
            return BatchSpeedTestWorkerThen(coreServs, null);
        }

        public bool RunSpeedTestOnSelectedServersBgQuiet()
        {
            return BatchSpeedTestWorkerThen(GetSelectedServers(), null);
        }

        public bool IsRunningSpeedTest()
        {
            var r = speedTestingBar.Install();
            if (r)
            {
                speedTestingBar.Remove();
            }
            return !r;
        }

        public void StopSpeedTest()
        {
            setting.isSpeedtestCancelled = true;
        }

        public void RunSpeedTestOnSelectedServersBg()
        {
            var success = BatchSpeedTestWorkerThen(
                GetSelectedServers(),
                () => MessageBox.Show(I18N.SpeedTestFinished));
            if (!success)
            {
                MessageBox.Show(I18N.LastTestNoFinishYet);
            }
        }

        public void RestartServersThen(
            List<VgcApis.Interfaces.ICoreServCtrl> coreServs,
            Action done = null)
        {
            void worker(int index, Action next)
            {
                var coreServ = coreServs[index];
                if (coreServ == null)
                {
                    next();
                }
                else
                {
                    coreServ.GetCoreCtrl().RestartCoreThen(next);
                }
            }
            Misc.Utils.ChainActionHelperAsync(coreServs.Count, worker, done);
        }

        public void WakeupServersInBootList()
        {
            List<Controllers.CoreServerCtrl> bootList = GenServersBootList();

            void worker(int index, Action next)
            {
                bootList[index].GetCoreCtrl().RestartCoreThen(next);
            }

            Misc.Utils.ChainActionHelperAsync(bootList.Count, worker);
        }

        public void RestartSelectedServersThen(Action done = null)
        {
            var list = GetSelectedServers();
            void worker(int index, Action next)
            {
                list[index].GetCoreCtrl().RestartCoreThen(next);
            }
            Misc.Utils.ChainActionHelperAsync(list.Count, worker, done);
        }

        public void StopSelectedServersThen(Action lambda = null)
        {
            var list = GetSelectedServers();

            void worker(int index, Action next)
            {
                list[index].GetCoreCtrl().StopCoreThen(next);
            }

            Misc.Utils.ChainActionHelperAsync(list.Count, worker, lambda);
        }

        public void StopAllServers()
        {
            var coreServs = GetRunningServers();
            foreach (var coreServ in coreServs)
            {
                coreServ.GetCoreCtrl().StopCore();
            }
        }

        public void RestartOneServerByUid(string uid)
        {
            StopAllServers();
            queryHandler.GetServersByUid(uid)?.GetCoreCtrl().RestartCore();
        }

        public void StopAllServersThen(Action lambda = null)
        {
            var list = GetRunningServers();
            void worker(int index, Action next)
            {
                list[index].GetCoreCtrl().StopCoreThen(next);
            }
            Misc.Utils.ChainActionHelperAsync(list.Count, worker, lambda);
        }

        public bool DeleteServerByUid(string uid)
        {
            Controllers.CoreServerCtrl coreServ = null;
            locker.EnterWriteLock();
            try
            {
                coreServ = DeleteServerByUidWorker(uid);
            }
            finally
            {
                locker.ExitWriteLock();
            }

            if (coreServ != null)
            {
                ReleaseEventsFrom(coreServ);
                coreServ.Dispose();
                RefreshUiAfterCoreServersAreDeleted();
            }
            return coreServ != null;
        }

        public int DeleteServerByUids(List<string> uids)
        {
            if (uids == null || uids.Count < 1)
            {
                return 0;
            }

            List<Controllers.CoreServerCtrl> coreServs = new List<Controllers.CoreServerCtrl>();
            locker.EnterWriteLock();
            try
            {
                foreach (var uid in uids)
                {
                    var coreServ = DeleteServerByUidWorker(uid);
                    if (coreServ != null)
                    {
                        coreServs.Add(coreServ);
                    }
                }
            }
            finally
            {
                locker.ExitWriteLock();
            }

            void housekeeping()
            {
                foreach (var coreServ in coreServs)
                {
                    ReleaseEventsFrom(coreServ);
                    coreServ.Dispose();
                }
            }

            VgcApis.Misc.Utils.RunInBackground(housekeeping);
            RefreshUiAfterCoreServersAreDeleted();
            return coreServs.Count;
        }

        public int DeleteSelectedServers()
        {
            List<string> uids = GetSelectedServers()
                    .Select(s => s.GetCoreStates().GetUid())
                    .ToList();
            return DeleteServerByUids(uids);
        }

        public void DeleteAllServers()
        {
            setting.isSpeedtestCancelled = true;
            var servs = queryHandler.GetAllServers();

            locker.EnterWriteLock();
            try
            {
                configCache.Clear();
                coreServCache.Clear();
                selectedServersCountCache = 0;
            }
            finally
            {
                locker.ExitWriteLock();
            }

            void housekeeping()
            {
                foreach (var serv in servs)
                {
                    var coreServ = (Controllers.CoreServerCtrl)serv;
                    ReleaseEventsFrom(coreServ);
                    coreServ.Dispose();
                }
            }

            VgcApis.Misc.Utils.RunInBackground(housekeeping);
            RefreshUiAfterCoreServersAreDeleted();
        }

        public void UpdateAllServersSummary()
        {
            var list = queryHandler.GetAllServers(); // clone
            foreach (var core in list)
            {
                try
                {
                    core.GetConfiger().UpdateSummary();
                }
                catch { }
            }
            RequireFormMainReload();
            InvokeEventOnServerPropertyChange(this, EventArgs.Empty);
        }

        public bool DeleteServerByConfig(string config, bool isQuiet)
        {
            config = VgcApis.Misc.Utils.FormatConfig(config);

            if (configCache.TryGetValue(config, out var uid) && !string.IsNullOrEmpty(uid))
            {
                return DeleteServerByUids(new List<string>() { uid }) == 1;
            }

            if (!isQuiet)
            {
                MessageBox.Show(I18N.CantFindOrgServDelFail);
            }
            return false;
        }

        public ICoreServCtrl GetServerByIndex(int index)
        {
            locker.EnterReadLock();
            try
            {
                return coreServCache.FirstOrDefault(kv => (int)kv.Value.GetCoreStates().GetIndex() == index).Value;
            }
            finally
            {
                locker.ExitReadLock();
            }
        }

        public VgcApis.Interfaces.ICoreServCtrl GetServerByUid(string uid)
        {
            if (!string.IsNullOrEmpty(uid) && coreServCache.TryGetValue(uid, out var coreServ))
            {
                return coreServ;
            }
            return null;
        }

        public bool IsServerExist(string config)
        {
            config = VgcApis.Misc.Utils.FormatConfig(config);
            return IsFormatedConfigInCache(config);
        }

        public bool AddServer(string config, string mark, bool quiet = false)
        {
            config = VgcApis.Misc.Utils.FormatConfig(config);
            return AddServerWithFormatedConfig(config, mark, quiet);
        }

        public bool ReplaceServerConfig(string orgConfig, string newConfig)
        {
            orgConfig = VgcApis.Misc.Utils.FormatConfig(orgConfig);
            newConfig = VgcApis.Misc.Utils.FormatConfig(newConfig);
            return ReplaceServerWithFormatedConfig(orgConfig, newConfig);
        }

        public ICoreServCtrl GetServerByConfig(string config)
        {
            config = VgcApis.Misc.Utils.FormatConfig(config);
            return GetServerByFormatedConfig(config);
        }

        public string ReplaceOrAddNewServer(string orgUid, string newConfig) =>
            ReplaceOrAddNewServer(orgUid, newConfig, @"");

        public string ReplaceOrAddNewServer(string orgUid, string newConfig, string mark)
        {
            newConfig = VgcApis.Misc.Utils.FormatConfig(newConfig);
            if (!string.IsNullOrEmpty(orgUid) && coreServCache.TryGetValue(orgUid, out var coreServ))
            {
                var oldConfig = coreServ.GetConfiger().GetConfig();

                ReplaceServerWithFormatedConfig(oldConfig, newConfig);
                return orgUid;
            }

            AddServerWithFormatedConfig(newConfig, mark);
            if (configCache.TryGetValue(newConfig, out var uid))
            {
                return uid;
            }
            return string.Empty;
        }

        public bool AddServerWithFormatedConfig(string config, string mark, bool quiet = false) =>
            AddServerWithFormatedConfigWorker(config, mark, quiet);
        #endregion

        #region private methods
        Controllers.CoreServerCtrl DeleteServerByUidWorker(string uid)
        {
            if (!string.IsNullOrEmpty(uid)
                && coreServCache.TryGetValue(uid, out var coreServ)
                && coreServ != null)
            {
                var cfg = coreServ.GetConfiger().GetConfig();
                configCache.TryRemove(cfg, out _);
                coreServCache.Remove(uid);
                return coreServ;
            }
            return null;
        }

        bool AddServerWithFormatedConfigWorker(string config, string mark, bool quiet = false)
        {
            // unknow bug 2023-05-08
            mark = mark ?? @"";

            // first check
            if (IsFormatedConfigInCache(config))
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
                uid = Guid.NewGuid().ToString(),
                lastModifiedUtcTicks = DateTime.UtcNow.Ticks,
            };

            var newServer = new Controllers.CoreServerCtrl(coreInfo);
            newServer.Run(cache, setting, configMgr, this);

            bool duplicated = true;
            locker.EnterWriteLock();
            try
            {
                // double check
                if (!IsFormatedConfigInCache(config))
                {
                    configCache.TryAdd(config, coreInfo.uid);
                    coreServCache.Add(coreInfo.uid, newServer);
                    var idx = coreServCache.Count();
                    newServer.GetCoreStates().SetIndexQuiet(idx);
                    AddNewMark(mark);
                    duplicated = false;
                }
            }
            finally
            {
                locker.ExitWriteLock();
            }

            if (duplicated)
            {
                newServer.Dispose();
                return false;
            }

            BindEventsTo(newServer);
            newServer.GetConfiger().UpdateSummary();

            if (!quiet)
            {
                // UpdateSummaryThen will invoke OnServerPropertyChange.
                InvokeEventOnServerCountChange(this, EventArgs.Empty);
                RequireFormMainReload();
            }
            lazyServerSettingsRecorder.Deadline();
            return true;
        }

        bool ReplaceServerWithFormatedConfig(string orgConfig, string newConfig)
        {
            Controllers.CoreServerCtrl coreServ = null;
            string uid;

            locker.EnterReadLock();
            try
            {
                if (configCache.TryGetValue(orgConfig, out uid) && !string.IsNullOrEmpty(uid))
                {
                    coreServCache.TryGetValue(uid, out coreServ);
                }
            }
            finally
            {
                locker.ExitReadLock();
            }

            if (coreServ == null)
            {
                return false;
            }

            configCache.TryRemove(orgConfig, out _);
            configCache.TryAdd(newConfig, uid);
            coreServ.GetConfiger().SetConfig(newConfig);
            coreServ.GetCoreStates().SetLastModifiedUtcTicks(DateTime.UtcNow.Ticks);
            return true;
        }

        ICoreServCtrl GetServerByFormatedConfig(string config)
        {
            if (configCache.TryGetValue(config, out var uid) && !string.IsNullOrEmpty(uid))
            {
                if (coreServCache.TryGetValue(uid, out var coreServ))
                {
                    return coreServ;
                }
            }
            return null;
        }

        void AddToBootList(HashSet<Controllers.CoreServerCtrl> set, string config)
        {
            if (string.IsNullOrEmpty(config))
            {
                return;
            }
            if (!configCache.TryGetValue(config, out var uid) || string.IsNullOrEmpty(uid))
            {
                return;
            }
            if (!coreServCache.TryGetValue(uid, out var coreServ)
                || coreServ == null
                || coreServ.GetCoreStates().IsUntrack())
            {
                return;
            }
            set.Add(coreServ);
        }

        List<Controllers.CoreServerCtrl> GenServersBootList()
        {
            var tracker = setting.GetServerTrackerSetting();

            var set = new HashSet<Controllers.CoreServerCtrl>();
            locker.EnterReadLock();
            try
            {
                foreach (var kv in coreServCache)
                {
                    var coreServ = kv.Value;
                    if (coreServ.GetCoreStates().IsAutoRun())
                    {
                        AddToBootList(set, coreServ.GetConfiger().GetConfig());
                    }
                }
                if (tracker.isTrackerOn)
                {
                    AddToBootList(set, tracker.curServer);
                    foreach (var config in tracker.serverList)
                    {
                        AddToBootList(set, config);
                    }

                    foreach (var uid in tracker.uids)
                    {
                        if (coreServCache.TryGetValue(uid, out var coreServ) && coreServ != null)
                        {
                            AddToBootList(set, coreServ.GetConfiger().GetConfig());
                        }
                    }
                }
            }
            finally
            {
                locker.ExitReadLock();
            }
            return set.ToList();
        }

        void RefreshUiAfterCoreServersAreDeleted()
        {
            UpdateMarkList();
            ResetIndexQuiet();
            InvokeEventOnServerCountChange(this, EventArgs.Empty);
            RequireFormMainReload();
            lazyServerSettingsRecorder.Deadline();
        }

        bool IsFormatedConfigInCache(string config)
        {
            if (string.IsNullOrEmpty(config))
            {
                return true;
            }
            return configCache.ContainsKey(config);
        }

        void SaveServersSettingsWorker()
        {
            List<VgcApis.Models.Datas.CoreInfo> coreInfoList = GetAllCoreInfos();
            setting.SaveServerList(coreInfoList);
        }

        private List<VgcApis.Models.Datas.CoreInfo> GetAllCoreInfos() =>
             queryHandler.GetAllServers()
                .Select(s => s.GetCoreStates().GetAllRawCoreInfo())
                .ToList();

        void SortServers(List<ICoreServCtrl> coreServs, Action<List<ICoreServCtrl>> sorter)
        {
            // do not lock here, sorter will apply write lock itself
            sorter?.Invoke(coreServs);
            RequireFormMainReload();
            InvokeEventOnServerPropertyChange(this, EventArgs.Empty);
        }

        void SortSelectedServers(Action<List<ICoreServCtrl>> sorter)
        {
            var selectedServers = GetSelectedServers();
            SortServers(selectedServers, sorter);
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

            var newConfig = VgcApis.Misc.Utils.FormatConfig(package);
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
            locker.EnterWriteLock();
            try
            {
                var coreInfoList = setting.LoadCoreInfoList();
                foreach (var coreInfo in coreInfoList)
                {
                    var server = new Controllers.CoreServerCtrl(coreInfo);
                    coreServCache.Add(coreInfo.uid, server);
                }
            }
            finally
            {
                locker.ExitWriteLock();
            }

            foreach (var kv in coreServCache)
            {
                var coreServ = kv.Value;
                coreServ.Run(cache, setting, configMgr, this);
                var cfg = coreServ.GetConfiger().GetConfig();
                configCache.TryAdd(cfg, coreServ.GetCoreStates().GetUid());
                BindEventsTo(coreServ);
            }
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
            var list = queryHandler.GetAllServers();

            var count = list.Count;
            VgcApis.Misc.Utils.RunInBackground(() =>
            {
                var taskList = new List<Task>();
                for (int i = 0; i < round; i++)
                {
                    var index = VgcApis.Libs.Infr.PseudoRandom.Next(0, count);
                    var isStopCore = VgcApis.Libs.Infr.PseudoRandom.Next(0, 2) == 0;
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
