using Newtonsoft.Json.Linq;
using System;

namespace V2RayGCon.Services.ShareLinkComponents.VeeCodecs
{
    internal sealed class Ss1c :
        VgcApis.BaseClasses.ComponentOf<VeeDecoder>,
        IVeeDecoder
    {
        Cache cache;

        public Ss1c(Cache cache)
        {
            this.cache = cache;
        }

        #region properties

        #endregion

        #region IVeeConfig
        public byte[] VeeConfig2Bytes(Models.Datas.VeeConfigsWithReality veeConfig)
        {
            var vee = new Models.VeeShareLinks.Ss1c();
            vee.CopyFromVeeConfig(veeConfig);
            return vee.ToBytes();
        }

        public Models.Datas.VeeConfigsWithReality Bytes2VeeConfig(byte[] bytes)
        {
            var vee = new Models.VeeShareLinks.Ss1c(bytes);
            return new Models.Datas.VeeConfigsWithReality(vee.ToVeeConfigs());
        }
        #endregion

        #region public methods
        public string GetSupportedVeeVersion() => Models.VeeShareLinks.Ss1c.version;
        public string GetSupportedEncodeProtocol() => Models.VeeShareLinks.Ss1c.proto;


        public byte[] Config2Bytes(JObject config)
        {
            var vee = Config2Vee(config);
            return vee?.ToBytes();
        }

        public Tuple<JObject, JToken> Bytes2Config(byte[] bytes)
        {
            var veeLink = new Models.VeeShareLinks.Ss1c(bytes);
            return VeeToConfig(veeLink);
        }

        #endregion

        #region private methods
        Models.VeeShareLinks.Ss1c Config2Vee(JObject config)
        {
            var bs = Comm.ExtractBasicConfig(config, @"shadowsocks", @"servers", out bool isUseV4, out string root);
            if (bs == null)
            {
                return null;
            }

            var GetStr = Misc.Utils.GetStringByPrefixAndKeyHelper(config);
            var vmess = new Models.VeeShareLinks.Ss1c(bs);

            var prefix = root + "." + "settings.servers.0";
            vmess.method = GetStr(prefix, "method");
            vmess.password = GetStr(prefix, "password");
            return vmess;
        }

        Tuple<JObject, JToken> VeeToConfig(Models.VeeShareLinks.Ss1c vee)
        {
            if (vee == null)
            {
                return null;
            }

            var outbSs = cache.tpl.LoadTemplate("outbVeeSs");

            var rvee = new Models.VeeShareLinks.BasicSettingsWithReality(vee);
            outbSs["streamSettings"] = Comm.GenStreamSetting(cache, rvee);

            var node = outbSs["settings"]["servers"][0];
            node["address"] = vee.address;
            node["port"] = vee.port;
            node["method"] = string.IsNullOrEmpty(vee.method) ? "none" : vee.method;
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
