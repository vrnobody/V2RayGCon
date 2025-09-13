using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using V2RayGCon.Resources.Resx;
using V2RayGCon.Services;
using VgcApis.Models.Consts;
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

        public SharelinkMetaData GetShareLinkMetaData()
        {
            var name = GetSibling<CoreStates>().GetName();
            var config = GetConfig();
            if (VgcApis.Misc.OutbMeta.TryParseConfig(config, out var meta) && meta != null)
            {
                meta.name = name;
                return meta;
            }
            return null;
        }

        public string GetShareLink()
        {
            var name = GetSibling<CoreStates>().GetName();
            var config = GetConfig();
            return ShareLinkMgr.Instance.EncodeConfigToShareLink(name, config);
        }

        public string GetFinalConfig() => GetFinalConfigFromCache(false);

        public string GenFinalConfig() => GetFinalConfigFromCache(true);

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

        #endregion

        #region private methods

        bool IsFinalConfigCacheValid(string sendThrough, bool resetStatsPort)
        {
            if (
                resetStatsPort
                || lastCacheSettings.enableUtls != setting.isEnableUtlsFingerprint
                || lastCacheSettings.fingerprint != setting.uTlsFingerprint
                || lastCacheSettings.enableStats != setting.isEnableStatistics
                || lastCacheSettings.sendThrough != sendThrough
                || lastCacheSettings.useSelfSignedCert != setting.isSupportSelfSignedCert
            )
            {
                return false;
            }
            return true;
        }

        string GetFinalConfigFromCache(bool resetStatsPort)
        {
            lock (finalConfigLocker)
            {
                var uid = states.GetUid();
                var send4 = states.IsIgnoreSendThrough() ? "" : setting.GetSendThroughIpv4();

                if (
                    IsFinalConfigCacheValid(send4, resetStatsPort)
                    && Misc.Caches.ZipStrLru.TryGet(uid, out var r)
                    && !string.IsNullOrEmpty(r)
                )
                {
                    VgcApis.Misc.Logger.Debug("get final config from cache");
                    return r;
                }
                r = GenFinalConfigCore(resetStatsPort);
                lastCacheSettings = new FinalConfigCacheSettings()
                {
                    enableUtls = setting.isEnableUtlsFingerprint,
                    fingerprint = setting.uTlsFingerprint,
                    sendThrough = send4,
                    enableStats = setting.isEnableStatistics,
                    useSelfSignedCert = setting.isSupportSelfSignedCert,
                };
                Misc.Caches.ZipStrLru.Put(uid, r);
                return r;
            }
        }

        string GenFinalConfigCore(bool resetStatsPort)
        {
            VgcApis.Misc.Logger.Debug("regenerate final config");

            var config = GetConfig();
            var tpls = GetCustomTemplates();
            var host = coreInfo.inbIp;
            var port = coreInfo.inbPort;

            var r = "";
            try
            {
                if (VgcApis.Misc.Utils.IsJson(config))
                {
                    r = GenJsonFinalConfig(config, tpls, host, port, resetStatsPort);
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
            VgcApis.Misc.Utils.CollectOnHighPressure(Math.Max(r.Length, config.Length));
            r = string.IsNullOrEmpty(r) ? config : r;
            return r;
        }

        string GenJsonFinalConfig(
            string config,
            List<Models.Datas.CustomConfigTemplate> tpls,
            string host,
            int port,
            bool resetStatsPort
        )
        {
            string r;
            var modifier = CreateTlsAndSendThroughModifier(tpls);
            var tuple = VgcApis.Misc.Utils.ParseAndSplitOutboundsFromConfig(config, modifier);

            var mergedTpls = tuple.Item1;
            if (setting.isEnableStatistics)
            {
                mergedTpls = InjectStatisticsConfig(mergedTpls, resetStatsPort);
            }
            MergeTemplatesSetting(ref mergedTpls, tpls, host, port);

            // coreServ.outbounds + tpl.outbounds
            var outbs = tuple
                .Item2.Concat(
                    (mergedTpls["outbounds"] ?? new JArray()).Select(outb =>
                    {
                        modifier?.Invoke(outb as JObject);
                        var padding = Config.OutboundsLeftPadding;
                        return VgcApis.Misc.Utils.FormatConfig(outb, padding);
                    })
                )
                .ToList();

            var placeholder = $"vgc-outbounds-{Guid.NewGuid()}";
            mergedTpls["outbounds"] = new JArray { placeholder };
            var c = VgcApis.Misc.Utils.FormatConfig(mergedTpls);
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

        Action<JObject> CreateTlsAndSendThroughModifier(
            IEnumerable<Models.Datas.CustomConfigTemplate> tpls
        )
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

            var outbTpls = tpls.Where(tpl => tpl.IsOutboundTemplate())
                .OrderBy(tpl => tpl.index)
                .Select(tpl =>
                {
                    try
                    {
                        var mixin = JObject.Parse(tpl.template);
                        var tagPrefix = tpl.mergeParams;
                        return new Tuple<JObject, string>(mixin, tagPrefix);
                    }
                    catch { }
                    return null;
                })
                .Where(tp => tp != null)
                .ToList();

            void M(JObject json)
            {
                if (json == null)
                {
                    return;
                }
                AddTlsAndSendthroughSettingsToOutbound(json, send4, selfSigned, uTlsFingerprint);
                if (outbTpls.Count > 0)
                {
                    foreach (var outbTpl in outbTpls)
                    {
                        MergeOutboundTemplate(json, outbTpl.Item1, outbTpl.Item2);
                    }
                }
            }

            return M;
        }

        void MergeOutboundTemplate(JObject json, JObject mixin, string tagPrefix)
        {
            if (string.IsNullOrEmpty(tagPrefix))
            {
                VgcApis.Misc.Utils.MergeJson(json, mixin);
                return;
            }

            var tag = VgcApis.Misc.Utils.GetValue<string>(json, "tag");
            if (!string.IsNullOrEmpty(tag) && tag.StartsWith(tagPrefix))
            {
                VgcApis.Misc.Utils.MergeJson(json, mixin);
            }
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
                if (!tplS.IsOutboundTemplate() && tplS.MergeToJObject(ref json, host, port) != true)
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

        JObject InjectStatisticsConfig(JObject json, bool resetStatPort)
        {
            var freePort = states.GetStatPort();
            if (resetStatPort)
            {
                freePort = VgcApis.Misc.Utils.GetFreeTcpPort();
                states.SetStatPort(freePort);
                if (freePort < 1)
                {
                    return json;
                }
            }

            var statsCfg = Misc.Caches.Jsons.LoadTemplate("statsApiV4Inb") as JObject;
            statsCfg["inbounds"][0]["port"] = freePort;
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
        public bool enableStats;
        public bool useSelfSignedCert;
    }
}
