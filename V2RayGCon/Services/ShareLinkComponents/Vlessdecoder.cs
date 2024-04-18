using System.Collections.Generic;

namespace V2RayGCon.Services.ShareLinkComponents
{
    internal sealed class VlessDecoder
        : VgcApis.BaseClasses.ComponentOf<Codecs>,
            VgcApis.Interfaces.IShareLinkDecoder
    {
        public VlessDecoder() { }

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
            if (Comm.TryParseConfig(config, out var vc) && vc != null && vc.proto == "vless")
            {
                vc.name = name;
                return vc.ToShareLink();
            }
            return null;
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
