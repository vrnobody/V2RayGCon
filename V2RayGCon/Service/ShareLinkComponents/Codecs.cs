using Newtonsoft.Json.Linq;

namespace V2RayGCon.Service.ShareLinkComponents
{
    public sealed class Codecs :
        VgcApis.Models.BaseClasses.ComponentOf<Codecs>
    {
        Setting setting;
        Cache cache;

        public Codecs() { }

        #region public methods
        public string Encode<TDecoder>(string config)
            where TDecoder :
                VgcApis.Models.BaseClasses.ComponentOf<Codecs>,
                VgcApis.Models.Interfaces.IShareLinkDecoder
        => GetComponent<TDecoder>()?.Encode(config);


        public string Decode(
            string shareLink,
            VgcApis.Models.Interfaces.IShareLinkDecoder decoder)
        {
            var tuple = decoder.Decode(shareLink);
            return GenerateConfing(tuple);

        }

        public string Decode<TDecoder>(string shareLink)
            where TDecoder :
                VgcApis.Models.BaseClasses.ComponentOf<Codecs>,
                VgcApis.Models.Interfaces.IShareLinkDecoder
        {
            var tuple = GetComponent<TDecoder>()?.Decode(shareLink);
            return GenerateConfing(tuple);
        }

        public void Run(
            Cache cache,
            Setting setting)
        {
            this.setting = setting;
            this.cache = cache;

            var ssDecoder = new SsDecoder(cache);
            var v2cfgDecoder = new V2cfgDecoder();
            var vmessDecoder = new VmessDecoder(cache);
            var veeDecoder = new VeeDecoder(cache, setting);

            veeDecoder.Prepare();

            Plug(this,ssDecoder);
            Plug(this,v2cfgDecoder);
            Plug(this,vmessDecoder);
            Plug(this,veeDecoder);
        }
        #endregion

        #region private methods
        private string GenerateConfing(System.Tuple<JObject, JToken> tuple)
        {
            if (tuple == null)
            {
                return null;
            }

            // special case for v2cfg:// ...
            if (tuple.Item2 == null)
            {
                return Lib.Utils.Config2String(tuple.Item1);
            }

            return InjectOutboundIntoTemplate(tuple.Item1, tuple.Item2);
        }


        string InjectOutboundIntoTemplate(JObject template, JToken outbound)
        {
            var isV4 = setting.isUseV4;

            var inb = Lib.Utils.CreateJObject(
                (isV4 ? "inbounds.0" : "inbound"),
                cache.tpl.LoadTemplate("inbSimSock"));

            var outb = Lib.Utils.CreateJObject(
                (isV4 ? "outbounds.0" : "outbound"),
                outbound);

            Lib.Utils.MergeJson(ref template, inb);
            Lib.Utils.MergeJson(ref template, outb);
            return Lib.Utils.Config2String(template as JObject);
        }
        #endregion
    }
}
