using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace V2RayGCon.Service.ShareLinkComponents
{
    internal sealed class SsDecoder :
        VgcApis.Models.BaseClasses.ComponentOf<Codecs>,
        VgcApis.Models.Interfaces.IShareLinkDecoder
    {
        Cache cache;

        public SsDecoder(Cache cache)
        {
            this.cache = cache;
        }

        #region properties

        #endregion

        #region public methods
        public Tuple<JObject, JToken> Decode(string shareLink)
        {
            // ss://(base64)#tag or ss://(base64)
            var parts = shareLink.Split('#');
            if (parts.Length > 2 || parts.Length < 1)
            {
                return null;
            }

            var outbound = SsLink2Outbound(parts[0]);
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

            return new Tuple<JObject, JToken>(tpl, outbound);
        }


        public string Encode(string config)
        {
            throw new NotImplementedException();
        }

        public List<string> ExtractLinksFromText(string text) =>
            Lib.Utils.ExtractLinks(
                text,
                VgcApis.Models.Datas.Enum.LinkTypes.ss);
        #endregion

        #region private methods
        JToken SsLink2Outbound(string ssLink)
        {
            Model.Data.Shadowsocks ss = Lib.Utils.SsLink2Ss(ssLink);
            if (ss == null)
            {
                return null;
            }

            VgcApis.Libs.Utils.TryParseIPAddr(ss.addr, out string ip, out int port);
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
