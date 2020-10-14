using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace V2RayGCon.Services.ShareLinkComponents
{
    internal sealed class VeeDecoder :
        VgcApis.BaseClasses.ComponentOf<Codecs>,
        VgcApis.Interfaces.IShareLinkDecoder
    {
        Cache cache;
        Settings setting;

        public VeeDecoder(
            Cache cache,
            Settings setting)
        {
            this.cache = cache;
            this.setting = setting;
        }

        #region properties

        #endregion

        #region public methods
        Dictionary<string, VeeCodecs.IVeeDecoder> decoders = new Dictionary<string, VeeCodecs.IVeeDecoder>();
        Dictionary<string, VeeCodecs.IVeeDecoder> encoders = new Dictionary<string, VeeCodecs.IVeeDecoder>();

        public override void Prepare()
        {
            AddChild(new VeeCodecs.Vmess0b(cache));
            AddChild(new VeeCodecs.Ss1c(cache));
            AddChild(new VeeCodecs.Socks2b(cache));
            AddChild(new VeeCodecs.Http3b(cache));
            AddChild(new VeeCodecs.Vless4b(cache));
            AddChild(new VeeCodecs.Trojan5b(cache));
            AddChild(new VeeCodecs.Obsolete.Vmess0a(cache));
            AddChild(new VeeCodecs.Obsolete.Ss1a(cache)); // support old decoder
            AddChild(new VeeCodecs.Obsolete.Ss1b(cache));
            AddChild(new VeeCodecs.Obsolete.Socks2a(cache));
            AddChild(new VeeCodecs.Obsolete.Http3a(cache));
            AddChild(new VeeCodecs.Obsolete.Trojan5a(cache));
            AddChild(new VeeCodecs.Obsolete.Vless4a(cache));

            foreach (var child in this.GetChildren())
            {
                var codec = child as VeeCodecs.IVeeDecoder;
                var ver = codec.GetSupportedVeeVersion();
                var protocol = codec.GetSupportedEncodeProtocol();

                decoders[ver] = codec;
                if (!string.IsNullOrEmpty(protocol))
                {
                    encoders[protocol] = codec;
                }
            }
        }

        public Tuple<JObject, JToken> Decode(string shareLink)
        {
            string message;
            try
            {
                return DecodeWorker(shareLink);
            }
            catch (Exception e)
            {
                message = e.Message;
            }

            if (!string.IsNullOrEmpty(message))
            {
                setting.SendLog(message);
            }
            return null;
        }

        public string Encode(string config)
        {
            try
            {
                return EncodeWorker(config);
            }
            catch (Exception e)
            {
                setting.SendLog(e.Message);
            }
            return null;
        }

        public List<string> ExtractLinksFromText(string text) =>
            Misc.Utils.ExtractLinks(
                text,
                VgcApis.Models.Datas.Enums.LinkTypes.v);
        #endregion

        #region IVeeLink2VeeConfig
        public string VeeConfig2VeeLink(Models.Datas.VeeConfigs vc)
        {
            var proto = vc?.proto;
            if (encoders.ContainsKey(proto))
            {
                var encoder = encoders[proto];
                var b = encoder.VeeConfig2Bytes(vc);
                return Bytes2VeeLink(b);
            }
            return null;
        }

        public Models.Datas.VeeConfigs VeeLink2VeeConfig(string veeLink)
        {
            var bytes = VeeLink2Bytes(veeLink);
            var ver = VgcApis.Libs.Streams.BitStream.ReadVersion(bytes);
            if (decoders.ContainsKey(ver))
            {
                var decoder = decoders[ver];
                return decoder.Bytes2VeeConfig(bytes);
            }
            return null;
        }
        #endregion

        #region private methods
        Tuple<JObject, JToken> DecodeWorker(string shareLink)
        {
            var bytes = VeeLink2Bytes(shareLink);
            var linkVersion = VgcApis.Libs.Streams.BitStream.ReadVersion(bytes);
            if (decoders.ContainsKey(linkVersion))
            {
                var decoder = decoders[linkVersion];
                return decoder.Bytes2Config(bytes);
            }

            throw new NotSupportedException($"Not supported vee share link version {linkVersion}");
        }

        string EncodeWorker(string config)
        {
            if (!VgcApis.Misc.Utils.TryParseJObject(
                config, out JObject json))
            {
                return null;
            }

            var protocol = Misc.Utils.GetProtocolFromConfig(json);
            if (encoders.ContainsKey(protocol))
            {
                var encoder = encoders[protocol];
                var bytes = encoder.Config2Bytes(json);
                return Bytes2VeeLink(bytes);
            }

            return null;
        }

        static byte[] VeeLink2Bytes(string veeLink)
        {
            // Do not use Misc.Utils.Base64Decode() 
            // Unicode encoder can not handle all possible byte values.

            string b64Body = Misc.Utils.GetLinkBody(veeLink);
            string b64Padded = Misc.Utils.Base64PadRight(b64Body);
            return Convert.FromBase64String(b64Padded);
        }

        static string Bytes2VeeLink(byte[] bytes)
        {
            if (bytes == null)
            {
                throw new ArgumentNullException(
                    @"Bytes is null!");
            }

            var b64Str = Convert.ToBase64String(bytes);
            return Misc.Utils.AddLinkPrefix(
                b64Str,
                VgcApis.Models.Datas.Enums.LinkTypes.v);
        }

        #endregion

        #region protected methods
        #endregion
    }
}
