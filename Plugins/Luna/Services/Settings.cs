using AutocompleteMenuNS;
using ScintillaNET;
using System.Collections.Generic;
using System.Linq;

namespace Luna.Services
{
    public class Settings :
        VgcApis.Models.BaseClasses.Disposable

    {
        VgcApis.Models.IServices.ISettingsService vgcSetting;
        readonly string pluginName = Properties.Resources.Name;
        Models.Data.UserSettings userSettings;
        Libs.LuaSnippet.LuaAcm luaAcm;

        public Settings() { }

        #region internal methods
        public AutocompleteMenu AttachSnippetsTo(Scintilla editor) =>
            luaAcm?.BindToEditor(editor);
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
            VgcApis.Models.IServices.ISettingsService vgcSetting)
        {
            this.vgcSetting = vgcSetting;
            this.luaAcm = new Libs.LuaSnippet.LuaAcm();

            userSettings = VgcApis.Libs.Utils
                .LoadPluginSetting<Models.Data.UserSettings>(
                    pluginName, vgcSetting);

            userSettings.NormalizeData();
        }

        public string GetLuaShareMemory(string key)
        {
            if (!userSettings.luaShareMemory.ContainsKey(key))
            {
                return @"";
            }
            return userSettings.luaShareMemory[key];
        }

        readonly object shareMemoryLocker = new object();
        public bool RemoveShareMemory(string key)
        {
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
            luaAcm?.Dispose();
        }

        public void SaveUserSettingsNow() =>
            VgcApis.Libs.Utils.SavePluginSetting(
                pluginName, userSettings, vgcSetting);

        #endregion
    }
}
