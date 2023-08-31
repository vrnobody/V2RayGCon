using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using VgcApis.Interfaces.Services;

namespace Luna.Services
{
    internal class Settings :
        VgcApis.BaseClasses.Disposable

    {
        ISettingsService vgcSetting;

        readonly string pluginName = Properties.Resources.Name;
        Models.Data.UserSettings userSettings;

        VgcApis.Libs.Tasks.LazyGuy lazyBookKeeper;


        public Settings()
        {
            lazyBookKeeper = new VgcApis.Libs.Tasks.LazyGuy(
                SaveUserSettingsNow,
                VgcApis.Models.Consts.Intervals.LazySaveLunaSettingsInterval,
                3000);
        }

        #region properties
        bool _isDisposing = false;
        public void SetIsDisposing(bool value) => _isDisposing = value;

        public bool isScreenLocked => vgcSetting.IsScreenLocked();


        #endregion


        #region public methods

        public void SendLog(string contnet)
        {
            var name = Properties.Resources.Name;
            vgcSetting.SendLog(string.Format("[{0}] {1}", name, contnet));
        }

        public bool IsClosing() => _isDisposing || vgcSetting.IsClosing();

        public void Run(ISettingsService vgcSetting)
        {
            this.vgcSetting = vgcSetting;

            userSettings = VgcApis.Misc.Utils
                .LoadPluginSetting<Models.Data.UserSettings>(
                    pluginName, vgcSetting);

            userSettings.NormalizeData();

            MigrateLocalStorageToVgcSettings(vgcSetting);
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

        public List<Models.Data.LuaCoreSetting> GetLuaCoreSettings() =>
            userSettings.luaServers;

        public void SaveUserSettingsLater() =>
            lazyBookKeeper?.Deadline();
        #endregion

        #region protected methods
        protected override void Cleanup()
        {
            lazyBookKeeper?.Dispose();
            SaveUserSettingsNow();
        }
        #endregion

        #region private methods
        private void MigrateLocalStorageToVgcSettings(ISettingsService vgcSetting)
        {
            if (userSettings.luaShareMemory.Count <= 0)
            {
                return;
            }

            // 如果以前有存数据就复制进新的位置
            foreach (var kv in userSettings.luaShareMemory)
            {
                vgcSetting.WriteLocalStorage(kv.Key, kv.Value);
            }
            userSettings.luaShareMemory.Clear();
        }

        void SaveUserSettingsNow() =>
            VgcApis.Misc.Utils.SavePluginSetting(
                pluginName, userSettings, vgcSetting);

        #endregion


    }
}
