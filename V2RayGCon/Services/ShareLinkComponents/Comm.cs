using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace V2RayGCon.Services.ShareLinkComponents
{
    public static class Comm
    {
        #region public methods
        public static Models.Datas.SharelinkMetadata ParseNonStandarUriShareLink(
            string proto,
            string url
        )
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

            var vc = new Models.Datas.SharelinkMetadata
            {
                name = parts.Last(),
                proto = proto,
                auth1 = parts[0]
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
            vc.streamType = GetValue("type", "tcp");
            vc.tlsType = GetValue("security", "none");
            vc.tlsServName = GetValue("sni", parts[1]);
            vc.tlsFingerPrint = GetValue("fp", "");
            vc.tlsAlpn = Uri.UnescapeDataString(GetValue("alpn", ""));
            vc.tlsParam1 = GetValue("pbk", "");
            vc.tlsParam2 = GetValue("sid", "");
            vc.tlsParam3 = Uri.UnescapeDataString(GetValue("spx", ""));

            switch (vc.streamType)
            {
                case "grpc":
                    vc.streamParam2 = GetValue("serviceName", @"");
                    vc.streamParam1 = (GetValue("mode", @"multi") == "multi").ToString().ToLower();
                    // 不知道guna怎么配置T.T
                    break;
                case "ws":
                case "h2":
                    vc.streamParam2 = GetValue("host", parts[1]);
                    vc.streamParam1 = GetValue("path", "/");
                    break;
                case "tcp":
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

        public static JToken GenStreamSetting(Models.Datas.SharelinkMetadata streamSettings)
        {
            var ss = streamSettings;
            // insert stream type
            string[] streamTypes = { "ws", "tcp", "kcp", "h2", "quic", "grpc" };
            string st = ss?.streamType?.ToLower();

            if (!streamTypes.Contains(st))
            {
                return JToken.Parse(@"{}");
            }

            string mainParam = ss.streamParam1;
            if (st == "tcp" && mainParam == "http")
            {
                st = "tcp_http";
            }
            var token = Misc.Caches.Jsons.LoadTemplate(st);
            try
            {
                FillInStreamSetting(ss, st, mainParam, token);
                FillInTlsSetting(ss, token);
            }
            catch { }

            return token;
        }

        public static Models.Datas.SharelinkMetadata ExtractFromJsonConfig(JObject config)
        {
            var GetStr = VgcApis.Misc.Utils.GetStringByPrefixAndKeyHelper(config);

            var root = "outbounds.0";
            var proto = GetStr(root, "protocol")?.ToLower();
            var isUseV4 = true;
            if (string.IsNullOrEmpty(proto))
            {
                root = "outbound";
                isUseV4 = false;
                proto = GetStr(root, "protocol")?.ToLower();
            }

            if (string.IsNullOrEmpty(proto))
            {
                return null;
            }

            var key = "servers";
            switch (proto)
            {
                case "vless":
                case "vmess":
                    key = "vnext";
                    break;
            }

            var mainPrefix = root + "." + $"settings.{key}.0";

            var result = new Models.Datas.SharelinkMetadata
            {
                proto = proto,
                host = GetStr(mainPrefix, "address"),
                port = VgcApis.Misc.Utils.Str2Int(GetStr(mainPrefix, "port")),
            };

            ExtractFirstOutboundFromJsonConfig(result, config, mainPrefix, proto);
            ExtractStreamSettings(result, config, isUseV4, root);
            return result;
        }
        #endregion

        #region private methods


        static void ExtractFirstOutboundFromJsonConfig(
            Models.Datas.SharelinkMetadata result,
            JObject config,
            string prefix,
            string protocol
        )
        {
            // var mainPrefix = root + "." + $"settings.{key}.0";
            var GetStr = VgcApis.Misc.Utils.GetStringByPrefixAndKeyHelper(config);
            switch (protocol)
            {
                case "vmess":
                    prefix += ".users.0";
                    result.auth1 = GetStr(prefix, "id");
                    break;
                case "vless":
                    prefix += ".users.0";
                    result.auth1 = GetStr(prefix, "id");
                    result.auth2 = GetStr(prefix, "flow");
                    break;
                case "trojan":
                    result.auth1 = GetStr(prefix, "password");
                    result.auth2 = GetStr(prefix, "flow");
                    break;
                case "shadowsocks":
                    result.auth1 = GetStr(prefix, "password");
                    result.auth2 = GetStr(prefix, "method");
                    break;
                case "socks":
                case "http":
                    prefix += ".users.0";
                    result.auth1 = GetStr(prefix, "user");
                    result.auth2 = GetStr(prefix, "pass");
                    break;
                default:
                    break;
            }
        }

        private static void FillInTlsSetting(Models.Datas.SharelinkMetadata ss, JToken token)
        {
            var tt = string.IsNullOrEmpty(ss.tlsType) ? "none" : ss.tlsType;
            token["security"] = tt;
            if (tt == "none")
            {
                return;
            }

            var o = new JObject();

            void SetValue(string key, string value)
            {
                if (!string.IsNullOrEmpty(value))
                {
                    o[key] = value;
                }
            }

            SetValue("serverName", ss.tlsServName);
            SetValue("fingerprint", ss.tlsFingerPrint);

            if (!string.IsNullOrEmpty(ss.tlsAlpn))
            {
                o["alpn"] = VgcApis.Misc.Utils.Str2JArray(ss.tlsAlpn);
            }

            if (tt == "reality")
            {
                o["publicKey"] = ss.tlsParam1;
                SetValue("shortId", ss.tlsParam2);
                SetValue("spiderX", ss.tlsParam3);
            }

            var k = $"{tt}Settings";
            token[k] = o;
        }

        private static void FillInStreamSetting(
            Models.Datas.SharelinkMetadata ss,
            string st,
            string mainParam,
            JToken token
        )
        {
            switch (st)
            {
                case "grpc":
                    token["grpcSettings"]["multiMode"] = mainParam.ToLower() == "true";
                    token["grpcSettings"]["serviceName"] = ss.streamParam2;
                    break;
                case "tcp":
                    if (string.IsNullOrEmpty(mainParam) || mainParam == "none")
                    {
                        token["tcpSettings"] = new JObject();
                    }
                    else
                    {
                        token["tcpSettings"]["header"]["type"] = mainParam;
                    }
                    break;
                case "tcp_http":
                    token["tcpSettings"]["header"]["type"] = mainParam;
                    token["tcpSettings"]["header"]["request"]["path"] =
                        VgcApis.Misc.Utils.Str2JArray(
                            string.IsNullOrEmpty(ss.streamParam2) ? "/" : ss.streamParam2
                        );
                    token["tcpSettings"]["header"]["request"]["headers"]["Host"] =
                        VgcApis.Misc.Utils.Str2JArray(ss.streamParam3);
                    break;
                case "kcp":
                    token["kcpSettings"]["header"]["type"] = mainParam;
                    if (!string.IsNullOrEmpty(ss.streamParam2))
                    {
                        token["kcpSettings"]["seed"] = ss.streamParam2;
                    }
                    break;
                case "ws":
                    token["wsSettings"]["path"] = mainParam;
                    token["wsSettings"]["headers"]["Host"] = ss.streamParam2;
                    break;
                case "h2":
                    token["httpSettings"]["path"] = mainParam;
                    token["httpSettings"]["host"] = VgcApis.Misc.Utils.Str2JArray(ss.streamParam2);
                    break;
                case "quic":
                    token["quicSettings"]["header"]["type"] = mainParam;
                    token["quicSettings"]["security"] = ss.streamParam2;
                    token["quicSettings"]["key"] = ss.streamParam3;
                    break;
                default:
                    break;
            }
        }

        static void ExtractStreamSettings(
            Models.Datas.SharelinkMetadata result,
            JObject config,
            bool isUseV4,
            string root
        )
        {
            var GetStr = VgcApis.Misc.Utils.GetStringByPrefixAndKeyHelper(config);

            var subPrefix = root + "." + "streamSettings";
            result.streamType = GetStr(subPrefix, "network");

            ExtractTlsSettings(result, config, GetStr, subPrefix);

            var mainParam = "";
            switch (result.streamType)
            {
                case "grpc":
                    mainParam = GetStr(subPrefix, "grpcSettings.multiMode").ToLower();
                    result.streamParam2 = GetStr(subPrefix, "grpcSettings.serviceName");
                    break;
                case "tcp":
                    mainParam = GetStr(subPrefix, "tcpSettings.header.type");
                    if (mainParam?.ToLower() == "http")
                    {
                        ExtractTcpHttpSettings(result, config, isUseV4);
                    }
                    break;
                case "kcp":
                    mainParam = GetStr(subPrefix, "kcpSettings.header.type");
                    result.streamParam2 = GetStr(subPrefix, "kcpSettings.seed");
                    break;
                case "ws":
                    mainParam = GetStr(subPrefix, "wsSettings.path");
                    result.streamParam2 = GetStr(subPrefix, "wsSettings.headers.Host");
                    break;
                case "h2":
                    mainParam = GetStr(subPrefix, "httpSettings.path");
                    try
                    {
                        var hosts = isUseV4
                            ? config["outbounds"][0]["streamSettings"]["httpSettings"]["host"]
                            : config["outbound"]["streamSettings"]["httpSettings"]["host"];
                        result.streamParam2 = VgcApis.Misc.Utils.JArray2Str(hosts as JArray);
                    }
                    catch { }
                    break;
                case "quic":
                    mainParam = GetStr(subPrefix, "quicSettings.header.type");
                    result.streamParam2 = GetStr(subPrefix, "quicSettings.security");
                    result.streamParam3 = GetStr(subPrefix, "quicSettings.key");
                    break;
                default:
                    break;
            }
            result.streamParam1 = mainParam;
        }

        static void ExtractTcpHttpSettings(
            Models.Datas.SharelinkMetadata result,
            JObject json,
            bool isUseV4
        )
        {
            try
            {
                var path = isUseV4
                    ? json["outbounds"][0]["streamSettings"]["tcpSettings"]["header"]["request"][
                        "path"
                    ]
                    : json["outbound"]["streamSettings"]["tcpSettings"]["header"]["request"][
                        "path"
                    ];
                result.streamParam2 = VgcApis.Misc.Utils.JArray2Str(path as JArray);
            }
            catch { }
            try
            {
                var hosts = isUseV4
                    ? json["outbounds"][0]["streamSettings"]["tcpSettings"]["header"]["request"][
                        "headers"
                    ]["Host"]
                    : json["outbound"]["streamSettings"]["tcpSettings"]["header"]["request"][
                        "headers"
                    ]["Host"];
                result.streamParam3 = VgcApis.Misc.Utils.JArray2Str(hosts as JArray);
            }
            catch { }
        }

        static void ExtractTlsSettings(
            Models.Datas.SharelinkMetadata result,
            JObject config,
            Func<string, string, string> reader,
            string prefix
        )
        {
            var tt = reader(prefix, "security")?.ToLower();
            result.tlsType = tt;

            if (tt != "xtls" && tt != "tls" && tt != "reality")
            {
                return;
            }

            var ts = $"{tt}Settings";
            result.tlsServName = reader(prefix, $"{ts}.serverName") ?? "";
            result.tlsFingerPrint = reader(prefix, $"{ts}.fingerprint") ?? "";

            try
            {
                // do not support v3.x config
                var alpn = config["outbounds"][0]["streamSettings"][ts]["alpn"];
                result.tlsAlpn = VgcApis.Misc.Utils.JArray2Str(alpn as JArray);
            }
            catch { }

            if (tt == "reality")
            {
                result.tlsParam1 = reader(prefix, $"{ts}.publicKey") ?? "";
                result.tlsParam2 = reader(prefix, $"{ts}.shortId") ?? "";
                result.tlsParam3 = reader(prefix, $"{ts}.spiderX") ?? "";
            }
        }
        #endregion
    }
}
