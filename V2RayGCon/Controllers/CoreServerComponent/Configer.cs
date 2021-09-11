using Newtonsoft.Json.Linq;
using System;
using V2RayGCon.Resources.Resx;

namespace V2RayGCon.Controllers.CoreServerComponent
{
    sealed public class Configer :
        VgcApis.BaseClasses.ComponentOf<CoreServerCtrl>,
        VgcApis.Interfaces.CoreCtrlComponents.IConfiger
    {
        Services.Settings setting;
        Services.Cache cache;
        Services.ConfigMgr configMgr;
        VgcApis.Models.Datas.CoreInfo coreInfo;

        CoreStates states;
        Logger logger;
        CoreCtrl coreCtrl;

        public Configer(
            Services.Settings setting,
            Services.Cache cache,
            Services.ConfigMgr configMgr,
            VgcApis.Models.Datas.CoreInfo coreInfo)
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
        public string GetShareLink()
        {
            var slinkMgr = Services.ShareLinkMgr.Instance;

            string url = null;

            try
            {
                var config = GetFinalConfig();
                var proto = Misc.Utils.GetValue<string>(config, "outbounds.0.protocol")?.ToLower();
                var cs = config?.ToString();
                switch (proto)
                {
                    case "ss":
                    case "shadowsocks":
                        url = slinkMgr.EncodeConfigToShareLink(
                            cs,
                            VgcApis.Models.Datas.Enums.LinkTypes.ss);
                        break;
                    case "vmess":
                        url = slinkMgr.EncodeConfigToShareLink(
                            cs,
                            VgcApis.Models.Datas.Enums.LinkTypes.vmess);
                        break;
                    case "vless":
                        url = slinkMgr.EncodeConfigToShareLink(
                            cs,
                            VgcApis.Models.Datas.Enums.LinkTypes.vless);
                        break;
                    default:
                        break;
                }

            }
            catch { }

            return url;
        }
        public JObject GetFinalConfig()
        {
            JObject finalConfig = configMgr.DecodeConfig(
                GetConfig(),
                true,
                false,
                coreInfo.isInjectImport);

            if (finalConfig == null)
            {
                return null;
            }

            if (!configMgr.ModifyInboundWithCustomSetting(
                ref finalConfig,
                coreInfo.customInbType,
                coreInfo.inbIp,
                coreInfo.inbPort))
            {
                return null;
            }

            InjectSkipCnSitesConfigOnDemand(ref finalConfig);
            InjectStatisticsConfigOnDemand(ref finalConfig);

            return finalConfig;
        }

        public string GetConfig() => coreInfo.config;

        public void UpdateSummary()
        {
            try
            {
                var c = GetFinalConfig();
                if (c != null)
                {
                    // update summary should not clear status
                    // this.status = string.Empty;
                    UpdateSummary(c);
                }
            }
            catch { }
            GetParent().InvokeEventOnPropertyChange();
        }

        public bool IsSuitableToBeUsedAsSysProxy(
          bool isGlobal, out bool isSocks, out int port)
        {
            isSocks = false;
            port = 0;

            var inboundInfo = GetterParsedInboundInfo(GetConfig());
            if (inboundInfo == null)
            {
                logger.Log(I18N.GetInboundInfoFail);
                return false;
            }

            var protocol = inboundInfo.Item1;
            port = inboundInfo.Item3;

            if (!IsProtocolMatchProxyRequirment(isGlobal, protocol))
            {
                return false;
            }

            isSocks = protocol == "socks";
            return true;
        }

        public void SetConfig(string newConfig)
        {
            var trimed = VgcApis.Misc.Utils.TrimConfig(newConfig);

            if (string.IsNullOrEmpty(trimed) || coreInfo.config == trimed)
            {
                return;
            }

            coreInfo.config = trimed;
            UpdateSummary();

            if (coreCtrl.IsCoreRunning())
            {
                coreCtrl.RestartCoreThen();
            }

        }

        public void GetterInfoForNotifyIconf(Action<string> next)
        {
            var cs = GetParent().GetCoreStates();
            var servInfo = $"{cs.GetIndex()}.[{cs.GetShortName()}]";

            VgcApis.Misc.Utils.RunInBackground(() =>
            {
                var inInfo = GetterParsedInboundInfo(GetConfig());
                if (inInfo == null)
                {
                    next(servInfo);
                    return;
                }
                if (string.IsNullOrEmpty(inInfo.Item2))
                {
                    next(string.Format("{0} {1}", servInfo, inInfo.Item1));
                    return;
                }
                next(string.Format("{0} {1}://{2}:{3}",
                    servInfo,
                    inInfo.Item1,
                    inInfo.Item2,
                    inInfo.Item3));
            });
        }

        #endregion

        #region private methods
        void InjectSkipCnSitesConfigOnDemand(ref JObject config)
        {
            if (!coreInfo.isInjectSkipCNSite)
            {
                return;
            }

            // 优先考虑兼容旧配置。
            configMgr.InjectSkipCnSiteSettingsIntoConfig(
                ref config,
                false);
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

        void UpdateSummary(JObject config)
        {
            var name = Misc.Utils.GetAliasFromConfig(config);
            coreInfo.name = name;
            coreInfo.summary = Misc.Utils.GetSummaryFromConfig(config);
            coreInfo.ClearCachedString();
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

        /// <summary>
        /// return Tuple(protocol, ip, port)
        /// </summary>
        /// <returns></returns>
        Tuple<string, string, int> GetterParsedInboundInfo(string rawConfig)
        {
            var protocol = Misc.Utils.InboundTypeNumberToName(coreInfo.customInbType);
            switch (protocol)
            {
                case "http":
                case "socks":
                    var info = new Tuple<string, string, int>(
                        protocol, coreInfo.inbIp, coreInfo.inbPort);
                    return info;
                case "config":
                    return GetInboundInfoFromConfig(rawConfig);
                case "custom":
                    return GetInboundInfoFromCustomInboundsSetting();
                default:
                    return null;
            }
        }

        Tuple<string, string, int> GetInboundInfoFromCustomInboundsSetting()
        {
            try
            {
                var jobj = JArray.Parse(setting.CustomDefInbounds)[0];
                var protocol = Misc.Utils.GetValue<string>(jobj, "protocol");
                string ip = Misc.Utils.GetValue<string>(jobj, "listen");
                int port = Misc.Utils.GetValue<int>(jobj, "port");
                return new Tuple<string, string, int>(protocol, ip, port);
            }
            catch
            {
                setting.SendLog(I18N.ParseCustomInboundsSettingFail);
            }
            return null;
        }

        Tuple<string, string, int> GetInboundInfoFromConfig(string rawConfig)
        {

            var parsedConfig = configMgr.DecodeConfig(
                rawConfig,
                true,
                false,
                coreInfo.isInjectImport);

            if (parsedConfig == null)
            {
                return null;
            }

            string prefix = "inbound";
            string protocol = "";
            foreach (var p in new string[] { "inbound", "inbounds.0" })
            {
                prefix = p;
                protocol = Misc.Utils.GetValue<string>(parsedConfig, prefix, "protocol");

                if (!string.IsNullOrEmpty(protocol))
                {
                    break;
                }
            }

            string ip = Misc.Utils.GetValue<string>(parsedConfig, prefix, "listen");
            int port = Misc.Utils.GetValue<int>(parsedConfig, prefix, "port");
            return new Tuple<string, string, int>(protocol, ip, port);
        }


        #endregion
    }
}
