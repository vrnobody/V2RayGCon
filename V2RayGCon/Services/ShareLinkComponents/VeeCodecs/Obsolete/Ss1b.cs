using Newtonsoft.Json.Linq;
using System;

namespace V2RayGCon.Services.ShareLinkComponents.VeeCodecs.Obsolete
{
    internal sealed class Ss1b :
        VgcApis.BaseClasses.ComponentOf<VeeDecoder>,
        IVeeDecoder
    {
        Cache cache;

        public Ss1b(Cache cache)
        {
            this.cache = cache;
        }

        #region properties

        #endregion

        #region IVeeConfig
        public byte[] VeeConfig2Bytes(Models.Datas.VeeConfigs veeConfig)
        {
            var vee = new Models.VeeShareLinks.Obsolete.Ss1b();
            vee.CopyFromVeeConfig(veeConfig);
            return vee.ToBytes();
        }

        public Models.Datas.VeeConfigs Bytes2VeeConfig(byte[] bytes)
        {
            var vee = new Models.VeeShareLinks.Obsolete.Ss1b(bytes);
            return vee.ToVeeConfigs();
        }
        #endregion

        #region public methods

        public string GetSupportedVeeVersion() => Models.VeeShareLinks.Obsolete.Ss1b.version;
        public string GetSupportedEncodeProtocol() => @"";

        public byte[] Config2Bytes(JObject config)
        {
            var vee = Config2Vee(config);
            return vee?.ToBytes();
        }

        public Tuple<JObject, JToken> Bytes2Config(byte[] bytes)
        {
            var veeLink = new Models.VeeShareLinks.Obsolete.Ss1b(bytes);
            return VeeToConfig(veeLink);
        }

        #endregion

        #region private methods
        Models.VeeShareLinks.Obsolete.Ss1b Config2Vee(JObject config)
        {
            var bs = Comm.ExtractBasicConfig(config, @"shadowsocks", @"servers", out bool isUseV4, out string root);
            if (bs == null)
            {
                return null;
            }

            var GetStr = Misc.Utils.GetStringByPrefixAndKeyHelper(config);
            var vmess = new Models.VeeShareLinks.Obsolete.Ss1b(bs);

            var prefix = root + "." + "settings.servers.0";
            vmess.method = GetStr(prefix, "method");
            vmess.password = GetStr(prefix, "password");
            vmess.isUseOta = GetStr(prefix, "ota")?.ToLower() == "true";
            return vmess;
        }

        Tuple<JObject, JToken> VeeToConfig(Models.VeeShareLinks.Obsolete.Ss1b vee)
        {
            if (vee == null)
            {
                return null;
            }

            var outbSs = cache.tpl.LoadTemplate("outbVeeSs");
            outbSs["streamSettings"] = Comm.GenStreamSetting(cache, vee);
            var node = outbSs["settings"]["servers"][0];
            node["ota"] = vee.isUseOta;
            node["address"] = vee.address;
            node["port"] = vee.port;
            node["method"] = vee.method;
            node["password"] = vee.password;

            var tpl = cache.tpl.LoadTemplate("tplImportSS") as JObject;
            tpl["v2raygcon"]["alias"] = vee.alias;
            tpl["v2raygcon"]["description"] = vee.description;
            return new Tuple<JObject, JToken>(tpl, outbSs);
        }

        #endregion

        #region protected methods

        #endregion
    }
}
