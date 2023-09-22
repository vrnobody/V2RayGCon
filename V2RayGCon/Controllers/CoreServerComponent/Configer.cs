using Newtonsoft.Json.Linq;
using System;
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
        readonly Services.ConfigMgr configMgr;
        readonly VgcApis.Models.Datas.CoreInfo coreInfo;

        CoreStates states;
        Logger logger;
        CoreCtrl coreCtrl;

        public Configer(
            Services.Settings setting,
            Services.Cache cache,
            Services.ConfigMgr configMgr,
            VgcApis.Models.Datas.CoreInfo coreInfo
        )
        {
            this.configMgr = configMgr;
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
            var config = GetFinalConfig();

            if (VgcApis.Misc.Utils.IsYaml(config))
            {
                return GetInboundInfoFromYaml(config);
            }

            var json = VgcApis.Misc.Utils.ParseJObject(config);
            return GetInboundInfoFromJson(json);
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
            var inbS = setting
                .GetCustomInboundsSetting()
                .FirstOrDefault(inb => inb.name == coreInfo.inbName);

            var ty = VgcApis.Misc.Utils.DetectConfigType(config);
            string r;
            var host = coreInfo.inbIp;
            var port = coreInfo.inbPort;
            switch (ty)
            {
                case VgcApis.Models.Datas.Enums.ConfigType.yaml:
                    r = inbS?.MergeToYaml(config, host, port);
                    break;
                case VgcApis.Models.Datas.Enums.ConfigType.json:
                    r = GenJsonFinalConfig(inbS, config, host, port);
                    break;
                default:
                    r = inbS?.MergeToText(config, host, port);
                    break;
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

            var inbInfo = GetInboundInfo();
            if (inbInfo == null)
            {
                logger.Log(I18N.GetInboundInfoFail);
                return false;
            }

            var protocol = inbInfo.protocol ?? "";
            port = inbInfo.port;

            if (!IsProtocolMatchProxyRequirment(isGlobal, protocol))
            {
                return false;
            }

            isSocks = protocol.StartsWith("socks");
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
                var inb = GetInboundInfo();
                if (inb == null)
                {
                    next(name);
                    return;
                }
                if (string.IsNullOrEmpty(inb.host))
                {
                    next(string.Format("{0} {1}", name, inb.port));
                    return;
                }
                next(string.Format("{0} {1}://{2}:{3}", name, inb.protocol, inb.host, inb.port));
            });
        }

        #endregion

        #region private methods
        string GenJsonFinalConfig(
            Models.Datas.CustomInboundSettings inbS,
            string config,
            string host,
            int port
        )
        {
            try
            {
                var json = JObject.Parse(config);
                InjectStatisticsConfigOnDemand(ref json);
                configMgr.MergeCustomTlsSettings(ref json);
                inbS?.MergeToJObject(ref json, host, port);
                var s = VgcApis.Misc.Utils.FormatConfig(json);
                return s;
            }
            catch { }
            return null;
        }

        VgcApis.Models.Datas.InboundInfo GetInboundInfoFromJson(JObject json)
        {
            if (json == null)
            {
                return null;
            }

            foreach (var p in new string[] { "inbound", "inbounds.0" })
            {
                var protocol = Misc.Utils.GetValue<string>(json, p, "protocol");
                if (!string.IsNullOrEmpty(protocol))
                {
                    string host = Misc.Utils.GetValue<string>(json, p, "listen");
                    return new VgcApis.Models.Datas.InboundInfo()
                    {
                        protocol = protocol,
                        host = host ?? VgcApis.Models.Consts.Webs.LoopBackIP,
                        port = Misc.Utils.GetValue<int>(json, p, "port")
                    };
                }
            }

            return null;
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
