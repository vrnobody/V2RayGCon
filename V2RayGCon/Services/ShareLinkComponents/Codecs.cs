using Newtonsoft.Json.Linq;

namespace V2RayGCon.Services.ShareLinkComponents
{
    public sealed class Codecs :
        VgcApis.BaseClasses.ComponentOf<Codecs>
    {
        Settings setting;
        Cache cache;

        public Codecs() { }

        #region public methods
        public string Encode<TDecoder>(string config)
            where TDecoder :
                VgcApis.BaseClasses.ComponentOf<Codecs>,
                VgcApis.Interfaces.IShareLinkDecoder
        => GetChild<TDecoder>()?.Encode(config);


        public string Decode(
            string shareLink,
            VgcApis.Interfaces.IShareLinkDecoder decoder)
        {
            try
            {
                var tuple = decoder.Decode(shareLink);
                return GenerateConfing(tuple);
            }
            catch { }

            return null;
        }

        public string Decode<TDecoder>(string shareLink)
            where TDecoder :
                VgcApis.BaseClasses.ComponentOf<Codecs>,
                VgcApis.Interfaces.IShareLinkDecoder
        {
            var tuple = GetChild<TDecoder>()?.Decode(shareLink);
            return GenerateConfing(tuple);
        }

        public void Run(
            Cache cache,
            Settings setting)
        {
            this.setting = setting;
            this.cache = cache;

            var ssDecoder = new SsDecoder(cache);
            var v2cfgDecoder = new V2cfgDecoder();
            var vmessDecoder = new VmessDecoder(cache, setting);
            var veeDecoder = new VeeDecoder(cache, setting);
            var trojanDecoder = new TrojanDecoder(veeDecoder);

            veeDecoder.Prepare();

            AddChild(trojanDecoder);
            AddChild(ssDecoder);
            AddChild(v2cfgDecoder);
            AddChild(vmessDecoder);
            AddChild(veeDecoder);
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
                return Misc.Utils.Config2String(tuple.Item1);
            }

            return InjectOutboundIntoTemplate(tuple.Item1, tuple.Item2);
        }


        string InjectOutboundIntoTemplate(JObject template, JToken outbound)
        {
            var isV4 = setting.isUseV4;

            var inb = Misc.Utils.CreateJObject(
                (isV4 ? "inbounds.0" : "inbound"),
                cache.tpl.LoadTemplate("inbSimSock"));

            var outb = Misc.Utils.CreateJObject(
                (isV4 ? "outbounds.0" : "outbound"),
                outbound);

            Misc.Utils.MergeJson(ref template, inb);
            Misc.Utils.MergeJson(ref template, outb);
            return Misc.Utils.Config2String(template as JObject);
        }
        #endregion
    }
}
