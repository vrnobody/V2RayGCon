﻿using System.Collections.Generic;

namespace VgcApis.Interfaces.Services
{
    public interface ISettingsService
    {
        bool isTunMode { get; set; }

        string GetSendThroughIpv4();

        bool SetSendThroughIpv4(string host);

        void SaveUserSettingsNow();

        int GetSpeedtestQueueLength();

        bool IsScreenLocked();

        bool IsClosing();

        void SavePluginsSetting(string pluginName, string value);
        string GetPluginsSetting(string pluginName);

        List<string> GetPluginsSettinKeys();
        bool RemovePluginsSettinKey(string key);

        List<string> GetLocalStorageKeys();

        bool RemoveLocalStorage(string key);

        string ReadLocalStorage(string key);

        string WriteLocalStorage(string key, string value);

        // for luna plug-in
        string GetSubscriptionConfig();
        void SetSubscriptionConfig(string cfgStr);

        string GetUserSettings(string props);

        bool SetUserSettings(string props);
    }
}
