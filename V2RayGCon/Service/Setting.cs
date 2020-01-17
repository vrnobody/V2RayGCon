using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using V2RayGCon.Resource.Resx;

namespace V2RayGCon.Service
{
    public class Setting :
        Model.BaseClass.SingletonService<Setting>,
        VgcApis.Models.IServices.ISettingsService
    {
        Model.Data.UserSettings userSettings;

        string serializedUserSettingsCache = @"";

        // Singleton need this private ctor.
        Setting()
        {
            userSettings = LoadUserSettings();
            userSettings.Normalized();  // replace null with empty object.
        }

        #region Properties
        public string AllPluginsSetting
        {
            get => userSettings.PluginsSetting;
            set
            {
                userSettings.PluginsSetting = value;
                LazySaveUserSettings();
            }
        }

        public VgcApis.Models.Datas.Enum.ShutdownReasons ShutdownReason { get; set; } =
            VgcApis.Models.Datas.Enum.ShutdownReasons.CloseByUser;

        public bool isDownloadWin32V2RayCore
        {
            get => userSettings.isDownloadWin32V2RayCore;
            set
            {
                userSettings.isDownloadWin32V2RayCore = value;
                LazySaveUserSettings();
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
                LazySaveUserSettings();
            }
        }

        public bool isEnableStatistics
        {
            get => userSettings.isEnableStat;
            set
            {
                userSettings.isEnableStat = value;
                LazySaveUserSettings();
            }
        }

        public bool isUseV4
        {
            get => userSettings.isUseV4Format;
            set
            {
                userSettings.isUseV4Format = value;
                LazySaveUserSettings();
            }
        }

        public bool CustomDefImportGlobalImport
        {
            get => userSettings.ImportOptions.IsInjectGlobalImport;
            set
            {
                userSettings.ImportOptions.IsInjectGlobalImport = value;
                LazySaveUserSettings();
            }
        }

        public bool CustomDefImportBypassCnSite
        {
            get => userSettings.ImportOptions.IsBypassCnSite;
            set
            {
                userSettings.ImportOptions.IsBypassCnSite = value;
                LazySaveUserSettings();
            }
        }

        public bool CustomDefImportIsFold
        {
            get => userSettings.ImportOptions.IsFold;
            set
            {
                userSettings.ImportOptions.IsFold = value;
                LazySaveUserSettings();
            }
        }

        public bool CustomDefImportSsShareLink
        {
            get => userSettings.ImportOptions.IsImportSsShareLink;
            set
            {
                userSettings.ImportOptions.IsImportSsShareLink = value;
                LazySaveUserSettings();
            }
        }

        public int CustomDefImportMode
        {
            get => VgcApis.Libs.Utils.Clamp(userSettings.ImportOptions.Mode, 0, 4);
            set
            {
                userSettings.ImportOptions.Mode = VgcApis.Libs.Utils.Clamp(value, 0, 4);
                LazySaveUserSettings();
            }
        }

        public string CustomDefImportIp
        {
            get => userSettings.ImportOptions.Ip;
            set
            {
                userSettings.ImportOptions.Ip = value;
                LazySaveUserSettings();
            }
        }

        public int CustomDefImportPort
        {
            get => userSettings.ImportOptions.Port;
            set
            {
                userSettings.ImportOptions.Port = value;
                LazySaveUserSettings();
            }
        }

        public string CustomSpeedtestUrl
        {
            get => userSettings.SpeedtestOptions.Url;
            set
            {
                userSettings.SpeedtestOptions.Url = value;
                LazySaveUserSettings();
            }
        }

        public int CustomSpeedtestTimeout
        {
            get => userSettings.SpeedtestOptions.Timeout;
            set
            {
                userSettings.SpeedtestOptions.Timeout = value;
                LazySaveUserSettings();
            }
        }

        public int CustomSpeedtestExpectedSizeInKib
        {
            get => userSettings.SpeedtestOptions.ExpectedSize;
            set
            {
                userSettings.SpeedtestOptions.ExpectedSize = value;
                LazySaveUserSettings();
            }
        }

        public int CustomSpeedtestCycles
        {
            get => userSettings.SpeedtestOptions.Cycles;
            set
            {
                userSettings.SpeedtestOptions.Cycles = value;
                LazySaveUserSettings();
            }
        }

        public bool isUseCustomSpeedtestSettings
        {
            get => userSettings.SpeedtestOptions.IsUse;
            set
            {
                userSettings.SpeedtestOptions.IsUse = value;
                LazySaveUserSettings();
            }
        }

        public bool isUpdateUseProxy
        {
            get => userSettings.isUpdateUseProxy;
            set
            {
                userSettings.isUpdateUseProxy = value;
                LazySaveUserSettings();
            }
        }

        public bool isCheckUpdateWhenAppStart
        {
            get => userSettings.isCheckUpdateWhenAppStart;
            set
            {
                userSettings.isCheckUpdateWhenAppStart = value;
                LazySaveUserSettings();
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
                LazySaveUserSettings();
            }
        }

        public bool isServerTrackerOn = false;

        public int serverPanelPageSize
        {
            get
            {
                var size = userSettings.ServerPanelPageSize;
                return Lib.Utils.Clamp(size, 1, 101);
            }
            set
            {
                userSettings.ServerPanelPageSize = Lib.Utils.Clamp(value, 1, 101);
                LazySaveUserSettings();
            }
        }

        public int maxConcurrentV2RayCoreNum
        {
            get
            {
                var size = userSettings.MaxConcurrentV2RayCoreNum;
                return Lib.Utils.Clamp(size, 10, 1001);
            }
            set
            {
                userSettings.MaxConcurrentV2RayCoreNum = Lib.Utils.Clamp(value, 10, 1001);
                LazySaveUserSettings();
            }
        }

        public CultureInfo orgCulture = null;

        VgcApis.Libs.Sys.QueueLogger qLogger = new VgcApis.Libs.Sys.QueueLogger();
        public long GetLogTimestamp() => qLogger.GetTimestamp();
        public string GetLogContent() => qLogger.GetLogAsString(true);
        public void SendLog(string log) => qLogger.Log(log);

        public Model.Data.Enum.Cultures culture
        {
            get
            {
                var cultures = Model.Data.Table.Cultures;
                var c = userSettings.Culture;

                if (!cultures.ContainsValue(c))
                {
                    return Model.Data.Enum.Cultures.auto;
                }

                return cultures.Where(s => s.Value == c).First().Key;
            }

            set
            {
                var cultures = Model.Data.Table.Cultures;
                var c = Model.Data.Enum.Cultures.auto;
                if (cultures.ContainsKey(value))
                {
                    c = value;
                }
                userSettings.Culture = Model.Data.Table.Cultures[c];
                LazySaveUserSettings();
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
                LazySaveUserSettings();
            }
        }

        public const int maxLogLines = 1000;

        #endregion

        #region public methods
        public void SaveV2RayCoreVersionList(List<string> versions)
        {
            // clone version list
            userSettings.V2RayCoreDownloadVersionList = new List<string>(versions);
            LazySaveUserSettings();
        }

        public ReadOnlyCollection<string> GetV2RayCoreVersionList()
        {
            var result = userSettings.V2RayCoreDownloadVersionList ??
                new List<string> { "v3.48", "v3.47", "v3.46" };
            return result.AsReadOnly();
        }

        // ISettingService thing
        bool isClosing = false;
        public bool IsClosing() => isClosing;
        public bool SetIsShutdown(bool isShutdown) => this.isClosing = isShutdown;

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

        public void SavePluginsSetting(string pluginName, string value)
        {
            if (string.IsNullOrEmpty(pluginName))
            {
                return;
            }

            var pluginsSetting = DeserializePluginsSetting();
            pluginsSetting[pluginName] = value;

            try
            {
                userSettings.PluginsSetting =
                    JsonConvert.SerializeObject(pluginsSetting);
            }
            catch { }
            LazySaveUserSettings();
        }

        VgcApis.Libs.Tasks.Bar saveUserSettingsBar = new VgcApis.Libs.Tasks.Bar();
        public void SaveUserSettingsNow()
        {
            if (!saveUserSettingsBar.Install())
            {
                return;
            }

            var serializedUserSettings = JsonConvert.SerializeObject(userSettings);
            if (ValidateSerializedUserSettings(serializedUserSettings))
            {
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
                }
            }
            else
            {
                if (ShutdownReason == VgcApis.Models.Datas.Enum.ShutdownReasons.CloseByUser)
                {
                    SendLog("UserSettings: " + Environment.NewLine + serializedUserSettings);
                    throw new ArgumentException("Validate serialized user settings fail!");
                }
            }
            saveUserSettingsBar.Remove();
        }

        /*
         * string something;
         * if(something == null){} // Boom!
         */
        Lib.Sys.CancelableTimeout lazyGCTimer = null;
        public void LazyGC()
        {
            // Create on demand.
            if (lazyGCTimer == null)
            {
                lazyGCTimer = new Lib.Sys.CancelableTimeout(
                    () => GC.Collect(),
                    VgcApis.Models.Consts.Intervals.LazyGcDelay);
            }

            lazyGCTimer.Start();
        }

        public void SaveServerTrackerSetting(Model.Data.ServerTracker serverTrackerSetting)
        {
            userSettings.ServerTracker =
                JsonConvert.SerializeObject(serverTrackerSetting);
            LazySaveUserSettings();
        }

        public Model.Data.ServerTracker GetServerTrackerSetting()
        {
            var empty = new Model.Data.ServerTracker();
            Model.Data.ServerTracker result;
            try
            {
                result = JsonConvert
                    .DeserializeObject<Model.Data.ServerTracker>(
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
                coreInfos = JsonConvert
                    .DeserializeObject<List<VgcApis.Models.Datas.CoreInfo>>(
                        userSettings.CoreInfoList);
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
            LazySaveUserSettings();
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
            form.Left = Lib.Utils.Clamp(rect.Left, 0, screen.Right - form.Width);
            form.Top = Lib.Utils.Clamp(rect.Top, 0, screen.Bottom - form.Height);
        }

        public List<Model.Data.ImportItem> GetGlobalImportItems()
        {
            try
            {
                var items = JsonConvert
                    .DeserializeObject<List<Model.Data.ImportItem>>(
                        userSettings.ImportUrls);

                if (items != null)
                {
                    return items;
                }
            }
            catch { };
            return new List<Model.Data.ImportItem>();
        }

        public void SaveGlobalImportItems(string options)
        {
            userSettings.ImportUrls = options;
            LazySaveUserSettings();
        }

        public List<Model.Data.PluginInfoItem> GetPluginInfoItems()
        {
            try
            {
                var items = JsonConvert
                    .DeserializeObject<List<Model.Data.PluginInfoItem>>(
                        userSettings.PluginInfoItems);

                if (items != null)
                {
                    return items;
                }
            }
            catch { };
            return new List<Model.Data.PluginInfoItem>();
        }

        /// <summary>
        /// Feel free to pass null.
        /// </summary>
        /// <param name="itemList"></param>
        public void SavePluginInfoItems(
            List<Model.Data.PluginInfoItem> itemList)
        {
            string json = JsonConvert.SerializeObject(
                itemList ?? new List<Model.Data.PluginInfoItem>());

            userSettings.PluginInfoItems = json;
            LazySaveUserSettings();
        }

        public List<Model.Data.SubscriptionItem> GetSubscriptionItems()
        {
            try
            {
                var items = JsonConvert
                    .DeserializeObject<List<Model.Data.SubscriptionItem>>(
                        userSettings.SubscribeUrls);

                if (items != null)
                {
                    return items;
                }
            }
            catch { };
            return new List<Model.Data.SubscriptionItem>();
        }

        public void SaveSubscriptionItems(string options)
        {
            userSettings.SubscribeUrls = options;
            LazySaveUserSettings();
        }

        public void SaveServerList(List<VgcApis.Models.Datas.CoreInfo> coreInfoList)
        {
            string json = JsonConvert.SerializeObject(
                coreInfoList ?? new List<VgcApis.Models.Datas.CoreInfo>());

            userSettings.CoreInfoList = json;
            LazySaveUserSettings();
        }
        #endregion

        #region private method
        bool ValidateSerializedUserSettings(string serializedUserSettings)
        {
            if (string.IsNullOrEmpty(serializedUserSettings))
            {
                return false;
            }
            try
            {
                var json = JsonConvert.DeserializeObject<Model.Data.UserSettings>(
                    serializedUserSettings);
                if (json != null)
                {
                    return true;
                }
            }
            catch { }
            return false;
        }


        Dictionary<string, string> DeserializePluginsSetting()
        {
            var empty = new Dictionary<string, string>();
            Dictionary<string, string> pluginsSetting = null;

            try
            {
                pluginsSetting = JsonConvert
                    .DeserializeObject<Dictionary<string, string>>(
                        userSettings.PluginsSetting);
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
                File.WriteAllText(mainUsFilename, serializedUserSettings);
                File.WriteAllText(bakUsFilename, serializedUserSettings);
                DebugSendLog("set portable option done");
                return;
            }
            catch { }

            if (ShutdownReason == VgcApis.Models.Datas.Enum.ShutdownReasons.CloseByUser)
            {
                // this is important do not use task
                var msg = string.Format(I18N.UnsetPortableModeFail, mainUsFilename);
                MessageBox.Show(msg);
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
            if (content.Equals(serializedUserSettingsCache))
            {
                VgcApis.Libs.Sys.FileLogger.Info("User settings equal to cache, skip.");
                return;
            }

            VgcApis.Libs.Sys.FileLogger.Info("Write user settings to file.");
            if (VgcApis.Libs.Utils.ClumsyWriter(
                content,
                Constants.Strings.MainUserSettingsFilename,
                Constants.Strings.BackupUserSettingsFilename))
            {
                serializedUserSettingsCache = content;
                return;
            }

            // main file or bak file write fail, clear cache
            serializedUserSettingsCache = @"";

            if (ShutdownReason == VgcApis.Models.Datas.Enum.ShutdownReasons.CloseByUser)
            {
                var msg = I18N.SaveUserSettingsToFileFail;
                if (isClosing)
                {
                    // 兄弟只能帮你到这了
                    VgcApis.Libs.Sys.NotepadHelper.ShowMessage(content, Properties.Resources.PortableUserSettingsFilename);
                    msg += Environment.NewLine + string.Format(I18N.AndThenSaveThisFileAs, Properties.Resources.PortableUserSettingsFilename);
                }

                // this is important do not use task!
                msg += Environment.NewLine + I18N.OrDisablePortableMode;
                MessageBox.Show(msg);
            }
        }

        Model.Data.UserSettings LoadUserSettingsFromPorperties()
        {
            try
            {
                var serializedUserSettings = Properties.Settings.Default.UserSettings;
                var us = JsonConvert.DeserializeObject<Model.Data.UserSettings>(serializedUserSettings);
                if (us != null)
                {
                    DebugSendLog("Read user settings from Properties.Usersettings");
                    return us;
                }
            }
            catch { }

            return null;
        }

        Model.Data.UserSettings LoadUserSettingsFromFile()
        {
            // try to load userSettings.json
            Model.Data.UserSettings result = null;
            try
            {
                var content = File.ReadAllText(Constants.Strings.MainUserSettingsFilename);
                serializedUserSettingsCache = content;
                result = JsonConvert.DeserializeObject<Model.Data.UserSettings>(content);
            }
            catch { }

            // try to load userSettings.bak
            if (result == null)
            {
                result = VgcApis.Libs.Utils.LoadAndParseJsonFile<Model.Data.UserSettings>(
                    Constants.Strings.BackupUserSettingsFilename);
            }

            if (result != null && result.isPortable)
            {
                return result;
            }
            return null;
        }

        Model.Data.UserSettings LoadUserSettings()
        {
            var mainUsFile = Constants.Strings.MainUserSettingsFilename;
            var bakUsFile = Constants.Strings.BackupUserSettingsFilename;

            var result = LoadUserSettingsFromFile() ?? LoadUserSettingsFromPorperties();
            if (result == null
                && (File.Exists(mainUsFile) || File.Exists(bakUsFile))
                && !Lib.UI.Confirm(I18N.ConfirmLoadDefaultUserSettings))
            {
                ShutdownReason = VgcApis.Models.Datas.Enum.ShutdownReasons.Abort;
            }

            return result ?? new Model.Data.UserSettings();
        }

        Lib.Sys.CancelableTimeout lazySaveUserSettingsTimer = null;
        void LazySaveUserSettings()
        {
            if (lazySaveUserSettingsTimer == null)
            {
                lazySaveUserSettingsTimer = new Lib.Sys.CancelableTimeout(
                    SaveUserSettingsNow,
                    VgcApis.Models.Consts.Intervals.LazySaveUserSettingsDelay);
            }

            lazySaveUserSettingsTimer.Start();
        }

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
            lazyGCTimer?.Release();
            lazySaveUserSettingsTimer?.Release();
            SaveUserSettingsNow();
            qLogger.Dispose();
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
