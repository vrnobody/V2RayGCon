using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using V2RayGCon.Resources.Resx;

namespace V2RayGCon.Services
{
    public class Settings :
        BaseClasses.SingletonService<Settings>,
        VgcApis.Interfaces.Services.ISettingsService
    {
        Models.Datas.UserSettings userSettings;

        VgcApis.Libs.Tasks.LazyGuy janitor, lazyBookKeeper;

        readonly object saveUserSettingsLocker = new object();
        string lastUserSettingsHash = @"";
        public event EventHandler OnPortableModeChanged;
        VgcApis.Models.Datas.Enums.ShutdownReasons shutdownReason = VgcApis.Models.Datas.Enums.ShutdownReasons.Undefined;

        // Singleton need this private ctor.
        Settings()
        {
            userSettings = LoadUserSettings();
            userSettings.Normalized();  // replace null with empty object.
            UpdateSpeedTestPool();
            UpdateFileLoggerSetting();

            janitor = new VgcApis.Libs.Tasks.LazyGuy(
                () => GC.Collect(),
                VgcApis.Models.Consts.Intervals.LazyGcDelay,
                5000)
            {
                Name = "Settings.GC()",
            };

            lazyBookKeeper = new VgcApis.Libs.Tasks.LazyGuy(
                SaveUserSettingsWorker,
                VgcApis.Models.Consts.Intervals.LazySaveUserSettingsDelay,
                500)
            {
                Name = "Settings.SaveSettings",
            };
        }

        #region Properties
        public int SpeedtestCounter = 0;
        public int GetSpeedtestQueueLength() => SpeedtestCounter;

        public bool CustomVmessDecodeTemplateEnabled
        {
            get => userSettings.CustomVmessDecodeTemplateEnabled;
            set
            {
                userSettings.CustomVmessDecodeTemplateEnabled = value;
                SaveSettingsLater();
            }
        }

        public string CustomVmessDecodeTemplateUrl
        {
            get => userSettings.CustomVmessDecodeTemplateUrl;
            set
            {
                if (userSettings.CustomVmessDecodeTemplateUrl == value)
                {
                    return;
                }
                userSettings.CustomVmessDecodeTemplateUrl = value;
                SaveSettingsLater();
            }
        }

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
            get
            {
                return Math.Max(0, userSettings.QuickSwitchServerLatency);
            }
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

        SemaphoreSlim _speedTestPool = null;
        public SemaphoreSlim SpeedTestPool
        {
            get => _speedTestPool;
            private set { }
        }

        public bool isSpeedtestCancelled = false;

        public string AllPluginsSetting
        {
            get => userSettings.PluginsSetting;
            set
            {
                userSettings.PluginsSetting = value;
                SaveSettingsLater();
            }
        }

        public VgcApis.Models.Datas.Enums.ShutdownReasons GetShutdownReason() => shutdownReason;

        public void SetShutdownReason(VgcApis.Models.Datas.Enums.ShutdownReasons reason)
        {
            VgcApis.Libs.Sys.FileLogger.Warn($"change shutdow reason to: {reason.ToString()}");
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

        public string decodeCache
        {
            get
            {
                return userSettings.DecodeCache;
            }
            set
            {
                userSettings.DecodeCache = value;
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

        public bool CustomDefImportGlobalImport
        {
            get => userSettings.ImportOptions.IsInjectGlobalImport;
            set
            {
                userSettings.ImportOptions.IsInjectGlobalImport = value;
                SaveSettingsLater();
            }
        }

        public bool CustomDefImportBypassCnSite
        {
            get => userSettings.ImportOptions.IsBypassCnSite;
            set
            {
                userSettings.ImportOptions.IsBypassCnSite = value;
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

        public bool CustomDefImportTrojanShareLink
        {
            get => userSettings.ImportOptions.IsImportTrojanShareLink;
            set
            {
                userSettings.ImportOptions.IsImportTrojanShareLink = value;
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

        public int CustomDefImportMode
        {
            get => VgcApis.Misc.Utils.Clamp(userSettings.ImportOptions.Mode, 0, 4);
            set
            {
                userSettings.ImportOptions.Mode = VgcApis.Misc.Utils.Clamp(value, 0, 4);
                SaveSettingsLater();
            }
        }

        public string CustomDefImportIp
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
            get
            {
                return userSettings.isPortable;
            }
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

        VgcApis.Libs.Sys.QueueLogger qLogger = new VgcApis.Libs.Sys.QueueLogger();
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
            get
            {
                return userSettings.CfgShowToolPanel == true;
            }
            set
            {
                userSettings.CfgShowToolPanel = value;
                SaveSettingsLater();
            }
        }

        public const int maxLogLines = 1000;

        #endregion

        #region public methods
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
            var result = userSettings.V2RayCoreDownloadVersionList ??
                new List<string> { "v4.27.5", "v4.27.0", "v4.26.0", "v4.25.1" };
            return result.AsReadOnly();
        }

        // ISettingService thing
        bool isClosing = false;
        public bool IsClosing() => isClosing;

        public bool SetIsClosing(bool isClosing) => this.isClosing = isClosing;

        /// <summary>
        /// return null if fail
        /// </summary>
        /// <param name="pluginName"></param>
        /// <returns></returns>
        public string GetPluginsSetting(string pluginName)
        {
            var pluginsSetting = DeserializePluginsSetting();

            if (pluginsSetting != null
                && pluginsSetting.ContainsKey(pluginName))
            {
                return pluginsSetting[pluginName];
            }
            return null;
        }

        static object pluginsSettingWriteLock = new object();
        public void SavePluginsSetting(string pluginName, string value)
        {
            if (string.IsNullOrEmpty(pluginName))
            {
                return;
            }

            lock (pluginsSettingWriteLock)
            {
                var pluginsSetting = DeserializePluginsSetting();
                pluginsSetting[pluginName] = value;

                try
                {
                    userSettings.PluginsSetting = string.Empty;
                    userSettings.CompressedPluginsSetting = VgcApis.Libs.Infr.ZipExtensions
                        .SerializeObjectToCompressedBase64(pluginsSetting);
                }
                catch { }
            }
            SaveSettingsLater();
        }

        public void SaveUserSettingsNow() => SaveUserSettingsWorker();

        public void LazyGC() => janitor?.Deadline();

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
                result = JsonConvert
                    .DeserializeObject<Models.Datas.ServerTracker>(
                        userSettings.ServerTracker);
                if (result != null && result.serverList == null)
                {
                    result.serverList = new List<string>();
                }
            }
            catch
            {
                return empty;
            }
            return result ?? empty;
        }

        public List<VgcApis.Models.Datas.CoreInfo> LoadCoreInfoList()
        {
            List<VgcApis.Models.Datas.CoreInfo> coreInfos = null;

            try
            {
                var cs = userSettings.CompressedCoreInfoList;
                if (string.IsNullOrEmpty(cs))
                {
                    coreInfos = JsonConvert
                        .DeserializeObject<List<VgcApis.Models.Datas.CoreInfo>>(
                            userSettings.CoreInfoList);
                }
                else
                {
                    coreInfos = VgcApis.Libs.Infr.ZipExtensions
                        .DeserializeObjectFromCompressedBase64<List<VgcApis.Models.Datas.CoreInfo>>(
                            cs);
                }

            }
            catch { }

            if (coreInfos == null)
            {
                return new List<VgcApis.Models.Datas.CoreInfo>();
            }

            // make sure every config of server can be parsed correctly
            var result = coreInfos.Where(c =>
             {
                 try
                 {
                     return JObject.Parse(c.config) != null;
                 }
                 catch { }
                 return false;
             }).ToList();

            return result;
        }

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

        public List<Models.Datas.MultiConfItem> GetMultiConfItems()
        {
            try
            {
                var items = JsonConvert
                    .DeserializeObject<List<Models.Datas.MultiConfItem>>(
                        userSettings.MultiConfItems);

                if (items != null)
                {
                    return items;
                }
            }
            catch { };
            return new List<Models.Datas.MultiConfItem>();
        }

        public void SaveMultiConfItems(string options)
        {
            userSettings.MultiConfItems = options;
            SaveSettingsLater();
        }

        public List<Models.Datas.ImportItem> GetGlobalImportItems()
        {
            try
            {
                var items = JsonConvert
                    .DeserializeObject<List<Models.Datas.ImportItem>>(
                        userSettings.ImportUrls);

                if (items != null)
                {
                    return items;
                }
            }
            catch { };
            return new List<Models.Datas.ImportItem>();
        }

        public void SaveGlobalImportItems(string options)
        {
            userSettings.ImportUrls = options;
            SaveSettingsLater();
        }

        public List<Models.Datas.PluginInfoItem> GetPluginInfoItems()
        {
            try
            {
                var items = JsonConvert
                    .DeserializeObject<List<Models.Datas.PluginInfoItem>>(
                        userSettings.PluginInfoItems);

                if (items != null)
                {
                    return items;
                }
            }
            catch { };
            return new List<Models.Datas.PluginInfoItem>();
        }

        /// <summary>
        /// Feel free to pass null.
        /// </summary>
        /// <param name="itemList"></param>
        public void SavePluginInfoItems(
            List<Models.Datas.PluginInfoItem> itemList)
        {
            string json = JsonConvert.SerializeObject(
                itemList ?? new List<Models.Datas.PluginInfoItem>());

            userSettings.PluginInfoItems = json;
            SaveSettingsLater();
        }

        public string GetSubscriptionConfig() => userSettings.SubscribeUrls;

        public List<Models.Datas.SubscriptionItem> GetSubscriptionItems()
        {
            try
            {
                var items = JsonConvert
                    .DeserializeObject<List<Models.Datas.SubscriptionItem>>(
                        userSettings.SubscribeUrls);

                if (items != null)
                {
                    return items;
                }
            }
            catch { };
            return new List<Models.Datas.SubscriptionItem>();
        }

        public void SetSubscriptionConfig(string cfgStr)
        {
            userSettings.SubscribeUrls = cfgStr;
            SaveSettingsLater();
        }

        public void SaveServerList(List<VgcApis.Models.Datas.CoreInfo> coreInfoList)
        {
            var cil = coreInfoList ?? new List<VgcApis.Models.Datas.CoreInfo>();
            string cs = VgcApis.Libs.Infr.ZipExtensions.SerializeObjectToCompressedBase64(cil);

            userSettings.CoreInfoList = string.Empty; // obsolete
            userSettings.CompressedCoreInfoList = cs;

            SaveSettingsLater();
        }
        #endregion

        #region private method
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
            string serializedUserSettings = "";
            try
            {
                serializedUserSettings = JsonConvert.SerializeObject(userSettings);
                if (userSettings.isPortable)
                {
                    // DebugSendLog("Try save settings to file.");
                    SaveUserSettingsToFile(serializedUserSettings);
                }
                else
                {
                    // DebugSendLog("Try save settings to properties");
                    SetUserSettingFileIsPortableToFalse();
                    SaveUserSettingsToProperties(serializedUserSettings);
                    VgcApis.Libs.Sys.FileLogger.Info("Settings.SaveUserSettingsToProperties() done");
                }
                // VgcApis.Libs.Sys.FileLogger.Info("Settings.SaveUserSettingsWorker() done");
                return;
            }
            catch { }

            VgcApis.Libs.Sys.FileLogger.Info("Settings.SaveUserSettingsWorker() error");
            if (GetShutdownReason() == VgcApis.Models.Datas.Enums.ShutdownReasons.CloseByUser)
            {
                SendLog("UserSettings: " + Environment.NewLine + serializedUserSettings);
                throw new ArgumentException("Validate serialized user settings fail!");
            }
        }

        void UpdateSpeedTestPool()
        {
            var poolSize = userSettings.MaxConcurrentV2RayCoreNum;
            _speedTestPool = new SemaphoreSlim(poolSize, poolSize);
        }

        Dictionary<string, string> DeserializePluginsSetting()
        {
            var empty = new Dictionary<string, string>();
            Dictionary<string, string> pluginsSetting = null;

            try
            {
                if (string.IsNullOrEmpty(userSettings.CompressedPluginsSetting))
                {
                    pluginsSetting = JsonConvert
                        .DeserializeObject<Dictionary<string, string>>(
                            userSettings.PluginsSetting);
                }
                else
                {
                    pluginsSetting = VgcApis.Libs.Infr.ZipExtensions
                        .DeserializeObjectFromCompressedBase64<Dictionary<string, string>>(
                            userSettings.CompressedPluginsSetting);
                }
            }
            catch { }
            if (pluginsSetting == null)
            {
                pluginsSetting = empty;
            }

            return pluginsSetting;
        }

        void SetUserSettingFileIsPortableToFalse()
        {
            DebugSendLog("Read user setting file");

            var mainUsFilename = Constants.Strings.MainUserSettingsFilename;
            var bakUsFilename = Constants.Strings.BackupUserSettingsFilename;
            if (!File.Exists(mainUsFilename) && !File.Exists(bakUsFilename))
            {
                DebugSendLog("setting file not exists");
                return;
            }

            DebugSendLog("set portable to false");
            userSettings.isPortable = false;
            try
            {
                var serializedUserSettings = JsonConvert.SerializeObject(userSettings);
                lock (saveUserSettingsLocker)
                {
                    VgcApis.Misc.Utils.ClumsyWriter(
                        serializedUserSettings,
                        mainUsFilename,
                        bakUsFilename);
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

        void SaveUserSettingsToProperties(string content)
        {
            try
            {
                Properties.Settings.Default.UserSettings = content;
                Properties.Settings.Default.Save();
            }
            catch
            {
                DebugSendLog("Save user settings to Properties fail!");
            }
        }

        void SaveUserSettingsToFile(string content)
        {
            var hash = VgcApis.Misc.Utils.Md5Base64(content);
            if (hash == lastUserSettingsHash)
            {
                VgcApis.Libs.Sys.FileLogger.Info("Settings.SaverUserSettingsToFile() no change, skip");
                return;
            }

            VgcApis.Libs.Sys.FileLogger.Info("Settings.SaverUserSettingsToFile() write file");

            lock (saveUserSettingsLocker)
            {
                var ok = VgcApis.Misc.Utils.ClumsyWriter(
                    content,
                    Constants.Strings.MainUserSettingsFilename,
                    Constants.Strings.BackupUserSettingsFilename);
                if (ok)
                {
                    lastUserSettingsHash = hash;
                    VgcApis.Libs.Sys.FileLogger.Info("Settings.SaverUserSettingsToFile() success");
                    return;
                }
            }

            VgcApis.Libs.Sys.FileLogger.Error("Settings.SaverUserSettingsToFile() failed");
            // main file or bak file write fail, clear cache
            lastUserSettingsHash = @"";
            WarnUserSaveSettingsFailed(content);
        }

        private void WarnUserSaveSettingsFailed(string content)
        {
            var msg = I18N.SaveUserSettingsToFileFail;

            if (isClosing)
            {
                // 兄弟只能帮你到这了
                VgcApis.Libs.Sys.NotepadHelper.ShowMessage(content, Properties.Resources.PortableUserSettingsFilename);
                msg += Environment.NewLine + string.Format(I18N.AndThenSaveThisFileAs, Properties.Resources.PortableUserSettingsFilename);
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
                var us = JsonConvert.DeserializeObject<Models.Datas.UserSettings>(serializedUserSettings);
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
                var content = File.ReadAllText(Constants.Strings.MainUserSettingsFilename);
                lastUserSettingsHash = VgcApis.Misc.Utils.Md5Base64(content);
                result = JsonConvert.DeserializeObject<Models.Datas.UserSettings>(content);
            }
            catch { }

            // try to load userSettings.bak
            if (result == null)
            {
                result = VgcApis.Misc.Utils.LoadAndParseJsonFile<Models.Datas.UserSettings>(
                    Constants.Strings.BackupUserSettingsFilename);
            }

            if (result != null && result.isPortable)
            {
                return result;
            }
            return null;
        }

        Models.Datas.UserSettings LoadUserSettings()
        {
            var mainUsFile = Constants.Strings.MainUserSettingsFilename;
            var bakUsFile = Constants.Strings.BackupUserSettingsFilename;

            var result = LoadUserSettingsFromFile() ?? LoadUserSettingsFromPorperties();
            if (result == null
                && (File.Exists(mainUsFile) || File.Exists(bakUsFile))
                && !Misc.UI.Confirm(I18N.ConfirmLoadDefaultUserSettings))
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
                winFormRectListCache = JsonConvert
                    .DeserializeObject<Dictionary<string, Rectangle>>(
                        userSettings.WinFormPosList);
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
            VgcApis.Libs.Sys.FileLogger.Info("Settings.Cleanup() begin");
            lazyBookKeeper?.Dispose();
            SaveUserSettingsNow();
            janitor?.Dispose();
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
