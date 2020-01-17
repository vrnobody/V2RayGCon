namespace ProxySetter.Model.Data
{
    class UserSettings
    {
        #region public properties
        public string tabBasicSetting { get; set; }
        public string tabPacWhiteList { get; set; }
        public string tabPacBlackList { get; set; }
        public string sysProxySetting { get; set; }

        #endregion

        public UserSettings()
        {
            sysProxySetting = string.Empty;
            tabBasicSetting = string.Empty;
            tabPacBlackList = string.Empty;
            tabPacWhiteList = string.Empty;
        }
    }
}
