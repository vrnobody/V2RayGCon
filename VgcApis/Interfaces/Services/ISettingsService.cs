namespace VgcApis.Interfaces.Services
{
    public interface ISettingsService
    {
        void SaveUserSettingsNow();

        int GetSpeedtestQueueLength();

        bool IsScreenLocked();

        bool IsClosing();

        string GetLogContent();

        void SendLog(string log);
        void SavePluginsSetting(string pluginName, string value);
        string GetPluginsSetting(string pluginName);

        string GetLocalStorage();

        void SaveLocalStorage(string value);

        // for luna plug-in
        string GetSubscriptionConfig();
        void SetSubscriptionConfig(string cfgStr);

        string GetUserSettings(string props);

        bool SetUserSettings(string props);
    }
}
