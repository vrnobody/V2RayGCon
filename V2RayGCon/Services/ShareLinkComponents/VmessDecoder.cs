using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using V2RayGCon.Resources.Resx;

namespace V2RayGCon.Services.ShareLinkComponents
{
    internal sealed class VmessDecoder
        : VgcApis.BaseClasses.ComponentOf<Codecs>,
            VgcApis.Interfaces.IShareLinkDecoder
    {
        readonly Cache cache;
        private readonly Settings setting;

        public VmessDecoder(Cache cache, Settings setting)
        {
            this.cache = cache;
            this.setting = setting;
        }
        #region properties

        #endregion

        #region public methods
        public VgcApis.Models.Datas.DecodeResult Decode(string shareLink)
        {
            var vmess = Misc.Utils.VmessLink2Vmess(shareLink);
            var config = Vmess2Config(vmess);
            return new VgcApis.Models.Datas.DecodeResult(vmess.ps, config);
        }

        public string Encode(string name, string config)
        {
            var vc = new Models.Datas.SharelinkMetadata(config);
            if (vc.proto != @"vmess")
            {
                return null;
            }
            vc.name = name;
            return vc.ToShareLink();
        }

        public List<string> ExtractLinksFromText(string text) =>
            Misc.Utils.ExtractLinks(text, VgcApis.Models.Datas.Enums.LinkTypes.vmess);
        #endregion

        #region private methods
        string Vmess2Config(Models.Datas.Vmess vmess)
        {
            if (vmess == null)
            {
                return null;
            }

            var outVmess = LoadVmessDecodeTemplate();

            var streamToken = JObject.Parse(@"{streamSettings:{}}");
            streamToken["streamSettings"] = GenStreamSetting(vmess);
            var o = outVmess as JObject;
            Misc.Utils.MergeJson(o, streamToken);

            var node = outVmess["settings"]["vnext"][0];
            node["address"] = vmess.add;
            node["port"] = VgcApis.Misc.Utils.Str2Int(vmess.port);
            node["users"][0]["id"] = vmess.id;
            node["users"][0]["alterId"] = VgcApis.Misc.Utils.Str2Int(vmess.aid);

            var tpl = cache.tpl.LoadTemplate("tplImportVmess") as JObject;
            return GetParent()?.GenerateJsonConfing(tpl, outVmess);
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
            string[] streamTypes = { "ws", "tcp", "kcp", "h2", "quic", "grpc" };
            string streamType = vmess?.net?.ToLower();

            if (!streamTypes.Contains(streamType))
            {
                return JToken.Parse(@"{}");
            }

            if (streamType == "tcp" && vmess.type == "http")
            {
                streamType = "tcp_http";
            }

            if (streamType == "grpc")
            {
                streamType = "grpc_multi";
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

            if (isUseTls && !string.IsNullOrWhiteSpace(vmess.sni))
            {
                try
                {
                    streamToken["tlsSettings"] = JObject.Parse(@"{}");
                    streamToken["tlsSettings"]["allowInsecure"] = false;
                    streamToken["tlsSettings"]["serverName"] = vmess.sni;
                }
                catch { }
            }
            return streamToken;
        }

        private static void FillStreamSettingsDetail(
            Models.Datas.Vmess vmess,
            string streamType,
            JToken streamToken
        )
        {
            switch (streamType)
            {
                case "grpc_multi":
                    streamToken["grpcSettings"]["serviceName"] = vmess.path; // quic.type
                    break;
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
                    streamToken["tcpSettings"]["header"]["request"]["path"] = Misc.Utils.Str2JArray(
                        string.IsNullOrEmpty(vmess.path) ? "/" : vmess.path
                    );
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
                    streamToken["wsSettings"]["path"] = string.IsNullOrEmpty(vmess.v)
                        ? vmess.host
                        : vmess.path;
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
