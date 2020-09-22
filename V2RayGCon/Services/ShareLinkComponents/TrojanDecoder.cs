using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace V2RayGCon.Services.ShareLinkComponents
{
    internal sealed class TrojanDecoder :
        VgcApis.BaseClasses.ComponentOf<Codecs>,
        VgcApis.Interfaces.IShareLinkDecoder
    {
        readonly VeeDecoder veeDecoder;
        public TrojanDecoder(VeeDecoder veeDecoder)
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
                var vc = ParseTrojanUrl(shareLink);
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
               VgcApis.Models.Datas.Enums.LinkTypes.trojan);
        #endregion

        #region private methods
        Models.Datas.VeeConfigs ParseTrojanUrl(string url)
        {
            var proto = "trojan";
            var header = proto + "://";

            if (!url.StartsWith(header))
            {
                return null;
            }

            var port = url.Split(':').LastOrDefault();
            if (string.IsNullOrEmpty(port))
            {
                return null;
            }

            var body = url.Substring(header.Length, url.Length - header.Length - port.Length - 1);
            var pa = body.Split('@');
            if (pa.Length != 2)
            {
                return null;
            }

            var vc = new Models.Datas.VeeConfigs();
            vc.proto = proto;
            vc.host = pa[1];
            vc.port = VgcApis.Misc.Utils.Str2Int(port);
            vc.auth1 = Uri.UnescapeDataString(pa[0]);

            vc.streamType = "tcp";
            vc.useTls = true;
            vc.streamParam1 = "none";
            return vc;
        }
        #endregion

        #region protected methods

        #endregion
    }
}
