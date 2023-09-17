using Newtonsoft.Json.Linq;
using System;
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
        public string GetShareLink()
        {
            var name = GetSibling<CoreStates>().GetName();
            var config = GetConfig();
            return Services.ShareLinkMgr.Instance.EncodeConfigToShareLink(name, config);
        }

        public string GetFinalConfig()
        {
            JObject json = VgcApis.Misc.Utils.ParseJObject(GetConfig());

            if (json != null)
            {
                if (
                    configMgr.ModifyInboundWithCustomSetting(
                        ref json,
                        coreInfo.customInbType,
                        coreInfo.inbIp,
                        coreInfo.inbPort
                    )
                )
                {
                    InjectStatisticsConfigOnDemand(ref json);
                    configMgr.MergeCustomTlsSettings(ref json);

                    var config = VgcApis.Misc.Utils.FormatConfig(json);
                    return config;
                }
            }

            return GetConfig();
        }

        public string GetRawConfig() => coreInfo.config;

        public string GetConfig() => coreInfo.GetConfig();

        public void UpdateSummary()
        {
            try
            {
                var json = VgcApis.Misc.Utils.ParseJObject(GetFinalConfig());
                if (json != null)
                {
                    // update summary should not clear status
                    // this.status = string.Empty;
                    UpdateSummary(json);
                }
            }
            catch { }
            GetParent().InvokeEventOnPropertyChange();
        }

        public bool IsSuitableToBeUsedAsSysProxy(bool isGlobal, out bool isSocks, out int port)
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
            var servInfo = $"{cs.GetIndex()}.[{cs.GetShortName()}]";

            VgcApis.Misc.Utils.RunInBgSlim(() =>
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
                next(
                    string.Format(
                        "{0} {1}://{2}:{3}",
                        servInfo,
                        inInfo.Item1,
                        inInfo.Item2,
                        inInfo.Item3
                    )
                );
            });
        }

        #endregion

        #region private methods

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
            var summary = Misc.Utils.GetSummaryFromConfig(config);
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
                        protocol,
                        coreInfo.inbIp,
                        coreInfo.inbPort
                    );
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
            if (rawConfig == null)
            {
                return null;
            }

            string prefix = "inbound";
            string protocol = "";
            foreach (var p in new string[] { "inbound", "inbounds.0" })
            {
                prefix = p;
                protocol = Misc.Utils.GetValue<string>(rawConfig, prefix, "protocol");

                if (!string.IsNullOrEmpty(protocol))
                {
                    break;
                }
            }

            string ip = Misc.Utils.GetValue<string>(rawConfig, prefix, "listen");
            int port = Misc.Utils.GetValue<int>(rawConfig, prefix, "port");
            return new Tuple<string, string, int>(protocol, ip, port);
        }

        #endregion
    }
}
