using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using V2RayGCon.Resources.Resx;

namespace V2RayGCon.Services
{
    public class Settings
        : BaseClasses.SingletonService<Settings>,
            VgcApis.Interfaces.Services.ISettingsService
    {
        readonly Models.Datas.CmdArgs cmdArgs;
        Models.Datas.UserSettings userSettings;
        VgcApis.Libs.Tasks.LazyGuy lazyBookKeeper;

        readonly object saveUserSettingsLocker = new object();
        Mutex fileMutex;

        public event EventHandler OnPortableModeChanged;
        VgcApis.Models.Datas.Enums.ShutdownReasons shutdownReason = VgcApis
            .Models
            .Datas
            .Enums
            .ShutdownReasons
            .Undefined;

        List<VgcApis.Models.Datas.CoreInfo> coreInfoCache =
            new List<VgcApis.Models.Datas.CoreInfo>();
        Dictionary<string, string> pluginsSettingCache = new Dictionary<string, string>();
        ConcurrentDictionary<string, string> localStorageCache =
            new ConcurrentDictionary<string, string>();

        // Singleton need this private ctor.
        Settings()
        {
            var args = Environment.GetCommandLineArgs();
            cmdArgs = new Models.Datas.CmdArgs(args);
            VgcApis.Misc.Utils.SetAppTag(cmdArgs.tag);

            if (cmdArgs.help)
            {
                var msg = cmdArgs.GetHelpMessage();
                SetShutdownReason(VgcApis.Models.Datas.Enums.ShutdownReasons.ShowHelpInfo);
                MessageBox.Show(msg);
                return;
            }

            fileMutex = VgcApis.Misc.Utils.TryLockFile(cmdArgs.userSettings);
            if (fileMutex == null)
            {
                SetShutdownReason(VgcApis.Models.Datas.Enums.ShutdownReasons.FileLocked);
                var msg = string.Format(I18N.UserSettingsFileIsLocked, cmdArgs.userSettings);
                MessageBox.Show(msg);
                return;
            }

            Init();
        }

        #region custom core settings
        public string DefaultCoreName
        {
            get => userSettings.ImportOptions.DefaultCoreName;
            set
            {
                if (value != userSettings.ImportOptions.DefaultCoreName)
                {
                    userSettings.ImportOptions.DefaultCoreName = value;
                    SaveSettingsLater();
                }
            }
        }

        public List<Models.Datas.CustomCoreSettings> GetCustomCoresSetting()
        {
            lock (saveUserSettingsLocker)
            {
                return userSettings.CustomCoreSettings.OrderBy(cs => cs.index).ToList();
            }
        }

        public void ResetCustomCoresIndex()
        {
            lock (saveUserSettingsLocker)
            {
                var cores = GetCustomCoresSetting();
                var idx = 1.0;
                foreach (var core in cores)
                {
                    core.index = idx++;
                }
            }
            SaveSettingsLater();
        }

        public bool RemoveCustomCoreByName(string name)
        {
            lock (saveUserSettingsLocker)
            {
                var coreSettings = userSettings.CustomCoreSettings.FirstOrDefault(
                    cs => cs.name == name
                );
                if (coreSettings != null)
                {
                    userSettings.CustomCoreSettings.Remove(coreSettings);
                    ResetCustomCoresIndex();
                    return true;
                }
            }
            return false;
        }

        public void AddOrReplaceCustomCoreSettings(Models.Datas.CustomCoreSettings coreSettings)
        {
            if (coreSettings == null)
            {
                return;
            }

            coreSettings.index = userSettings.CustomCoreSettings.Count + 1;

            lock (saveUserSettingsLocker)
            {
                var coreS = userSettings.CustomCoreSettings.FirstOrDefault(
                    cs => cs.name == coreSettings.name
                );
                if (coreS != null)
                {
                    coreSettings.index = coreS.index;
                    userSettings.CustomCoreSettings.Remove(coreS);
                }
                userSettings.CustomCoreSettings.Add(coreSettings);
            }
            SaveSettingsLater();
        }

        #endregion

        #region custom inbound settings
        List<Models.Datas.CustomConfigTemplate> configTemplateCache;

        void InitConfigTemplatesCache()
        {
            try
            {
                configTemplateCache =
                    VgcApis.Libs.Infr.ZipExtensions.DeserializeObjectFromCompressedUnicodeBase64<
                        List<Models.Datas.CustomConfigTemplate>
                    >(userSettings.CompressedUnicodeCustomConfigTemplates);
            }
            catch { }
            configTemplateCache =
                configTemplateCache
                ?? userSettings.CustomInboundSettings?.ToList()
                ?? new List<Models.Datas.CustomConfigTemplate>();
        }

        void SerializeCustomConfigTemplate()
        {
            userSettings.CompressedUnicodeCustomConfigTemplates =
                VgcApis.Libs.Infr.ZipExtensions.SerializeObjectToCompressedUnicodeBase64(
                    configTemplateCache
                );
            userSettings.CustomInboundSettings = null;
        }

        public string DefaultInboundName
        {
            get => userSettings.ImportOptions.DefaultInboundName;
            set
            {
                if (value != userSettings.ImportOptions.DefaultInboundName)
                {
                    userSettings.ImportOptions.DefaultInboundName = value;
                    SaveSettingsLater();
                }
            }
        }

        public List<Models.Datas.CustomConfigTemplate> GetCustomConfigTemplates()
        {
            lock (saveUserSettingsLocker)
            {
                return configTemplateCache.OrderBy(inb => inb.index).ToList();
            }
        }

        public void ResetCustomConfigTemplatesIndex()
        {
            lock (saveUserSettingsLocker)
            {
                var inbs = GetCustomConfigTemplates();
                var idx = 1.0;
                foreach (var inb in inbs)
                {
                    inb.index = idx++;
                }
                SerializeCustomConfigTemplate();
            }
            SaveSettingsLater();
        }

        public bool RemoveCustomConfigTemplateByName(string name)
        {
            lock (saveUserSettingsLocker)
            {
                var tpl = configTemplateCache.FirstOrDefault(t => t.name == name);
                if (tpl != null)
                {
                    configTemplateCache.Remove(tpl);
                    ResetCustomConfigTemplatesIndex();
                    return true;
                }
            }
            return false;
        }

        public void AddOrReplaceCustomConfigTemplateSettings(Models.Datas.CustomConfigTemplate tpl)
        {
            if (tpl == null)
            {
                return;
            }

            tpl.index = configTemplateCache.Count + 1;

            lock (saveUserSettingsLocker)
            {
                var oldTpl = configTemplateCache.FirstOrDefault(t => t.name == tpl.name);
                if (oldTpl != null)
                {
                    tpl.index = oldTpl.index;
                    configTemplateCache.Remove(oldTpl);
                }
                configTemplateCache.Add(tpl);
                SerializeCustomConfigTemplate();
            }
            SaveSettingsLater();
        }

        #endregion

        #region Properties
        public bool isTunMode { get; set; } = false;

        public bool isLoad3rdPartyPlugins
        {
            get => userSettings.isLoad3rdPartyPlugins;
            set
            {
                userSettings.isLoad3rdPartyPlugins = value;
                SaveSettingsLater();
            }
        }

        public bool isUseCustomUserAgent
        {
            get { return userSettings.isUseCustomUserAgent; }
            set
            {
                userSettings.isUseCustomUserAgent = value;
                UpdateVgcApisUserAgent();
                SaveSettingsLater();
            }
        }

        public string CustomUserAgent
        {
            get { return userSettings.customUserAgent; }
            set
            {
                userSettings.customUserAgent = value;
                UpdateVgcApisUserAgent();
                SaveSettingsLater();
            }
        }

        public int SpeedtestCounter = 0;

        public int GetSpeedtestQueueLength() => SpeedtestCounter;

        public string CustomDefInbounds
        {
            get => userSettings.CustomInbounds;
            set
            {
                if (userSettings.CustomInbounds == value)
                {
                    return;
                }
                userSettings.CustomInbounds = value;
                SaveSettingsLater();
            }
        }

        public string DebugLogFilePath
        {
            get => userSettings.DebugLogFilePath;
            set
            {
                UpdateFileLoggerSetting();
                if (userSettings.DebugLogFilePath == value)
                {
                    return;
                }
                userSettings.DebugLogFilePath = value;
                SaveSettingsLater();
            }
        }

        public bool isEnableDebugLogFile
        {
            get => userSettings.isEnableDebugFile;
            set
            {
                UpdateFileLoggerSetting();

                if (userSettings.isEnableDebugFile == value)
                {
                    return;
                }
                userSettings.isEnableDebugFile = value;
                SaveSettingsLater();
            }
        }

        public int QuickSwitchServerLantency
        {
            get { return Math.Max(0, userSettings.QuickSwitchServerLatency); }
            set
            {
                var d = Math.Max(0, value);
                if (userSettings.QuickSwitchServerLatency == d)
                {
                    return;
                }
                userSettings.QuickSwitchServerLatency = d;
                SaveSettingsLater();
            }
        }

        public VgcApis.Libs.Tasks.TicketPool SpeedTestPool = new VgcApis.Libs.Tasks.TicketPool();

        public bool isSpeedtestCancelled = false;

        public VgcApis.Models.Datas.Enums.ShutdownReasons GetShutdownReason() => shutdownReason;

        public void SetShutdownReason(VgcApis.Models.Datas.Enums.ShutdownReasons reason)
        {
            VgcApis.Libs.Sys.FileLogger.Warn($"change shutdow reason to: {reason}");
            this.shutdownReason = reason;
        }

        public string v2rayCoreDownloadSource
        {
            get => userSettings.v2rayCoreDownloadSource;
            set
            {
                userSettings.v2rayCoreDownloadSource = value;
                SaveSettingsLater();
            }
        }

        public bool isDownloadWin32V2RayCore
        {
            get => userSettings.isDownloadWin32V2RayCore;
            set
            {
                userSettings.isDownloadWin32V2RayCore = value;
                SaveSettingsLater();
            }
        }

        public bool isAutoPatchSubsInfo
        {
            get => userSettings.isAutoPatchSubsInfo;
            set
            {
                userSettings.isAutoPatchSubsInfo = value;
                SaveSettingsLater();
            }
        }

        public Dictionary<string, string> decodeCache
        {
            get
            {
                Dictionary<string, string> r = null;
                try
                {
                    r =
                        VgcApis.Libs.Infr.ZipExtensions.DeserializeObjectFromCompressedUnicodeBase64<
                            Dictionary<string, string>
                        >(userSettings.CompressedUnicodeDecodeCache);
                }
                catch { }
                if (r != null)
                {
                    return r;
                }
                return r ?? new Dictionary<string, string>();
            }
            set
            {
                userSettings.CompressedUnicodeDecodeCache =
                    VgcApis.Libs.Infr.ZipExtensions.SerializeObjectToCompressedUnicodeBase64(value);
                SaveSettingsLater();
            }
        }

        public bool isEnableStatistics
        {
            get => userSettings.isEnableStat;
            set
            {
                userSettings.isEnableStat = value;
                SaveSettingsLater();
            }
        }

        public bool isUseV4
        {
            get => userSettings.isUseV4Format;
            set
            {
                userSettings.isUseV4Format = value;
                SaveSettingsLater();
            }
        }

        public string uTlsFingerprint
        {
            get => userSettings.uTlsFingerprint;
            set
            {
                userSettings.uTlsFingerprint = value;
                SaveSettingsLater();
            }
        }

        public bool isEnableUtlsFingerprint
        {
            get => userSettings.isEnableUtlsFingerprint;
            set
            {
                userSettings.isEnableUtlsFingerprint = value;
                SaveSettingsLater();
            }
        }

        public bool isSupportSelfSignedCert
        {
            get => userSettings.isSupportSelfSignedCert;
            set
            {
                userSettings.isSupportSelfSignedCert = value;
                SaveSettingsLater();
            }
        }

        public bool CustomDefImportVmessShareLink
        {
            get => userSettings.ImportOptions.IsImportVmessShareLink;
            set
            {
                userSettings.ImportOptions.IsImportVmessShareLink = value;
                SaveSettingsLater();
            }
        }

        public bool CustomDefImportVlessShareLink
        {
            get => userSettings.ImportOptions.IsImportVlessShareLink;
            set
            {
                userSettings.ImportOptions.IsImportVlessShareLink = value;
                SaveSettingsLater();
            }
        }

        public bool CustomDefImportTrojanShareLink
        {
            get => userSettings.ImportOptions.IsImportTrojanShareLink;
            set
            {
                userSettings.ImportOptions.IsImportTrojanShareLink = value;
                SaveSettingsLater();
            }
        }

        public bool CustomDefImportSocksShareLink
        {
            get => userSettings.ImportOptions.IsImportSocksShareLink;
            set
            {
                userSettings.ImportOptions.IsImportSocksShareLink = value;
                SaveSettingsLater();
            }
        }

        public bool CustomDefImportSsShareLink
        {
            get => userSettings.ImportOptions.IsImportSsShareLink;
            set
            {
                userSettings.ImportOptions.IsImportSsShareLink = value;
                SaveSettingsLater();
            }
        }

        public string CustomDefImportHost
        {
            get => userSettings.ImportOptions.Ip;
            set
            {
                userSettings.ImportOptions.Ip = value;
                SaveSettingsLater();
            }
        }

        public int CustomDefImportPort
        {
            get => userSettings.ImportOptions.Port;
            set
            {
                userSettings.ImportOptions.Port = value;
                SaveSettingsLater();
            }
        }

        public string CustomSpeedtestUrl
        {
            get => userSettings.SpeedtestOptions.Url;
            set
            {
                userSettings.SpeedtestOptions.Url = value;
                SaveSettingsLater();
            }
        }

        public int CustomSpeedtestTimeout
        {
            get => userSettings.SpeedtestOptions.Timeout;
            set
            {
                userSettings.SpeedtestOptions.Timeout = value;
                SaveSettingsLater();
            }
        }

        public int CustomSpeedtestExpectedSizeInKib
        {
            get => userSettings.SpeedtestOptions.ExpectedSize;
            set
            {
                userSettings.SpeedtestOptions.ExpectedSize = value;
                SaveSettingsLater();
            }
        }

        public int CustomSpeedtestCycles
        {
            get => userSettings.SpeedtestOptions.Cycles;
            set
            {
                userSettings.SpeedtestOptions.Cycles = value;
                SaveSettingsLater();
            }
        }

        public bool isUseCustomSpeedtestSettings
        {
            get => userSettings.SpeedtestOptions.IsUse;
            set
            {
                userSettings.SpeedtestOptions.IsUse = value;
                SaveSettingsLater();
            }
        }

        public bool isUpdateUseProxy
        {
            get => userSettings.isUpdateUseProxy;
            set
            {
                userSettings.isUpdateUseProxy = value;
                SaveSettingsLater();
            }
        }

        public bool isCheckV2RayCoreUpdateWhenAppStart
        {
            get => userSettings.isCheckV2RayCoreUpdateWhenAppStart;
            set
            {
                userSettings.isCheckV2RayCoreUpdateWhenAppStart = value;
                SaveSettingsLater();
            }
        }

        public bool isCheckVgcUpdateWhenAppStart
        {
            get => userSettings.isCheckUpdateWhenAppStart;
            set
            {
                userSettings.isCheckUpdateWhenAppStart = value;
                SaveSettingsLater();
            }
        }

        public bool isPortable
        {
            get { return userSettings.isPortable; }
            set
            {
                userSettings.isPortable = value;
                SaveSettingsLater();
                try
                {
                    OnPortableModeChanged?.Invoke(this, EventArgs.Empty);
                }
                catch { }
            }
        }

        public string SystrayLeftClickCommand
        {
            get { return userSettings.SystrayLeftClickCommand; }
            set
            {
                userSettings.SystrayLeftClickCommand = value;
                SaveSettingsLater();
            }
        }

        public bool isEnableSystrayLeftClickCommand
        {
            get { return userSettings.isEnableSystrayLeftClickCommand; }
            set
            {
                userSettings.isEnableSystrayLeftClickCommand = value;
                SaveSettingsLater();
            }
        }

        public bool isServerTrackerOn = false;

        public int serverPanelPageSize
        {
            get
            {
                var size = userSettings.ServerPanelPageSize;
                return Misc.Utils.Clamp(size, 1, 101);
            }
            set
            {
                userSettings.ServerPanelPageSize = Misc.Utils.Clamp(value, 1, 101);
                SaveSettingsLater();
            }
        }

        public int maxConcurrentV2RayCoreNum
        {
            get
            {
                var size = userSettings.MaxConcurrentV2RayCoreNum;
                return Misc.Utils.Clamp(size, 1, 1001);
            }
            set
            {
                userSettings.MaxConcurrentV2RayCoreNum = Misc.Utils.Clamp(value, 1, 1001);
                UpdateSpeedTestPool();
                SaveSettingsLater();
            }
        }

        public CultureInfo orgCulture = null;
        readonly VgcApis.Libs.Sys.QueueLogger qLogger = new VgcApis.Libs.Sys.QueueLogger(400, 800);

        public long GetLogTimestamp() => qLogger.GetTimestamp();

        public string GetLogContent() => qLogger.GetLogAsString(true);

        public void SendLog(string log) => qLogger.Log(log);

        public Models.Datas.Enums.Cultures culture
        {
            get
            {
                var cultures = Models.Datas.Table.Cultures;
                var c = userSettings.Culture;

                if (!cultures.ContainsValue(c))
                {
                    return Models.Datas.Enums.Cultures.auto;
                }

                return cultures.Where(s => s.Value == c).First().Key;
            }
            set
            {
                var cultures = Models.Datas.Table.Cultures;
                var c = Models.Datas.Enums.Cultures.auto;
                if (cultures.ContainsKey(value))
                {
                    c = value;
                }
                userSettings.Culture = Models.Datas.Table.Cultures[c];
                SaveSettingsLater();
            }
        }

        public bool isShowConfigerToolsPanel
        {
            get { return userSettings.CfgShowToolPanel == true; }
            set
            {
                userSettings.CfgShowToolPanel = value;
                SaveSettingsLater();
            }
        }

        public const int maxLogLines = 1000;

        #endregion

        #region public methods
        public void DisposeFileMutex()
        {
            var m = fileMutex;
            if (m == null)
            {
                return;
            }
            fileMutex = null;
            m.ReleaseMutex();
            m.Dispose();
        }

        string sendThroughHostIpv4 = "";

        public string GetSendThroughIpv4() => sendThroughHostIpv4;

        /// <summary>
        /// Return false if not changed.
        /// </summary>
        /// <param name="host">IP address</param>
        /// <returns>false if not changed</returns>
        public bool SetSendThroughIpv4(string host)
        {
            if (host == sendThroughHostIpv4)
            {
                return false;
            }
            sendThroughHostIpv4 = host;
            return true;
        }

        bool _isScreenLocked = false;

        public bool IsScreenLocked() => _isScreenLocked;

        public void SetScreenLockingState(bool isLocked)
        {
            _isScreenLocked = isLocked;
        }

        public void SaveV2RayCoreVersionList(List<string> versions)
        {
            // clone version list
            userSettings.V2RayCoreDownloadVersionList = new List<string>(versions);
            SaveSettingsLater();
        }

        public ReadOnlyCollection<string> GetV2RayCoreVersionList()
        {
            var result =
                userSettings.V2RayCoreDownloadVersionList
                ?? new List<string> { "v4.27.5", "v4.27.0", "v4.26.0", "v4.25.1" };
            return result.AsReadOnly();
        }

        // ISettingService thing
        bool isClosing = false;

        public bool IsClosing() => isClosing;

        public bool SetIsClosing(bool isClosing) => this.isClosing = isClosing;

        public string GetUserSettings(string props)
        {
            // backward compactible
            if (string.IsNullOrEmpty(props))
            {
                return JsonConvert.SerializeObject(userSettings);
            }

            try
            {
                var names = JsonConvert.DeserializeObject<List<string>>(props);
                var r = this.GetType()
                    .GetProperties(
                        BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance
                    )
                    .Where(p => names.Contains(p.Name))
                    .Select(p =>
                    {
                        var n = p.Name;
                        var v = p.GetValue(this);
                        return new KeyValuePair<string, object>(n, v);
                    })
                    .ToDictionary(kv => kv.Key, kv => kv.Value);
                return JsonConvert.SerializeObject(r);
            }
            catch { }
            return null;
        }

        bool TryChangeUserSetting(Type type, string name, object value)
        {
            try
            {
                var prop = type.GetProperty(name);
                if (prop == null)
                {
                    return false;
                }
                var t = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                object safeValue = (value == null) ? null : Convert.ChangeType(value, t);
                prop.SetValue(this, safeValue);
                return true;
            }
            catch { }
            return false;
        }

        public bool SetUserSettings(string props)
        {
            try
            {
                var settings = JsonConvert.DeserializeObject<Dictionary<string, object>>(props);
                var type = this.GetType();
                foreach (var kv in settings)
                {
                    var name = kv.Key;
                    var value = kv.Value;
                    TryChangeUserSetting(type, name, value);
                }
                return true;
            }
            catch { }
            return false;
        }

        public List<string> GetLocalStorageKeys()
        {
            return localStorageCache.Keys.ToList();
        }

        public bool RemoveLocalStorage(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return false;
            }

            var r = localStorageCache.TryRemove(key, out var _);
            SaveSettingsLater();
            return r;
        }

        public string ReadLocalStorage(string key)
        {
            if (!string.IsNullOrEmpty(key) && localStorageCache.TryGetValue(key, out string s))
            {
                return s;
            }
            return null;
        }

        public string WriteLocalStorage(string key, string value)
        {
            var r = localStorageCache.AddOrUpdate(key, value, (_, __) => value);
            SaveSettingsLater();
            return r;
        }

        /// <summary>
        /// return null if fail
        /// </summary>
        /// <param name="pluginName"></param>
        /// <returns></returns>
        public string GetPluginsSetting(string pluginName)
        {
            lock (saveUserSettingsLocker)
            {
                if (pluginsSettingCache.ContainsKey(pluginName))
                {
                    return pluginsSettingCache[pluginName];
                }
            }
            return null;
        }

        public void SavePluginsSetting(string pluginName, string value)
        {
            if (string.IsNullOrEmpty(pluginName))
            {
                return;
            }

            lock (saveUserSettingsLocker)
            {
                pluginsSettingCache[pluginName] = value;
            }
            SaveSettingsLater();
        }

        public void SaveUserSettingsNow() => SaveUserSettingsWorker();

        public void SaveServerTrackerSetting(Models.Datas.ServerTracker serverTrackerSetting)
        {
            userSettings.ServerTracker = JsonConvert.SerializeObject(serverTrackerSetting);
            SaveSettingsLater();
        }

        public Models.Datas.ServerTracker GetServerTrackerSetting()
        {
            var empty = new Models.Datas.ServerTracker();
            Models.Datas.ServerTracker result;
            try
            {
                result = JsonConvert.DeserializeObject<Models.Datas.ServerTracker>(
                    userSettings.ServerTracker
                );

                if (result != null)
                {
                    result.serverList = result.serverList ?? new List<string>();
                    result.uids = result.uids ?? new List<string>();
                }
            }
            catch
            {
                return empty;
            }
            return result ?? empty;
        }

        public List<VgcApis.Models.Datas.CoreInfo> LoadCoreInfoList() => coreInfoCache;

        public void SaveFormRect(Form form)
        {
            var key = form.GetType().Name;
            var list = GetWinFormRectList();
            list[key] = new Rectangle(form.Left, form.Top, form.Width, form.Height);
            userSettings.WinFormPosList = JsonConvert.SerializeObject(list);
            SaveSettingsLater();
        }

        public void RestoreFormRect(Form form)
        {
            var key = form.GetType().Name;
            var list = GetWinFormRectList();

            if (!list.ContainsKey(key))
            {
                return;
            }

            var rect = list[key];
            var screen = Screen.PrimaryScreen.WorkingArea;

            form.Width = Math.Max(rect.Width, 300);
            form.Height = Math.Max(rect.Height, 200);
            form.Left = Misc.Utils.Clamp(rect.Left, 0, screen.Right - form.Width);
            form.Top = Misc.Utils.Clamp(rect.Top, 0, screen.Bottom - form.Height);
        }

        public List<Models.Datas.PluginInfoItem> GetPluginInfoItems()
        {
            try
            {
                var items = JsonConvert.DeserializeObject<List<Models.Datas.PluginInfoItem>>(
                    userSettings.PluginInfoItems
                );

                if (items != null)
                {
                    return items;
                }
            }
            catch { }
            return new List<Models.Datas.PluginInfoItem>();
        }

        /// <summary>
        /// Feel free to pass null.
        /// </summary>
        /// <param name="itemList"></param>
        public void SavePluginInfoItems(List<Models.Datas.PluginInfoItem> itemList)
        {
            string json = JsonConvert.SerializeObject(
                itemList ?? new List<Models.Datas.PluginInfoItem>()
            );

            userSettings.PluginInfoItems = json;
            SaveSettingsLater();
        }

        public string GetSubscriptionConfig() => userSettings.SubscribeUrls;

        public List<Models.Datas.SubscriptionItem> GetSubscriptionItems()
        {
            try
            {
                var items = JsonConvert.DeserializeObject<List<Models.Datas.SubscriptionItem>>(
                    userSettings.SubscribeUrls
                );

                if (items != null)
                {
                    return items;
                }
            }
            catch { }
            ;
            return new List<Models.Datas.SubscriptionItem>();
        }

        public void SetSubscriptionConfig(string cfgStr)
        {
            userSettings.SubscribeUrls = cfgStr;
            SaveSettingsLater();
        }

        public void SaveServerList(List<VgcApis.Models.Datas.CoreInfo> coreInfoList)
        {
            lock (saveUserSettingsLocker)
            {
                coreInfoCache = coreInfoList;
            }
            SaveSettingsLater();
        }
        #endregion

        #region private method
        void Init()
        {
            userSettings = LoadUserSettings();
            userSettings.Normalized(); // replace null with empty object.

            InitCoreInfoCache();
            InitLocalStorageCache(); // must init before plug-ins setting
            InitPluginsSettingCache();
            InitConfigTemplatesCache();

            UpdateVgcApisUserAgent();

            UpdateSpeedTestPool();
            UpdateFileLoggerSetting();

            lazyBookKeeper = new VgcApis.Libs.Tasks.LazyGuy(
                SaveUserSettingsWorker,
                VgcApis.Models.Consts.Intervals.LazySaveUserSettingsDelay,
                500
            )
            {
                Name = "Settings.SaveSettings",
            };
        }

        void InitLocalStorageCache()
        {
            try
            {
                var ls =
                    VgcApis.Libs.Infr.ZipExtensions.DeserializeObjectFromCompressedUnicodeBase64<
                        ConcurrentDictionary<string, string>
                    >(userSettings.CompressedUnicodeLocalStorage);
                localStorageCache = ls;
            }
            catch { }
        }

        void InitCoreInfoCache()
        {
            List<VgcApis.Models.Datas.CoreInfo> coreInfos = null;

            try
            {
                var ucs = userSettings.CompressedUnicodeCoreInfoList;

                coreInfos =
                    VgcApis.Libs.Infr.ZipExtensions.DeserializeObjectFromCompressedUnicodeBase64<
                        List<VgcApis.Models.Datas.CoreInfo>
                    >(ucs);

                userSettings.CompressedUnicodeCoreInfoList = VgcApis
                    .Models
                    .Consts
                    .Libs
                    .coreInfoPlaceHolder;
            }
            catch { }

            if (coreInfos == null)
            {
                coreInfos = new List<VgcApis.Models.Datas.CoreInfo>();
            }

            // make sure every config of server can be parsed correctly
            var result = coreInfos
                .Where(c =>
                {
                    try
                    {
                        return JObject.Parse(c.GetConfig()) != null;
                    }
                    catch { }
                    return false;
                })
                .ToList();

            coreInfoCache = coreInfos;
        }

        void UpdateVgcApisUserAgent()
        {
            var cua = userSettings.isUseCustomUserAgent
                ? userSettings.customUserAgent
                : VgcApis.Models.Consts.Webs.ChromeUserAgent;
            var key = VgcApis.Models.Consts.Webs.UserAgentKey;
            var ua = $"{key}: {cua}";
            VgcApis.Models.Consts.Webs.CustomUserAgent = cua;
            VgcApis.Models.Consts.Webs.UserAgent = ua;
        }

        void UpdateFileLoggerSetting()
        {
            if (userSettings.isEnableDebugFile)
            {
                VgcApis.Libs.Sys.FileLogger.LogFilename = userSettings.DebugLogFilePath;
            }
        }

        void SaveUserSettingsWorker()
        {
            // VgcApis.Libs.Sys.FileLogger.Info("Settings.SaveUserSettingsWorker() begin");
            try
            {
                // local storage is design for storing small values
                userSettings.CompressedUnicodeLocalStorage =
                    VgcApis.Libs.Infr.ZipExtensions.SerializeObjectToCompressedUnicodeBase64(
                        localStorageCache
                    );

                if (userSettings.isPortable)
                {
                    lock (saveUserSettingsLocker)
                    {
                        // DebugSendLog("Try save settings to file.");
                        SaveUserSettingsToFile();
                    }
                }
                else
                {
                    lock (saveUserSettingsLocker)
                    {
                        // DebugSendLog("Try save settings to properties");
                        SetUserSettingFileIsPortableToFalse();
                        SaveUserSettingsToProperties();
                    }
                    VgcApis.Libs.Sys.FileLogger.Info(
                        "Settings.SaveUserSettingsToProperties() done"
                    );
                }
                // VgcApis.Libs.Sys.FileLogger.Info("Settings.SaveUserSettingsWorker() done");
                return;
            }
            catch { }

            VgcApis.Libs.Sys.FileLogger.Info("Settings.SaveUserSettingsWorker() error");
            if (GetShutdownReason() == VgcApis.Models.Datas.Enums.ShutdownReasons.CloseByUser)
            {
                var serializedUserSettings = JsonConvert.SerializeObject(userSettings);
                SendLog("UserSettings: " + Environment.NewLine + serializedUserSettings);
                throw new ArgumentException("Validate serialized user settings fail!");
            }
        }

        void UpdateSpeedTestPool()
        {
            var poolSize = userSettings.MaxConcurrentV2RayCoreNum;
            SpeedTestPool.SetPoolSize(poolSize);
        }

        void InitPluginsSettingCache()
        {
            var empty = new Dictionary<string, string>();

            Dictionary<string, string> pluginsSetting = null;
            try
            {
                string ucps = userSettings.CompressedUnicodePluginsSetting;
                pluginsSetting =
                    VgcApis.Libs.Infr.ZipExtensions.DeserializeObjectFromCompressedUnicodeBase64<
                        Dictionary<string, string>
                    >(ucps);
                userSettings.CompressedUnicodePluginsSetting = VgcApis
                    .Models
                    .Consts
                    .Libs
                    .pluginPlaceHolder;
            }
            catch { }

            pluginsSettingCache = pluginsSetting ?? empty;
        }

        void SetUserSettingFileIsPortableToFalse()
        {
            DebugSendLog("Read user setting file");

            var mainUsFilename = cmdArgs.userSettings;
            var bakUsFilename = cmdArgs.userSettingsBak;
            if (!File.Exists(mainUsFilename) && !File.Exists(bakUsFilename))
            {
                DebugSendLog("setting file not exists");
                return;
            }

            DebugSendLog("set portable to false");
            userSettings.isPortable = false;
            try
            {
                lock (saveUserSettingsLocker)
                {
                    Misc.Utils.ClumsyWriter(
                        userSettings,
                        null,
                        null,
                        mainUsFilename,
                        bakUsFilename
                    );
                }
                DebugSendLog("set portable option done");
                return;
            }
            catch { }

            if (GetShutdownReason() == VgcApis.Models.Datas.Enums.ShutdownReasons.CloseByUser)
            {
                var msg = string.Format(I18N.UnsetPortableModeFail, mainUsFilename);
                // do not block any function in background service
                VgcApis.Misc.UI.MsgBoxAsync(msg);
            }
        }

        void SaveUserSettingsToProperties()
        {
            try
            {
                userSettings.CompressedUnicodeCoreInfoList =
                    VgcApis.Libs.Infr.ZipExtensions.SerializeObjectToCompressedUnicodeBase64(
                        coreInfoCache
                    );

                userSettings.CompressedUnicodePluginsSetting =
                    VgcApis.Libs.Infr.ZipExtensions.SerializeObjectToCompressedUnicodeBase64(
                        pluginsSettingCache
                    );

                var us = JsonConvert.SerializeObject(userSettings);
                Properties.Settings.Default.UserSettings = us;
                Properties.Settings.Default.Save();
            }
            catch
            {
                DebugSendLog("Save user settings to Properties fail!");
            }
        }

        void SaveUserSettingsToFile()
        {
            VgcApis.Libs.Sys.FileLogger.Info("Settings.SaverUserSettingsToFile() write file");

            lock (saveUserSettingsLocker)
            {
                var ok = Misc.Utils.ClumsyWriter(
                    userSettings,
                    coreInfoCache,
                    pluginsSettingCache,
                    cmdArgs.userSettings,
                    cmdArgs.userSettingsBak
                );
                if (ok)
                {
                    VgcApis.Libs.Sys.FileLogger.Info("Settings.SaverUserSettingsToFile() success");
                    return;
                }
            }

            VgcApis.Libs.Sys.FileLogger.Error("Settings.SaverUserSettingsToFile() failed");
            // main file or bak file write fail, clear cache

            WarnUserSaveSettingsFailed();
        }

        private void WarnUserSaveSettingsFailed()
        {
            var msg = I18N.SaveUserSettingsToFileFail;

            if (isClosing)
            {
                // 兄弟只能帮你到这了
                var content = JsonConvert.SerializeObject(userSettings);
                VgcApis.Libs.Sys.NotepadHelper.ShowMessage(content, cmdArgs.userSettings);
                msg +=
                    Environment.NewLine
                    + string.Format(I18N.AndThenSaveThisFileAs, cmdArgs.userSettings);
            }

            msg += Environment.NewLine + I18N.OrDisablePortableMode;
            // do not block any function in background service
            VgcApis.Misc.UI.MsgBoxAsync(msg);
        }

        Models.Datas.UserSettings LoadUserSettingsFromPorperties()
        {
            try
            {
                var serializedUserSettings = Properties.Settings.Default.UserSettings;
                var us = JsonConvert.DeserializeObject<Models.Datas.UserSettings>(
                    serializedUserSettings
                );
                if (us != null)
                {
                    DebugSendLog("Read user settings from Properties.Usersettings");
                    return us;
                }
            }
            catch { }

            return null;
        }

        Models.Datas.UserSettings LoadUserSettingsFromFile()
        {
            // try to load userSettings.json
            Models.Datas.UserSettings result = null;
            try
            {
                var content = File.ReadAllText(cmdArgs.userSettings);
                result = JsonConvert.DeserializeObject<Models.Datas.UserSettings>(content);
            }
            catch { }

            // try to load userSettings.bak
            if (result == null)
            {
                result = VgcApis.Misc.Utils.LoadAndParseJsonFile<Models.Datas.UserSettings>(
                    cmdArgs.userSettingsBak
                );
            }

            if (result != null && result.isPortable)
            {
                return result;
            }
            return null;
        }

        Models.Datas.UserSettings LoadUserSettings()
        {
            var mainUsFile = cmdArgs.userSettings;
            var bakUsFile = cmdArgs.userSettingsBak;

            var result = LoadUserSettingsFromFile() ?? LoadUserSettingsFromPorperties();
            if (
                result == null
                && (File.Exists(mainUsFile) || File.Exists(bakUsFile))
                && !VgcApis.Misc.UI.Confirm(I18N.ConfirmLoadDefaultUserSettings)
            )
            {
                SetShutdownReason(VgcApis.Models.Datas.Enums.ShutdownReasons.Abort);
            }

            return result ?? new Models.Datas.UserSettings();
        }

        void SaveSettingsLater() => lazyBookKeeper?.Deadline();

        Dictionary<string, Rectangle> winFormRectListCache = null;

        Dictionary<string, Rectangle> GetWinFormRectList()
        {
            if (winFormRectListCache != null)
            {
                return winFormRectListCache;
            }

            try
            {
                winFormRectListCache = JsonConvert.DeserializeObject<Dictionary<string, Rectangle>>(
                    userSettings.WinFormPosList
                );
            }
            catch { }

            if (winFormRectListCache == null)
            {
                winFormRectListCache = new Dictionary<string, Rectangle>();
            }

            return winFormRectListCache;
        }

        #endregion

        #region protected methods
        protected override void Cleanup()
        {
            DisposeFileMutex();
            VgcApis.Libs.Sys.FileLogger.Info("Settings.Cleanup() begin");
            lazyBookKeeper?.Dispose();
            SaveUserSettingsNow();
            qLogger.Dispose();
            VgcApis.Libs.Sys.FileLogger.Info("Settings.Cleanup() done");
        }
        #endregion

        #region debug
        void DebugSendLog(string content)
        {
#if DEBUG
            SendLog($"(Debug) {content}");
#endif
        }
        #endregion
    }
}
