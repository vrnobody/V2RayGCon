using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using V2RayGCon.Resources.Resx;

namespace V2RayGCon.Controllers.CoreServerComponent
{
    public sealed class Configer
        : VgcApis.BaseClasses.ComponentOf<CoreServerCtrl>,
            VgcApis.Interfaces.CoreCtrlComponents.IConfiger
    {
        readonly Services.Settings setting;
        readonly Services.Cache cache;
        readonly VgcApis.Models.Datas.CoreInfo coreInfo;

        CoreStates states;
        Logger logger;
        CoreCtrl coreCtrl;

        public Configer(
            Services.Settings setting,
            Services.Cache cache,
            VgcApis.Models.Datas.CoreInfo coreInfo
        )
        {
            this.setting = setting;
            this.cache = cache;
            this.coreInfo = coreInfo;
        }

        public override void Prepare()
        {
            states = GetSibling<CoreStates>();
            logger = GetSibling<Logger>();
            coreCtrl = GetSibling<CoreCtrl>();
        }

        #region public methods
        public VgcApis.Models.Datas.InboundInfo GetInboundInfo()
        {
            return GetAllInboundsInfo().FirstOrDefault();
        }

        public List<VgcApis.Models.Datas.InboundInfo> GetAllInboundsInfo()
        {
            var config = GetFinalConfig();

            if (VgcApis.Misc.Utils.IsYaml(config))
            {
                var r = new List<VgcApis.Models.Datas.InboundInfo>();
                var info = GetInboundInfoFromYaml(config);
                if (info != null)
                {
                    r.Add(info);
                }
                return r;
            }

            var json = VgcApis.Misc.Utils.ParseJObject(config);
            return GetInboundsInfoFromJson(json);
        }

        public string GetShareLink()
        {
            var name = GetSibling<CoreStates>().GetName();
            var config = GetConfig();
            return Services.ShareLinkMgr.Instance.EncodeConfigToShareLink(name, config);
        }

        public string GetFinalConfig()
        {
            var config = GetConfig();
            var cfgTpls = setting.GetCustomConfigTemplates();

            var names =
                coreInfo.templates?.Replace(", ", ",")?.Split(',')?.ToList() ?? new List<string>();
            names.Add(coreInfo.inbName);

            var tplsS = names
                .Select(n => cfgTpls.FirstOrDefault(t => t.name == n))
                .Where(t => t != null)
                .ToList();

            string r;
            var host = coreInfo.inbIp;
            var port = coreInfo.inbPort;
            if (VgcApis.Misc.Utils.IsJson(config))
            {
                r = GenJsonFinalConfig(tplsS, config, host, port);
            }
            else
            {
                r = config;
                foreach (var tplS in tplsS)
                {
                    r = tplS.MergeToConfig(r, host, port);
                    if (string.IsNullOrEmpty(r))
                    {
                        break;
                    }
                }
            }
            return string.IsNullOrEmpty(r) ? config : r;
        }

        public string GetRawConfig() => coreInfo.config;

        public string GetConfig() => coreInfo.GetConfig();

        public void UpdateSummary()
        {
            var config = GetFinalConfig();
            UpdateSummaryCore(config);
            GetParent().InvokeEventOnPropertyChange();
        }

        public bool IsSuitableToBeUsedAsSysProxy(bool isGlobal, out bool isSocks, out int port)
        {
            isSocks = false;
            port = 0;

            var inbs = GetAllInboundsInfo();
            var info = inbs.FirstOrDefault(inb => inb.protocol == "http");
            if (info == null)
            {
                info = inbs.FirstOrDefault(inb => inb.protocol == "socks");
            }

            if (info == null)
            {
                logger.Log(I18N.GetInboundInfoFail);
                return false;
            }

            var protocol = info.protocol ?? "";
            port = info.port;

            if (!IsProtocolMatchProxyRequirment(isGlobal, protocol))
            {
                return false;
            }

            isSocks = protocol == "socks";
            return true;
        }

        public void SetConfig(string newConfig)
        {
            if (string.IsNullOrEmpty(newConfig) || coreInfo.GetConfig() == newConfig)
            {
                return;
            }

            coreInfo.SetConfig(newConfig);
            UpdateSummary();

            if (coreCtrl.IsCoreRunning())
            {
                coreCtrl.RestartCoreThen();
            }
        }

        public void GetterInfoForNotifyIconf(Action<string> next)
        {
            var cs = GetParent().GetCoreStates();
            var name = $"{cs.GetIndex()}.[{cs.GetShortName()}]";

            VgcApis.Misc.Utils.RunInBgSlim(() =>
            {
                var lines = new List<string>() { name };

                var inbs = GetAllInboundsInfo();
                foreach (var inb in inbs)
                {
                    lines.Add($"{inb.protocol}://{inb.host}:{inb.port}");
                }

                next(string.Join(Environment.NewLine, lines));
            });
        }

        #endregion

        #region private methods
        void MergeCustomTlsSettings(ref JObject config)
        {
            var outB =
                Misc.Utils.GetKey(config, "outbound") ?? Misc.Utils.GetKey(config, "outbounds.0");

            if (outB == null)
            {
                return;
            }

            if (!(Misc.Utils.GetKey(outB, "streamSettings") is JObject streamSettings))
            {
                return;
            }

            if (setting.isSupportSelfSignedCert)
            {
                var selfSigned = JObject.Parse(@"{tlsSettings: {allowInsecure: true}}");
                Misc.Utils.MergeJson(streamSettings, selfSigned);
            }

            if (setting.isEnableUtlsFingerprint)
            {
                var uTlsFingerprint = JObject.Parse(@"{tlsSettings: {}}");
                uTlsFingerprint["tlsSettings"]["fingerprint"] = setting.uTlsFingerprint;
                Misc.Utils.MergeJson(streamSettings, uTlsFingerprint);
            }
        }

        string GenJsonFinalConfig(
            IEnumerable<Models.Datas.CustomConfigTemplate> tplsS,
            string config,
            string host,
            int port
        )
        {
            try
            {
                var json = JObject.Parse(config);
                InjectStatisticsConfigOnDemand(ref json);
                MergeCustomTlsSettings(ref json);
                foreach (var tplS in tplsS)
                {
                    if (tplS.MergeToJObject(ref json, host, port) != true)
                    {
                        break;
                    }
                }

                var send4 = setting.GetSendThroughIpv4();
                if (!string.IsNullOrEmpty(send4))
                {
                    if (json["outbounds"] is JArray outbs)
                    {
                        foreach (var outb in outbs)
                        {
                            outb["sendThrough"] = send4;
                        }
                    }
                }
                var s = VgcApis.Misc.Utils.FormatConfig(json);
                return s;
            }
            catch { }
            return null;
        }

        List<VgcApis.Models.Datas.InboundInfo> GetInboundsInfoFromJson(JObject json)
        {
            var r = new List<VgcApis.Models.Datas.InboundInfo>();
            if (json == null)
            {
                return r;
            }

            try
            {
                var arr = json["inbounds"] as JArray;
                foreach (JObject inb in arr)
                {
                    if (inb == null)
                    {
                        continue;
                    }
                    var info = new VgcApis.Models.Datas.InboundInfo()
                    {
                        protocol = Misc.Utils.GetValue<string>(inb, "protocol"),
                        host = Misc.Utils.GetValue<string>(inb, "listen"),
                        port = Misc.Utils.GetValue<int>(inb, "port"),
                    };
                    if (!string.IsNullOrEmpty(info.protocol) && !string.IsNullOrEmpty(info.host))
                    {
                        r.Add(info);
                    }
                }
            }
            catch { }

            return r;
        }

        VgcApis.Models.Datas.InboundInfo GetInboundInfoFromYaml(string config)
        {
            var pat = @"(.*):\r?\n +listen: ?([^\r\n]+)";
            var g = Regex.Match(config, pat).Groups;
            if (g.Count > 2)
            {
                VgcApis.Misc.Utils.TryParseAddress(g[2].Value, out var host, out var port);
                return new VgcApis.Models.Datas.InboundInfo()
                {
                    protocol = g[1].Value,
                    host = host,
                    port = port,
                };
            }
            return null;
        }

        void InjectStatisticsConfigOnDemand(ref JObject config)
        {
            if (!setting.isEnableStatistics)
            {
                return;
            }

            var freePort = VgcApis.Misc.Utils.GetFreeTcpPort();
            if (freePort <= 0)
            {
                return;
            }

            states.SetStatPort(freePort);

            var result = cache.tpl.LoadTemplate("statsApiV4Inb") as JObject;
            result["inbounds"][0]["port"] = freePort;
            Misc.Utils.CombineConfigWithRoutingInFront(ref result, config);
            result["inbounds"][0]["tag"] = "agentin";

            var statsTpl = cache.tpl.LoadTemplate("statsApiV4Tpl") as JObject;
            Misc.Utils.CombineConfigWithRoutingInFront(ref result, statsTpl);
            config = result;
        }

        void UpdateSummaryCore(string config)
        {
            var ty = VgcApis.Misc.Utils.DetectConfigType(config);
            var summary = $"<unknow {ty}>";
            try
            {
                var s = "";
                switch (ty)
                {
                    case VgcApis.Models.Datas.Enums.ConfigType.json:
                        s = Misc.Utils.ExtractSummaryFromJson(config);
                        break;
                    case VgcApis.Models.Datas.Enums.ConfigType.yaml:
                        s = Misc.Utils.ExtractSummaryFromYaml(config);
                        break;
                    default:
                        break;
                }
                if (!string.IsNullOrEmpty(s))
                {
                    summary = s;
                }
            }
            catch { }

            coreInfo.summary = VgcApis.Misc.Utils.FilterControlChars(summary);

            // update title & longname
            var cs = GetSibling<CoreStates>();
            cs.GetUid();

            coreInfo.title = string.Empty;
            cs.GetTitle();
        }

        bool IsProtocolMatchProxyRequirment(bool isGlobalProxy, string protocol)
        {
            if (isGlobalProxy && protocol != "http")
            {
                return false;
            }

            if (protocol != "socks" && protocol != "http")
            {
                return false;
            }

            return true;
        }

        #endregion
    }
}
