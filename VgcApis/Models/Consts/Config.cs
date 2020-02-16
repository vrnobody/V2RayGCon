using System.Collections.Generic;

namespace VgcApis.Models.Consts
{
    public static class Config
    {
        public static double CustomSpeedtestMeanWeight = 0.6;
        public static double FloatPointNumberTolerance = 0.000001;

        public static int ConfigEditorServerMenuGroupSize = 30;
        public static int NotifyIconServerMenuGroupSize = 25;
        public static int FormMainStatusPagerMenuGroupSize = 25;

        public const string ProtocolNameVmess = @"vmess";
        public const string ProtocolNameSs = @"shadowsocks";
        public const string ProtocolNameSocks = @"socks";
        public const string ProtocolNameHttp = @"http";

        public const string JsonArray = @"[]";
        public const string JsonObject = @"{}";

        public const string ConfigDotJson = "config.json";

        public const int ConfigSectionDefDepth = 2;

        public const string ConfigSectionDefRootKey = @"ROOT";

        public static Dictionary<string, int> ConfigSectionDefSetting = new Dictionary<string, int>
        {
            { $"{ConfigSectionDefRootKey}.api", 0 },
            { $"{ConfigSectionDefRootKey}.inbound", 0 },
            { $"{ConfigSectionDefRootKey}.outbound", 0 },
            { $"{ConfigSectionDefRootKey}.routing", 2 },
            { $"{ConfigSectionDefRootKey}.routing.settings", 2 },
        };

        public static Dictionary<string, string> GetDefCfgSections() =>
            new Dictionary<string, string>
            {
                { "v2raygcon", JsonObject},
                { "log", JsonObject},
                { "inbounds", JsonArray},
                { "outbounds", JsonArray},
                { "routing", JsonObject},
                { "policy", JsonObject},
                { "api", JsonObject},
                { "dns", JsonObject},
                { "stats", JsonObject},
                { "transport", JsonObject},
                { "reverse", JsonObject},
            };
    }
}
