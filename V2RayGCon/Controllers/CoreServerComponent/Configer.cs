using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using V2RayGCon.Resources.Resx;
using V2RayGCon.Services;
using VgcApis.Models.Datas;

namespace V2RayGCon.Controllers.CoreServerComponent
{
    public sealed class Configer
        : VgcApis.BaseClasses.ComponentOf<CoreServerCtrl>,
            VgcApis.Interfaces.CoreCtrlComponents.IConfiger
    {
        readonly Settings setting;
        private readonly Servers servers;

        readonly CoreInfo coreInfo;

        CoreStates states;
        Logger logger;
        CoreCtrl coreCtrl;

        readonly object finalConfigLocker = new object();
        FinalConfigCacheSettings lastCacheSettings = new FinalConfigCacheSettings();

        public Configer(Settings setting, Servers servers, CoreInfo coreInfo)
        {
            this.setting = setting;
            this.servers = servers;
            this.coreInfo = coreInfo;
        }

        public override void Prepare()
        {
            states = GetSibling<CoreStates>();
            logger = GetSibling<Logger>();
            coreCtrl = GetSibling<CoreCtrl>();
        }

        #region properties
        List<InboundInfo> InbsInfoCache
        {
            get => coreInfo.inboundsInfoCache;
            set
            {
                coreInfo.inboundsInfoCache = value;
                GetParent().InvokeEventOnPropertyChange();
            }
        }
        #endregion

        #region public methods
        public InboundInfo GetInboundInfo()
        {
            return GetAllInboundsInfo().FirstOrDefault();
        }

        public ReadOnlyCollection<string> GetFormattedInboundsInfoFromCache()
        {
            var cache = InbsInfoCache;
            if (cache == null)
            {
                // VgcApis.Misc.Logger.Debug("re-gen inbounds info");
                cache = GetAllInboundsInfo();
                InbsInfoCache = cache;
            }
            else
            {
                // VgcApis.Misc.Logger.Debug("get inbounds info from cache");
            }
            return cache.Select(c => c.ToString()).ToList().AsReadOnly();
            ;
        }

        public void GatherInfoForNotifyIcon(Action<string> next)
        {
            if (next == null)
            {
                return;
            }

            var inbsInfo = GetFormattedInboundsInfoFromCache();
            VgcApis.Misc.Utils.RunInBackground(() =>
            {
                var cs = GetParent().GetCoreStates();
                var name = $"{cs.GetIndex()}.[{cs.GetShortName()}]";
                var lines = new List<string>() { name };
                var inbs = GetFormattedInboundsInfoFromCache();
                lines.AddRange(inbs);
                var info = string.Join(Environment.NewLine, lines);
                next(info);
            });
        }

        public List<InboundInfo> GetAllInboundsInfo()
        {
            var config = GetFinalConfig();
            var ty = VgcApis.Misc.Utils.DetectConfigType(config);
            switch (ty)
            {
                case Enums.ConfigType.yaml:
                    return GetInboundsInfoFromYaml(config);
                case Enums.ConfigType.json:
                    return VgcApis.Misc.Utils.GetInboundsInfoFromJsonConfig(config);
                default:
                    return new List<InboundInfo>();
            }
        }

        public string GetShareLink()
        {
            var name = GetSibling<CoreStates>().GetName();
            var config = GetConfig();
            return ShareLinkMgr.Instance.EncodeConfigToShareLink(name, config);
        }

        public string GetFinalConfig()
        {
            lock (finalConfigLocker)
            {
                var uid = states.GetUid();
                var send4 = states.IsIgnoreSendThrough() ? "" : setting.GetSendThroughIpv4();
                if (
                    IsFinalConfigCacheValid(send4)
                    && Misc.Caches.ZipStrLru.TryGet(uid, out var r)
                    && !string.IsNullOrEmpty(r)
                )
                {
                    VgcApis.Misc.Logger.Debug("get final config from cache");
                    return r;
                }
                r = GenFinalConfigCore(false);
                lastCacheSettings = new FinalConfigCacheSettings()
                {
                    enableUtls = setting.isEnableUtlsFingerprint,
                    fingerprint = setting.uTlsFingerprint,
                    sendThrough = send4,
                };
                Misc.Caches.ZipStrLru.Put(uid, r);
                return r;
            }
        }

        public void ClearFinalConfigCache() => Misc.Caches.ZipStrLru.TryRemove(states.GetUid());

        public string GetRawConfig() => coreInfo.config;

        public string GetConfig() => coreInfo.GetConfig();

        public void UpdateSummary()
        {
            ClearFinalConfigCache();
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

        public bool SetConfigQuiet(string newConfig)
        {
            if (string.IsNullOrEmpty(newConfig) || coreInfo.GetConfig() == newConfig)
            {
                return false;
            }
            coreInfo.SetConfig(newConfig);
            var uid = GetParent().GetCoreStates().GetUid();
            servers.TryUpdateConfigCache(uid, newConfig);
            UpdateSummary();
            return true;
        }

        public void SetConfig(string newConfig)
        {
            if (!SetConfigQuiet(newConfig))
            {
                return;
            }

            if (coreCtrl.IsCoreRunning())
            {
                coreCtrl.RestartCoreThen();
            }
        }

        public string GenFinalConfig()
        {
            lock (finalConfigLocker)
            {
                var r = GenFinalConfigCore(true);
                return r;
            }
        }

        #endregion

        #region private methods

        bool IsFinalConfigCacheValid(string sendThrough)
        {
            if (
                lastCacheSettings.enableUtls != setting.isEnableUtlsFingerprint
                || lastCacheSettings.fingerprint != setting.uTlsFingerprint
            )
            {
                return false;
            }

            if (lastCacheSettings.sendThrough != sendThrough)
            {
                return false;
            }
            return true;
        }

        string GenFinalConfigCore(bool isSetStatPort)
        {
            VgcApis.Misc.Logger.Debug("regenerate final config");

            var tpls = GetCustomTemplates();
            var config = GetConfig();
            var host = coreInfo.inbIp;
            var port = coreInfo.inbPort;

            var r = "";
            try
            {
                if (VgcApis.Misc.Utils.IsJson(config))
                {
                    r = GenJsonFinalConfig(tpls, config, isSetStatPort, host, port);
                }
                else
                {
                    r = config;
                    foreach (var tpl in tpls)
                    {
                        r = tpl.MergeToConfig(r, host, port);
                        if (string.IsNullOrEmpty(r))
                        {
                            break;
                        }
                    }
                }
            }
            catch { }
            r = string.IsNullOrEmpty(r) ? config : r;
            return r;
        }

        private string GenJsonFinalConfig(
            List<Models.Datas.CustomConfigTemplate> tpls,
            string config,
            bool isSetStatPort,
            string host,
            int port
        )
        {
            var padding = VgcApis.Models.Consts.Config.FormatOutboundPaddingLeft;
            string r;
            var modifier = CreateTlsAndSendThroughModifier();
            var tuple = VgcApis.Misc.Utils.ParseJsonIntoBasicConfigAndOutbounds(config, modifier);

            var json = InjectStatisticsConfigOnDemand(tuple.Item1, isSetStatPort);
            MergeTemplatesSetting(ref json, tpls, host, port);

            var outbs = (json["outbounds"] ?? new JArray())
                .Select(outb =>
                {
                    modifier?.Invoke(outb as JObject);
                    return VgcApis.Misc.Utils.FormatConfig(outb, padding);
                })
                .Concat(tuple.Item2)
                .ToList();

            var placeholder = $"vgc-outbounds-{Guid.NewGuid()}";
            json["outbounds"] = new JArray { placeholder };
            var c = VgcApis.Misc.Utils.FormatConfig(json);
            r = VgcApis.Misc.Utils.InjectOutboundsIntoBasicConfig(c, placeholder, outbs);
            return r;
        }

        private List<Models.Datas.CustomConfigTemplate> GetCustomTemplates()
        {
            var cfgTpls = setting.GetCustomConfigTemplates();
            var names =
                coreInfo.templates?.Replace(", ", ",")?.Split(',')?.ToList() ?? new List<string>();
            names.Add(coreInfo.inbName);

            var tpls = coreInfo.isAcceptInjection
                ? cfgTpls.Where(tpl => tpl.isInject).ToList()
                : new List<Models.Datas.CustomConfigTemplate>();

            tpls.AddRange(
                names
                    .Select(n => cfgTpls.FirstOrDefault(tpl => tpl.name == n))
                    .Where(t => t != null)
            );
            return tpls;
        }

        private Action<JObject> CreateTlsAndSendThroughModifier()
        {
            var send4 = states.IsIgnoreSendThrough() ? "" : setting.GetSendThroughIpv4();
            JObject selfSigned = null;
            JObject uTlsFingerprint = null;
            if (setting.isSupportSelfSignedCert)
            {
                selfSigned = JObject.Parse(@"{tlsSettings: {allowInsecure: true}}");
            }
            if (setting.isEnableUtlsFingerprint)
            {
                uTlsFingerprint = JObject.Parse(@"{tlsSettings: {}}");
                uTlsFingerprint["tlsSettings"]["fingerprint"] = setting.uTlsFingerprint;
            }

            return (json) =>
                AddTlsAndSendthroughSettingsToOutbound(json, send4, selfSigned, uTlsFingerprint);
        }

        void MergeTemplatesSetting(
            ref JObject json,
            IEnumerable<Models.Datas.CustomConfigTemplate> tplSs,
            string host,
            int port
        )
        {
            foreach (var tplS in tplSs)
            {
                if (tplS.MergeToJObject(ref json, host, port) != true)
                {
                    break;
                }
            }
        }

        void AddTlsAndSendthroughSettingsToOutbound(
            JObject json,
            string send4,
            JObject selfSigned,
            JObject uTlsFingerprint
        )
        {
            if (json == null)
            {
                return;
            }

            if (VgcApis.Misc.Utils.GetKey(json, "streamSettings") is JObject streamS)
            {
                if (selfSigned != null)
                {
                    VgcApis.Misc.Utils.MergeJson(streamS, selfSigned);
                }

                if (uTlsFingerprint != null)
                {
                    VgcApis.Misc.Utils.MergeJson(streamS, uTlsFingerprint);
                }
            }

            var key = "sendThrough";
            if (string.IsNullOrEmpty(send4))
            {
                if (json.ContainsKey(key))
                {
                    json[key].Remove();
                }
            }
            else
            {
                json[key] = send4;
            }
        }

        List<InboundInfo> GetInboundsInfoFromYaml(string config)
        {
            var r = new List<InboundInfo>();
            var pat = @"(.*):\r?\n +listen: ?([^\r\n]+)";
            var match = Regex.Match(config, pat);
            while (match.Success)
            {
                var g = match.Groups;

                if (g.Count > 2)
                {
                    VgcApis.Misc.Utils.TryParseAddress(g[2].Value, out var host, out var port);
                    var protocol = g[1].Value?.ToLower() ?? "";
                    var inb = new InboundInfo(protocol, host, port);
                    r.Add(inb);
                }
                match = match.NextMatch();
            }
            return r;
        }

        JObject InjectStatisticsConfigOnDemand(JObject json, bool isSetStatPort)
        {
            if (!setting.isEnableStatistics)
            {
                return json;
            }

            if (isSetStatPort)
            {
                var freePort = VgcApis.Misc.Utils.GetFreeTcpPort();
                if (freePort < 1)
                {
                    return json;
                }
                states.SetStatPort(freePort);
            }

            var statsCfg = Misc.Caches.Jsons.LoadTemplate("statsApiV4Inb") as JObject;
            statsCfg["inbounds"][0]["port"] = states.GetStatPort();
            VgcApis.Misc.Utils.CombineConfigWithRoutingInFront(ref statsCfg, json);

            var statsTpl = Misc.Caches.Jsons.LoadTemplate("statsApiV4Tpl") as JObject;
            VgcApis.Misc.Utils.CombineConfigWithRoutingInFront(ref statsCfg, statsTpl);
            return statsCfg;
        }

        void UpdateSummaryCore(string config)
        {
            var ty = VgcApis.Misc.Utils.DetectConfigType(config);
            var summary = $"<unknow {ty}>";
            List<InboundInfo> inbsInfo = null;
            try
            {
                var s = "";
                switch (ty)
                {
                    case Enums.ConfigType.json:
                        inbsInfo = VgcApis.Misc.Utils.GetInboundsInfoFromJsonConfig(config);
                        s = VgcApis.Misc.Utils.ExtractSummaryFromJsonConfig(config);
                        break;
                    case Enums.ConfigType.yaml:
                        s = VgcApis.Misc.Utils.ExtractSummaryFromYaml(config);
                        inbsInfo = GetInboundsInfoFromYaml(config);
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
            InbsInfoCache = inbsInfo;

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

    struct FinalConfigCacheSettings
    {
        public string fingerprint;
        public bool enableUtls;
        public string sendThrough;
    }
}
