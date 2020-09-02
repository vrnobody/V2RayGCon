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
        public override void Prepare()
        {
            AddChild(new VeeCodecs.Vmess0a(cache));
            AddChild(new VeeCodecs.Ss1a(cache)); // support old decoder
            AddChild(new VeeCodecs.Ss1b(cache));
            AddChild(new VeeCodecs.Socks2a(cache));
            AddChild(new VeeCodecs.Http3a(cache));
            AddChild(new VeeCodecs.Vless4a(cache));
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

        #region private methods
        Tuple<JObject, JToken> DecodeWorker(string shareLink)
        {
            var bytes = VeeLink2Bytes(shareLink);
            var linkVersion = VgcApis.Libs.Streams.BitStream.ReadVersion(bytes);
            var children = GetChildren();
            foreach (var component in children)
            {
                var decoder = component as VeeCodecs.IVeeDecoder;
                if (decoder.IsDecoderFor(linkVersion))
                {
                    return decoder.Bytes2Config(bytes);
                }
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
            var codecs = GetChildren();
            foreach (var codec in codecs)
            {
                var encoder = codec as VeeCodecs.IVeeDecoder;
                if (encoder.IsEncoderFor(protocol))
                {
                    var bytes = encoder?.Config2Bytes(json);
                    return Bytes2VeeLink(bytes);
                }
            }
            return null;
        }

        public static byte[] VeeLink2Bytes(string veeLink)
        {
            // Do not use Misc.Utils.Base64Decode() 
            // Unicode encoder can not handle all possible byte values.

            string b64Body = Misc.Utils.GetLinkBody(veeLink);
            string b64Padded = Misc.Utils.Base64PadRight(b64Body);
            return Convert.FromBase64String(b64Padded);
        }

        public static string Bytes2VeeLink(byte[] bytes)
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
