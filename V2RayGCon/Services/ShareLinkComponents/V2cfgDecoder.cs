using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace V2RayGCon.Services.ShareLinkComponents
{
    internal sealed class V2cfgDecoder
        : VgcApis.BaseClasses.ComponentOf<Codecs>,
            VgcApis.Interfaces.IShareLinkDecoder
    {
        public V2cfgDecoder() { }

        #region properties

        #endregion

        #region public methods
        public VgcApis.Models.Datas.DecodeResult Decode(string shareLink)
        {
            try
            {
                var linkBody = VgcApis.Misc.Utils.GetLinkBody(shareLink);
                if (VgcApis.Libs.Infr.ZipExtensions.IsCompressedBase64(linkBody))
                {
                    return DecodeV2cfg(linkBody);
                }
                return DecodeV2cfgVer1(linkBody);
            }
            catch { }
            return null;
        }

        public string Encode(string name, string config)
        {
            if (string.IsNullOrEmpty(config))
            {
                return null;
            }
            var v2cfg = new VgcApis.Models.Datas.V2Cfg(name, config);
            var body = v2cfg.ToCompressedString();
            return VgcApis.Misc.Utils.AddLinkPrefix(
                body,
                VgcApis.Models.Datas.Enums.LinkTypes.v2cfg
            );
        }

        public List<string> ExtractLinksFromText(string text) =>
            Misc.Utils.ExtractLinks(text, VgcApis.Models.Datas.Enums.LinkTypes.v2cfg);
        #endregion

        #region private methods
        VgcApis.Models.Datas.DecodeResult DecodeV2cfg(string linkBody)
        {
            var v2cfg = new VgcApis.Models.Datas.V2Cfg(linkBody);
            if (v2cfg.IsValid())
            {
                return new VgcApis.Models.Datas.DecodeResult(v2cfg.name, v2cfg.config);
            }
            return null;
        }

        VgcApis.Models.Datas.DecodeResult DecodeV2cfgVer1(string linkBody)
        {
            var config = VgcApis.Misc.Utils.Base64DecodeToString(linkBody);
            var json = JObject.Parse(config);
            var name = VgcApis.Misc.Utils.GetAliasFromConfig(json);
            json.Remove(VgcApis.Models.Consts.Config.SectionKeyV2rayGCon);
            var cfg = VgcApis.Misc.Utils.FormatConfig(json);
            return new VgcApis.Models.Datas.DecodeResult(name, cfg);
        }

        #endregion

        #region protected methods

        #endregion
    }
}
