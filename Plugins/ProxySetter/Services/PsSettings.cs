﻿namespace ProxySetter.Services
{
    class PsSettings
    {
        VgcApis.Interfaces.Services.ISettingsService setting;
        readonly string pluginName = Properties.Resources.Name;
        public Model.Data.UserSettings userSettings;

        public PsSettings() { }

        public void Run(VgcApis.Interfaces.Services.ISettingsService setting)
        {
            this.setting = setting;
            userSettings = LoadUserSettings();
        }

        #region public methods
        bool _IsDisposing = false;

        public void SetIsDisposing(bool value) => _IsDisposing = value;

        public bool IsClosing() => _IsDisposing || setting.IsClosing();

        public void DebugLog(string content)
        {
#if DEBUG
            SendLog($"(debug) {content}");
#endif
        }

        public void SendLog(string contents)
        {
            var name = Properties.Resources.Name;
            VgcApis.Misc.Logger.Log($"[{name}]", contents);
        }

        public Model.Data.TunaSettings GetTunaSettings()
        {
            return userSettings.tunaSettings;
        }

        public void SaveTunaSettings(Model.Data.TunaSettings tunaSettings)
        {
            userSettings.tunaSettings = tunaSettings;
            SaveUserSettings();
        }

        /// <summary>
        /// return string.empty on error
        /// 0: white list  1: black list
        /// </summary>
        /// <returns></returns>
        public string[] GetCustomPacSetting()
        {
            return new string[]
            {
                userSettings.tabPacWhiteList ?? string.Empty,
                userSettings.tabPacBlackList ?? string.Empty,
            };
        }

        /// <summary>
        /// 0: white list  1: black list
        /// </summary>
        /// <returns></returns>
        public void SaveCustomPacSetting(string[] customPacList)
        {
            userSettings.tabPacWhiteList = customPacList[0];
            userSettings.tabPacBlackList = customPacList[1];
            SaveUserSettings();
        }

        public void SaveBasicSetting(Model.Data.BasicSettings basicSetting)
        {
            userSettings.tabBasicSetting = VgcApis.Misc.Utils.SerializeObject(basicSetting);
            SaveUserSettings();
        }

        /// <summary>
        /// on error return default setting
        /// </summary>
        /// <returns></returns>
        public Model.Data.BasicSettings GetBasicSetting()
        {
            var empty = new Model.Data.BasicSettings();
            try
            {
                var result = VgcApis.Misc.Utils.DeserializeObject<Model.Data.BasicSettings>(
                    userSettings.tabBasicSetting
                );
                if (result != null)
                {
                    return result;
                }
            }
            catch { }
            return empty;
        }

        public void SaveUserSettings()
        {
            try
            {
                var content = VgcApis.Misc.Utils.SerializeObject(userSettings);
                setting.SavePluginsSetting(pluginName, content);
            }
            catch { }
        }

        public void Cleanup() { }
        #endregion

        #region private methods
        Model.Data.UserSettings LoadUserSettings()
        {
            var empty = new Model.Data.UserSettings();
            var userSettingString = setting.GetPluginsSetting(pluginName);
            if (string.IsNullOrEmpty(userSettingString))
            {
                return empty;
            }

            try
            {
                var result = VgcApis.Misc.Utils.DeserializeObject<Model.Data.UserSettings>(
                    userSettingString
                );
                return result ?? empty;
            }
            catch { }

            return empty;
        }
        #endregion
    }
}
