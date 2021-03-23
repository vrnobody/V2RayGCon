using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using V2RayGCon.Resources.Resx;

namespace V2RayGCon.Services.ShareLinkComponents
{
    internal sealed class VmessDecoder :
        VgcApis.BaseClasses.ComponentOf<Codecs>,
        VgcApis.Interfaces.IShareLinkDecoder
    {
        Cache cache;
        private readonly Settings setting;

        public VmessDecoder(
            Cache cache,
            Settings setting)
        {
            this.cache = cache;
            this.setting = setting;
        }
        #region properties

        #endregion

        #region public methods
        public Tuple<JObject, JToken> Decode(string shareLink)
        {
            var vmess = Misc.Utils.VmessLink2Vmess(shareLink);
            return Vmess2Config(vmess);
        }

        public string Encode(string config) =>
            ConfigString2Vmess(config)?.ToVmessLink();

        public List<string> ExtractLinksFromText(string text) =>
            Misc.Utils.ExtractLinks(text, VgcApis.Models.Datas.Enums.LinkTypes.vmess);
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

        Models.Datas.Vmess ConfigString2Vmess(string config)
        {
            if (!TryParseConfig(config, out JObject json))
            {
                return null;
            }

            var GetStr = Misc.Utils.GetStringByPrefixAndKeyHelper(json);
            if (!TryDetectConfigVersion(GetStr, out bool isUseV4, out string root))
            {
                return null;
            }

            var basicPrefix = root + "." + "settings.vnext.0";
            Models.Datas.Vmess vmess = ExtractBasicInfo(GetStr, basicPrefix);

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
                    vmess.path = GetStr(streamPrefix, "kcpSettings.seed");
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
                        vmess.host = Misc.Utils.JArray2Str(hosts as JArray);
                    }
                    catch { }
                    break;
                case "":
                    // stream type none
                    break;
                default:
                    // unsupported stream type
                    return null;
            }
            return vmess;
        }

        void ExtractTcpHttpSettings(JObject json, bool isUseV4, Models.Datas.Vmess vmess)
        {
            try
            {
                var path = isUseV4 ?
                    json["outbounds"][0]["streamSettings"]["tcpSettings"]["header"]["request"]["path"] :
                    json["outbound"]["streamSettings"]["tcpSettings"]["header"]["request"]["path"];
                vmess.path = Misc.Utils.JArray2Str(path as JArray);
            }
            catch { }
            try
            {
                var hosts = isUseV4 ?
                    json["outbounds"][0]["streamSettings"]["tcpSettings"]["header"]["request"]["headers"]["Host"] :
                    json["outbound"]["streamSettings"]["tcpSettings"]["header"]["request"]["headers"]["Host"];
                vmess.host = Misc.Utils.JArray2Str(hosts as JArray);
            }
            catch { }
        }

        Models.Datas.Vmess ExtractBasicInfo(
            Func<string, string, string> GetStr, string prefix)
        {
            Models.Datas.Vmess vmess = new Models.Datas.Vmess
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

        Tuple<JObject, JToken> Vmess2Config(Models.Datas.Vmess vmess)
        {
            if (vmess == null)
            {
                return null;
            }

            var outVmess = LoadVmessDecodeTemplate();

            var streamToken = JObject.Parse(@"{streamSettings:{}}");
            streamToken["streamSettings"] = GenStreamSetting(vmess);
            var o = outVmess as JObject;
            Misc.Utils.MergeJson(ref o, streamToken);

            var node = outVmess["settings"]["vnext"][0];
            node["address"] = vmess.add;
            node["port"] = VgcApis.Misc.Utils.Str2Int(vmess.port);
            node["users"][0]["id"] = vmess.id;
            node["users"][0]["alterId"] = VgcApis.Misc.Utils.Str2Int(vmess.aid);

            var tpl = cache.tpl.LoadTemplate("tplImportVmess") as JObject;
            tpl["v2raygcon"]["alias"] = vmess.ps;
            return new Tuple<JObject, JToken>(tpl, outVmess);
        }

        JToken LoadVmessDecodeTemplate()
        {
            if (!setting.CustomVmessDecodeTemplateEnabled)
            {
                return cache.tpl.LoadTemplate("outbVmess");
            }

            try
            {
                var str = System.IO.File.ReadAllText(setting.CustomVmessDecodeTemplateUrl);
                return JObject.Parse(str);
            }
            catch
            {
                setting.SendLog(I18N.LoadVemssDecodeTemplateFail);
                throw;
            }
        }


        JToken GenStreamSetting(Models.Datas.Vmess vmess)
        {
            // insert stream type
            string[] streamTypes = { "ws", "tcp", "kcp", "h2", "quic" };
            string streamType = vmess?.net?.ToLower();

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

            var isUseTls = vmess.tls?.ToLower() == "tls";
            try
            {
                streamToken["security"] = isUseTls ? "tls" : "none";
            }
            catch { }
            return streamToken;
        }

        private static void FillStreamSettingsDetail(Models.Datas.Vmess vmess, string streamType, JToken streamToken)
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
                        Misc.Utils.Str2JArray(string.IsNullOrEmpty(vmess.path) ? "/" : vmess.path);
                    streamToken["tcpSettings"]["header"]["request"]["headers"]["Host"] =
                        Misc.Utils.Str2JArray(vmess.host);
                    break;
                case "kcp":
                    streamToken["kcpSettings"]["header"]["type"] = vmess.type;
                    if (!string.IsNullOrEmpty(vmess.path))
                    {
                        streamToken["kcpSettings"]["seed"] = vmess.path;
                    }
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
                    streamToken["httpSettings"]["host"] = Misc.Utils.Str2JArray(vmess.host);
                    break;
            }
        }
        #endregion

        #region protected methods

        #endregion
    }
}
