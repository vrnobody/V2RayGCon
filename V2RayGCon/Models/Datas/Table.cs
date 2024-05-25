using System.Collections.Generic;

namespace V2RayGCon.Models.Datas
{
    class Table
    {
        public static readonly Dictionary<Enums.Cultures, string> Cultures = new Dictionary<
            Enums.Cultures,
            string
        >
        {
            { Enums.Cultures.auto, "auto" },
            { Enums.Cultures.enUS, "en" },
            { Enums.Cultures.zhCN, "cn" },
        };

        public static readonly Dictionary<int, StreamComponent> streamSettings = new Dictionary<
            int,
            StreamComponent
        >
        {
            //public bool dropDownStyle;
            //public string name;
            //public string network;
            //public string optionPath;
            //public Dictionary<string, string> options;

            // kcp
            {
                0,
                new StreamComponent
                {
                    dropDownStyle = true,
                    name = "mKCP",
                    network = "kcp",
                    paths = new List<string>
                    {
                        "kcpSettings.header.type", // streamParam1 label text = [type]
                        "kcpSettings.seed", // streamParam2 label text = [seed]
                    },
                    options = new Dictionary<string, string>
                    {
                        { "none", "kcp" },
                        { "srtp", "kcp_srtp" },
                        { "utp", "kcp_utp" },
                        { "wechat-video", "kcp_wechat-video" },
                        { "dtls", "kcp_dtls" },
                        { "wireguard", "kcp_wireguard" },
                    },
                }
            },
            // tcp
            {
                1,
                new StreamComponent
                {
                    dropDownStyle = true,
                    name = "TCP",
                    network = "tcp",
                    paths = new List<string>
                    {
                        "tcpSettings.header.type",
                        "tcpSettings.header.request.path",
                        "tcpSettings.header.request.headers.Host",
                    },
                    options = new Dictionary<string, string>
                    {
                        { "none", "tcp" },
                        { "http", "tcp_http" },
                    },
                }
            },
            // h2 ws dsock
            {
                2,
                new StreamComponent
                {
                    dropDownStyle = false,
                    name = "HTTP/2",
                    network = "h2",
                    paths = new List<string> { "httpSettings.path", "httpSettings.host", },
                    options = new Dictionary<string, string> { { "none", "h2" }, },
                }
            },
            {
                3,
                new StreamComponent
                {
                    dropDownStyle = false,
                    name = "WebSocket",
                    network = "ws",
                    paths = new List<string> { "wsSettings.path", "wsSettings.headers.Host", },
                    options = new Dictionary<string, string> { { "none", "ws" }, },
                }
            },
            // quic
            {
                4,
                new StreamComponent
                {
                    dropDownStyle = true,
                    name = "QUIC",
                    network = "quic",
                    paths = new List<string>
                    {
                        "quicSettings.header.type",
                        "quicSettings.security",
                        "quicSettings.key",
                    },
                    options = new Dictionary<string, string>
                    {
                        { "none", "quic" },
                        { "srtp", "quic_srtp" },
                        { "utp", "quic_utp" },
                        { "wechat-video", "quic_wechat-video" },
                        { "dtls", "quic_dtls" },
                        { "wireguard", "quic_wireguard" },
                    },
                }
            },
            // grpc
            {
                5,
                new StreamComponent
                {
                    dropDownStyle = true,
                    name = "GRPC",
                    network = "grpc",
                    paths = new List<string>
                    {
                        "grpcSettings.multiMode",
                        "grpcSettings.serviceName",
                        "grpcSettings.authority",
                    },
                    options = new Dictionary<string, string>
                    {
                        { "true", "grpc_multi" },
                        { "false", "grpc" },
                    },
                }
            },
            {
                6,
                new StreamComponent
                {
                    dropDownStyle = false,
                    name = "HTTPupgrade",
                    network = "httpupgrade",
                    paths = new List<string>
                    {
                        "httpupgradeSettings.path",
                        "httpupgradeSettings.host",
                    },
                    options = new Dictionary<string, string> { { "none", "httpupgrade" }, },
                }
            },
        };
    }
}
