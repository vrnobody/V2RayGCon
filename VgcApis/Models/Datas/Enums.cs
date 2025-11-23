using System;

namespace VgcApis.Models.Datas
{
    public static class Enums
    {
        [Flags]
        public enum ConfigType
        {
            text,
            json,
            yaml,
        }

        public enum PackageTypes
        {
            Chain,
            Balancer,
        }

        public enum BalancerStrategies
        {
            Random = 0,
            LeastPing = 1,
            RoundRobin = 2,
            LeastLoad = 3,
        }

        [Flags]
        public enum ModifierKeys
        {
            Alt = 1,
            Control = 2,
            Shift = 4,
            Win = 8,
        }

        public enum ShutdownReasons
        {
            Undefined, // default
            CloseByUser, // close by user
            PowerOff, // system shut down
            Abort, // attacked by aliens :>
            FileLocked, // userSettings is occupied
            ShowHelpInfo,
        }

        /// <summary>
        /// 数值需要连续,否则无法和ComboBox的selectedIndex对应
        /// </summary>
        [Flags]
        public enum Sections
        {
            Config = 0,
            Log = 1,
            Inbound = 2,
            Outbound = 3,
            Routing = 4,
            Policy = 5,
            V2raygcon = 6,
            Api = 7,
            Dns = 8,
            Stats = 9,
            Transport = 10,
            Reverse = 11,

            Seperator = 12, // eq first array

            Inbounds = 12,
            Outbounds = 13,
            InboundDetour = 14,
            OutboundDetour = 15,
        }

        public enum Cultures
        {
            auto = 0,
            enUS = 1,
            zhCN = 2,
        }

        [Flags]
        public enum LinkTypes
        {
            vmess = 0,
            v2cfg = 1,
            ss = 2,
            http = 3,
            https = 4,

            // v = 5, // disabled 2023-09-14

            trojan = 6,
            vless = 7,

            socks = 8,
            mob = 9,
            unknow = 256, // for enum parse
        }

        /// <summary>
        /// Inbound types
        /// </summary>
        public enum ProxyTypes
        {
            Config = 0,
            HTTP = 1,
            SOCKS = 2,
            Custom = 3,
        }

        public enum FormLocations
        {
            TopLeft,
            BottomLeft,
            TopRight,
            BottomRight,
        }

        public enum SaveFileErrorCode
        {
            Fail,
            Cancel,
            Success,
        }
    }
}
