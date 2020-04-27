namespace VgcApis.Interfaces.Services
{
    public interface ISettingsService
    {
        bool IsScreenLocked();

        bool IsClosing();

        void SendLog(string log);
        void SavePluginsSetting(string pluginName, string value);
        string GetPluginsSetting(string pluginName);

        // for luna plug-in
        string GetSubscriptionConfig();
    }
}
