using Newtonsoft.Json.Linq;
using System;

namespace V2RayGCon.Services.ShareLinkComponents.VeeCodecs.Obsolete
{
    internal sealed class Vless4b : VgcApis.BaseClasses.ComponentOf<VeeDecoder>, IVeeDecoder
    {
        private readonly Cache cache;

        public Vless4b(Cache cache)
        {
            this.cache = cache;
        }

        #region properties

        #endregion
        #region IVeeConfig
        public byte[] VeeConfig2Bytes(Models.Datas.VeeConfigsWithReality veeConfig)
        {
            var vee = new Models.VeeShareLinks.Obsolete.Vless4b();
            vee.CopyFromVeeConfig(veeConfig);
            return vee.ToBytes();
        }

        public Models.Datas.VeeConfigsWithReality Bytes2VeeConfig(byte[] bytes)
        {
            var vee = new Models.VeeShareLinks.Obsolete.Vless4b(bytes);
            return new Models.Datas.VeeConfigsWithReality(vee.ToVeeConfigs());
        }
        #endregion
        #region public methods
        public string GetSupportedVeeVersion() => Models.VeeShareLinks.Obsolete.Vless4b.version;

        public string GetSupportedEncodeProtocol() => @"";

        public byte[] Config2Bytes(JObject config)
        {
            var vee = Config2Vee(config);
            return vee?.ToBytes();
        }

        public Tuple<JObject, JToken> Bytes2Config(byte[] bytes)
        {
            var veeLink = new Models.VeeShareLinks.Obsolete.Vless4b(bytes);
            return VeeToConfig(veeLink);
        }

        #endregion

        #region private methods
        Models.VeeShareLinks.Obsolete.Vless4b Config2Vee(JObject config)
        {
            var bs = Comm.ExtractBasicConfig(
                config,
                @"vless",
                @"vnext",
                out bool isUseV4,
                out string root
            );

            if (bs == null)
            {
                return null;
            }

            var GetStr = Misc.Utils.GetStringByPrefixAndKeyHelper(config);
            var vless = new Models.VeeShareLinks.Obsolete.Vless4b(bs);
            var userInfoPrefix = root + ".settings.vnext.0.users.0";
            var enc = GetStr(userInfoPrefix, "encryption");
            vless.encryption = string.IsNullOrEmpty(enc) ? "none" : enc;
            vless.uuid = Guid.Parse(GetStr(userInfoPrefix, "id"));
            vless.flow = GetStr(userInfoPrefix, "flow");
            return vless;
        }

        Tuple<JObject, JToken> VeeToConfig(Models.VeeShareLinks.Obsolete.Vless4b vee)
        {
            if (vee == null)
            {
                return null;
            }

            var outVmess = cache.tpl.LoadTemplate("outbVeeVmess");

            var rvee = new Models.VeeShareLinks.BasicSettingsWithReality(vee);
            outVmess["streamSettings"] = Comm.GenStreamSetting(cache, rvee);

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
