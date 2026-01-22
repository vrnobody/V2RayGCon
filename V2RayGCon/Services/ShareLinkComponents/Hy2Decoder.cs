using System.Collections.Generic;

namespace V2RayGCon.Services.ShareLinkComponents
{
    internal sealed class Hy2Decoder
        : VgcApis.BaseClasses.ComponentOf<Codecs>,
            VgcApis.Interfaces.IShareLinkDecoder
    {
        public Hy2Decoder() { }

        #region public methods
        public VgcApis.Models.Datas.DecodeResult Decode(string shareLink)
        {
            try
            {
                var vc = VgcApis.Misc.OutbMeta.ParseHy2ShareLink("hy2", shareLink);
                var config = GetParent()?.Hy2ToConfig(vc);
                return new VgcApis.Models.Datas.DecodeResult(vc?.name, config);
            }
            catch { }
            return null;
        }

        public string Encode(string name, string config)
        {
            if (
                VgcApis.Misc.OutbMeta.TryParseConfig(config, out var vc)
                && vc != null
                && vc.proto == "hysteria"
            )
            {
                vc.name = name;
                return vc.ToShareLink();
            }
            return null;
        }

        public List<string> ExtractLinksFromText(string text) =>
            Misc.Utils.ExtractLinks(text, VgcApis.Models.Datas.Enums.LinkTypes.hy2);
        #endregion

        #region private methods

        #endregion

        #region protected methods

        #endregion
    }
}
