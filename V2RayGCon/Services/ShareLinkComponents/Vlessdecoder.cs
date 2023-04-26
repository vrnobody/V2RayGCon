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
                var vc = VeeCodecs.Comm.ParseNonStandarUriShareLink("vless", shareLink);
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
            return VeeCodecs.Comm.EncodeUriShareLink("vless", config);
        }

        public List<string> ExtractLinksFromText(string text) =>
           Misc.Utils.ExtractLinks(
               text,
               VgcApis.Models.Datas.Enums.LinkTypes.vless);
        #endregion

        #region private methods


        #endregion

        #region protected methods

        #endregion
    }
}
