using Newtonsoft.Json.Linq;
using System;

namespace V2RayGCon.Services.ShareLinkComponents.VeeCodecs
{
    internal sealed class Trojan5d : VgcApis.BaseClasses.ComponentOf<VeeDecoder>, IVeeDecoder
    {
        Cache cache;

        public Trojan5d(Cache cache)
        {
            this.cache = cache;
        }

        #region properties

        #endregion

        #region public methods
        public string GetSupportedVeeVersion() => Models.VeeShareLinks.Trojan5d.version;

        public string GetSupportedEncodeProtocol() => Models.VeeShareLinks.Trojan5d.proto;

        public byte[] Config2Bytes(JObject config)
        {
            var vee = Config2Vee(config);
            return vee?.ToBytes();
        }

        public Tuple<JObject, JToken> Bytes2Config(byte[] bytes)
        {
            var veeLink = new Models.VeeShareLinks.Trojan5d(bytes);
            return VeeToConfig(veeLink);
        }

        #endregion

        #region IVeeConfig
        public byte[] VeeConfig2Bytes(Models.Datas.VeeConfigsWithReality veeConfig)
        {
            var vee = new Models.VeeShareLinks.Trojan5d();
            vee.CopyFromVeeConfig(veeConfig);
            return vee.ToBytes();
        }

        public Models.Datas.VeeConfigsWithReality Bytes2VeeConfig(byte[] bytes)
        {
            var vee = new Models.VeeShareLinks.Trojan5d(bytes);
            return new Models.Datas.VeeConfigsWithReality(vee.ToVeeConfigs());
        }
        #endregion

        #region private methods
        Models.VeeShareLinks.Trojan5d Config2Vee(JObject config)
        {
            var bs = Comm.ExtractBasicConfig(
                config,
                @"trojan",
                @"servers",
                out bool isUseV4,
                out string root
            );
            if (bs == null)
            {
                return null;
            }

            var GetStr = Misc.Utils.GetStringByPrefixAndKeyHelper(config);
            var vee = new Models.VeeShareLinks.Trojan5d(bs);

            var prefix = root + "." + "settings.servers.0";
            vee.password = GetStr(prefix, "password");
            vee.flow = GetStr(prefix, "flow");
            return vee;
        }

        Tuple<JObject, JToken> VeeToConfig(Models.VeeShareLinks.Trojan5d vee)
        {
            if (vee == null)
            {
                return null;
            }

            var outbSs = cache.tpl.LoadTemplate("outbVeeTrojan5a");

            outbSs["streamSettings"] = Comm.GenStreamSetting(cache, vee);
            var node = outbSs["settings"]["servers"][0];
            node["address"] = vee.address;
            node["port"] = vee.port;
            node["password"] = vee.password;
            node["flow"] = vee.flow;

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
