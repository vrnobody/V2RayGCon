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
        readonly Services.Settings setting;
        private readonly Servers servers;
        readonly Services.Cache cache;
        readonly CoreInfo coreInfo;

        List<InboundInfo> inbsInfoCache = null;

        CoreStates states;
        Logger logger;
        CoreCtrl coreCtrl;

        public Configer(
            Services.Settings setting,
            Services.Servers servers,
            Services.Cache cache,
            CoreInfo coreInfo
        )
        {
            this.setting = setting;
            this.servers = servers;
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
        public InboundInfo GetInboundInfo()
        {
            return GetAllInboundsInfo().FirstOrDefault();
        }

        public ReadOnlyCollection<string> GetFormattedInboundsInfoFromCache()
        {
            var cache = inbsInfoCache;
            if (cache == null)
            {
                setting.DebugSendLog("re-gen inbounds info");
                cache = GetAllInboundsInfo();
                inbsInfoCache = cache;
            }
            else
            {
                setting.DebugSendLog("get inbounds info from cache");
            }
            return FormatInboundInfo(cache).AsReadOnly();
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

            if (VgcApis.Misc.Utils.IsYaml(config))
            {
                return GetInboundsInfoFromYaml(config);
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

        public string GetFinalConfig() => GenFinalConfig(false);

        public void ClearFinalConfigCache() => coreInfo.SetFinalConfigCache(null);

        public string GetRawConfig() => coreInfo.config;

        public string GetConfig() => coreInfo.GetConfig();

        public void UpdateSummary()
        {
            coreInfo.SetFinalConfigCache(null);
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

        public string GenFinalConfig(bool isSetStatPort)
        {
            var r = isSetStatPort ? "" : coreInfo.GetFinalConfigCache();
            if (!string.IsNullOrEmpty(r))
            {
                return r;
            }

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
            var host = coreInfo.inbIp;
            var port = coreInfo.inbPort;

            var config = GetConfig();
            try
            {
                if (VgcApis.Misc.Utils.IsJson(config))
                {
                    var json = InjectStatisticsConfigOnDemand(config, isSetStatPort);
                    r = GenJsonFinalConfig(ref json, tpls, host, port);
                    VgcApis.Misc.JsonRecycleBin.Put(r, json);
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
            coreInfo.SetFinalConfigCache(r);
            return r;
        }

        #endregion

        #region private methods
        List<string> FormatInboundInfo(IEnumerable<InboundInfo> inbsInfo)
        {
            var r = new List<string>();
            foreach (var inb in inbsInfo)
            {
                r.Add($"{inb.protocol}://{inb.host}:{inb.port}");
            }
            return r;
        }

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
            ref JObject json,
            IEnumerable<Models.Datas.CustomConfigTemplate> tplSs,
            string host,
            int port
        )
        {
            MergeCustomTlsSettings(ref json);
            foreach (var tplS in tplSs)
            {
                if (tplS.MergeToJObject(ref json, host, port) != true)
                {
                    break;
                }
            }

            var send4 = setting.GetSendThroughIpv4();
            if (!states.IsIgnoreSendThrough() && !string.IsNullOrEmpty(send4))
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

        List<InboundInfo> GetInboundsInfoFromJson(JObject json)
        {
            var r = new List<InboundInfo>();
            if (json == null)
            {
                return r;
            }

            try
            {
                var arr = json["inbounds"] as JArray;
                foreach (JObject inb in arr.Cast<JObject>())
                {
                    if (inb == null)
                    {
                        continue;
                    }

                    var info = new InboundInfo()
                    {
                        protocol = Misc.Utils.GetValue<string>(inb, "protocol")?.ToLower() ?? "",
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
                    var inb = new InboundInfo()
                    {
                        protocol = g[1].Value?.ToLower() ?? "",
                        host = host,
                        port = port,
                    };
                    r.Add(inb);
                }
                match = match.NextMatch();
            }
            return r;
        }

        JObject InjectStatisticsConfigOnDemand(string config, bool isSetStatPort)
        {
            if (!VgcApis.Misc.JsonRecycleBin.TryTake(config, out var json))
            {
                setting.DebugSendLog($"parse config len: {config.Length / 1024} KiB");
                json = JObject.Parse(config);
            }
            else
            {
                setting.DebugSendLog("get config from recyclebin");
            }

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

            var statsCfg = cache.tpl.LoadTemplate("statsApiV4Inb") as JObject;
            statsCfg["inbounds"][0]["port"] = states.GetStatPort();
            Misc.Utils.CombineConfigWithRoutingInFront(ref statsCfg, json);

            var statsTpl = cache.tpl.LoadTemplate("statsApiV4Tpl") as JObject;
            Misc.Utils.CombineConfigWithRoutingInFront(ref statsCfg, statsTpl);
            return statsCfg;
        }

        void UpdateSummaryCore(string config)
        {
            var ty = VgcApis.Misc.Utils.DetectConfigType(config);
            var summary = $"<unknow {ty}>";
            inbsInfoCache = null;
            try
            {
                var s = "";
                switch (ty)
                {
                    case Enums.ConfigType.json:
                        if (!VgcApis.Misc.JsonRecycleBin.TryTake(config, out var json))
                        {
                            json = VgcApis.Misc.Utils.ParseJObject(config);
                            setting.DebugSendLog(
                                $"update summary parse json: {config.Length / 1024} KiB"
                            );
                        }
                        else
                        {
                            setting.DebugSendLog("update summary get json from recyclebin");
                        }
                        inbsInfoCache = GetInboundsInfoFromJson(json);
                        s = Misc.Utils.ExtractSummaryFromJson(json);
                        break;
                    case Enums.ConfigType.yaml:
                        s = Misc.Utils.ExtractSummaryFromYaml(config);
                        inbsInfoCache = GetInboundsInfoFromYaml(config);
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
