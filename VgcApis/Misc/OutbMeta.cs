using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using VgcApis.Models.Datas;

namespace VgcApis.Misc
{
    public static class OutbMeta
    {
        #region public methods
        public static bool TryParseConfig(string config, out SharelinkMetaData meta)
        {
            try
            {
                var json = Utils.ExtractTrimedRoutingAndOutbound(config);
                meta = ExtractMetaFromOutboundConfig(json);
                return true;
            }
            catch { }
            meta = null;
            return false;
        }

        public static SharelinkMetaData ParseHy2ShareLink(string proto, string url)
        {
            var header = proto + "://";

            if (!url.StartsWith(header))
            {
                return null;
            }

            // 抄袭自： https://github.com/musva/V2RayW/commit/e54f387e8d8181da833daea8464333e41f0f19e6 GPLv3
            List<string> parts = url.Substring(header.Length)
                .Replace("/?", "?")
                .Split(new char[] { '@', '?', '&', '#', '=' })
                .Select(s => Uri.UnescapeDataString(s))
                .ToList();

            if (parts.Count < 2)
            {
                return null;
            }

            var hy = "hysteria";

            var vc = new SharelinkMetaData
            {
                name = url.Contains("#") ? parts.Last() : "",
                proto = hy,
                streamType = hy,
                streamParam1 = parts[0],
            };

            var addr = parts[1];
            if (!VgcApis.Misc.Utils.TryParseAddress(addr, out vc.host, out vc.port))
            {
                return null;
            }

            string GetValue(string key, string def)
            {
                if (parts.Contains(key))
                {
                    var v = parts[parts.IndexOf(key) + 1];
                    if (!string.IsNullOrEmpty(v))
                    {
                        return v;
                    }
                }
                return def;
            }

            vc.tlsType = "tls";
            vc.tlsAlpn = "h3";
            vc.tlsServName = GetValue("sni", "");
            vc.tlsParam2 = GetValue("pcs", "");

            return vc;
        }

        public static SharelinkMetaData ParseNonStandarUriShareLink(string proto, string url)
        {
            var header = proto + "://";

            if (!url.StartsWith(header))
            {
                return null;
            }

            // 抄袭自： https://github.com/musva/V2RayW/commit/e54f387e8d8181da833daea8464333e41f0f19e6 GPLv3
            List<string> parts = url.Substring(header.Length)
                .Replace("/?", "?")
                .Split(new char[] { '@', '?', '&', '#', '=' })
                .Select(s => Uri.UnescapeDataString(s))
                .ToList();

            if (parts.Count < 2)
            {
                return null;
            }

            var vc = new SharelinkMetaData
            {
                name = url.Contains("#") ? parts.Last() : "",
                proto = proto,
                auth1 = parts[0],
            };

            var addr = parts[1];
            if (!VgcApis.Misc.Utils.TryParseAddress(addr, out vc.host, out vc.port))
            {
                return null;
            }

            string GetValue(string key, string def)
            {
                if (parts.Contains(key))
                {
                    var v = parts[parts.IndexOf(key) + 1];
                    if (!string.IsNullOrEmpty(v))
                    {
                        return v;
                    }
                }
                return def;
            }

            vc.auth2 = GetValue("flow", "");
            vc.auth3 = GetValue("encryption", "none");
            vc.streamType = GetValue("type", "tcp");
            vc.tlsType = GetValue("security", "none");
            vc.tlsServName = GetValue("sni", "");
            vc.tlsFingerPrint = GetValue("fp", "");
            vc.tlsAlpn = Uri.UnescapeDataString(GetValue("alpn", ""));
            vc.tlsParam1 = GetValue("pbk", "");
            vc.tlsParam2 = GetValue("sid", "");
            vc.tlsParam3 = Uri.UnescapeDataString(GetValue("spx", ""));
            vc.tlsParam4 = Uri.UnescapeDataString(GetValue("pqv", ""));
            if (vc.tlsType == "tls")
            {
                vc.tlsParam1 = GetValue("ech", "");
                vc.tlsParam2 = GetValue("pcs", "");
            }

            // patch sni
            var hostIsDomain = !VgcApis.Misc.Utils.TryParseIp(vc.host, out _);
            if (string.IsNullOrEmpty(vc.tlsServName) && hostIsDomain)
            {
                vc.tlsServName = vc.host;
            }

            switch (vc.streamType)
            {
                case "grpc":
                    vc.streamParam3 = GetValue("authority", @"");
                    vc.streamParam2 = GetValue("serviceName", @"");
                    vc.streamParam1 = (GetValue("mode", @"multi") == "multi").ToString().ToLower();
                    // 不知道guna怎么配置T.T
                    break;
                case "ws":
                case "h2":
                case "httpupgrade":
                case "splithttp":
                    vc.streamParam1 = GetValue("path", "/");
                    vc.streamParam2 = GetValue("host", "");
                    if (string.IsNullOrEmpty(vc.streamParam2) && hostIsDomain)
                    {
                        vc.streamParam2 = vc.host;
                    }
                    break;
                case "xhttp":
                    vc.streamParam1 = GetValue("mode", "auto");
                    vc.streamParam2 = GetValue("path", "/");
                    vc.streamParam3 = GetValue("host", "");
                    if (string.IsNullOrEmpty(vc.streamParam3) && hostIsDomain)
                    {
                        vc.streamParam3 = vc.host;
                    }
                    break;
                case "tcp":
                case "raw":
                    // 木有 tcp.http ??
                    vc.streamParam1 = "none";
                    break;
                case "kcp":
                    vc.streamParam1 = GetValue("headerType", "none");
                    vc.streamParam2 = GetValue("seed", "");
                    break;
                case "quic":
                    vc.streamParam2 = GetValue("quicSecurity", "none");
                    vc.streamParam3 = GetValue("key", "");
                    vc.streamParam1 = GetValue("headerType", "none");
                    break;
                default:
                    break;
            }

            return vc;
        }

        public static SharelinkMetaData ExtractMetaFromOutboundConfig(JObject config)
        {
            var GetStr = Utils.GetStringByPrefixAndKeyHelper(config);

            var root = "outbounds.0";
            var proto = GetStr(root, "protocol")?.ToLower();

            if (string.IsNullOrEmpty(proto))
            {
                return null;
            }

            var result = new SharelinkMetaData { proto = proto };
            var prefix = $"{root}.settings";
            var address = GetStr(prefix, "address");
            if (!string.IsNullOrEmpty(address))
            {
                // simple config
                result.host = address;
                result.port = Utils.Str2Int(GetStr(prefix, "port"));
                ExtractFirstOutboundInfoFromXraySimpleConfig(result, GetStr, prefix, proto);
            }
            else
            {
                // old config
                var key = "servers";
                switch (proto)
                {
                    case "vless":
                    case "vmess":
                        key = "vnext";
                        break;
                }

                var protoPrefix = $"{prefix}.{key}.0";
                result.host = GetStr(protoPrefix, "address");
                result.port = Utils.Str2Int(GetStr(protoPrefix, "port"));
                ExtractFirstOutboundInfoFromJsonV4Config(result, GetStr, protoPrefix, proto);
            }

            ExtractStreamSettings(result, config, root);
            return result;
        }
        #endregion

        #region private methods
        static void ExtractFirstOutboundInfoFromXraySimpleConfig(
            SharelinkMetaData meta,
            Func<string, string, string> GetStr,
            string prefix,
            string protocol
        )
        {
            // var mainPrefix = root + "." + $"settings";
            switch (protocol)
            {
                case "vmess":
                    meta.auth1 = GetStr(prefix, "id");
                    break;
                case "vless":
                    meta.auth1 = GetStr(prefix, "id");
                    meta.auth2 = GetStr(prefix, "flow");
                    meta.auth3 = GetStr(prefix, "encryption");
                    break;
                case "trojan":
                    meta.auth1 = GetStr(prefix, "password");
                    meta.auth2 = GetStr(prefix, "flow");
                    break;
                case "shadowsocks":
                    meta.auth1 = GetStr(prefix, "password");
                    meta.auth2 = GetStr(prefix, "method");
                    break;
                case "socks":
                case "http":
                    meta.auth1 = GetStr(prefix, "user");
                    meta.auth2 = GetStr(prefix, "pass");
                    break;
                default:
                    break;
            }
        }

        static void ExtractFirstOutboundInfoFromJsonV4Config(
            SharelinkMetaData meta,
            Func<string, string, string> GetStr,
            string prefix,
            string protocol
        )
        {
            // var mainPrefix = root + "." + $"settings.{key}.0";
            switch (protocol)
            {
                case "vmess":
                    prefix += ".users.0";
                    meta.auth1 = GetStr(prefix, "id");
                    break;
                case "vless":
                    prefix += ".users.0";
                    meta.auth1 = GetStr(prefix, "id");
                    meta.auth2 = GetStr(prefix, "flow");
                    meta.auth3 = GetStr(prefix, "encryption");
                    break;
                case "trojan":
                    meta.auth1 = GetStr(prefix, "password");
                    meta.auth2 = GetStr(prefix, "flow");
                    break;
                case "shadowsocks":
                    meta.auth1 = GetStr(prefix, "password");
                    meta.auth2 = GetStr(prefix, "method");
                    break;
                case "socks":
                case "http":
                    prefix += ".users.0";
                    meta.auth1 = GetStr(prefix, "user");
                    meta.auth2 = GetStr(prefix, "pass");
                    break;
                default:
                    break;
            }
        }

        static void ExtractStreamSettings(SharelinkMetaData meta, JObject config, string root)
        {
            var GetStr = VgcApis.Misc.Utils.GetStringByPrefixAndKeyHelper(config);

            var subPrefix = root + "." + "streamSettings";
            meta.streamType = GetStr(subPrefix, "network");

            ExtractTlsSettings(meta, config, GetStr, subPrefix);

            var mainParam = "";
            switch (meta.streamType)
            {
                case "hysteria":
                    mainParam = GetStr(subPrefix, "hysteriaSettings.auth");
                    break;
                case "grpc":
                    mainParam = GetStr(subPrefix, "grpcSettings.multiMode").ToLower();
                    meta.streamParam2 = GetStr(subPrefix, "grpcSettings.serviceName");
                    meta.streamParam3 = GetStr(subPrefix, "grpcSettings.authority");
                    break;
                case "tcp":
                case "raw":
                    var tkey = $"{meta.streamType}Settings";
                    mainParam = GetStr(subPrefix, $"{tkey}Settings.header.type");
                    if (mainParam?.ToLower() == "http")
                    {
                        ExtractTcpHttpSettings(meta, config, tkey);
                    }
                    break;
                case "kcp":
                    mainParam = GetStr(subPrefix, "kcpSettings.header.type");
                    meta.streamParam2 = GetStr(subPrefix, "kcpSettings.seed");
                    break;
                case "ws":
                    mainParam = GetStr(subPrefix, "wsSettings.path");
                    var wsHost = GetStr(subPrefix, "wsSettings.host");
                    if (string.IsNullOrEmpty(wsHost))
                    {
                        wsHost = GetStr(subPrefix, "wsSettings.headers.Host");
                    }
                    meta.streamParam2 = wsHost;
                    break;
                case "httpupgrade":
                case "splithttp":
                    mainParam = GetStr(subPrefix, $"{meta.streamType}Settings.path");
                    meta.streamParam2 = GetStr(subPrefix, $"{meta.streamType}Settings.host");
                    break;
                case "xhttp":
                    mainParam = GetStr(subPrefix, $"{meta.streamType}Settings.mode");
                    meta.streamParam2 = GetStr(subPrefix, $"{meta.streamType}Settings.path");
                    meta.streamParam3 = GetStr(subPrefix, $"{meta.streamType}Settings.host");
                    break;
                case "h2":
                    mainParam = GetStr(subPrefix, "httpSettings.path");
                    try
                    {
                        var hosts = config["outbounds"][0]["streamSettings"]["httpSettings"][
                            "host"
                        ];
                        meta.streamParam2 = VgcApis.Misc.Utils.JArray2Str(hosts as JArray);
                    }
                    catch { }
                    break;
                case "quic":
                    mainParam = GetStr(subPrefix, "quicSettings.header.type");
                    meta.streamParam2 = GetStr(subPrefix, "quicSettings.security");
                    meta.streamParam3 = GetStr(subPrefix, "quicSettings.key");
                    break;
                default:
                    break;
            }
            meta.streamParam1 = mainParam;
        }

        static void ExtractTcpHttpSettings(SharelinkMetaData meta, JObject json, string key)
        {
            try
            {
                var path = json["outbounds"][0]["streamSettings"][key]["header"]["request"]["path"];
                meta.streamParam2 = VgcApis.Misc.Utils.JArray2Str(path as JArray);
            }
            catch { }
            try
            {
                var hosts = json["outbounds"][0]["streamSettings"][key]["header"]["request"][
                    "headers"
                ]["Host"];
                meta.streamParam3 = VgcApis.Misc.Utils.JArray2Str(hosts as JArray);
            }
            catch { }
        }

        static void ExtractTlsSettings(
            SharelinkMetaData meta,
            JObject config,
            Func<string, string, string> reader,
            string prefix
        )
        {
            var tt = reader(prefix, "security")?.ToLower();
            meta.tlsType = tt;

            if (tt != "xtls" && tt != "tls" && tt != "reality")
            {
                return;
            }

            var ts = $"{tt}Settings";
            meta.tlsServName = reader(prefix, $"{ts}.serverName") ?? "";
            meta.tlsFingerPrint = reader(prefix, $"{ts}.fingerprint") ?? "";

            if (tt == "tls")
            {
                meta.tlsParam1 = reader(prefix, $"{ts}.echConfigList") ?? "";
                meta.tlsParam2 = reader(prefix, $"{ts}.pinnedPeerCertSha256") ?? "";
            }

            try
            {
                // do not support v3.x config
                var alpn = config["outbounds"][0]["streamSettings"][ts]["alpn"];
                meta.tlsAlpn = VgcApis.Misc.Utils.JArray2Str(alpn as JArray);
            }
            catch { }

            if (tt == "reality")
            {
                meta.tlsParam1 = reader(prefix, $"{ts}.publicKey") ?? "";
                meta.tlsParam2 = reader(prefix, $"{ts}.shortId") ?? "";
                meta.tlsParam3 = reader(prefix, $"{ts}.spiderX") ?? "";
                meta.tlsParam4 = reader(prefix, $"{ts}.mldsa65Verify") ?? "";
            }
        }
        #endregion
    }
}
