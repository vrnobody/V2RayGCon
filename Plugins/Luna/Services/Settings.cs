using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using VgcApis.Interfaces.Services;

namespace Luna.Services
{
    internal class Settings :
        VgcApis.BaseClasses.Disposable

    {
        VgcApis.Interfaces.Services.ISettingsService vgcSetting;

        readonly string pluginName = Properties.Resources.Name;
        Models.Data.UserSettings userSettings;
        Dictionary<string, string> localStorage;

        VgcApis.Libs.Tasks.LazyGuy lazyBookKeeper, lazyRecorder;


        public Settings()
        {
            lazyRecorder = new VgcApis.Libs.Tasks.LazyGuy(
                SaveLocalStorageNow,
                VgcApis.Models.Consts.Intervals.LazySaveLunaSettingsInterval,
                3000);

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

        public void Run(
            VgcApis.Interfaces.Services.ISettingsService vgcSetting)
        {
            this.vgcSetting = vgcSetting;

            userSettings = VgcApis.Misc.Utils
                .LoadPluginSetting<Models.Data.UserSettings>(
                    pluginName, vgcSetting);

            userSettings.NormalizeData();

            InitLocalStorage(vgcSetting);
        }

        public string GetLuaShareMemory(string key)
        {
            if (string.IsNullOrEmpty(key) || !localStorage.ContainsKey(key))
            {
                return @"";
            }
            return localStorage[key];
        }

        readonly object shareMemoryLocker = new object();
        public bool RemoveShareMemory(string key)
        {
            if (string.IsNullOrEmpty(key) || !localStorage.ContainsKey(key))
            {
                return false;
            }

            bool success;
            lock (shareMemoryLocker)
            {
                success = localStorage.Remove(key);
            }
            SaveLocalStorageLater();
            return success;
        }

        public List<string> ShareMemoryKeys()
        {
            lock (shareMemoryLocker)
            {
                return localStorage.Keys.ToList();
            }
        }

        public void SetLuaShareMemory(string key, string value)
        {
            if (string.IsNullOrEmpty(key))
            {
                return;
            }

            lock (shareMemoryLocker)
            {
                localStorage[key] = value;
            }
            SaveLocalStorageLater();
        }

        public List<Models.Data.LuaCoreSetting> GetLuaCoreSettings() =>
            userSettings.luaServers;

        public void SaveUserSettingsLater() =>
            lazyBookKeeper?.Deadline();

        public void SaveLocalStorageLater() =>
            lazyRecorder?.Deadline();
        #endregion

        #region protected methods
        protected override void Cleanup()
        {
            lazyRecorder?.Dispose();
            lazyBookKeeper?.Dispose();

            SaveLocalStorageNow();
            SaveUserSettingsNow();

        }
        #endregion

        #region private methods
        private void InitLocalStorage(ISettingsService vgcSetting)
        {
            Dictionary<string, string> storage = null;
            try
            {
                var compressedStr = vgcSetting.GetLocalStorage();
                storage = VgcApis.Libs.Infr.ZipExtensions.DeserializeObjectFromCompressedUnicodeBase64<Dictionary<string, string>>(compressedStr);
            }
            catch { }

            if (storage == null || storage.Count < 1)
            {
                storage = userSettings.luaShareMemory;
            }

            localStorage = storage ?? new Dictionary<string, string>();
            userSettings.luaShareMemory = new Dictionary<string, string>();
        }

        void SaveLocalStorageNow()
        {
            var str = VgcApis.Libs.Infr.ZipExtensions
                        .SerializeObjectToCompressedUnicodeBase64(localStorage);
            vgcSetting.SaveLocalStorage(str);
        }

        void SaveUserSettingsNow() =>
            VgcApis.Misc.Utils.SavePluginSetting(
                pluginName, userSettings, vgcSetting);

        #endregion


    }
}
