using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace V2RayGCon.Services.ShareLinkComponents
{
    internal sealed class SsDecoder
        : VgcApis.BaseClasses.ComponentOf<Codecs>,
            VgcApis.Interfaces.IShareLinkDecoder
    {
        Cache cache;

        public SsDecoder(Cache cache)
        {
            this.cache = cache;
        }

        #region properties

        #endregion

        #region public methods
        public string Decode(string shareLink)
        {
            // ss://(base64)#tag or ss://(base64)
            var parts = shareLink.Split('#');
            if (parts.Length > 2 || parts.Length < 1)
            {
                return null;
            }

            string body = Misc.Utils.GetLinkBody(parts[0]);

            var ss = Misc.Utils.TranslateSIP002Body(body);
            var outbound = SsLink2Outbound(ss);
            if (outbound == null)
            {
                return null;
            }

            var tpl = cache.tpl.LoadTemplate("tplImportSS") as JObject;
            if (parts.Length > 1 && !string.IsNullOrEmpty(parts[1]))
            {
                var name = Uri.UnescapeDataString(parts[1]);
                tpl["v2raygcon"]["alias"] = name;
            }

            return GetParent()?.GenerateJsonConfing(tpl, outbound);
        }

        public string Encode(string config)
        {
            var vc = new Models.Datas.SharelinkMetadata(config);
            if (vc.proto != @"shadowsocks")
            {
                return null;
            }

            var auth = string.Format("{0}:{1}", vc.auth2, vc.auth1);
            var userinfo = VgcApis.Misc.Utils
                .Base64EncodeString(auth)
                .Replace("=", "")
                .Replace('+', '-')
                .Replace('/', '_');

            var url = string.Format(
                "ss://{0}@{1}:{2}#{3}",
                userinfo,
                Uri.EscapeDataString(vc.host),
                vc.port,
                Uri.EscapeDataString(vc.name)
            );

            return url;
        }

        public List<string> ExtractLinksFromText(string text) =>
            Misc.Utils.ExtractLinks(text, VgcApis.Models.Datas.Enums.LinkTypes.ss);
        #endregion

        #region private methods
        JToken SsLink2Outbound(string ssLink)
        {
            Models.Datas.Shadowsocks ss = Misc.Utils.SsLink2Ss(ssLink);
            if (ss == null)
            {
                return null;
            }

            VgcApis.Misc.Utils.TryParseAddress(ss.addr, out string ip, out int port);
            var outbSs = cache.tpl.LoadTemplate("outbSs");
            var node = outbSs["settings"]["servers"][0];
            node["address"] = ip;
            node["port"] = port;
            node["method"] = ss.method;
            node["password"] = ss.pass;

            return outbSs;
        }
        #endregion

        #region protected methods

        #endregion
    }
}
