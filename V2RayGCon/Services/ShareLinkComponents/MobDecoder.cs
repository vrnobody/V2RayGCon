using System.Collections.Generic;

namespace V2RayGCon.Services.ShareLinkComponents
{
    internal sealed class MobDecoder
        : VgcApis.BaseClasses.ComponentOf<Codecs>,
            VgcApis.Interfaces.IShareLinkDecoder
    {
        public MobDecoder() { }

        #region public methods
        public VgcApis.Models.Datas.DecodeResult Decode(string shareLink)
        {
            try
            {
                var body = VgcApis.Misc.Utils.GetLinkBody(shareLink);
                var json = VgcApis.Misc.Utils.Base64DecodeToString(body);
                var mob = VgcApis.Misc.Utils.DeserializeObject<VgcApis.Models.Datas.MobItem>(json);
                var meta = mob.ToShareLinkMetaData();
                var config = GetParent()?.MobToConfigSimple(meta);
                return new VgcApis.Models.Datas.DecodeResult(meta?.name, config);
            }
            catch { }
            return null;
        }

        public string Encode(string name, string config)
        {
            if (VgcApis.Misc.OutbMeta.TryParseConfig(config, out var vc) && vc != null)
            {
                vc.name = name;
                var mob = new VgcApis.Models.Datas.MobItem(vc);
                return mob.ToShareLink();
            }
            return null;
        }

        public List<string> ExtractLinksFromText(string text) =>
            Misc.Utils.ExtractLinks(text, VgcApis.Models.Datas.Enums.LinkTypes.mob);
        #endregion

        #region private methods

        #endregion

        #region protected methods

        #endregion
    }
}
