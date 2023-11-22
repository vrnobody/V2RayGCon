using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace V2RayGCon.Services.ShareLinkComponents
{
    internal sealed class SocksDecoder
        : VgcApis.BaseClasses.ComponentOf<Codecs>,
            VgcApis.Interfaces.IShareLinkDecoder
    {
        readonly Cache cache;

        public SocksDecoder(Cache cache)
        {
            this.cache = cache;
        }

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

            var tpl = cache.tpl.LoadTemplate("tplLogWarn") as JObject;
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

            if (parts == null || parts.Length != 2)
            {
                return null;
            }

            VgcApis.Misc.Utils.TryParseAddress(parts[1], out string ip, out int port);

            var outb = cache.tpl.LoadTemplate("outbSocks");
            var node = outb["settings"]["servers"][0];
            node["address"] = ip;
            node["port"] = port;

            var auths = VgcApis.Misc.Utils.Base64DecodeToString(parts[0])?.Split(':');
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
