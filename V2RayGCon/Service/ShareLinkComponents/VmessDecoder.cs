using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace V2RayGCon.Service.ShareLinkComponents
{
    internal sealed class VmessDecoder :
        VgcApis.Models.BaseClasses.ComponentOf<Codecs>,
        VgcApis.Models.Interfaces.IShareLinkDecoder
    {
        Cache cache;

        public VmessDecoder(Cache cache)
        {
            this.cache = cache;
        }
        #region properties

        #endregion

        #region public methods
        public Tuple<JObject, JToken> Decode(string shareLink)
        {
            var vmess = Lib.Utils.VmessLink2Vmess(shareLink);
            return Vmess2Config(vmess);
        }

        public string Encode(string config) =>
            ConfigString2Vmess(config)?.ToVmessLink();

        public List<string> ExtractLinksFromText(string text) =>
            Lib.Utils.ExtractLinks(text, VgcApis.Models.Datas.Enum.LinkTypes.vmess);
        #endregion

        #region private methods
        bool TryParseConfig(string config, out JObject json)
        {
            json = null;
            try
            {
                json = JObject.Parse(config);
                return json != null;
            }
            catch { }
            return false;
        }

        bool TryDetectConfigVersion(
            Func<string, string, string> GetStr,
            out bool isUseV4,
            out string root)
        {
            isUseV4 = (GetStr("outbounds.0", "protocol")?.ToLower()) == "vmess";
            root = isUseV4 ? "outbounds.0" : "outbound";
            if (isUseV4)
            {
                return true;
            }

            if (GetStr(root, "protocol")?.ToLower() == "vmess")
            {
                return true;
            }
            return false;
        }

        Model.Data.Vmess ConfigString2Vmess(string config)
        {
            if (!TryParseConfig(config, out JObject json))
            {
                return null;
            }

            var GetStr = Lib.Utils.GetStringByPrefixAndKeyHelper(json);
            if (!TryDetectConfigVersion(GetStr, out bool isUseV4, out string root))
            {
                return null;
            }

            var basicPrefix = root + "." + "settings.vnext.0";
            Model.Data.Vmess vmess = ExtractBasicInfo(GetStr, basicPrefix);

            var streamPrefix = root + "." + "streamSettings";
            vmess.net = GetStr(streamPrefix, "network");
            vmess.tls = GetStr(streamPrefix, "security");

            switch (vmess.net)
            {
                case "quic":
                    vmess.type = GetStr(streamPrefix, "quicSettings.header.type");
                    vmess.host = GetStr(streamPrefix, "quicSettings.security");
                    vmess.path = GetStr(streamPrefix, "quicSettings.key");
                    break;
                case "tcp":
                    vmess.type = GetStr(streamPrefix, "tcpSettings.header.type");
                    if (vmess.type?.ToLower() == "http")
                    {
                        ExtractTcpHttpSettings(json, isUseV4, vmess);
                    }
                    break;
                case "kcp":
                    vmess.type = GetStr(streamPrefix, "kcpSettings.header.type");
                    break;
                case "ws":
                    vmess.path = GetStr(streamPrefix, "wsSettings.path");
                    vmess.host = GetStr(streamPrefix, "wsSettings.headers.Host");
                    break;
                case "h2":
                    try
                    {
                        vmess.path = GetStr(streamPrefix, "httpSettings.path");
                        var hosts = isUseV4 ?
                            json["outbounds"][0]["streamSettings"]["httpSettings"]["host"] :
                            json["outbound"]["streamSettings"]["httpSettings"]["host"];
                        vmess.host = Lib.Utils.JArray2Str(hosts as JArray);
                    }
                    catch { }
                    break;
                default:
                    break;
            }
            return vmess;
        }

        void ExtractTcpHttpSettings(JObject json, bool isUseV4, Model.Data.Vmess vmess)
        {
            try
            {
                var path = isUseV4 ?
                    json["outbounds"][0]["streamSettings"]["tcpSettings"]["header"]["request"]["path"] :
                    json["outbound"]["streamSettings"]["tcpSettings"]["header"]["request"]["path"];
                vmess.path = Lib.Utils.JArray2Str(path as JArray);
            }
            catch { }
            try
            {
                var hosts = isUseV4 ?
                    json["outbounds"][0]["streamSettings"]["tcpSettings"]["header"]["request"]["headers"]["Host"] :
                    json["outbound"]["streamSettings"]["tcpSettings"]["header"]["request"]["headers"]["Host"];
                vmess.host = Lib.Utils.JArray2Str(hosts as JArray);
            }
            catch { }
        }

        Model.Data.Vmess ExtractBasicInfo(
            Func<string, string, string> GetStr, string prefix)
        {
            Model.Data.Vmess vmess = new Model.Data.Vmess
            {
                v = "2",
                ps = GetStr("v2raygcon", "alias")
            };
            vmess.add = GetStr(prefix, "address");
            vmess.port = GetStr(prefix, "port");
            vmess.id = GetStr(prefix, "users.0.id");
            vmess.aid = GetStr(prefix, "users.0.alterId");
            return vmess;
        }

        Tuple<JObject, JToken> Vmess2Config(Model.Data.Vmess vmess)
        {
            if (vmess == null)
            {
                return null;
            }

            var outVmess = cache.tpl.LoadTemplate("outbVmess");
            outVmess["streamSettings"] = GenStreamSetting(vmess);
            var node = outVmess["settings"]["vnext"][0];
            node["address"] = vmess.add;
            node["port"] = Lib.Utils.Str2Int(vmess.port);
            node["users"][0]["id"] = vmess.id;
            node["users"][0]["alterId"] = Lib.Utils.Str2Int(vmess.aid);

            var tpl = cache.tpl.LoadTemplate("tplImportVmess") as JObject;
            tpl["v2raygcon"]["alias"] = vmess.ps;
            return new Tuple<JObject, JToken>(tpl, outVmess);
        }

        JToken GenStreamSetting(Model.Data.Vmess vmess)
        {
            // insert stream type
            string[] streamTypes = { "ws", "tcp", "kcp", "h2", "quic" };
            string streamType = vmess.net.ToLower();

            if (!streamTypes.Contains(streamType))
            {
                return JToken.Parse(@"{}");
            }

            if (streamType == "tcp" && vmess.type == "http")
            {
                streamType = "tcp_http";
            }
            var streamToken = cache.tpl.LoadTemplate(streamType);
            try
            {
                FillStreamSettingsDetail(vmess, streamType, streamToken);
            }
            catch { }

            try
            {
                streamToken["security"] = (vmess.tls?.ToLower() == "tls") ? "tls" : "none";
            }
            catch { }

            return streamToken;
        }

        private static void FillStreamSettingsDetail(Model.Data.Vmess vmess, string streamType, JToken streamToken)
        {
            switch (streamType)
            {
                case "quic":
                    streamToken["quicSettings"]["header"]["type"] = vmess.type; // quic.type
                    streamToken["quicSettings"]["security"] = vmess.host; // quic.security
                    streamToken["quicSettings"]["key"] = vmess.path; // quic.key
                    break;
                case "tcp":
                    // issue #7 should keep header.type = none
                    // streamToken["tcpSettings"]["header"]["type"] = vmess.type;
                    break;
                case "tcp_http":
                    streamToken["tcpSettings"]["header"]["type"] = vmess.type;
                    streamToken["tcpSettings"]["header"]["request"]["path"] =
                        Lib.Utils.Str2JArray(string.IsNullOrEmpty(vmess.path) ? "/" : vmess.path);
                    streamToken["tcpSettings"]["header"]["request"]["headers"]["Host"] =
                        Lib.Utils.Str2JArray(vmess.host);
                    break;
                case "kcp":
                    streamToken["kcpSettings"]["header"]["type"] = vmess.type;
                    break;
                case "ws":
                    streamToken["wsSettings"]["path"] =
                        string.IsNullOrEmpty(vmess.v) ? vmess.host : vmess.path;
                    if (vmess.v == "2" && !string.IsNullOrEmpty(vmess.host))
                    {
                        streamToken["wsSettings"]["headers"]["Host"] = vmess.host;
                    }
                    break;
                case "h2":
                    streamToken["httpSettings"]["path"] = vmess.path;
                    streamToken["httpSettings"]["host"] = Lib.Utils.Str2JArray(vmess.host);
                    break;
            }
        }
        #endregion

        #region protected methods

        #endregion
    }
}
