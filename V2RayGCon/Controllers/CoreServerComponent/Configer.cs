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

        public void UpdateSummaryThen(Action next = null)
        {
            VgcApis.Misc.Utils.RunInBackground(() =>
            {
                var configString = coreInfo.isInjectImport ?
                    configMgr.InjectImportTpls(coreInfo.config, false, true) :
                    coreInfo.config;
                try
                {
                    UpdateSummary(
                        configMgr.ParseImport(configString));
                }
                catch
                {
                    UpdateSummary(JObject.Parse(configString));
                }

                // update summary should not clear status
                // this.status = string.Empty;
                GetParent().InvokeEventOnPropertyChange();
                next?.Invoke();
            });
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

            if (string.IsNullOrEmpty(trimed)
                || coreInfo.config == trimed)
            {
                return;
            }

            coreInfo.config = trimed;
            UpdateSummaryThen(() =>
            {
                GetParent().InvokeEventOnPropertyChange();
            });

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
            var ip = coreInfo.inbIp;
            var port = coreInfo.inbPort;

            if (protocol != "config")
            {
                return new Tuple<string, string, int>(protocol, ip, port);
            }

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
            foreach (var p in new string[] { "inbound", "inbounds.0" })
            {
                prefix = p;
                protocol = Misc.Utils.GetValue<string>(
                    parsedConfig, prefix, "protocol");

                if (!string.IsNullOrEmpty(protocol))
                {
                    break;
                }
            }

            ip = Misc.Utils.GetValue<string>(parsedConfig, prefix, "listen");
            port = Misc.Utils.GetValue<int>(parsedConfig, prefix, "port");
            return new Tuple<string, string, int>(protocol, ip, port);
        }
        #endregion
    }
}
