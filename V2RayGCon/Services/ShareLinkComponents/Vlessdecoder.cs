using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace V2RayGCon.Services.ShareLinkComponents
{
    internal sealed class VlessDecoder :
        VgcApis.BaseClasses.ComponentOf<Codecs>,
        VgcApis.Interfaces.IShareLinkDecoder
    {
        readonly VeeDecoder veeDecoder;
        public VlessDecoder(VeeDecoder veeDecoder)
        {
            this.veeDecoder = veeDecoder;
        }

        #region properties

        #endregion

        #region public methods
        public Tuple<JObject, JToken> Decode(string shareLink)
        {
            try
            {
                var vc = ParseVlessUrl(shareLink);
                if (vc != null)
                {
                    var vee = vc.ToVeeShareLink();
                    return veeDecoder.Decode(vee);
                }
            }
            catch { }
            return null;
        }


        public string Encode(string config)
        {
            var vc = new Models.Datas.VeeConfigs(config);
            if (vc.proto != @"vless")
            {
                return null;
            }

            var ps = new Dictionary<string, string>();
            ps["type"] = vc.streamType;
            ps["security"] = vc.tlsType;

            if (!string.IsNullOrWhiteSpace(vc.auth2))
            {
                ps["flow"] = vc.auth2;
            }

            if (!string.IsNullOrWhiteSpace(vc.tlsServName))
            {
                ps["sni"] = vc.tlsServName;
            }

            switch (vc.streamType)
            {
                case "grpc":
                    ps["serviceName"] = vc.streamParam2;
                    ps["mode"] = vc.streamParam1 == @"false" ? "gun" : "multi";
                    // 不知道guna怎么配置T.T
                    break;
                case "ws":
                case "h2":
                    if (!string.IsNullOrWhiteSpace(vc.streamParam1))
                    {
                        ps["path"] = vc.streamParam1;
                    }
                    if (!string.IsNullOrWhiteSpace(vc.streamParam2))
                    {
                        ps["host"] = vc.streamParam2;
                    }
                    break;
                case "kcp":
                    if (!string.IsNullOrWhiteSpace(vc.streamParam1))
                    {
                        ps["headerType"] = vc.streamParam1;
                    }
                    if (!string.IsNullOrWhiteSpace(vc.streamParam2))
                    {
                        ps["seed"] = vc.streamParam2;
                    }
                    break;
                case "quic":
                    if (!string.IsNullOrWhiteSpace(vc.streamParam2))
                    {
                        ps["quicSecurity"] = vc.streamParam2;
                    }
                    if (!string.IsNullOrWhiteSpace(vc.streamParam3))
                    {
                        ps["key"] = vc.streamParam3;
                    }
                    if (!string.IsNullOrWhiteSpace(vc.streamParam1))
                    {
                        ps["headerType"] = vc.streamParam1;
                    }
                    break;
                default:
                    break;
            }

            var pms = ps
                .Select(kv => string.Format("{0}={1}", kv.Key, Uri.EscapeDataString(kv.Value)))
                .ToList();

            var url = string.Format(
                "{0}://{1}@{2}:{3}?{4}#{5}",
                vc.proto,
                Uri.EscapeDataString(vc.auth1),
                Uri.EscapeDataString(vc.host),
                vc.port,
                string.Join("&", pms),
                Uri.EscapeDataString(vc.name));
            return url;
        }

        public List<string> ExtractLinksFromText(string text) =>
           Misc.Utils.ExtractLinks(
               text,
               VgcApis.Models.Datas.Enums.LinkTypes.vless);
        #endregion

        #region private methods


        Models.Datas.VeeConfigs ParseVlessUrl(string url)
        {
            var proto = "vless";
            var header = proto + "://";

            if (!url.StartsWith(header))
            {
                return null;
            }

            // 抄袭自： https://github.com/musva/V2RayW/commit/e54f387e8d8181da833daea8464333e41f0f19e6 GPLv3
            List<string> parts = url
                .Substring(header.Length)
                .Split(new char[6] { ':', '@', '?', '&', '#', '=' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(s => Uri.UnescapeDataString(s))
                .ToList();

            if (parts.Count < 5)
            {
                return null;
            }

            var vc = new Models.Datas.VeeConfigs();
            vc.name = parts.Last();
            vc.proto = proto;
            vc.host = parts[1];
            vc.port = VgcApis.Misc.Utils.Str2Int(parts[2]);
            vc.auth1 = parts[0];

            string GetValue(string key, string def)
            {
                return parts.Contains(key) ? parts[parts.IndexOf(key) + 1] : def;
            }

            if (parts.Contains("flow"))
            {
                vc.auth2 = parts[parts.IndexOf("flow") + 1];
            }

            vc.streamType = GetValue("type", "tcp");

            vc.tlsType = GetValue("security", "none");
            vc.tlsServName = GetValue("sni", parts[1]);

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
                    if (parts.Contains("seed"))
                    {
                        vc.streamParam2 = parts[parts.IndexOf("seed") + 1];
                    }
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
        #endregion

        #region protected methods

        #endregion
    }
}
