namespace VgcApis.Models.Datas
{
    public class Enum
    {
        public enum ShutdownReasons
        {
            CloseByUser,  // close by user
            Poweroff, // system shut down
            Abort, // attack by aliens :>
        }

        /// <summary>
        /// 数值需要连续,否则无法和ComboBox的selectedIndex对应
        /// </summary>
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

        public enum LinkTypes
        {
            vmess = 0,
            v2cfg = 1,
            ss = 2,
            http = 3,
            https = 4,
            v = 5,
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
