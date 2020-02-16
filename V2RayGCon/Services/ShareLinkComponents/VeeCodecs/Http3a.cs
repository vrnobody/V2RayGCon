using Newtonsoft.Json.Linq;
using System;

namespace V2RayGCon.Services.ShareLinkComponents.VeeCodecs
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

        #region public methods
        public string GetSupportedVersion() => Models.VeeShareLinks.Http3a.SupportedVersion();

        public byte[] Config2Bytes(JObject config)
        {
            var vee = Config2Vee(config);
            return vee?.ToBytes();
        }

        public Tuple<JObject, JToken> Bytes2Config(byte[] bytes)
        {
            var veeLink = new Models.VeeShareLinks.Http3a(bytes);
            return VeeToConfig(veeLink);
        }

        #endregion

        #region private methods
        Models.VeeShareLinks.Http3a Config2Vee(JObject config)
        {
            var bs = Comm.ExtractBasicConfig(
                config, @"http", @"servers", out bool isUseV4, out string root);

            if (bs == null)
            {
                return null;
            }

            var GetStr = Misc.Utils.GetStringByPrefixAndKeyHelper(config);

            var http = new Models.VeeShareLinks.Http3a(bs);
            var userInfoPrefix = root + ".settings.servers.0.users.0";
            http.userName = GetStr(userInfoPrefix, "user") ?? string.Empty;
            http.userPassword = GetStr(userInfoPrefix, "pass") ?? string.Empty;
            return http;
        }

        Tuple<JObject, JToken> VeeToConfig(Models.VeeShareLinks.Http3a http)
        {
            if (http == null)
            {
                return null;
            }

            var outbHttp = cache.tpl.LoadTemplate("outbVeeHttp");
            outbHttp["streamSettings"] = Comm.GenStreamSetting(cache, http);
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
