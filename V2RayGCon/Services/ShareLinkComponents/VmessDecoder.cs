using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using VgcApis.Models.Datas;

namespace V2RayGCon.Services.ShareLinkComponents
{
    internal sealed class VmessDecoder
        : VgcApis.BaseClasses.ComponentOf<Codecs>,
            VgcApis.Interfaces.IShareLinkDecoder
    {
        public VmessDecoder() { }
        #region properties

        #endregion

        #region public methods
        public DecodeResult Decode(string shareLink)
        {
            var vmess = Misc.Utils.VmessLink2Vmess(shareLink);
            var config = Vmess2Config(vmess);
            return new DecodeResult(vmess.ps, config);
        }

        public string Encode(string name, string config)
        {
            if (Comm.TryParseConfig(config, out var vc) && vc != null && vc.proto == "vmess")
            {
                vc.name = name;
                return vc.ToShareLink();
            }
            return null;
        }

        public List<string> ExtractLinksFromText(string text) =>
            Misc.Utils.ExtractLinks(text, Enums.LinkTypes.vmess);
        #endregion

        #region private methods
        string Vmess2Config(Vmess vmess)
        {
            if (vmess == null)
            {
                return null;
            }

            var outVmess = Misc.Caches.Jsons.LoadTemplate("outbVmess");
            var streamToken = JObject.Parse(@"{streamSettings:{}}");
            streamToken["streamSettings"] = GenStreamSetting(vmess);
            var o = outVmess as JObject;
            VgcApis.Misc.Utils.MergeJson(o, streamToken);

            var node = outVmess["settings"]["vnext"][0];
            node["address"] = vmess.add;
            node["port"] = VgcApis.Misc.Utils.Str2Int(vmess.port);
            node["users"][0]["id"] = vmess.id;
            node["users"][0]["alterId"] = VgcApis.Misc.Utils.Str2Int(vmess.aid);

            var tpl = Misc.Caches.Jsons.LoadTemplate("tplLogWarn") as JObject;
            return GetParent()?.GenerateJsonConfing(tpl, outVmess);
        }

        JToken GenStreamSetting(Vmess vmess)
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

            var streamToken = Misc.Caches.Jsons.LoadTemplate(streamType);
            try
            {
                FillInTlsSettings(vmess, streamToken);
                FillStreamSettingsDetail(vmess, streamType, streamToken);
            }
            catch { }
            return streamToken;
        }

        private static void FillInTlsSettings(Vmess vmess, JToken stream)
        {
            var ty = vmess.tls?.ToLower() == "tls" ? "tls" : "none";
            stream["security"] = ty;
            if (ty == "none")
            {
                return;
            }
            var o = new JObject();

            void SetValue(string key, string value)
            {
                if (!string.IsNullOrEmpty(value))
                {
                    o[key] = value;
                }
            }
            SetValue("serverName", vmess.sni);
            SetValue("fingerprint", vmess.fp);

            o["allowInsecure"] = false;
            if (!string.IsNullOrEmpty(vmess.alpn))
            {
                o["alpn"] = VgcApis.Misc.Utils.Str2JArray(vmess.alpn);
            }

            stream["tlsSettings"] = o;
        }

        private static void FillStreamSettingsDetail(
            Vmess vmess,
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
                    streamToken["tcpSettings"]["header"]["request"]["path"] =
                        VgcApis.Misc.Utils.Str2JArray(
                            string.IsNullOrEmpty(vmess.path) ? "/" : vmess.path
                        );
                    streamToken["tcpSettings"]["header"]["request"]["headers"]["Host"] =
                        VgcApis.Misc.Utils.Str2JArray(vmess.host);
                    break;
                case "kcp":
                    streamToken["kcpSettings"]["header"]["type"] = vmess.type;
                    if (!string.IsNullOrEmpty(vmess.path))
                    {
                        streamToken["kcpSettings"]["seed"] = vmess.path;
                    }
                    break;
                case "ws":
                    // 2024-05-17 应该没人用v1了吧。
                    // 2024-05-25 竟然还有人用v1。
                    if (!string.IsNullOrEmpty(vmess.host))
                    {
                        if (vmess.host.StartsWith("/"))
                        {
                            // v1
                            streamToken["wsSettings"]["path"] = vmess.host;
                        }
                        else
                        {
                            streamToken["wsSettings"]["host"] = vmess.host;
                            streamToken["wsSettings"]["headers"]["Host"] = vmess.host;
                        }
                    }
                    // many thanks to those guys who do not set vmess.v to "2"
                    if (!string.IsNullOrEmpty(vmess.path))
                    {
                        streamToken["wsSettings"]["path"] = vmess.path;
                    }
                    break;
                case "h2":
                    streamToken["httpSettings"]["path"] = vmess.path;
                    streamToken["httpSettings"]["host"] = VgcApis.Misc.Utils.Str2JArray(vmess.host);
                    break;
            }
        }
        #endregion

        #region protected methods

        #endregion
    }
}
