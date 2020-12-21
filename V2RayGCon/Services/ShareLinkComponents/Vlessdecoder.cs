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
            /* 
             * trojan://password@remote_host:remote_port
             * in which the password is url-encoded in case it contains illegal characters.
             */

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
            throw new NotImplementedException();
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
            if (parts.Contains("flow"))
            {
                vc.auth2 = parts[parts.IndexOf("flow") + 1];
            }

            vc.streamType = parts.Contains("type") ? parts[parts.IndexOf("type") + 1] : "tcp";
            vc.tlsType = parts.Contains("security") ? parts[parts.IndexOf("security") + 1] : "none";
            vc.tlsServName = parts.Contains("sni") ? parts[parts.IndexOf("sni") + 1] : parts[1];

            switch (vc.streamType)
            {
                case "ws":
                case "h2":
                    vc.streamParam2 = parts.Contains("host") ? parts[parts.IndexOf("host") + 1] : parts[1];
                    vc.streamParam1 = parts.Contains("path") ? parts[parts.IndexOf("path") + 1] : "/";
                    break;
                case "tcp":
                    // 木有 tcp.http ??
                    vc.streamParam1 = "none";
                    break;
                case "kcp":
                    vc.streamParam1 = parts.Contains("headerType") ? parts[parts.IndexOf("headerType") + 1] : "none";
                    if (parts.Contains("seed"))
                    {
                        vc.streamParam2 = parts[parts.IndexOf("seed") + 1];
                    }
                    break;
                case "quic":
                    vc.streamParam2 = parts.Contains("quicSecurity") ? parts[parts.IndexOf("quicSecurity") + 1] : "none";
                    vc.streamParam3 = parts.Contains("key") ? parts[parts.IndexOf("key") + 1] : "";
                    vc.streamParam1 = parts.Contains("headerType") ? parts[parts.IndexOf("headerType") + 1] : "none";
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
