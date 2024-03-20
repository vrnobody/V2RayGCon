using System.Collections.Generic;

namespace VgcApis.Models.Consts
{
    public static class Config
    {
        public static readonly string OutboundsLeftPadding = @"    ";

        public static readonly string ServsPkgTagPrefix = @"agentout";

        public static readonly string HttpInboundsTemplate =
            @"[{'tag': 'agentin','protocol': 'http','port': %port%,'listen': '%host%','settings': {}}]";

        public static int QuickSwitchMenuItemNum = 9;

        public static double CustomSpeedtestMeanWeight = 0.6;
        public static double FloatPointNumberTolerance = 0.000001;

        public static int MinDynamicMenuSize = 512;
        public static int MenuItemGroupSize = 16;

        public const string ProtocolNameVless = @"vless";
        public const string ProtocolNameVmess = @"vmess";
        public const string ProtocolNameSs = @"shadowsocks";
        public const string ProtocolNameSocks = @"socks";
        public const string ProtocolNameHttp = @"http";

        public const string JsonArray = @"[]";
        public const string JsonObject = @"{}";

        public const string SectionKeyV2rayGCon = @"v2raygcon";

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
    }
}
