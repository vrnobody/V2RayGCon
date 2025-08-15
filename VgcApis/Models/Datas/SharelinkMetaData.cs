using System;
using System.Collections.Generic;
using System.Linq;

namespace VgcApis.Models.Datas
{
    public class SharelinkMetaData
    {
        public string name = string.Empty;
        public string proto = string.Empty;
        public string host = string.Empty;
        public int port = 0;
        public string auth1 = string.Empty;
        public string auth2 = string.Empty;

        public string streamType = string.Empty;
        public string streamParam1 = string.Empty;
        public string streamParam2 = string.Empty;
        public string streamParam3 = string.Empty;

        public string _tlsType = @"none";
        static HashSet<string> supportedTlsTypes = new HashSet<string>
        {
            "none",
            "tls",
            "xtls",
            "reality",
        };
        public string tlsType
        {
            get => _tlsType;
            set
            {
                var lower = value?.ToLower() ?? "";
                _tlsType = supportedTlsTypes.Contains(lower) ? lower : "none";
            }
        }

        public string tlsServName = @"";
        public bool useSelfSignCert = false;

        // tls.fingerprint
        public string tlsFingerPrint = string.Empty;

        // tls.alpn
        public string tlsAlpn = string.Empty;

        // tls.ech
        // reality.publicKey
        public string tlsParam1 = string.Empty;

        // reality.shortId
        public string tlsParam2 = string.Empty;

        // reality.spiderX
        public string tlsParam3 = string.Empty;

        // reality.ML-DSA-65
        public string tlsParam4 = string.Empty;

        public SharelinkMetaData() { }

        #region public methods

        public string ToJson()
        {
            try
            {
                return Misc.Utils.ToJson(this);
            }
            catch { }
            return null;
        }

        public string ToHash()
        {
            var json = ToJson();
            try
            {
                return Misc.Utils.Sha256Hex(json);
            }
            catch { }
            return null;
        }

        public string ToShareLink()
        {
            switch (proto)
            {
                case "vless":
                case "trojan":
                    return EncodeToUriShareLink();
                case "ss":
                case "shadowsocks":
                    return EncodeToSsShareLink();
                case "socks":
                    return EncodeToSocksShareLink();
                case "vmess":
                    var vmess = new Vmess(this);
                    return vmess?.ToVmessLink();
            }
            return null;
        }
        #endregion

        #region private methods
        string EncodeToSocksShareLink()
        {
            var auth = string.Format("{0}:{1}", auth1, auth2);
            var userinfo = Misc.Utils.Base64EncodeString(auth).Replace('+', '-').Replace('/', '_');

            var url = string.Format(
                "socks://{0}@{1}:{2}#{3}",
                userinfo,
                Misc.Utils.FormatHost(host),
                port,
                Uri.EscapeDataString(name)
            );

            return url;
        }

        string EncodeToSsShareLink()
        {
            var auth = string.Format("{0}:{1}", auth2, auth1);
            var userinfo = Misc
                .Utils.Base64EncodeString(auth)
                .Replace("=", "")
                .Replace('+', '-')
                .Replace('/', '_');

            var url = string.Format(
                "ss://{0}@{1}:{2}#{3}",
                userinfo,
                Uri.EscapeDataString(host),
                port,
                Uri.EscapeDataString(name)
            );

            return url;
        }

        void EncodeStreamSettings(Dictionary<string, string> ps)
        {
            switch (streamType)
            {
                case "grpc":
                    ps["serviceName"] = streamParam2;
                    ps["mode"] = streamParam1 == @"false" ? "gun" : "multi";
                    ps["authority"] = streamParam3;
                    // 不知道guna怎么配置T.T
                    break;
                case "splithttp":
                case "httpupgrade":
                case "ws":
                case "h2":
                    if (!string.IsNullOrWhiteSpace(streamParam1))
                    {
                        ps["path"] = streamParam1;
                    }
                    if (!string.IsNullOrWhiteSpace(streamParam2))
                    {
                        ps["host"] = streamParam2;
                    }
                    break;
                case "xhttp":
                    ps["mode"] = streamParam1;
                    if (!string.IsNullOrWhiteSpace(streamParam2))
                    {
                        ps["path"] = streamParam2;
                    }
                    if (!string.IsNullOrWhiteSpace(streamParam3))
                    {
                        ps["host"] = streamParam3;
                    }
                    break;
                case "kcp":
                    if (!string.IsNullOrWhiteSpace(streamParam1))
                    {
                        ps["headerType"] = streamParam1;
                    }
                    if (!string.IsNullOrWhiteSpace(streamParam2))
                    {
                        ps["seed"] = streamParam2;
                    }
                    break;
                case "quic":
                    if (!string.IsNullOrWhiteSpace(streamParam2))
                    {
                        ps["quicSecurity"] = streamParam2;
                    }
                    if (!string.IsNullOrWhiteSpace(streamParam3))
                    {
                        ps["key"] = streamParam3;
                    }
                    if (!string.IsNullOrWhiteSpace(streamParam1))
                    {
                        ps["headerType"] = streamParam1;
                    }
                    break;
                default:
                    break;
            }
        }

        string EncodeToUriShareLink()
        {
            var ps = new Dictionary<string, string>();
            EncodeTlsSettings(ps);
            ps["flow"] = auth2;
            EncodeStreamSettings(ps);

            var pms = ps.Where(kv => !string.IsNullOrEmpty(kv.Value))
                .Select(kv => string.Format("{0}={1}", kv.Key, Uri.EscapeDataString(kv.Value)))
                .ToList();

            var url = string.Format(
                "{0}://{1}@{2}:{3}?{4}#{5}",
                proto,
                Uri.EscapeDataString(auth1),
                Misc.Utils.FormatHost(host),
                port,
                string.Join("&", pms),
                Uri.EscapeDataString(name)
            );
            return url;
        }

        void EncodeTlsSettings(Dictionary<string, string> ps)
        {
            ps["type"] = streamType;
            ps["security"] = tlsType;
            ps["fp"] = tlsFingerPrint;
            ps["alpn"] = tlsAlpn;

            if (tlsType == "tls")
            {
                ps["ech"] = tlsParam1;
            }
            else
            {
                ps["pbk"] = tlsParam1;
            }

            ps["sid"] = tlsParam2;
            ps["spx"] = tlsParam3;
            ps["pqv"] = tlsParam4;

            if (!string.IsNullOrWhiteSpace(tlsServName))
            {
                ps["sni"] = tlsServName;
            }
        }
        #endregion

        #region protected methods
        protected void CopyFrom(SharelinkMetaData source)
        {
            name = source.name;
            proto = source.proto;
            host = source.host;
            port = source.port;
            auth1 = source.auth1;
            auth2 = source.auth2;

            streamType = source.streamType;
            streamParam1 = source.streamParam1;
            streamParam2 = source.streamParam2;
            streamParam3 = source.streamParam3;

            tlsType = source.tlsType;
            useSelfSignCert = source.useSelfSignCert;
            tlsServName = source.tlsServName;

            tlsAlpn = source.tlsAlpn;
            tlsFingerPrint = source.tlsFingerPrint;

            tlsParam1 = source.tlsParam1;
            tlsParam2 = source.tlsParam2;
            tlsParam3 = source.tlsParam3;
            tlsParam4 = source.tlsParam4;
        }

        #endregion
    }
}
