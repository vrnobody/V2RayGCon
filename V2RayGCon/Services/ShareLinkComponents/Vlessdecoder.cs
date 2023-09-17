using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace V2RayGCon.Services.ShareLinkComponents
{
    internal sealed class VlessDecoder
        : VgcApis.BaseClasses.ComponentOf<Codecs>,
            VgcApis.Interfaces.IShareLinkDecoder
    {
        public VlessDecoder() { }

        #region properties

        #endregion

        #region public methods
        public VgcApis.Models.Datas.DecodeResult Decode(string shareLink)
        {
            try
            {
                var vc = Comm.ParseNonStandarUriShareLink("vless", shareLink);
                var config = GetParent()?.VlessToConfig(vc);
                return new VgcApis.Models.Datas.DecodeResult(vc?.name, config);
            }
            catch { }
            return null;
        }

        public string Encode(string name, string config)
        {
            return Comm.EncodeUriShareLink("vless", name, config);
        }

        public List<string> ExtractLinksFromText(string text) =>
            Misc.Utils.ExtractLinks(text, VgcApis.Models.Datas.Enums.LinkTypes.vless);
        #endregion

        #region private methods

        #endregion

        #region protected methods

        #endregion
    }
}
