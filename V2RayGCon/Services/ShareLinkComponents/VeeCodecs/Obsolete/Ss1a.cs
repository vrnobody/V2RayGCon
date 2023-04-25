using Newtonsoft.Json.Linq;
using System;
using System.Linq;

namespace V2RayGCon.Services.ShareLinkComponents.VeeCodecs.Obsolete
{
    internal sealed class Ss1a :
        VgcApis.BaseClasses.ComponentOf<VeeDecoder>,
        IVeeDecoder
    {
        Cache cache;

        public Ss1a(Cache cache)
        {
            this.cache = cache;
        }

        #region properties

        #endregion
        #region IVeeConfig
        public byte[] VeeConfig2Bytes(Models.Datas.VeeConfigsWithReality veeConfig)
        {
            throw new NotImplementedException();
        }

        public Models.Datas.VeeConfigsWithReality Bytes2VeeConfig(byte[] bytes)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region public methods
        public string GetSupportedVeeVersion() => Models.VeeShareLinks.Obsolete.Ss1a.version;
        public string GetSupportedEncodeProtocol() => @"";

        public byte[] Config2Bytes(JObject config)
        {
            var vee = Config2Vee(config);
            return vee?.ToBytes();
        }

        public Tuple<JObject, JToken> Bytes2Config(byte[] bytes)
        {
            var veeLink = new Models.VeeShareLinks.Obsolete.Ss1a(bytes);
            return VeeToConfig(veeLink);
        }

        #endregion

        #region private methods
        Models.VeeShareLinks.Obsolete.Ss1a Config2Vee(JObject config)
        {
            var GetStr = Misc.Utils.GetStringByPrefixAndKeyHelper(config);
            var isUseV4 = (GetStr("outbounds.0", "protocol")?.ToLower()) ==
                VgcApis.Models.Consts.Config.ProtocolNameSs;

            var root = isUseV4 ? "outbounds.0" : "outbound";
            if (!isUseV4)
            {
                var protocol = GetStr(root, "protocol")?.ToLower();
                if (protocol == null ||
                    protocol != VgcApis.Models.Consts.Config.ProtocolNameSs)
                {
                    return null;
                }
            }

            var mainPrefix = root + "." + "settings.servers.0";
            var vee = new Models.VeeShareLinks.Obsolete.Ss1a
            {
                alias = GetStr("v2raygcon", "alias"),
                description = GetStr("v2raygcon", "description"),

                address = GetStr(mainPrefix, "address"),
                port = VgcApis.Misc.Utils.Str2Int(GetStr(mainPrefix, "port")),
                method = GetStr(mainPrefix, "method"),
                password = GetStr(mainPrefix, "password"),
                isUseOta = GetStr(mainPrefix, "ota")?.ToLower() == "true",
            };

            var networkTypeName = GetStr(mainPrefix, "network");
            vee.SetNetworkType(networkTypeName);

            var subPrefix = root + "." + "streamSettings";
            vee.streamType = GetStr(subPrefix, "network");
            vee.isUseTls = GetStr(subPrefix, "security")?.ToLower() == "tls";
            var mainParam = "";
            switch (vee.streamType)
            {
                case "kcp":
                    mainParam = GetStr(subPrefix, "kcpSettings.header.type");
                    vee.streamParam2 = GetStr(subPrefix, "kcpSettings.seed");
                    break;
                case "ws":
                    mainParam = GetStr(subPrefix, "wsSettings.path");
                    vee.streamParam2 = GetStr(subPrefix, "wsSettings.headers.Host");
                    break;
                case "h2":
                    mainParam = GetStr(subPrefix, "httpSettings.path");
                    try
                    {
                        var hosts = isUseV4 ?
                            config["outbounds"][0]["streamSettings"]["httpSettings"]["host"] :
                            config["outbound"]["streamSettings"]["httpSettings"]["host"];
                        vee.streamParam2 = Misc.Utils.JArray2Str(hosts as JArray);
                    }
                    catch { }
                    break;
                case "quic":
                    mainParam = GetStr(subPrefix, "quicSettings.header.type");
                    vee.streamParam2 = GetStr(subPrefix, "quicSettings.security");
                    vee.streamParam3 = GetStr(subPrefix, "quicSettings.key");
                    break;
                default:
                    break;
            }
            vee.streamParam1 = mainParam;
            return vee;
        }

        JToken GenOutboundFrom(Models.VeeShareLinks.Obsolete.Ss1a vee)
        {
            var outbSs = cache.tpl.LoadTemplate("outbVeeSs");
            var node = outbSs["settings"]["servers"][0];

            node["ota"] = vee.isUseOta;
            node["address"] = vee.address;
            node["port"] = vee.port;
            node["method"] = vee.method;
            node["password"] = vee.password;

            if (vee.GetCurNetworkTypeIndex() != 0)
            {
                node["network"] = vee.GetCurNetworkTypeName();
            }
            return outbSs;
        }

        Tuple<JObject, JToken> VeeToConfig(Models.VeeShareLinks.Obsolete.Ss1a vee)
        {
            if (vee == null)
            {
                return null;
            }

            var outbSs = GenOutboundFrom(vee);
            outbSs["streamSettings"] = GenStreamSetting(vee);
            var tpl = cache.tpl.LoadTemplate("tplImportSS") as JObject;
            tpl["v2raygcon"]["alias"] = vee.alias;
            tpl["v2raygcon"]["description"] = vee.description;
            return new Tuple<JObject, JToken>(tpl, outbSs);
        }

        JToken GenStreamSetting(Models.VeeShareLinks.Obsolete.Ss1a vee)
        {
            // insert stream type
            string[] streamTypes = { "ws", "tcp", "kcp", "h2", "quic" };
            string streamType = vee.streamType.ToLower();

            if (!streamTypes.Contains(streamType))
            {
                return JToken.Parse(@"{}");
            }

            string mainParam = vee.streamParam1;

            var streamToken = cache.tpl.LoadTemplate(streamType);
            try
            {
                switch (streamType)
                {
                    case "kcp":
                        streamToken["kcpSettings"]["header"]["type"] = mainParam;
                        if (!string.IsNullOrEmpty(vee.streamParam2))
                        {
                            streamToken["kcpSettings"]["seed"] = vee.streamParam2;
                        }
                        break;
                    case "ws":
                        streamToken["wsSettings"]["path"] = mainParam;
                        streamToken["wsSettings"]["headers"]["Host"] = vee.streamParam2;
                        break;
                    case "h2":
                        streamToken["httpSettings"]["path"] = mainParam;
                        streamToken["httpSettings"]["host"] = Misc.Utils.Str2JArray(vee.streamParam2);
                        break;
                    case "quic":
                        streamToken["quicSettings"]["header"]["type"] = mainParam;
                        streamToken["quicSettings"]["security"] = vee.streamParam2;
                        streamToken["quicSettings"]["key"] = vee.streamParam3;
                        break;
                    default:
                        break;
                }
            }
            catch { }

            try
            {
                streamToken["security"] = vee.isUseTls ? "tls" : "none";
            }
            catch { }

            return streamToken;
        }
        #endregion

        #region protected methods

        #endregion
    }
}
