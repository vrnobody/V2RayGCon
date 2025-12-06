using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.ConstrainedExecution;
using System.Threading;
using System.Windows.Forms;
using Newtonsoft.Json;
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
                var coreSettings = userSettings.CustomCoreSettings.FirstOrDefault(cs =>
                    cs.name == name
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
                var coreS = userSettings.CustomCoreSettings.FirstOrDefault(cs =>
                    cs.name == coreSettings.name
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
            List<Models.Datas.CustomConfigTemplate> tpl = null;
            try
            {
                tpl = VgcApis.Libs.Infr.ZipExtensions.DeserializeObjectFromZstdBase64<
                    List<Models.Datas.CustomConfigTemplate>
                >(userSettings.ZstdCustomConfigTemplates);
            }
            catch { }

            // obsolete delete after 2026-10
            if (tpl == null)
            {
                try
                {
                    tpl =
                        VgcApis.Libs.Infr.ZipExtensions.DeserializeObjectFromCompressedUnicodeBase64<
                            List<Models.Datas.CustomConfigTemplate>
                        >(userSettings.CompressedUnicodeCustomConfigTemplates);
                }
                catch { }
            }
            configTemplateCache = tpl ?? new List<Models.Datas.CustomConfigTemplate>();
        }

        // use by TabDefault
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

        public ReadOnlyCollection<Models.Datas.CustomConfigTemplate> GetCustomConfigTemplates()
        {
            lock (saveUserSettingsLocker)
            {
                return configTemplateCache.OrderBy(inb => inb.index).ToList().AsReadOnly();
            }
        }

        string CalcInjectTemplatesHash(IEnumerable<Models.Datas.CustomConfigTemplate> templates)
        {
            var hashs = templates.Select(tpl => VgcApis.Misc.Utils.Sha256Hex(tpl.template));
            return string.Join(",", hashs);
        }

        public bool ReplaceCustomConfigTemplates(List<Models.Datas.CustomConfigTemplate> tpls)
        {
            var injectTemplatesChanged = false;
            lock (saveUserSettingsLocker)
            {
                var oldHash = CalcInjectTemplatesHash(configTemplateCache);
                var newHash = CalcInjectTemplatesHash(tpls);
                injectTemplatesChanged = oldHash != newHash;
                configTemplateCache = VgcApis.Misc.Utils.Clone(tpls);
            }
            SaveSettingsLater();
            return injectTemplatesChanged;
        }

        #endregion

        #region Properties
        public long LastBootTimestamp
        {
            get => userSettings.LastBootTimestamp;
            set
            {
                userSettings.LastBootTimestamp = value;
                SaveSettingsLater();
            }
        }

        public string CustomFilterKeywords
        {
            get => userSettings.CustomFilterKeywords;
            set
            {
                userSettings.CustomFilterKeywords = value;
                SaveSettingsLater();
            }
        }

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
            VgcApis.Libs.Sys.FileLogger.Warn($"set shutdow reason: {reason}");
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

        public bool isDownloadWin7XrayCore
        {
            get => userSettings.isDownloadWin7XrayCore;
            set
            {
                userSettings.isDownloadWin7XrayCore = value;
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

        public bool isEnableStatistics
        {
            get => userSettings.isEnableStat;
            set
            {
                userSettings.isEnableStat = value;
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

        public bool CustomDefImportMobShareLink
        {
            get => userSettings.ImportOptions.IsImportMobShareLink;
            set
            {
                userSettings.ImportOptions.IsImportMobShareLink = value;
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

        public int serverPanelPageSize
        {
            get
            {
                var size = userSettings.ServerPanelPageSize;
                return VgcApis.Misc.Utils.Clamp(size, 1, 101);
            }
            set
            {
                userSettings.ServerPanelPageSize = VgcApis.Misc.Utils.Clamp(value, 1, 101);
                SaveSettingsLater();
            }
        }

        public int maxConcurrentV2RayCoreNum
        {
            get
            {
                var size = userSettings.MaxConcurrentV2RayCoreNum;
                return VgcApis.Misc.Utils.Clamp(size, 1, 1001);
            }
            set
            {
                userSettings.MaxConcurrentV2RayCoreNum = VgcApis.Misc.Utils.Clamp(value, 1, 1001);
                UpdateSpeedTestPool();
                SaveSettingsLater();
            }
        }

        public CultureInfo orgCulture = null;

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

        public bool IsPowerOff() =>
            GetShutdownReason() == VgcApis.Models.Datas.Enums.ShutdownReasons.PowerOff;

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
            var result = userSettings.V2RayCoreDownloadVersionList;
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
            string config = null;
            lock (saveUserSettingsLocker)
            {
                if (pluginsSettingCache.ContainsKey(pluginName))
                {
                    config = pluginsSettingCache[pluginName];
                }
            }

            if (VgcApis.Libs.Infr.ZipExtensions.IsZstdBase64(config))
            {
                return VgcApis.Libs.Infr.ZipExtensions.ZstdFromBase64(config);
            }
            else if (VgcApis.Libs.Infr.ZipExtensions.IsCompressedBase64(config))
            {
                return VgcApis.Libs.Infr.ZipExtensions.DecompressFromBase64(config);
            }
            return config;
        }

        public List<string> GetPluginsSettinKeys()
        {
            lock (saveUserSettingsLocker)
            {
                return pluginsSettingCache.Keys.ToList();
            }
        }

        public bool RemovePluginsSettinKey(string key)
        {
            lock (saveUserSettingsLocker)
            {
                if (pluginsSettingCache.ContainsKey(key))
                {
                    return pluginsSettingCache.Remove(key);
                }
            }
            return false;
        }

        public void SavePluginsSetting(string pluginName, string config)
        {
            if (string.IsNullOrEmpty(pluginName))
            {
                return;
            }
            if (
                VgcApis.Misc.Utils.StrLenInBytes(config)
                > VgcApis.Models.Consts.Libs.MinCompressStringLength
            )
            {
                config = VgcApis.Libs.Infr.ZipExtensions.ZstdToBase64(config);
            }
            lock (saveUserSettingsLocker)
            {
                pluginsSettingCache[pluginName] = config;
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
                    result.uids = result.uids ?? new List<string>();
                }
            }
            catch
            {
                result = empty;
            }
            return result;
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

            var ratio = 0.95;
            if (rect.Width > screen.Width * ratio && rect.Height > screen.Height * ratio)
            {
                form.WindowState = FormWindowState.Maximized;
                return;
            }

            form.Width = Math.Max(rect.Width, 300);
            form.Height = Math.Max(rect.Height, 200);
            form.Left = VgcApis.Misc.Utils.Clamp(rect.Left, 0, screen.Right - form.Width);
            form.Top = VgcApis.Misc.Utils.Clamp(rect.Top, 0, screen.Bottom - form.Height);
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
            CreatePlaceHolders();

            userSettings = LoadUserSettings();
            userSettings.Normalized(); // replace null with empty object.

            InitCoreInfoCache();
            InitLocalStorageCache(); // must init before plug-ins setting
            InitPluginsSettingCache();
            InitConfigTemplatesCache();
            ReplaceLargeStringsWithPlaceHolder();

            UpdateVgcApisUserAgent();

            UpdateSpeedTestPool();
            UpdateFileLoggerSetting();
            VgcApis.Libs.Sys.FileLogger.Ready();

            lazyBookKeeper = new VgcApis.Libs.Tasks.LazyGuy(
                SaveUserSettingsWorker,
                VgcApis.Models.Consts.Intervals.LazySaveUserSettingsDelay,
                60 * 1000
            )
            {
                Name = "Settings.SaveSettings",
            };
        }

        void InitLocalStorageCache()
        {
            ConcurrentDictionary<string, string> ls = null;

            try
            {
                ls = VgcApis.Libs.Infr.ZipExtensions.DeserializeObjectFromZstdBase64<
                    ConcurrentDictionary<string, string>
                >(userSettings.ZstdLocalStorage);
            }
            catch { }

            // obsolete delete after 2026-10
            if (ls == null)
            {
                try
                {
                    ls =
                        VgcApis.Libs.Infr.ZipExtensions.DeserializeObjectFromCompressedUnicodeBase64<
                            ConcurrentDictionary<string, string>
                        >(userSettings.CompressedUnicodeLocalStorage);
                }
                catch { }
            }
            localStorageCache = ls ?? new ConcurrentDictionary<string, string>();
        }

        void InitCoreInfoCache()
        {
            List<VgcApis.Models.Datas.CoreInfo> coreInfos = null;

            try
            {
                coreInfos = VgcApis.Libs.Infr.ZipExtensions.DeserializeObjectFromZstdBase64<
                    List<VgcApis.Models.Datas.CoreInfo>
                >(userSettings.ZstdCoreInfoList);
            }
            catch { }

            // fallback
            // obsolete delete after 2026-10
            if (coreInfos == null)
            {
                try
                {
                    coreInfos =
                        VgcApis.Libs.Infr.ZipExtensions.DeserializeObjectFromCompressedUnicodeBase64<
                            List<VgcApis.Models.Datas.CoreInfo>
                        >(userSettings.CompressedUnicodeCoreInfoList);
                }
                catch { }
            }
            coreInfoCache = coreInfos ?? new List<VgcApis.Models.Datas.CoreInfo>();
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
                VgcApis.Libs.Sys.FileLogger.SetLogFilename(userSettings.DebugLogFilePath);
            }
        }

        enum PlaceHolders
        {
            LocalStorage,
            CoreInfoList,
            PluginsSetting,
            CustomConfigTemplates,
        }

        Dictionary<PlaceHolders, string> placeHolderTable = new Dictionary<PlaceHolders, string>();
        Dictionary<string, PlaceHolders> placeHolderLookupTable =
            new Dictionary<string, PlaceHolders>();

        void CreatePlaceHolders()
        {
            var mark = @"vgc-placeholder";
            var uid = Guid.NewGuid().ToString();
            var ph = placeHolderTable;
            var plt = placeHolderLookupTable;
            foreach (var nameStr in Enum.GetNames(typeof(PlaceHolders)))
            {
                if (Enum.TryParse<PlaceHolders>(nameStr, out var name))
                {
                    var v = $"{nameStr}-{mark}-{uid}";
                    ph[name] = v;
                    plt[v] = name;
                }
            }
        }

        void ReplaceLargeStringsWithPlaceHolder()
        {
            userSettings.CompressedUnicodeLocalStorage = "";
            userSettings.ZstdLocalStorage = placeHolderTable[PlaceHolders.LocalStorage];

            userSettings.CompressedUnicodeCoreInfoList = "";
            userSettings.ZstdCoreInfoList = placeHolderTable[PlaceHolders.CoreInfoList];

            userSettings.CompressedUnicodePluginsSetting = "";
            userSettings.ZstdPluginsSetting = placeHolderTable[PlaceHolders.PluginsSetting];

            userSettings.CompressedUnicodeCustomConfigTemplates = "";
            userSettings.ZstdCustomConfigTemplates = placeHolderTable[
                PlaceHolders.CustomConfigTemplates
            ];
        }

        object GetTargetByPlaceHolderName(PlaceHolders name)
        {
            switch (name)
            {
                case PlaceHolders.CustomConfigTemplates:
                    return configTemplateCache;
                case PlaceHolders.LocalStorage:
                    return localStorageCache;
                case PlaceHolders.CoreInfoList:
                    return coreInfoCache;
                case PlaceHolders.PluginsSetting:
                    return pluginsSettingCache;
                default:
                    throw new ArgumentException($"Unknown placeholder name: {name}");
            }
        }

        string ReplacePlaceHoldersWithData(string configSlim)
        {
            var parts = VgcApis.Misc.Utils.SplitAndKeep(configSlim, placeHolderLookupTable.Keys);
            for (int i = 0; i < parts.Count; i++)
            {
                var ph = parts[i];
                if (!placeHolderLookupTable.TryGetValue(ph, out var name))
                {
                    continue;
                }
                var o = GetTargetByPlaceHolderName(name);
                var s = VgcApis.Libs.Infr.ZipExtensions.SerializeObjectToZstdBase64(o);
                parts[i] = s;
            }
            return string.Join("", parts);
        }

        void SaveUserSettingsWorker()
        {
            VgcApis.Libs.Sys.FileLogger.Info("Settings.SaveUserSettingsWorker() begin");
            string configSlim = null;
            var isPortable = true;
            try
            {
                lock (saveUserSettingsLocker)
                {
                    configSlim = JsonConvert.SerializeObject(userSettings, Formatting.Indented);
                    isPortable = userSettings.isPortable;
                }

                if (!string.IsNullOrEmpty(configSlim))
                {
                    if (isPortable)
                    {
                        VgcApis.Misc.Logger.Debug("Try save settings to file.");
                        SaveUserSettingsToFile(
                            configSlim,
                            cmdArgs.userSettings,
                            cmdArgs.userSettingsBak
                        );
                    }
                    else
                    {
                        VgcApis.Misc.Logger.Debug("Try save settings to %AppData%");
                        SetUserSettingFileIsPortableToFalse(configSlim);
                        var usFiles = GetSysAppDataUserSettingFilenames();
                        Misc.Utils.CreateSysAppDataFolder();
                        SaveUserSettingsToFile(configSlim, usFiles[0], usFiles[1]);
                    }
                    VgcApis.Libs.Sys.FileLogger.Info("Settings.SaveUserSettingsWorker() done");
                    return;
                }
            }
            catch { }

            VgcApis.Libs.Sys.FileLogger.Info("Settings.SaveUserSettingsWorker() error");
            if (GetShutdownReason() == VgcApis.Models.Datas.Enums.ShutdownReasons.CloseByUser)
            {
                var serializedUserSettings = JsonConvert.SerializeObject(userSettings);
                VgcApis.Misc.Logger.Log(
                    "UserSettings:",
                    Environment.NewLine,
                    serializedUserSettings
                );
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
            Dictionary<string, string> pluginsSetting = null;
            try
            {
                pluginsSetting = VgcApis.Libs.Infr.ZipExtensions.DeserializeObjectFromZstdBase64<
                    Dictionary<string, string>
                >(userSettings.ZstdPluginsSetting);
            }
            catch { }

            // obsolete delete after 2026-10
            if (pluginsSetting == null)
            {
                try
                {
                    pluginsSetting =
                        VgcApis.Libs.Infr.ZipExtensions.DeserializeObjectFromCompressedUnicodeBase64<
                            Dictionary<string, string>
                        >(userSettings.CompressedUnicodePluginsSetting);
                }
                catch { }
            }

            pluginsSettingCache = pluginsSetting ?? new Dictionary<string, string>();
        }

        void SetUserSettingFileIsPortableToFalse(string us)
        {
            VgcApis.Misc.Logger.Debug("set portable to false");
            var mainUsFilename = cmdArgs.userSettings;
            var bakUsFilename = cmdArgs.userSettingsBak;
            if (!File.Exists(mainUsFilename) && !File.Exists(bakUsFilename))
            {
                VgcApis.Misc.Logger.Debug("setting file not exists");
                return;
            }

            try
            {
                Misc.Utils.ClumsyWriter(us, mainUsFilename, bakUsFilename);
                VgcApis.Misc.Logger.Debug("set portable option done");
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

        void StreamUserSettingsToFile(List<string> parts, string filename)
        {
            // https://stackoverflow.com/questions/25366534/file-writealltext-not-flushing-data-to-disk
            var bufferSize = VgcApis.Models.Consts.Libs.FilestreamBufferSize;
            using (var stream = File.Create(filename, bufferSize, FileOptions.WriteThrough))
            using (var w = new StreamWriter(stream))
            {
                foreach (var part in parts)
                {
                    if (placeHolderLookupTable.TryGetValue(part, out var name))
                    {
                        var o = GetTargetByPlaceHolderName(name);
                        VgcApis.Libs.Infr.ZipExtensions.SerializeObjectAsZstdBase64ToStream(
                            stream,
                            o
                        );
                    }
                    else
                    {
                        w.Write(part);
                        w.Flush();
                    }
                }
            }
        }

        private readonly object writeUserSettingFilesLocker = new object();

        bool TryWriteUserSettings(List<string> parts, string main, string bak)
        {
            lock (writeUserSettingFilesLocker)
            {
                try
                {
                    VgcApis.Libs.Sys.FileLogger.Info($"Write file: {main}");
                    StreamUserSettingsToFile(parts, main);
                    if (!IsPowerOff())
                    {
                        VgcApis.Libs.Sys.FileLogger.Info($"Write file: {bak}");
                        StreamUserSettingsToFile(parts, bak);
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    VgcApis.Libs.Sys.FileLogger.Error($"Write file failed: {ex.Message}");
                }
            }
            return false;
        }

        void SaveUserSettingsToFile(string configSlim, string mainFilename, string bakFilename)
        {
            var parts = VgcApis.Misc.Utils.SplitAndKeep(configSlim, placeHolderLookupTable.Keys);
            if (TryWriteUserSettings(parts, mainFilename, bakFilename))
            {
                VgcApis.Libs.Sys.FileLogger.Info("Settings.SaverUserSettingsToFile() success");
                return;
            }
            VgcApis.Libs.Sys.FileLogger.Error("Settings.SaverUserSettingsToFile() failed");

            // main file or bak file write fail, clear cache
            WarnUserSaveSettingsFailed(configSlim);
        }

        private void WarnUserSaveSettingsFailed(string configSlim)
        {
            var msg = I18N.SaveUserSettingsToFileFail;

            if (IsClosing())
            {
                // 兄弟只能帮你到这了
                var fullConfig = ReplacePlaceHoldersWithData(configSlim);
                VgcApis.Libs.Sys.NotepadHelper.ShowMessage(fullConfig, cmdArgs.userSettings);
                msg +=
                    Environment.NewLine
                    + string.Format(I18N.AndThenSaveThisFileAs, cmdArgs.userSettings);
            }

            msg += Environment.NewLine + I18N.OrDisablePortableMode;
            // do not block any function in background service
            if (IsClosing())
            {
                VgcApis.Libs.Sys.NotepadHelper.ShowMessage(msg, "Warning!!");
            }
            else
            {
                VgcApis.Misc.UI.MsgBoxAsync(msg);
            }
        }

        // obsolete! delete after 2026-06-01
        Models.Datas.UserSettings LoadUserSettingsFromPorperties()
        {
            VgcApis.Libs.Sys.FileLogger.Info("Settings.LoadUserSettingsFromPorperties() begin");
            try
            {
                var serializedUserSettings = Properties.Settings.Default.UserSettings;
                var us = JsonConvert.DeserializeObject<Models.Datas.UserSettings>(
                    serializedUserSettings
                );
                if (us != null)
                {
                    VgcApis.Misc.Logger.Debug("Read user settings from Properties.Usersettings");
                    return us;
                }
            }
            catch { }
            finally
            {
                VgcApis.Libs.Sys.FileLogger.Info("Settings.LoadUserSettingsFromPorperties() done");
            }
            return null;
        }

        Models.Datas.UserSettings LoadUserSettingsFromFile(
            string mainFilename,
            string bakFilename,
            bool ignorePartableSetting
        )
        {
            VgcApis.Libs.Sys.FileLogger.Info("Settings.LoadUserSettingsFromFile() begin");
            // try to load userSettings.json
            Models.Datas.UserSettings result = null;
            try
            {
                VgcApis.Libs.Sys.FileLogger.Info($"Try load: {mainFilename}");
                var content = File.ReadAllText(mainFilename);
                result = JsonConvert.DeserializeObject<Models.Datas.UserSettings>(content);
            }
            catch { }

            // try to load userSettings.bak
            if (result == null)
            {
                VgcApis.Libs.Sys.FileLogger.Info($"Try load: {mainFilename}");
                result = VgcApis.Misc.Utils.LoadAndParseJsonFile<Models.Datas.UserSettings>(
                    bakFilename
                );
            }

            VgcApis.Libs.Sys.FileLogger.Info("Settings.LoadUserSettingsFromFile() done");
            if (result != null && (ignorePartableSetting || result.isPortable))
            {
                return result;
            }
            return null;
        }

        string[] GetSysAppDataUserSettingFilenames()
        {
            var folder = Misc.Utils.GetSysAppDataFolder();
            var main = Path.Combine(folder, Properties.Resources.PortableUserSettingsFilename);
            var bak = VgcApis.Misc.Utils.ReplaceFileExtention(main, ".bak");
            return new string[] { main, bak };
        }

        Models.Datas.UserSettings LoadUserSettings()
        {
            VgcApis.Libs.Sys.FileLogger.Info("Settings.LoadUserSettings() begin");
            var mainUsFile = cmdArgs.userSettings;
            var bakUsFile = cmdArgs.userSettingsBak;
            var appDataUsFiles = GetSysAppDataUserSettingFilenames();

            var result =
                LoadUserSettingsFromFile(cmdArgs.userSettings, cmdArgs.userSettingsBak, false)
                ?? LoadUserSettingsFromFile(appDataUsFiles[0], appDataUsFiles[1], true)
                ?? LoadUserSettingsFromPorperties();
            VgcApis.Libs.Sys.FileLogger.Info("Settings.LoadUserSettings() done");

            var appVer = Misc.Utils.GetAssemblyVersion().ToString();
            var cfgVer = result?.ConfigVer;
            if (VgcApis.Misc.Utils.IsOlderVersion(appVer, cfgVer))
            {
                VgcApis.Libs.Sys.FileLogger.Warn(
                    $"Loading new version config v{cfgVer} from old GUI v{appVer}"
                );
                if (!VgcApis.Misc.UI.Confirm(I18N.ConfirmUseOldGui))
                {
                    SetShutdownReason(VgcApis.Models.Datas.Enums.ShutdownReasons.Abort);
                }
            }

            if (
                result == null
                && (File.Exists(mainUsFile) || File.Exists(bakUsFile))
                && !VgcApis.Misc.UI.Confirm(I18N.ConfirmLoadDefaultUserSettings)
            )
            {
                SetShutdownReason(VgcApis.Models.Datas.Enums.ShutdownReasons.Abort);
            }

            result = result ?? new Models.Datas.UserSettings();
            result.ConfigVer = appVer;
            return result;
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
            if (IsPowerOff())
            {
                VgcApis.Libs.Sys.FileLogger.Info(
                    "Settings.Cleanup() skip SaveUserSettingsNow() during power off"
                );
            }
            else
            {
                SaveUserSettingsNow();
            }
            VgcApis.Libs.Sys.FileLogger.Info("Settings.Cleanup() done");
        }
        #endregion
    }
}
