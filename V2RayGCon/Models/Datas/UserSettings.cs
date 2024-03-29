﻿using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace V2RayGCon.Models.Datas
{
    class UserSettings
    {
        #region public properties

        // obsolete 预计2024-06删除
        // ----------------------------------
        public string CustomInbounds { get; set; } = @"[]";

        public List<CustomConfigTemplate> CustomInboundSettings = null;

        public bool CustomVmessDecodeTemplateEnabled { get; set; } = false;
        public string CustomVmessDecodeTemplateUrl { get; set; } = "";

        // ----------------------------------

        // obsolete 预计2024-09删除
        // ----------------------------------
        public string CompressedUnicodeDecodeCache { get; set; }

        public bool isUseV4Format { get; set; }

        // ----------------------------------


        public bool isLoad3rdPartyPlugins { get; set; }
        public string CompressedUnicodeLocalStorage { get; set; }

        public bool isUseCustomUserAgent { get; set; }
        public string customUserAgent { get; set; }

        public string SystrayLeftClickCommand { get; set; }
        public bool isEnableSystrayLeftClickCommand { get; set; }

        public string DebugLogFilePath { get; set; }
        public bool isEnableDebugFile { get; set; }
        public int QuickSwitchServerLatency { get; set; }
        public bool isAutoPatchSubsInfo { get; set; }

        // FormOption->Defaults->Mode
        public ImportSharelinkOptions ImportOptions = null;

        // FormOption->Defaults->Speedtest
        public SpeedTestOptions SpeedtestOptions = null;

        // FormDownloadCore
        public bool isDownloadWin32V2RayCore { get; set; }
        public string v2rayCoreDownloadSource { get; set; }
        public List<string> V2RayCoreDownloadVersionList = null;

        public bool isSupportSelfSignedCert { get; set; }
        public string uTlsFingerprint { get; set; }

        public bool isEnableUtlsFingerprint { get; set; }

        public int ServerPanelPageSize { get; set; }
        public bool isEnableStat { get; set; } = false;

        public bool CfgShowToolPanel { get; set; }
        public bool isPortable { get; set; }
        public bool isCheckUpdateWhenAppStart { get; set; }

        public bool isCheckV2RayCoreUpdateWhenAppStart { get; set; }
        public bool isUpdateUseProxy { get; set; }

        // v2ray-core v4.23.1 multiple config file supports
        public string MultiConfItems { get; set; }

        public string ImportUrls { get; set; }

        public string SubscribeUrls { get; set; }

        public string PluginInfoItems { get; set; }

        public string CompressedUnicodePluginsSetting { get; set; }

        public string Culture { get; set; }
        public string CompressedUnicodeCoreInfoList { get; set; }

        public string PacServerSettings { get; set; }
        public string SysProxySetting { get; set; }
        public string ServerTracker { get; set; }
        public string WinFormPosList { get; set; }

        public int MaxConcurrentV2RayCoreNum { get; set; }

        public List<CustomCoreSettings> CustomCoreSettings = null;

        public string CompressedUnicodeCustomConfigTemplates { get; set; } = string.Empty;
        #endregion


        public UserSettings()
        {
            isLoad3rdPartyPlugins = false;

            CompressedUnicodeLocalStorage = string.Empty;

            isUseCustomUserAgent = false;

            customUserAgent = VgcApis.Models.Consts.Webs.ChromeUserAgent;

            isEnableSystrayLeftClickCommand = false;

            SystrayLeftClickCommand = "http://localhost:4000/";

            isDownloadWin32V2RayCore = true;
            v2rayCoreDownloadSource = VgcApis.Models.Consts.Core.GetSourceUrlByIndex(0);

            DebugLogFilePath = @"";
            isEnableDebugFile = false;

            QuickSwitchServerLatency = 0;

            isSupportSelfSignedCert = false;
            uTlsFingerprint = @"";
            isEnableUtlsFingerprint = false;

            isAutoPatchSubsInfo = false;

            ServerPanelPageSize = 12;

            MaxConcurrentV2RayCoreNum = 12;

            isCheckUpdateWhenAppStart = false;

            isCheckV2RayCoreUpdateWhenAppStart = false;

            isUpdateUseProxy = true;
            CfgShowToolPanel = true;
            isPortable = true;

            MultiConfItems = string.Empty;
            ImportUrls = string.Empty;

            SubscribeUrls = @"[]";

            // PluginInfoItems = "[{\"filename\":\"ProxySetter\",\"isUse\":true}]";
            PluginInfoItems = string.Empty;

            CompressedUnicodePluginsSetting = string.Empty;

            Culture = string.Empty;

            CompressedUnicodeCoreInfoList = string.Empty;

            PacServerSettings = string.Empty;
            SysProxySetting = string.Empty;
            ServerTracker = string.Empty;
            WinFormPosList = string.Empty;
        }

        #region public methods
        public void Normalized()
        {
            V2RayCoreDownloadVersionList = V2RayCoreDownloadVersionList ?? new List<string>();

            ImportOptions = ImportOptions ?? new ImportSharelinkOptions();
            FixImportOptions();

            SpeedtestOptions = SpeedtestOptions ?? new SpeedTestOptions();
            CustomCoreSettings = CustomCoreSettings ?? new List<CustomCoreSettings>();

            // plan to delete on 2024-06
            // ----------------
            CustomInboundSettings = CustomInboundSettings ?? CreateDefaultConfigTemplates();
            FixCustomInbounds(); // 必须在CustomInboundSettings创建后
            // ----------------
        }
        #endregion

        #region private methods
        // 预计2024-06删除
        void FixImportOptions()
        {
            if (!string.IsNullOrEmpty(ImportOptions.DefaultInboundName))
            {
                return;
            }

            string name;
            var inbTy = (Enums.ProxyTypes)ImportOptions.Mode;
            switch (inbTy)
            {
                case Enums.ProxyTypes.Config:
                    name = "config";
                    break;
                case Enums.ProxyTypes.SOCKS:
                    name = "socks";
                    break;
                case Enums.ProxyTypes.Custom:
                    name = "custom";
                    break;
                default:
                    name = "http";
                    break;
            }
            ImportOptions.DefaultInboundName = name;
        }

        // 预计2024-06删除
        void FixCustomInbounds()
        {
            if (string.IsNullOrEmpty(CustomInbounds))
            {
                return;
            }

            try
            {
                var inbs = JArray.Parse(CustomInbounds);
                if (inbs.Count > 0)
                {
                    var index = CustomInboundSettings.Count + 1;
                    var tpl = string.Format("{{inbounds:{0}}}", CustomInbounds);
                    var config = VgcApis.Misc.Utils.FormatConfig(tpl);
                    CustomInboundSettings.Add(
                        new CustomConfigTemplate()
                        {
                            index = index,
                            name = "custom",
                            template = config,
                        }
                    );
                }
                CustomInbounds = @"";
            }
            catch { }
        }

        List<CustomConfigTemplate> CreateDefaultConfigTemplates()
        {
            var r = new List<CustomConfigTemplate>();

            var config = new CustomConfigTemplate() { index = 1, name = "config", };

            var socks = new CustomConfigTemplate()
            {
                index = 2,
                name = "socks",
                isSocks5Inbound = true,
                template =
                    @"{
    ""inbounds"":[
        {
            ""tag"": ""agentin"",
            ""protocol"": ""socks"",
            ""port"": %port%,
            ""listen"": ""%host%"",
            ""settings"": {
                ""udp"": true
            }
        }
    ]
}",
            };

            var http = new CustomConfigTemplate()
            {
                index = 3,
                name = "http",
                template =
                    @"{
    ""inbounds"":[
        {
            ""tag"": ""agentin"",
            ""protocol"": ""http"",
            ""port"": %port%,
            ""listen"": ""%host%"",
            ""settings"": { }
        }
    ]
}",
            };
            r.AddRange(new CustomConfigTemplate[] { config, socks, http });
            return r;
        }
        #endregion
    }
}
