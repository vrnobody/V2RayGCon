namespace ProxySetter.Services
{
    class PsSettings
    {
        VgcApis.Models.IServices.ISettingsService setting;
        readonly string pluginName = Properties.Resources.Name;
        public Model.Data.UserSettings userSettings;

        public PsSettings() { }

        public void Run(
            VgcApis.Models.IServices.ISettingsService setting)
        {
            this.setting = setting;
            userSettings = LoadUserSettings();
        }

        #region properties
        public bool isCleaning { get; set; } = false;
        #endregion

        #region public methods
        public void DebugLog(string content)
        {
#if DEBUG
            SendLog($"(Degug) {content}");
#endif
        }

        public void SendLog(string contnet)
        {
            var name = Properties.Resources.Name;
            setting.SendLog(string.Format("[{0}] {1}", name, contnet));
        }

        /// <summary>
        /// return string.empty on error
        /// 0: white list  1: black list
        /// </summary>
        /// <returns></returns>
        public string[] GetCustomPacSetting()
        {
            return new string[] {
                userSettings.tabPacWhiteList??string.Empty,
                userSettings.tabPacBlackList??string.Empty,
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
            userSettings.tabBasicSetting = VgcApis.Libs.Utils.SerializeObject(basicSetting);
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
                var result = VgcApis.Libs.Utils
                    .DeserializeObject<Model.Data.BasicSettings>(
                    userSettings.tabBasicSetting);
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
                var content = VgcApis.Libs.Utils.SerializeObject(userSettings);
                setting.SavePluginsSetting(pluginName, content);
            }
            catch { }
        }

        public void Cleanup()
        {

        }
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
                var result = VgcApis.Libs.Utils
                    .DeserializeObject<Model.Data.UserSettings>(
                        userSettingString);
                return result ?? empty;
            }
            catch { }

            return empty;
        }
        #endregion

    }

}
