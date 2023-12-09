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
    public class Servers
        : BaseClasses.SingletonService<Servers>,
            VgcApis.Interfaces.Services.IServersService
    {
        Settings setting = null;
        Cache cache = null;
        ConfigMgr configMgr;

        ServersComponents.QueryHandler queryHandler;
        ServersComponents.IndexHandler indexHandler;

        public event EventHandler OnCoreStart, // ICoreServCtrl sender
            OnCoreClosing,
            OnCoreStop,
            OnServerPropertyChange,
            OnServerCountChange,
            OnRequireFlyPanelReload;

        // uid => CoreServ
        readonly Dictionary<string, Controllers.CoreServerCtrl> coreServCache =
            new Dictionary<string, Controllers.CoreServerCtrl>();
        readonly ConcurrentDictionary<string, bool> markList =
            new ConcurrentDictionary<string, bool>();
        readonly ServersComponents.ConfigCache configCache = new ServersComponents.ConfigCache();
        readonly VgcApis.Libs.Tasks.LazyGuy lazyServerSettingsRecorder;
        readonly ReaderWriterLockSlim locker = new ReaderWriterLockSlim();
        readonly VgcApis.Libs.Tasks.Bar speedTestingBar = new VgcApis.Libs.Tasks.Bar();

        Servers()
        {
            lazyServerSettingsRecorder = new VgcApis.Libs.Tasks.LazyGuy(
                SaveServersSettingsWorker,
                VgcApis.Models.Consts.Intervals.LazySaveServerListIntreval,
                2 * 1000
            )
            {
                Name = "Servers.SaveSettings()",
            };
        }

        public void Run(Settings setting, Cache cache, ConfigMgr configMgr)
        {
            this.configMgr = configMgr;
            this.cache = cache;
            this.setting = setting;
            InitServerCtrlList();
            UpdateMarkList();

            queryHandler = new ServersComponents.QueryHandler(locker, coreServCache);

            indexHandler = new ServersComponents.IndexHandler(locker, coreServCache);

            indexHandler.OnIndexChanged += ClearSortedCoreServCacheHandler;
        }

        #region reflection
        #endregion

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
            SortSelectedServers(
                (list) => indexHandler.SortCoreServCtrlListBySpeedTestResult(ref list)
            );
        }

        public void SortServersBySpeedTest(List<string> uids)
        {
            var coreServs = queryHandler.GetServersByUids(uids);
            SortServers(
                coreServs,
                (list) => indexHandler.SortCoreServCtrlListBySpeedTestResult(ref list)
            );
        }

        public void SortSelectedByDownloadTotal()
        {
            SortSelectedServers(
                (list) => indexHandler.SortCoreServerCtrlListByDownloadTotal(ref list)
            );
        }

        public void SortServersByDownloadTotal(List<string> uids)
        {
            var coreServs = queryHandler.GetServersByUids(uids);
            SortServers(
                coreServs,
                (list) => indexHandler.SortCoreServerCtrlListByDownloadTotal(ref list)
            );
        }

        public void SortSelectedByUploadTotal()
        {
            SortSelectedServers(
                (list) => indexHandler.SortCoreServerCtrlListByUploadTotal(ref list)
            );
        }

        public void SortServersByUploadTotal(List<string> uids)
        {
            var coreServs = queryHandler.GetServersByUids(uids);
            SortServers(
                coreServs,
                (list) => indexHandler.SortCoreServerCtrlListByUploadTotal(ref list)
            );
        }

        public void SortSelectedByLastModifiedDate()
        {
            SortSelectedServers(
                (list) => indexHandler.SortCoreServerCtrlListByLastModifyDate(ref list)
            );
        }

        public void SortServersByLastModifiedDate(List<string> uids)
        {
            var coreServs = queryHandler.GetServersByUids(uids);
            SortServers(
                coreServs,
                (list) => indexHandler.SortCoreServerCtrlListByLastModifyDate(ref list)
            );
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
        public List<ICoreServCtrl> GetSelectedServers() => queryHandler.GetSelectedServers();

        public List<ICoreServCtrl> GetRunningServers() => queryHandler.GetRunningServers();

        List<ICoreServCtrl> sortedCoreServListCache = null;
        readonly object sortedCoreServListCacheLocker = new object();

        public List<ICoreServCtrl> GetAllServersOrderByIndex()
        {
            lock (sortedCoreServListCacheLocker)
            {
                if (sortedCoreServListCache == null)
                {
                    sortedCoreServListCache = queryHandler.GetAllServers(false);
                }
                // copy
                return sortedCoreServListCache.ToList();
            }
        }

        public List<ICoreServCtrl> GetServersByUids(IEnumerable<string> uids) =>
            queryHandler.GetServersByUids(uids);

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
            server.OnIndexChanged += ClearSortedCoreServCacheHandler;
        }

        void ReleaseEventsFrom(Controllers.CoreServerCtrl server)
        {
            server.OnCoreClosing -= InvokeEventOnCoreClosingIgnoreError;
            server.OnCoreStart -= OnTrackCoreStartHandler;
            server.OnCoreStop -= OnTrackCoreStopHandler;
            server.OnPropertyChanged -= InvokeEventOnServerPropertyChange;
            server.OnIndexChanged -= ClearSortedCoreServCacheHandler;
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

        public List<VgcApis.Models.Datas.InboundInfo> GetAllActiveInboundsInfo()
        {
            var servs = GetRunningServers();
            return servs
                .Select(s => s.GetConfiger().GetAllInboundsInfo())
                .SelectMany(inf => inf)
                .ToList();
        }

        /// <summary>
        /// return -1 when fail
        /// </summary>
        /// <returns></returns>
        public int GetAvailableHttpProxyPort() =>
            GetAvailableProxyPortCore(new List<string>() { "http" });

        public int GetAvailableSocksProxyPort() =>
            GetAvailableProxyPortCore(new List<string>() { "socks", "socks5" });

        public bool GetAvailableProxyInfo(out bool isSocks5, out int port)
        {
            var protocols = new List<string>() { "http", "socks", "socks5" };
            var servs = GetRunningServers();
            foreach (var serv in servs)
            {
                var inbs = serv.GetConfiger().GetAllInboundsInfo();
                foreach (var inb in inbs)
                {
                    if (inb.protocol != null && protocols.Contains(inb.protocol))
                    {
                        isSocks5 = inb.protocol != "http";
                        port = inb.port;
                        return true;
                    }
                }
            }
            isSocks5 = false;
            port = -1;
            return false;
        }

        public void UpdateServerTrackerSettings(bool isTrackerOn)
        {
            var tracker = new Models.Datas.ServerTracker
            {
                isTrackerOn = isTrackerOn,
                curServer = string.Empty, // obsolete
                serverList = new List<string>() //obsolete
            };

            tracker.uids = tracker.isTrackerOn
                ? GetRunningServers()
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
                    selectedServersCountCache = coreServCache.Count(
                        kv => kv.Value.GetCoreStates().IsSelected()
                    );
                }
                finally
                {
                    locker.ExitReadLock();
                }
            }

            return selectedServersCountCache;
        }

        public int Count() => coreServCache.Count;

        public string[] GetMarkList() => markList.Keys.OrderBy(x => x).ToArray();

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
            string orgUid,
            string pkgName,
            string interval,
            string url,
            VgcApis.Models.Datas.Enums.BalancerStrategies strategy,
            VgcApis.Models.Datas.Enums.PackageTypes packageType
        )
        {
            var coreServs = queryHandler.GetServersByUids(uids);
            return PackServersIntoV4PackageWorker(
                coreServs,
                orgUid,
                pkgName,
                interval,
                url,
                strategy,
                packageType
            );
        }

        public string PackSelectedServersV4(
            string orgUid,
            string pkgName,
            string interval,
            string url,
            VgcApis.Models.Datas.Enums.BalancerStrategies strategy,
            VgcApis.Models.Datas.Enums.PackageTypes packageType
        )
        {
            var servList = GetSelectedServers();
            return PackServersIntoV4PackageWorker(
                servList,
                orgUid,
                pkgName,
                interval,
                url,
                strategy,
                packageType
            );
        }

        /// <summary>
        /// packageName is Null or empty ? "PackageV4" : packageName
        /// </summary>
        /// <param name="packageName"></param>
        /// <param name="servList"></param>
        public string PackServersV4Ui(
            List<ICoreServCtrl> servList,
            string orgUid,
            string packageName,
            string interval,
            string url,
            VgcApis.Models.Datas.Enums.BalancerStrategies strategy,
            VgcApis.Models.Datas.Enums.PackageTypes packageType
        )
        {
            if (servList == null || servList.Count < 1)
            {
                VgcApis.Misc.UI.MsgBoxAsync(I18N.ListIsEmpty);
                return "";
            }

            var uid = PackServersIntoV4PackageWorker(
                servList,
                orgUid,
                packageName,
                interval,
                url,
                strategy,
                packageType
            );
            Misc.UI.ShowMessageBoxDoneAsync();
            return uid;
        }

        public bool RunSpeedTestOnSelectedServers()
        {
            var evDone = new AutoResetEvent(false);
            var success = BatchSpeedTestWorkerThen(GetSelectedServers(), () => evDone.Set());
            evDone.WaitOne();
            evDone.Dispose();
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
                () => VgcApis.Misc.UI.MsgBox(I18N.SpeedTestFinished)
            );
            if (!success)
            {
                VgcApis.Misc.UI.MsgBox(I18N.LastTestNoFinishYet);
            }
        }

        public void RestartServersThen(List<ICoreServCtrl> coreServs, Action done = null)
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
            if (configCache.TryGetValue(config, out var uid) && !string.IsNullOrEmpty(uid))
            {
                return DeleteServerByUid(uid);
            }

            if (!isQuiet)
            {
                VgcApis.Misc.UI.MsgBox(I18N.CantFindOrgServDelFail);
            }
            return false;
        }

        public ICoreServCtrl GetServerByIndex(int index)
        {
            locker.EnterReadLock();
            try
            {
                return coreServCache
                    .FirstOrDefault(kv => (int)kv.Value.GetCoreStates().GetIndex() == index)
                    .Value;
            }
            finally
            {
                locker.ExitReadLock();
            }
        }

        public ICoreServCtrl GetServerByUid(string uid)
        {
            if (!string.IsNullOrEmpty(uid) && coreServCache.TryGetValue(uid, out var coreServ))
            {
                return coreServ;
            }
            return null;
        }

        public bool IsServerExist(string config)
        {
            return IsConfigInCache(config);
        }

        public string AddServer(string name, string config, string mark, bool quiet)
        {
            return AddServerWithConfigWorker(name, config, mark, quiet);
        }

        public bool ReplaceServerConfig(string orgConfig, string newConfig)
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

        public ICoreServCtrl GetServerByConfig(string config)
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

        public string ReplaceOrAddNewServer(
            string orgUid,
            string newName,
            string newConfig,
            string mark
        )
        {
            if (
                !string.IsNullOrEmpty(orgUid) && coreServCache.TryGetValue(orgUid, out var coreServ)
            )
            {
                var oldConfig = coreServ.GetConfiger().GetConfig();
                ReplaceServerConfig(oldConfig, newConfig);
                if (!string.IsNullOrEmpty(newName))
                {
                    coreServ.GetCoreStates().SetName(newName);
                }
                return orgUid;
            }

            var uid = AddServerWithConfigWorker(newName, newConfig, mark, false);
            return uid;
        }

        #endregion

        #region private methods
        int GetAvailableProxyPortCore(IEnumerable<string> protocols)
        {
            var servs = GetRunningServers();
            foreach (var serv in servs)
            {
                var inbs = serv.GetConfiger().GetAllInboundsInfo();
                foreach (var inb in inbs)
                {
                    if (inb.protocol != null && protocols.Contains(inb.protocol))
                    {
                        return inb.port;
                    }
                }
            }
            return -1;
        }

        void ClearSortedCoreServCacheHandler(object sender, EventArgs args)
        {
            if (sortedCoreServListCache != null)
            {
                lock (sortedCoreServListCache)
                {
                    sortedCoreServListCache = null;
                }
            }
        }

        Controllers.CoreServerCtrl DeleteServerByUidWorker(string uid)
        {
            if (
                !string.IsNullOrEmpty(uid)
                && coreServCache.TryGetValue(uid, out var coreServ)
                && coreServ != null
            )
            {
                var cfg = coreServ.GetConfiger().GetConfig();
                configCache.TryRemove(cfg, out _);
                coreServCache.Remove(uid);
                return coreServ;
            }
            return null;
        }

        string AddServerWithConfigWorker(
            string name,
            string config,
            string mark,
            bool quiet = false
        )
        {
            // unknow bug 2023-05-08
            mark = mark ?? @"";

            // first check
            if (IsConfigInCache(config))
            {
                return "";
            }

            var uid = Guid.NewGuid().ToString();
            var coreInfo = new VgcApis.Models.Datas.CoreInfo
            {
                inbName = setting.DefaultInboundName,
                inbIp = setting.CustomDefImportHost,
                inbPort = setting.CustomDefImportPort,
                customMark = mark,
                customCoreName = setting.DefaultCoreName,
                uid = uid,
                lastModifiedUtcTicks = DateTime.UtcNow.Ticks,
            };

            coreInfo.SetConfig(config);

            var newServer = new Controllers.CoreServerCtrl(coreInfo);
            newServer.Run(cache, setting, configMgr, this);

            bool duplicated = true;
            locker.EnterWriteLock();
            try
            {
                // double check
                if (!IsConfigInCache(config))
                {
                    configCache.TryAdd(config, coreInfo.uid);
                    newServer.GetCoreStates().SetName(name);
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
                return "";
            }

            BindEventsTo(newServer);
            newServer.GetConfiger().UpdateSummary();

            // clear sorted core serv list cache
            ClearSortedCoreServCacheHandler(newServer, EventArgs.Empty);

            if (!quiet)
            {
                // UpdateSummaryThen will invoke OnServerPropertyChange.
                InvokeEventOnServerCountChange(this, EventArgs.Empty);
                RequireFormMainReload();
            }
            lazyServerSettingsRecorder.Deadline();
            return uid;
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
            if (
                !coreServCache.TryGetValue(uid, out var coreServ)
                || coreServ == null
                || coreServ.GetCoreStates().IsUntrack()
            )
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
            ClearSortedCoreServCacheHandler(null, EventArgs.Empty);
            InvokeEventOnServerCountChange(this, EventArgs.Empty);
            RequireFormMainReload();
            lazyServerSettingsRecorder.Deadline();
        }

        bool IsConfigInCache(string config)
        {
            return configCache.ContainsKey(config);
        }

        void SaveServersSettingsWorker()
        {
            List<VgcApis.Models.Datas.CoreInfo> coreInfoList = GetAllCoreInfos();
            setting.SaveServerList(coreInfoList);
        }

        private List<VgcApis.Models.Datas.CoreInfo> GetAllCoreInfos() =>
            queryHandler
                .GetAllServers()
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
            VgcApis.Models.Datas.Enums.BalancerStrategies strategy
        )
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
                        config["routing"]["balancers"][0]["strategy"] = JObject.Parse(
                            "{type:'leastPing'}"
                        );
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
            VgcApis.Models.Datas.Enums.PackageTypes packageType
        )
        {
            if (servList == null || servList.Count < 1)
            {
                return "";
            }

            JObject package = configMgr.GenV4ServersPackageConfig(servList, packageType);
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

            if (string.IsNullOrEmpty(packageName))
            {
                packageName = mark;
            }

            var newConfig = VgcApis.Misc.Utils.FormatConfig(package);
            string newUid = ReplaceOrAddNewServer(orgUid, packageName, newConfig, mark);

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
            foreach (var coreServ in randList)
            {
                coreServ.GetCoreCtrl().RunSpeedTestThen();
            }

            VgcApis.Misc.Utils.RunInBackground(() =>
            {
                while (setting.SpeedtestCounter > 0)
                {
                    setting.SpeedTestPool.WaitUntilEmpty();
                }
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

                // 预计2024-06删除这两个函数
                PatchConfig(coreServ);
                FixInboundName(coreServ);

                var cfg = coreServ.GetConfiger().GetConfig();
                configCache.TryAdd(cfg, coreServ.GetCoreStates().GetUid());
                BindEventsTo(coreServ);
            }
        }

        void FixInboundName(Controllers.CoreServerCtrl coreServ)
        {
            var name = coreServ.GetCoreStates().GetInboundName();
            if (!string.IsNullOrEmpty(name))
            {
                return;
            }

            var coreInfo = coreServ.GetCoreStates().GetAllRawCoreInfo();
            switch ((VgcApis.Models.Datas.Enums.ProxyTypes)coreInfo.customInbType)
            {
                case VgcApis.Models.Datas.Enums.ProxyTypes.SOCKS:
                    name = "socks";
                    break;
                case VgcApis.Models.Datas.Enums.ProxyTypes.Custom:
                    name = "custom";
                    break;
                case VgcApis.Models.Datas.Enums.ProxyTypes.Config:
                    name = "config";
                    break;
                default:
                    name = "http";
                    break;
            }
            coreInfo.inbName = name;
        }

        void PatchConfig(Controllers.CoreServerCtrl coreServ)
        {
            var config = coreServ.GetConfiger().GetConfig();
            if (VgcApis.Misc.Utils.IsJson(config))
            {
                return;
            }
            try
            {
                var json = JObject.Parse(config);
                var coreState = coreServ.GetCoreStates();
                var name = coreState.GetName();
                if (string.IsNullOrEmpty(name))
                {
                    name = Misc.Utils.GetAliasFromConfig(json);
                    coreState.SetName(name);
                }
                json.Remove(VgcApis.Models.Consts.Config.SectionKeyV2rayGCon);
                var formated = VgcApis.Misc.Utils.FormatConfig(json);
                coreServ.GetConfiger().SetConfig(formated);
            }
            catch { }
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

            indexHandler.OnIndexChanged -= ClearSortedCoreServCacheHandler;
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

                    var task = new Task(
                        () =>
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
                            sayGoodbye.Dispose();
                        },
                        TaskCreationOptions.LongRunning
                    );

                    taskList.Add(task);
                    task.Start();
                }

                Task.WaitAll(taskList.ToArray());
                VgcApis.Misc.UI.MsgBox(I18N.Done);
            });
        }
#endif
        #endregion
    }
}
