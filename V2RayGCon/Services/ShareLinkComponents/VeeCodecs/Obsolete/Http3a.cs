using Newtonsoft.Json.Linq;
using System;

namespace V2RayGCon.Services.ShareLinkComponents.VeeCodecs.Obsolete
{
    internal sealed class Http3a :
        VgcApis.BaseClasses.ComponentOf<VeeDecoder>,
        IVeeDecoder
    {
        Cache cache;

        public Http3a(Cache cache)
        {
            this.cache = cache;
        }

        #region properties

        #endregion
        #region IVeeConfig
        public byte[] VeeConfig2Bytes(Models.Datas.VeeConfigsWithReality veeConfig)
        {
            var vee = new Models.VeeShareLinks.Obsolete.Http3a();
            vee.CopyFromVeeConfig(veeConfig);
            return vee.ToBytes();
        }

        public Models.Datas.VeeConfigsWithReality Bytes2VeeConfig(byte[] bytes)
        {
            var vee = new Models.VeeShareLinks.Obsolete.Http3a(bytes);
            return new Models.Datas.VeeConfigsWithReality(vee.ToVeeConfigs());
        }
        #endregion

        #region public methods
        public string GetSupportedVeeVersion() => Models.VeeShareLinks.Obsolete.Http3a.version;
        public string GetSupportedEncodeProtocol() => @"";

        public byte[] Config2Bytes(JObject config)
        {
            var vee = Config2Vee(config);
            return vee?.ToBytes();
        }

        public Tuple<JObject, JToken> Bytes2Config(byte[] bytes)
        {
            var veeLink = new Models.VeeShareLinks.Obsolete.Http3a(bytes);
            return VeeToConfig(veeLink);
        }

        #endregion

        #region private methods
        Models.VeeShareLinks.Obsolete.Http3a Config2Vee(JObject config)
        {
            var bs = Comm.ExtractBasicConfig(
                config, @"http", @"servers", out bool isUseV4, out string root);

            if (bs == null)
            {
                return null;
            }

            var GetStr = Misc.Utils.GetStringByPrefixAndKeyHelper(config);

            var http = new Models.VeeShareLinks.Obsolete.Http3a(bs);
            var userInfoPrefix = root + ".settings.servers.0.users.0";
            http.userName = GetStr(userInfoPrefix, "user") ?? string.Empty;
            http.userPassword = GetStr(userInfoPrefix, "pass") ?? string.Empty;
            return http;
        }

        Tuple<JObject, JToken> VeeToConfig(Models.VeeShareLinks.Obsolete.Http3a http)
        {
            if (http == null)
            {
                return null;
            }

            var outbHttp = cache.tpl.LoadTemplate("outbVeeHttp");

            var rhttp = new Models.VeeShareLinks.BasicSettingsWithReality(http);
            outbHttp["streamSettings"] = Comm.GenStreamSetting(cache, rhttp);
            var node = outbHttp["settings"]["servers"][0];
            node["address"] = http.address;
            node["port"] = http.port;

            if (!string.IsNullOrEmpty(http.userName) || !string.IsNullOrEmpty(http.userPassword))
            {
                node["users"] = JToken.Parse(
                    $"[{{user: \"{http.userName ?? string.Empty}\"," +
                    $"pass:\"{http.userPassword ?? string.Empty}\"}}]");
            }

            var tpl = cache.tpl.LoadTemplate("tplImportVmess") as JObject;
            tpl["v2raygcon"]["alias"] = http.alias;
            tpl["v2raygcon"]["description"] = http.description;
            return new Tuple<JObject, JToken>(tpl, outbHttp);
        }

        #endregion

        #region protected methods

        #endregion
    }
}
