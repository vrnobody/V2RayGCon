using Newtonsoft.Json.Linq;
using System.Linq;

namespace V2RayGCon.Services.ShareLinkComponents.VeeCodecs
{
    public static class Comm
    {
        public static Models.VeeShareLinks.BasicSettings ExtractBasicConfig(
            JObject config, string protocol, string key, out bool isUseV4, out string root)
        {
            var GetStr = Misc.Utils.GetStringByPrefixAndKeyHelper(config);

            isUseV4 = (GetStr("outbounds.0", "protocol")?.ToLower()) == protocol;
            root = isUseV4 ? "outbounds.0" : "outbound";
            if (!isUseV4)
            {
                var p = GetStr(root, "protocol")?.ToLower();
                if (p == null || p != protocol)
                {
                    return null;
                }
            }

            var mainPrefix = root + "." + $"settings.{key}.0";

            var result = new Models.VeeShareLinks.BasicSettings
            {
                alias = GetStr("v2raygcon", "alias"),
                address = GetStr(mainPrefix, "address"),
                port = VgcApis.Misc.Utils.Str2Int(GetStr(mainPrefix, "port")),
                description = GetStr("v2raygcon", "description"),
            };

            FillInStreamSettings(result, config, isUseV4, root);

            return result;
        }

        static void FillInStreamSettings(
            Models.VeeShareLinks.BasicSettings result,
            JObject config, bool isUseV4, string root)
        {
            var GetStr = Misc.Utils.GetStringByPrefixAndKeyHelper(config);

            var subPrefix = root + "." + "streamSettings";
            result.streamType = GetStr(subPrefix, "network");

            var tt = GetStr(subPrefix, "security")?.ToLower();
            result.tlsType = tt;
            result.isUseTls = tt == "tls";
            if (tt == "xtls" || tt == "tls")
            {
                var pfx = $"{tt}Settings";
                result.isSecTls = GetStr(subPrefix, $"{pfx}.allowInsecure")?.ToLower() != "true";
            }

            var mainParam = "";
            switch (result.streamType)
            {
                case "tcp":
                    mainParam = GetStr(subPrefix, "tcpSettings.header.type");
                    if (mainParam?.ToLower() == "http")
                    {
                        ExtractTcpHttpSettings(config, isUseV4, result);
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
                        var hosts = isUseV4 ?
                            config["outbounds"][0]["streamSettings"]["httpSettings"]["host"] :
                            config["outbound"]["streamSettings"]["httpSettings"]["host"];
                        result.streamParam2 = Misc.Utils.JArray2Str(hosts as JArray);
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
            JObject json, bool isUseV4, Models.VeeShareLinks.BasicSettings streamSettings)
        {
            try
            {
                var path = isUseV4 ?
                    json["outbounds"][0]["streamSettings"]["tcpSettings"]["header"]["request"]["path"] :
                    json["outbound"]["streamSettings"]["tcpSettings"]["header"]["request"]["path"];
                streamSettings.streamParam2 = Misc.Utils.JArray2Str(path as JArray);
            }
            catch { }
            try
            {
                var hosts = isUseV4 ?
                    json["outbounds"][0]["streamSettings"]["tcpSettings"]["header"]["request"]["headers"]["Host"] :
                    json["outbound"]["streamSettings"]["tcpSettings"]["header"]["request"]["headers"]["Host"];
                streamSettings.streamParam3 = Misc.Utils.JArray2Str(hosts as JArray);
            }
            catch { }
        }

        public static JToken GenStreamSetting(
            Cache cache, Models.VeeShareLinks.BasicSettings streamSettings)
        {
            var ss = streamSettings;
            // insert stream type
            string[] streamTypes = { "ws", "tcp", "kcp", "h2", "quic" };
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
            var token = cache.tpl.LoadTemplate(st);
            try
            {
                switch (st)
                {
                    case "tcp":
                        token["tcpSettings"]["header"]["type"] = mainParam;
                        break;
                    case "tcp_http":
                        token["tcpSettings"]["header"]["type"] = mainParam;
                        token["tcpSettings"]["header"]["request"]["path"] =
                            Misc.Utils.Str2JArray(string.IsNullOrEmpty(ss.streamParam2) ? "/" : ss.streamParam2);
                        token["tcpSettings"]["header"]["request"]["headers"]["Host"] =
                            Misc.Utils.Str2JArray(ss.streamParam3);
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
                        token["httpSettings"]["host"] = Misc.Utils.Str2JArray(ss.streamParam2);
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
            catch { }

            if (ss.isUseTls)
            {
                // backward compatible
                token["security"] = "tls";
                token["tlsSettings"] = ss.isSecTls ?
                    JObject.Parse(@"{allowInsecure: false}") :
                    JObject.Parse(@"{allowInsecure: true}");

            }
            else
            {
                var tt = ss.tlsType;
                token["security"] = tt;
                if (tt == "tls" || tt == "xtls")
                {
                    var k = $"{tt}Settings";
                    var insecure = ss.isSecTls ? "false" : "true"; // lower case
                    var v = $"{{ allowInsecure: {insecure} }}";
                    token[k] = JObject.Parse(v);
                }
            }

            return token;
        }
    }
}
