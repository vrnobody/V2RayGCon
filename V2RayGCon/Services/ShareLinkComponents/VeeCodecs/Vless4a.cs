using Newtonsoft.Json.Linq;
using System;

namespace V2RayGCon.Services.ShareLinkComponents.VeeCodecs
{
    internal sealed class Vless4a :
        VgcApis.BaseClasses.ComponentOf<VeeDecoder>,
        IVeeDecoder
    {
        private readonly Cache cache;

        public Vless4a(Cache cache)
        {
            this.cache = cache;
        }

        #region properties

        #endregion
        #region IVeeConfig
        public byte[] VeeConfig2Bytes(Models.Datas.VeeConfigs veeConfig)
        {
            var vee = new Models.VeeShareLinks.Vless4a();
            vee.CopyFromVeeConfig(veeConfig);
            return vee.ToBytes();
        }

        public Models.Datas.VeeConfigs Bytes2VeeConfig(byte[] bytes)
        {
            var vee = new Models.VeeShareLinks.Vless4a(bytes);
            return vee.ToVeeConfigs();
        }
        #endregion
        #region public methods
        public bool IsDecoderFor(string version) => Models.VeeShareLinks.Vless4a.IsDecoderFor(version);

        public bool IsEncoderFor(string protocol) => Models.VeeShareLinks.Vless4a.IsEncoderFor(protocol);

        public byte[] Config2Bytes(JObject config)
        {
            var vee = Config2Vee(config);
            return vee?.ToBytes();
        }

        public Tuple<JObject, JToken> Bytes2Config(byte[] bytes)
        {
            var veeLink = new Models.VeeShareLinks.Vless4a(bytes);
            return VeeToConfig(veeLink);
        }

        #endregion

        #region private methods
        Models.VeeShareLinks.Vless4a Config2Vee(JObject config)
        {
            var bs = Comm.ExtractBasicConfig(config, @"vless", @"vnext", out bool isUseV4, out string root);

            if (bs == null)
            {
                return null;
            }

            var GetStr = Misc.Utils.GetStringByPrefixAndKeyHelper(config);
            var vless = new Models.VeeShareLinks.Vless4a(bs);
            var userInfoPrefix = root + ".settings.vnext.0.users.0";
            var enc = GetStr(userInfoPrefix, "encryption");
            vless.encryption = string.IsNullOrEmpty(enc) ? "none" : enc;
            vless.uuid = Guid.Parse(GetStr(userInfoPrefix, "id"));
            vless.flow = GetStr(userInfoPrefix, "flow");
            return vless;
        }


        Tuple<JObject, JToken> VeeToConfig(Models.VeeShareLinks.Vless4a vee)
        {
            if (vee == null)
            {
                return null;
            }

            var outVmess = cache.tpl.LoadTemplate("outbVeeVmess");
            outVmess["streamSettings"] = Comm.GenStreamSetting(cache, vee);

            outVmess["protocol"] = "vless";
            var node = outVmess["settings"]["vnext"][0];
            node["address"] = vee.address;
            node["port"] = vee.port;
            node["users"][0]["id"] = vee.uuid;
            node["users"][0]["flow"] = vee.flow;
            node["users"][0]["encryption"] = vee.encryption;
            var tpl = cache.tpl.LoadTemplate("tplImportVmess") as JObject;
            tpl["v2raygcon"]["alias"] = vee.alias;
            tpl["v2raygcon"]["description"] = vee.description;
            return new Tuple<JObject, JToken>(tpl, outVmess);
        }

        #endregion

        #region protected methods

        #endregion
    }
}
