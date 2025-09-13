using System.Collections.Generic;

namespace V2RayGCon.Services.ShareLinkComponents
{
    internal sealed class TrojanDecoder
        : VgcApis.BaseClasses.ComponentOf<Codecs>,
            VgcApis.Interfaces.IShareLinkDecoder
    {
        public TrojanDecoder() { }

        #region properties

        #endregion

        #region public methods
        public VgcApis.Models.Datas.DecodeResult Decode(string shareLink)
        {
            /*
             * trojan://password@remote_host:remote_port
             * in which the password is url-encoded in case it contains illegal characters.
             */

            try
            {
                var vc = VgcApis.Misc.OutbMeta.ParseNonStandarUriShareLink("trojan", shareLink);
                var config = GetParent()?.TrojanToConfig(vc);
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
                && vc.proto == "trojan"
            )
            {
                vc.name = name;
                return vc.ToShareLink();
            }
            return null;
        }

        public List<string> ExtractLinksFromText(string text) =>
            Misc.Utils.ExtractLinks(text, VgcApis.Models.Datas.Enums.LinkTypes.trojan);
        #endregion

        #region private methods
        #endregion

        #region protected methods

        #endregion
    }
}
