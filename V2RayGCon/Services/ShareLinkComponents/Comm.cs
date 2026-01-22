using System;
using System.Linq;
using Newtonsoft.Json.Linq;
using VgcApis.Models.Datas;

namespace V2RayGCon.Services.ShareLinkComponents
{
    public static class Comm
    {
        #region public methods
        public static JToken GenStreamSetting(SharelinkMetaData meta)
        {
            // insert stream type
            string[] streamTypes =
            {
                "ws",
                "raw",
                "tcp",
                "kcp",
                "h2",
                "quic",
                "grpc",
                "httpupgrade",
                "splithttp",
                "xhttp",
                "hysteria",
            };
            string st = meta?.streamType?.ToLower();

            if (!streamTypes.Contains(st))
            {
                return JToken.Parse(@"{}");
            }

            string mainParam = meta.streamParam1;
            if (st == "tcp" && mainParam == "http")
            {
                st = "tcp_http";
            }
            var token = Misc.Caches.Jsons.LoadTemplate(st);
            try
            {
                FillInStreamSetting(meta, st, mainParam, token);
                FillInTlsSetting(meta, token);
            }
            catch { }

            return token;
        }
        #endregion

        #region private region
        private static void FillInTlsSetting(SharelinkMetaData meta, JToken token)
        {
            var tt = string.IsNullOrEmpty(meta.tlsType) ? "none" : meta.tlsType;
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

            SetValue("serverName", meta.tlsServName);
            SetValue("fingerprint", meta.tlsFingerPrint);
            if (tt == "tls")
            {
                SetValue("echConfigList", meta.tlsParam1);
                SetValue("pinnedPeerCertSha256", meta.tlsParam2);
            }

            if (!string.IsNullOrEmpty(meta.tlsAlpn))
            {
                o["alpn"] = VgcApis.Misc.Utils.Str2JArray(meta.tlsAlpn);
            }

            if (tt == "reality")
            {
                o["publicKey"] = meta.tlsParam1;
                SetValue("shortId", meta.tlsParam2);
                SetValue("spiderX", meta.tlsParam3);
                SetValue("mldsa65Verify", meta.tlsParam4);
            }

            var k = $"{tt}Settings";
            token[k] = o;
        }

        private static void FillInStreamSetting(
            SharelinkMetaData meta,
            string streamType,
            string mainParam,
            JToken token
        )
        {
            switch (streamType)
            {
                case "hysteria":
                    token["hysteriaSettings"]["auth"] = meta.streamParam1;
                    break;
                case "grpc":
                    var auth = meta.streamParam3;
                    if (!string.IsNullOrEmpty(auth))
                    {
                        token["grpcSettings"]["authority"] = auth;
                    }
                    token["grpcSettings"]["multiMode"] = mainParam.ToLower() == "true";
                    var sn = meta.streamParam2;
                    if (!string.IsNullOrEmpty(sn))
                    {
                        token["grpcSettings"]["serviceName"] = sn;
                    }
                    break;
                case "raw":
                case "tcp":
                    var tkey = $"{streamType}Settings";
                    if (string.IsNullOrEmpty(mainParam) || mainParam == "none")
                    {
                        token[tkey] = new JObject();
                    }
                    else
                    {
                        token[tkey]["header"]["type"] = mainParam;
                    }
                    break;
                case "tcp_http":
                    token["tcpSettings"]["header"]["type"] = mainParam;
                    token["tcpSettings"]["header"]["request"]["path"] =
                        VgcApis.Misc.Utils.Str2JArray(
                            string.IsNullOrEmpty(meta.streamParam2) ? "/" : meta.streamParam2
                        );
                    token["tcpSettings"]["header"]["request"]["headers"]["Host"] =
                        VgcApis.Misc.Utils.Str2JArray(meta.streamParam3);
                    break;
                case "kcp":
                    token["kcpSettings"]["header"]["type"] = mainParam;
                    if (!string.IsNullOrEmpty(meta.streamParam2))
                    {
                        token["kcpSettings"]["seed"] = meta.streamParam2;
                    }
                    break;
                case "httpupgrade":
                case "splithttp":
                    var hkey = $"{streamType}Settings";
                    token[hkey]["path"] = mainParam;
                    if (!string.IsNullOrEmpty(meta.streamParam2))
                    {
                        token[hkey]["host"] = meta.streamParam2;
                    }
                    break;
                case "xhttp":
                    var xkey = $"{streamType}Settings";
                    token[xkey]["mode"] = mainParam;
                    if (!string.IsNullOrEmpty(meta.streamParam2))
                    {
                        token[xkey]["path"] = meta.streamParam2;
                    }
                    if (!string.IsNullOrEmpty(meta.streamParam3))
                    {
                        token[xkey]["host"] = meta.streamParam3;
                    }
                    break;
                case "ws":
                    token["wsSettings"]["path"] = mainParam;
                    if (!string.IsNullOrEmpty(meta.streamParam2))
                    {
                        token["wsSettings"]["host"] = meta.streamParam2;
                        token["wsSettings"]["headers"]["Host"] = meta.streamParam2;
                    }
                    break;
                case "h2":
                    token["httpSettings"]["path"] = mainParam;
                    token["httpSettings"]["host"] = VgcApis.Misc.Utils.Str2JArray(
                        meta.streamParam2
                    );
                    break;
                case "quic":
                    token["quicSettings"]["header"]["type"] = mainParam;
                    token["quicSettings"]["security"] = meta.streamParam2;
                    token["quicSettings"]["key"] = meta.streamParam3;
                    break;
                default:
                    break;
            }
        }
        #endregion
    }
}
