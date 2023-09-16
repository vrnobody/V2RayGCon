using System.Collections.Generic;
using VgcApis.Interfaces.Services;

namespace NeoLuna.Services
{
    internal class Settings : VgcApis.BaseClasses.Disposable
    {
        ISettingsService vgcSetting;

        readonly string pluginName = Properties.Resources.Name;
        Models.Data.UserSettings userSettings;
        readonly VgcApis.Libs.Tasks.LazyGuy lazyBookKeeper;

        readonly VgcApis.Libs.Sys.QueueLogger qLogger = new VgcApis.Libs.Sys.QueueLogger();

        public Settings()
        {
            lazyBookKeeper = new VgcApis.Libs.Tasks.LazyGuy(
                SaveUserSettingsNow,
                VgcApis.Models.Consts.Intervals.LazySaveLunaSettingsInterval,
                3000
            );
        }

        #region properties
        bool _isDisposing = false;

        public void SetIsDisposing(bool value) => _isDisposing = value;

        public bool isScreenLocked => vgcSetting.IsScreenLocked();

        #endregion


        #region public methods
        public VgcApis.Libs.Sys.QueueLogger GetLogger() => qLogger;

        public void SendLog(string content)
        {
            var name = Properties.Resources.Name;
            qLogger.Log(content);
            vgcSetting.SendLog(string.Format("[{0}] {1}", name, content));
        }

        public bool IsClosing() => _isDisposing || vgcSetting.IsClosing();

        public void Run(ISettingsService vgcSetting)
        {
            this.vgcSetting = vgcSetting;

            userSettings = VgcApis.Misc.Utils.LoadPluginSetting<Models.Data.UserSettings>(
                pluginName,
                vgcSetting
            );

            userSettings.NormalizeData();
        }

        public string GetLuaShareMemory(string key)
        {
            return vgcSetting.ReadLocalStorage(key);
        }

        public bool RemoveShareMemory(string key)
        {
            return vgcSetting.RemoveLocalStorage(key);
        }

        public List<string> ShareMemoryKeys()
        {
            return vgcSetting.GetLocalStorageKeys();
        }

        public void SetLuaShareMemory(string key, string value)
        {
            vgcSetting.WriteLocalStorage(key, value);
        }

        public List<Models.Data.LuaCoreSetting> GetLuaCoreSettings() => userSettings.luaServers;

        public void SaveUserSettingsLater() => lazyBookKeeper?.Deadline();

        #endregion

        #region protected methods
        protected override void Cleanup()
        {
            lazyBookKeeper?.Dispose();
            SaveUserSettingsNow();
        }
        #endregion

        #region private methods
        void SaveUserSettingsNow() =>
            VgcApis.Misc.Utils.SavePluginSetting(pluginName, userSettings, vgcSetting);

        #endregion
    }
}
