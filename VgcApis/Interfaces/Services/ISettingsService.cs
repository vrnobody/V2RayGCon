namespace VgcApis.Interfaces.Services
{
    public interface ISettingsService
    {
        int GetSpeedtestQueueLength();

        bool IsScreenLocked();

        bool IsClosing();

        string GetLogContent();

        void SendLog(string log);
        void SavePluginsSetting(string pluginName, string value);
        string GetPluginsSetting(string pluginName);

        // for luna plug-in
        string GetSubscriptionConfig();
        void SetSubscriptionConfig(string cfgStr);

        string GetUserSettings();
        bool SetUserSettings(string userSettings);
    }
}
