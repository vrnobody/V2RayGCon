using System.Collections.Generic;

namespace V2RayGCon.Models.Datas
{
    class UserSettings
    {
        #region public properties

        public string CustomFilterKeywords { get; set; } =
            "// show tested servers orderby latency\n#latency ~ 1 30000 & #orderby latency";

        public bool isLoad3rdPartyPlugins { get; set; }

        // obsolete delete after 2026-10
        public string CompressedUnicodeLocalStorage { get; set; } = "";
        public string ZstdLocalStorage { get; set; } = "";

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
        public bool isDownloadWin32V2RayCore { get; set; } = false;
        public bool isDownloadWin7XrayCore { get; set; } = false;
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

        // obsolete delete after 2026-10
        public string CompressedUnicodePluginsSetting { get; set; } = "";

        public string ZstdPluginsSetting { get; set; } = "";

        public string Culture { get; set; }

        // obsolete delete after 2026-10
        public string CompressedUnicodeCoreInfoList { get; set; } = "";

        public string ZstdCoreInfoList { get; set; } = "";

        public string PacServerSettings { get; set; }
        public string SysProxySetting { get; set; }
        public string ServerTracker { get; set; }
        public string WinFormPosList { get; set; }

        public int MaxConcurrentV2RayCoreNum { get; set; }

        public List<CustomCoreSettings> CustomCoreSettings = null;

        // obsolete delete after 2026-10
        public string CompressedUnicodeCustomConfigTemplates { get; set; } = "";
        public string ZstdCustomConfigTemplates { get; set; } = "";
        #endregion


        public UserSettings()
        {
            isLoad3rdPartyPlugins = false;

            isUseCustomUserAgent = false;

            customUserAgent = VgcApis.Models.Consts.Webs.ChromeUserAgent;

            isEnableSystrayLeftClickCommand = false;

            SystrayLeftClickCommand = "http://localhost:4000/";

            v2rayCoreDownloadSource = VgcApis.Models.Consts.Core.GetSourceUrlByIndex(2);

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

            Culture = string.Empty;

            PacServerSettings = string.Empty;
            SysProxySetting = string.Empty;
            ServerTracker = string.Empty;
            WinFormPosList = string.Empty;
        }

        #region public methods
        public void Normalized()
        {
            V2RayCoreDownloadVersionList =
                V2RayCoreDownloadVersionList
                ?? new List<string> { "v1.8.18", "v1.8.15", "v1.8.13" };
            ImportOptions = ImportOptions ?? new ImportSharelinkOptions();
            SpeedtestOptions = SpeedtestOptions ?? new SpeedTestOptions();
            CustomCoreSettings = CustomCoreSettings ?? new List<CustomCoreSettings>();

            if (
                string.IsNullOrEmpty(CompressedUnicodeCustomConfigTemplates) // obsolete delete after 2026-10
                && string.IsNullOrEmpty(ZstdCustomConfigTemplates)
            )
            {
                ZstdCustomConfigTemplates =
                    VgcApis.Libs.Infr.ZipExtensions.SerializeObjectToZstdBase64(
                        CreateDefaultConfigTemplates()
                    );
            }
        }
        #endregion

        #region private methods
        List<CustomConfigTemplate> CreateDefaultConfigTemplates()
        {
            var r = new List<CustomConfigTemplate>();

            var config = new CustomConfigTemplate() { index = 1, name = "config" };

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
