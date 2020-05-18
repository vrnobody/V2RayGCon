using System.Collections.Generic;
using System.Linq;

namespace Luna.Services
{
    internal class Settings :
        VgcApis.BaseClasses.Disposable

    {
        VgcApis.Interfaces.Services.ISettingsService vgcSetting;

        readonly string pluginName = Properties.Resources.Name;
        Models.Data.UserSettings userSettings;
        Libs.LuaSnippet.SnippetsCache snpCache;

        public Settings() { }

        #region properties
        public bool isScreenLocked => vgcSetting.IsScreenLocked();


        #endregion

        #region internal methods

        public Libs.LuaSnippet.BestMatchSnippets CreateBestMatchSnippet(ScintillaNET.Scintilla editor)
            => snpCache?.CreateBestMatchSnippets(editor);

        #endregion

        #region public methods

        public void SendLog(string contnet)
        {
            var name = Properties.Resources.Name;
            vgcSetting.SendLog(string.Format("[{0}] {1}", name, contnet));
        }

        bool isDisposing = false;
        public bool IsClosing() => isDisposing || vgcSetting.IsClosing();

        public void SetIsDisposing(bool value) => isDisposing = value;

        public void Run(
            VgcApis.Interfaces.Services.ISettingsService vgcSetting)
        {
            this.vgcSetting = vgcSetting;

            this.snpCache = new Libs.LuaSnippet.SnippetsCache();

            userSettings = VgcApis.Misc.Utils
                .LoadPluginSetting<Models.Data.UserSettings>(
                    pluginName, vgcSetting);

            userSettings.NormalizeData();
        }

        public string GetLuaShareMemory(string key)
        {
            if (string.IsNullOrEmpty(key) || !userSettings.luaShareMemory.ContainsKey(key))
            {
                return @"";
            }
            return userSettings.luaShareMemory[key];
        }

        readonly object shareMemoryLocker = new object();
        public bool RemoveShareMemory(string key)
        {
            if (string.IsNullOrEmpty(key)
                || !userSettings.luaShareMemory.ContainsKey(key))
            {
                return false;
            }

            bool success;
            lock (shareMemoryLocker)
            {
                success = userSettings.luaShareMemory.Remove(key);
            }
            SaveUserSettingsNow();
            return success;
        }

        public List<string> ShareMemoryKeys()
        {
            lock (shareMemoryLocker)
            {
                return userSettings.luaShareMemory.Keys.ToList();
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
                userSettings.luaShareMemory[key] = value;
            }
            SaveUserSettingsNow();
        }

        public List<Models.Data.LuaCoreSetting> GetLuaCoreSettings() =>
            userSettings.luaServers;

        #endregion

        #region protected methods
        protected override void Cleanup()
        {
            snpCache?.Dispose();
        }

        public void SaveUserSettingsNow() =>
            VgcApis.Misc.Utils.SavePluginSetting(
                pluginName, userSettings, vgcSetting);

        #endregion
    }
}
