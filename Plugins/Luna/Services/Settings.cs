using ScintillaNET;
using System.Collections.Generic;
using System.Linq;

namespace Luna.Services
{
    internal class Settings :
        VgcApis.BaseClasses.Disposable

    {
        VgcApis.Interfaces.Services.ISettingsService vgcSetting;
        VgcApis.Interfaces.Services.INotifierService vgcNotifier;

        readonly string pluginName = Properties.Resources.Name;
        Models.Data.UserSettings userSettings;
        Libs.LuaSnippet.SnippetsCache snpCache;

        public Settings() { }

        #region properties
        public bool isScreenLocked => vgcSetting.IsScreenLocked();

        public bool isEnableAdvanceAutoComplete
        {
            get => userSettings.isEnableAdvanceAutoComplete;
            set
            {
                if (userSettings.isEnableAdvanceAutoComplete == value)
                {
                    return;
                }
                userSettings.isEnableAdvanceAutoComplete = value;
                SaveUserSettingsNow();
            }
        }

        public bool isLoadClrLib
        {
            get => userSettings.isLoadClr;
            set
            {
                if (userSettings.isLoadClr == value)
                {
                    return;
                }
                userSettings.isLoadClr = value;
                SaveUserSettingsNow();
            }
        }
        #endregion

        #region internal methods
        public Libs.LuaSnippet.LuaAcm AttachSnippetsTo(Scintilla editor) =>
            snpCache?.BindToEditor(editor);
        #endregion

        #region public methods
        public void SendLog(string contnet)
        {
            var name = Properties.Resources.Name;
            vgcSetting.SendLog(string.Format("[{0}] {1}", name, contnet));
        }

        bool isDisposing = false;
        public bool IsShutdown() => isDisposing || vgcSetting.IsClosing();

        public void SetIsDisposing(bool value) => isDisposing = value;

        public void Run(
            VgcApis.Interfaces.Services.ISettingsService vgcSetting,
            VgcApis.Interfaces.Services.INotifierService vgcNotifier)
        {
            this.vgcSetting = vgcSetting;
            this.vgcNotifier = vgcNotifier;

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
