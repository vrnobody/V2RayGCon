using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace V2RayGCon.Services.ShareLinkComponents
{
    internal sealed class SocksDecoder
        : VgcApis.BaseClasses.ComponentOf<Codecs>,
            VgcApis.Interfaces.IShareLinkDecoder
    {
        public SocksDecoder() { }

        #region properties

        #endregion

        #region public methods
        public VgcApis.Models.Datas.DecodeResult Decode(string shareLink)
        {
            // socks://Og==@[fc00::1]:1080#uri%20encoded%20name
            var parts = shareLink.Split('#');
            if (parts == null || parts.Length < 1)
            {
                return null;
            }

            var outbound = SocksLink2Outbound(parts[0]);
            if (outbound == null)
            {
                return null;
            }

            var name = "";
            if (parts.Length > 1 && !string.IsNullOrEmpty(parts[1]))
            {
                name = Uri.UnescapeDataString(parts[1]);
            }

            var tpl = Misc.Caches.Jsons.LoadTemplate("tplLogWarn") as JObject;
            var config = GetParent()?.GenerateJsonConfing(tpl, outbound);
            return new VgcApis.Models.Datas.DecodeResult(name, config);
        }

        public string Encode(string name, string config)
        {
            if (
                Models.Datas.SharelinkMetadata.TryParseConfig(config, out var vc)
                && vc != null
                && vc.proto == "socks"
            )
            {
                vc.name = name;
                return vc.ToShareLink();
            }
            return null;
        }

        public List<string> ExtractLinksFromText(string text) =>
            Misc.Utils.ExtractLinks(text, VgcApis.Models.Datas.Enums.LinkTypes.socks);
        #endregion

        #region private methods
        JToken SocksLink2Outbound(string ssLink)
        {
            var body = Misc.Utils.GetLinkBody(ssLink);
            var parts = body?.Split(new char[] { '@' }, StringSplitOptions.RemoveEmptyEntries);

            if (parts == null || parts.Length < 1 || parts.Length > 2)
            {
                return null;
            }

            var outb = Misc.Caches.Jsons.LoadTemplate("outbSocks");
            var node = outb["settings"]["servers"][0];

            // host:port
            VgcApis.Misc.Utils.TryParseAddress(parts.Last(), out string ip, out int port);
            node["address"] = ip;
            node["port"] = port;

            if (parts.Length == 1)
            {
                return outb;
            }

            var s = parts[0].Contains(":")
                ? parts[0]
                : VgcApis.Misc.Utils.Base64DecodeToString(parts[0]);

            var auths = s?.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
            if (
                auths != null
                && auths.Length == 2
                && !(string.IsNullOrEmpty(auths[0]) && string.IsNullOrEmpty(auths[1]))
            )
            {
                node["users"] = new JArray
                {
                    new JObject { ["user"] = auths[0], ["pass"] = auths[1] }
                };
            }

            return outb;
        }
        #endregion

        #region protected methods

        #endregion
    }
}
