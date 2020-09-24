using System.Collections.Generic;
using V2RayGCon.Resources.Resx;

namespace V2RayGCon.Models.Datas
{
    class Table
    {
        public static readonly Dictionary<Models.Datas.Enums.Cultures, string> Cultures = new Dictionary<Enums.Cultures, string>
        {
            { Enums.Cultures.auto,"auto" },
            { Enums.Cultures.enUS,"en" },
            { Enums.Cultures.zhCN,"cn" },
        };

        public static readonly string[] EnviromentVariablesName = new string[] {
            "V2RAY_BUF_READV",
            "V2RAY_LOCATION_ASSET",
            "V2RAY_LOCATION_CONFIG",
            "V2RAY_LOCATION_CONFDIR",
            "V2RAY_RAY_BUFFER_SIZE",
        };

        public static readonly string[] customInbTypeNames = new string[] {
            "config",
            "http",
            "socks",
            "custom",
        };

        public static readonly Dictionary<int, string> ssMethods = new Dictionary<int, string>
        {
            { 0,"aes-128-cfb"},
            { 1,"aes-128-gcm"},
            { 2,"aes-256-cfb"},
            { 3,"aes-256-gcm"},
            { 4,"chacha20"},
            { 5,"chacha20-ietf"},
            { 6,"chacha20-poly1305"},
            { 7,"chacha20-ietf-poly1305"},
        };

        public static readonly Dictionary<int, string> kcpTypes = new Dictionary<int, string> {
            {0, "none" },
            {1, "srtp" },
            {2, "utp" },
            {3, "wechat-video" },
            {4, "dtls" },
            {5, "wireguard" },
        };

        public static readonly Dictionary<int, StreamComponent> streamSettings = new Dictionary<int, Models.Datas.StreamComponent>
        {
            //public bool dropDownStyle;
            //public string name;
            //public string network;
            //public string optionPath;
            //public Dictionary<string, string> options;

            // kcp
            { 0, new StreamComponent{
                dropDownStyle=true,
                name="mKCP",
                network="kcp",
                paths=new List<string>{
                    "kcpSettings.header.type",
                    "kcpSettings.seed",
                },
                options=new Dictionary<string,string>{
                    { "none", "kcp"},
                    { "srtp", "kcp_srtp" },
                    { "utp", "kcp_utp"},
                    { "wechat-video", "kcp_wechat-video" },
                    { "dtls", "kcp_dtls"},
                    { "wireguard", "kcp_wireguard"},
                },
            } },

            // tcp
            { 1, new StreamComponent{
                dropDownStyle=true,
                name="TCP",
                network="tcp",
                paths=new List<string>{
                    "tcpSettings.header.type",
                    "tcpSettings.header.request.path",
                    "tcpSettings.header.request.headers.Host",
                },
                options=new Dictionary<string, string>{
                    { "none","tcp" },
                    { "http","tcp_http" },
                },
            } },

            // h2 ws dsock
            { 2, new StreamComponent{
                dropDownStyle=false,
                name="HTTP/2",
                network="h2",
                paths=new List<string>{
                    "httpSettings.path",
                    "httpSettings.host",
                },
                options=new Dictionary<string, string>{
                    { "none","h2" },
                },
            } },
            { 3, new StreamComponent{
                dropDownStyle=false,
                name="WebSocket",
                network="ws",
                paths=new List<string>{
                    "wsSettings.path",
                    "wsSettings.headers.Host",
                },
                options=new Dictionary<string, string>{
                    { "none","ws" },
                },
            } },
            { 4, new StreamComponent{
                dropDownStyle=false,
                name="DomainSocket",
                network="domainsocket",
                paths=new List<string>{
                    "dsSettings.path",
                },
                options=new Dictionary<string, string>{
                    { "none","dsock" },
                },
            } },

            // quic
            { 5, new StreamComponent{
                dropDownStyle=true,
                name="QUIC",
                network="quic",
                paths=new List<string>{
                    "quicSettings.header.type",
                    "quicSettings.security",
                    "quicSettings.key",
                },
                options=new Dictionary<string,string>{
                    { "none", "quic"},
                    { "srtp", "quic_srtp" },
                    { "utp", "quic_utp"},
                    { "wechat-video", "quic_wechat-video" },
                    { "dtls", "quic_dtls"},
                    { "wireguard", "quic_wireguard"},
                },
            } },
        };

        // editor examples
        public static readonly Dictionary<string, List<string[]>> examples = ExampleHelper();
        static Dictionary<string, List<string[]>> ExampleHelper()
        {
            string[] SS(string description, string key)
            {
                return new string[] { description, key };
            }

            string[] SSS(string description, string key, string protocol)
            {
                return new string[] { description, key, protocol };
            }

            List<string[]> NewList()
            {
                return new List<string[]>();
            }

            var d = new Dictionary<string, List<string[]>>();

            List<string[]> list;

            // { 0, "config.json"},
            list = NewList();
            list.Add(SS(I18N.Default, "cfgMin"));
            list.Add(SS("Empty", "cfgEmpty"));
            d.Add("config.json", list);

            //{ 1, "log"},
            list = NewList();
            list.Add(SS(I18N.Default, "logFile"));
            list.Add(SS("None", "logNone"));
            list.Add(SS("Error", "logError"));
            list.Add(SS("Warning", "logWarning"));
            list.Add(SS("Info", "logInfo"));
            list.Add(SS("Debug", "logDebug"));
            d.Add("log", list);

            //{ 2, "inbound"},
            list = NewList();
            list.Add(SSS("HTTP", "inHTTP", "http"));
            list.Add(SSS("SOCKS", "inSocks", "socks"));
            list.Add(SSS("Shadowsocks", "inSS", "shadowsocks"));
            list.Add(SSS("Trojan", "inTrojan", "trojan"));
            list.Add(SSS("VLess", "inVless", "vless"));
            list.Add(SSS("VMess", "inVmess", "vmess"));
            list.Add(SSS("Dokodemo-door", "inDoko", "dokodemo-door"));
            d.Add("inbound", list);

            // 20 inbounds
            d.Add("inbounds", list);

            //{ 3, "outbound"},
            list = NewList();
            list.Add(SSS("Freedom", "outFree", "freedom"));
            list.Add(SSS("HTTP", "outHttp", "http"));
            list.Add(SSS("SOCKS", "outSocks", "socks"));
            list.Add(SSS("Shadowsocks", "outSS", "shadowsocks"));
            list.Add(SSS("Trojan", "outTrojan", "trojan"));
            list.Add(SSS("VLess", "outVless", "vless"));
            list.Add(SSS("VMess", "outVmess", "vmess"));
            list.Add(SSS("Black hole", "outBlackHole", "blackhole"));
            d.Add("outbound", list);

            // 21 outbounds
            d.Add("outbounds", list);

            //{ 4, "routing"},
            list = NewList();
            list.Add(SS(I18N.Default, "routeAll"));
            list.Add(SS("v4", "routeDefV4"));
            list.Add(SS("skip CN site", "routeCNIP"));
            list.Add(SS("skip CN site v4", "routeCnipV4"));
            list.Add(SS("Inbound to Outbound", "routeIn2Out"));
            d.Add("routing", list);

            //{ 5, "policy"},
            list = NewList();
            list.Add(SS(I18N.Default, "policyDefault"));
            d.Add("policy", list);

            //{ 6,"v2raygcon" },
            list = NewList();
            list.Add(SS(I18N.Default, "v2raygcon"));
            list.Add(SS(I18N.Import, "vgcImport"));
            d.Add("v2raygcon", list);

            //{ 7, "api"},
            list = NewList();
            list.Add(SS(I18N.Default, "apiDefault"));
            d.Add("api", list);

            //{ 8, "dns"},
            list = NewList();
            list.Add(SS(I18N.Default, "dnsDefault"));
            list.Add(SS(I18N.CFnGoogle, "dnsCFnGoogle"));
            d.Add("dns", list);

            //{ 9, "stats"},

            //{ 10, "transport"},
            list = NewList();
            list.Add(SS(I18N.Default, "transDefault"));
            d.Add("transport", list);

            // { 11, "reverse" }
            list = NewList();
            list.Add(SS(I18N.Default, "reverseDefault"));
            d.Add("reverse", list);

            //{ 22,"inboundDetour"}, 
            list = NewList();
            list.Add(SS(I18N.Default, "inDtrDefault"));
            d.Add("inboundDetour", list);

            //{ 23,"outboundDetour"}, outDtrDefault
            list = NewList();
            list.Add(SS(I18N.Default, "outDtrDefault"));
            list.Add(SS("Freedom", "outDtrFreedom"));
            d.Add("outboundDetour", list);

            return d;
        }
    }
}
