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
            result.isUseTls = GetStr(subPrefix, "security")?.ToLower() == "tls";
            result.isSecTls = GetStr(subPrefix, "tlsSettings.allowInsecure")?.ToLower() != "true";

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
            // insert stream type
            string[] streamTypes = { "ws", "tcp", "kcp", "h2", "quic" };
            string st = streamSettings?.streamType?.ToLower();

            if (!streamTypes.Contains(st))
            {
                return JToken.Parse(@"{}");
            }

            string mainParam = streamSettings.streamParam1;
            if (st == "tcp" && mainParam == "http")
            {
                st = "tcp_http";
            }
            var streamToken = cache.tpl.LoadTemplate(st);
            try
            {
                switch (st)
                {
                    case "tcp":
                        streamToken["tcpSettings"]["header"]["type"] = mainParam;
                        break;
                    case "tcp_http":
                        streamToken["tcpSettings"]["header"]["type"] = mainParam;
                        streamToken["tcpSettings"]["header"]["request"]["path"] =
                            Misc.Utils.Str2JArray(string.IsNullOrEmpty(streamSettings.streamParam2) ? "/" : streamSettings.streamParam2);
                        streamToken["tcpSettings"]["header"]["request"]["headers"]["Host"] =
                            Misc.Utils.Str2JArray(streamSettings.streamParam3);
                        break;
                    case "kcp":
                        streamToken["kcpSettings"]["header"]["type"] = mainParam;
                        if (!string.IsNullOrEmpty(streamSettings.streamParam2))
                        {
                            streamToken["kcpSettings"]["seed"] = streamSettings.streamParam2;
                        }
                        break;
                    case "ws":
                        streamToken["wsSettings"]["path"] = mainParam;
                        streamToken["wsSettings"]["headers"]["Host"] = streamSettings.streamParam2;
                        break;
                    case "h2":
                        streamToken["httpSettings"]["path"] = mainParam;
                        streamToken["httpSettings"]["host"] = Misc.Utils.Str2JArray(streamSettings.streamParam2);
                        break;
                    case "quic":
                        streamToken["quicSettings"]["header"]["type"] = mainParam;
                        streamToken["quicSettings"]["security"] = streamSettings.streamParam2;
                        streamToken["quicSettings"]["key"] = streamSettings.streamParam3;
                        break;
                    default:
                        break;
                }
            }
            catch { }

            streamToken["security"] = streamSettings.isUseTls ? "tls" : "none";

            if (streamSettings.isUseTls)
            {
                streamToken["tlsSettings"] = streamSettings.isSecTls ?
                     JObject.Parse(@"{allowInsecure: false}") :
                     JObject.Parse(@"{allowInsecure: true}");
            }

            return streamToken;
        }
    }
}
