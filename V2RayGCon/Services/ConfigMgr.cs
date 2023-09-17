using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using V2RayGCon.Resources.Resx;

namespace V2RayGCon.Services
{
    public sealed class ConfigMgr
        : BaseClasses.SingletonService<ConfigMgr>,
            VgcApis.Interfaces.Services.IConfigMgrService
    {
        Settings setting;
        Cache cache;

        static readonly long TIMEOUT = VgcApis.Models.Consts.Core.SpeedtestTimeout;

        ConfigMgr() { }

        #region public methods
        public string FetchWithCustomConfig(
            string rawConfig,
            string coreName,
            string title,
            string url,
            int timeout
        )
        {
            var text = string.Empty;
            var port = VgcApis.Misc.Utils.GetFreeTcpPort();
            if (port < 0)
            {
                return text;
            }
            try
            {
                var config = CreateSpeedTestConfig(rawConfig, port);
                var core = new Libs.V2Ray.Core(setting) { title = title };
                core.SetCustomCoreName(coreName);
                core.RestartCoreIgnoreError(config);
                if (WaitUntilCoreReady(core))
                {
                    text = Misc.Utils.Fetch(url, port, timeout);
                }
                core.StopCore();
            }
            catch
            {
                return string.Empty;
            }
            return text;
        }

        public long RunCustomSpeedTest(
            string rawConfig,
            string coreName,
            string testUrl,
            int testTimeout
        ) =>
            QueuedSpeedTesting(
                rawConfig,
                "Custom speed-test",
                coreName,
                testUrl,
                testTimeout,
                null
            ).Item1;

        public long RunSpeedTest(string rawConfig)
        {
            var url = GetDefaultSpeedtestUrl();
            return QueuedSpeedTesting(
                rawConfig,
                "Default speed-test",
                "",
                url,
                GetDefaultTimeout(),
                null
            ).Item1;
        }

        public Tuple<long, long> RunDefaultSpeedTest(
            string config,
            string title,
            string coreName,
            EventHandler<VgcApis.Models.Datas.StrEvent> logDeliever
        )
        {
            var url = GetDefaultSpeedtestUrl();
            return QueuedSpeedTesting(
                config,
                title,
                coreName,
                url,
                GetDefaultTimeout(),
                logDeliever
            );
        }

        public void MergeCustomTlsSettings(ref JObject config)
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

        public bool ModifyInboundWithCustomSetting(
            ref JObject config,
            int inbType,
            string ip,
            int port
        )
        {
            if (inbType == (int)Models.Datas.Enums.ProxyTypes.Config)
            {
                return true;
            }

            if (inbType == (int)Models.Datas.Enums.ProxyTypes.Custom)
            {
                try
                {
                    var inbs = JArray.Parse(setting.CustomDefInbounds);
                    config["inbounds"] = inbs;
                    return true;
                }
                catch
                {
                    setting.SendLog(I18N.ParseCustomInboundsSettingFail);
                }
                return false;
            }

            if (
                inbType != (int)Models.Datas.Enums.ProxyTypes.HTTP
                && inbType != (int)Models.Datas.Enums.ProxyTypes.SOCKS
            )
            {
                return false;
            }

            var protocol = Misc.Utils.InboundTypeNumberToName(inbType);
            var tplKey = protocol + "In";
            try
            {
                JObject o = CreateInboundSetting(inbType, ip, port, protocol, tplKey);

                ReplaceInboundSetting(ref config, o);
#if DEBUG
                var debug = config.ToString(Formatting.Indented);
#endif
                return true;
            }
            catch
            {
                setting.SendLog(I18N.CoreCantSetLocalAddr);
            }
            return false;
        }

        /// <summary>
        /// ref means config will change after the function is executed.
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public void InjectSkipCnSiteSettingsIntoConfig(ref JObject config, bool useV4)
        {
            var c = JObject.Parse(@"{}");

            var dict = new Dictionary<string, string>
            {
                { "dns", "dnsCFnGoogle" },
                { "routing", GetRoutingTplName(config, useV4) },
            };

            foreach (var item in dict)
            {
                var tpl = Misc.Utils.CreateJObject(item.Key);
                var value = cache.tpl.LoadExample(item.Value);
                tpl[item.Key] = value;

                if (!Misc.Utils.Contains(config, tpl))
                {
                    c[item.Key] = value;
                }
            }

            // put dns/routing settings in front of user settings
            Misc.Utils.CombineConfigWithRoutingInFront(ref config, c);

            // put outbounds after user settings
            var hasOutbounds = Misc.Utils.GetKey(config, "outbounds") != null;
            var hasOutDtr = Misc.Utils.GetKey(config, "outboundDetour") != null;

            var outboundTag = "outboundDetour";
            if (!hasOutDtr && (hasOutbounds || useV4))
            {
                outboundTag = "outbounds";
            }

            var o = Misc.Utils.CreateJObject(outboundTag, cache.tpl.LoadExample("outDtrFreedom"));

            if (!Misc.Utils.Contains(config, o))
            {
                Misc.Utils.CombineConfigWithRoutingInFront(ref o, config);
                config = o;
            }
        }

        public JObject GenV4ServersPackageConfig(
            List<VgcApis.Interfaces.ICoreServCtrl> servList,
            VgcApis.Models.Datas.Enums.PackageTypes packageType
        )
        {
            JObject package;
            switch (packageType)
            {
                case VgcApis.Models.Datas.Enums.PackageTypes.Chain:
                    package = GenV4ChainConfig(servList);
                    break;
                case VgcApis.Models.Datas.Enums.PackageTypes.Balancer:
                default:
                    package = GenV4BalancerConfig(servList);
                    break;
            }
            return package;
        }

        public void Run(Settings setting, Cache cache)
        {
            this.setting = setting;
            this.cache = cache;
        }

        #endregion

        #region private methods
        JObject GenV4ChainConfig(List<VgcApis.Interfaces.ICoreServCtrl> servList)
        {
            var package = cache.tpl.LoadPackage("chainV4Tpl");
            var outbounds = package["outbounds"] as JArray;

            JObject prev = null;
            for (var i = 0; i < servList.Count; i++)
            {
                var s = servList[i];
                var finalConfig = s.GetConfiger().GetFinalConfig();
                var json = VgcApis.Misc.Utils.ParseJObject(finalConfig);
                var parts = Misc.Utils.ExtractOutboundsFromConfig(json);
                var c = 0;
                foreach (JObject p in parts)
                {
                    var tag = $"node{i}s{c++}";
                    p["tag"] = tag;
                    if (prev != null)
                    {
                        prev["proxySettings"] = JObject.Parse(@"{tag: '',transportLayer: true}");
                        prev["proxySettings"]["tag"] = tag;
                        outbounds.Add(prev);
                    }
                    prev = p;
                }
                var name = s.GetCoreStates().GetName();
                if (c == 0)
                {
                    setting.SendLog(I18N.PackageFail + ": " + name);
                }
                else
                {
                    setting.SendLog(I18N.PackageSuccess + ": " + name);
                }
            }
            outbounds.Add(prev);
            return package;
        }

        private JObject GenV4BalancerConfig(List<VgcApis.Interfaces.ICoreServCtrl> servList)
        {
            var package = cache.tpl.LoadPackage("pkgV4Tpl");
            var outbounds = package["outbounds"] as JArray;
            for (var i = 0; i < servList.Count; i++)
            {
                var s = servList[i];
                var finalConfig = s.GetConfiger().GetFinalConfig();
                var json = VgcApis.Misc.Utils.ParseJObject(finalConfig);
                var parts = Misc.Utils.ExtractOutboundsFromConfig(json);
                var c = 0;
                foreach (JObject p in parts)
                {
                    p["tag"] = $"agentout{i}s{c++}";
                    outbounds.Add(p);
                }
                var name = s.GetCoreStates().GetName();
                if (c == 0)
                {
                    setting.SendLog(I18N.PackageFail + ": " + name);
                }
                else
                {
                    setting.SendLog(I18N.PackageSuccess + ": " + name);
                }
            }
            return package;
        }

        int GetDefaultTimeout()
        {
            var customTimeout = setting.CustomSpeedtestTimeout;
            if (customTimeout > 0)
            {
                return customTimeout;
            }
            return VgcApis.Models.Consts.Intervals.DefaultSpeedTestTimeout;
        }

        string GetDefaultSpeedtestUrl() =>
            setting.isUseCustomSpeedtestSettings
                ? setting.CustomSpeedtestUrl
                : VgcApis.Models.Consts.Webs.GoogleDotCom;

        Tuple<long, long> QueuedSpeedTesting(
            string rawConfig,
            string title,
            string coreName,
            string testUrl,
            int testTimeout,
            EventHandler<VgcApis.Models.Datas.StrEvent> logDeliever
        )
        {
            Interlocked.Increment(ref setting.SpeedtestCounter);

            // setting.SpeedTestPool may change while testing
            var pool = setting.SpeedTestPool;
            pool.Wait();

            var result = new Tuple<long, long>(VgcApis.Models.Consts.Core.SpeedtestAbort, 0);
            if (!setting.isSpeedtestCancelled)
            {
                var port = VgcApis.Misc.Utils.GetFreeTcpPort();
                var cfg = CreateSpeedTestConfig(rawConfig, port);
                result = DoSpeedTesting(
                    cfg,
                    title,
                    coreName,
                    testUrl,
                    testTimeout,
                    port,
                    logDeliever
                );
            }

            pool.Release();
            Interlocked.Decrement(ref setting.SpeedtestCounter);
            return result;
        }

        bool WaitUntilCoreReady(Libs.V2Ray.Core core)
        {
            const int jiff = 300;
            int cycle = 30 * 1000 / jiff;
            int i;
            for (i = 0; i < cycle && !core.isReady && core.isRunning; i++)
            {
                VgcApis.Misc.Utils.Sleep(jiff);
            }

            if (!core.isRunning)
            {
                return false;
            }

            if (i < cycle)
            {
                return true;
            }
            return false;
        }

        Tuple<long, long> DoSpeedTesting(
            string config,
            string title,
            string coreName,
            string testUrl,
            int testTimeout,
            int port,
            EventHandler<VgcApis.Models.Datas.StrEvent> logDeliever
        )
        {
            void log(string content) =>
                logDeliever?.Invoke(this, new VgcApis.Models.Datas.StrEvent(content));

            log($"{I18N.SpeedtestPortNum}{port}");
            if (string.IsNullOrEmpty(config))
            {
                log(I18N.DecodeImportFail);
                return new Tuple<long, long>(TIMEOUT, 0);
            }

            var speedTester = new Libs.V2Ray.Core(setting) { title = title };
            speedTester.SetCustomCoreName(coreName);
            if (logDeliever != null)
            {
                speedTester.OnLog += logDeliever;
            }

            long latency = TIMEOUT;
            long len = 0;
            try
            {
                speedTester.RestartCoreIgnoreError(config);
                if (WaitUntilCoreReady(speedTester))
                {
                    var expectedSizeInKib = setting.isUseCustomSpeedtestSettings
                        ? setting.CustomSpeedtestExpectedSizeInKib
                        : -1;
                    var r = VgcApis.Misc.Utils.TimedDownloadTest(
                        testUrl,
                        port,
                        expectedSizeInKib,
                        testTimeout
                    );
                    latency = r.Item1;
                    len = r.Item2;
                }
                speedTester.StopCore();
            }
            catch { }
            if (logDeliever != null)
            {
                speedTester.OnLog -= logDeliever;
            }
            return new Tuple<long, long>(latency, len);
        }

        JObject CreateInboundSetting(
            int inboundType,
            string ip,
            int port,
            string protocol,
            string tplKey
        )
        {
            var o = JObject.Parse(@"{}");
            o["tag"] = "agentin";
            o["protocol"] = protocol;
            o["listen"] = ip;
            o["port"] = port;
            o["settings"] = cache.tpl.LoadTemplate(tplKey);

            if (inboundType == (int)Models.Datas.Enums.ProxyTypes.SOCKS)
            {
                o["settings"]["ip"] = ip;
            }

            return o;
        }

        string CreateSpeedTestConfig(string rawConfig, int port)
        {
            var empty = string.Empty;
            if (port <= 0)
            {
                return empty;
            }

            var config = VgcApis.Misc.Utils.ParseJObject(rawConfig);

            if (config == null)
            {
                return empty;
            }

            // inject log config
            var nodeLog = Misc.Utils.GetKey(config, "log");
            if (nodeLog != null && nodeLog is JObject)
            {
                nodeLog["loglevel"] = "warning";
            }
            else
            {
                config["log"] = JToken.Parse("{'loglevel': 'warning'}");
            }

            if (
                !ModifyInboundWithCustomSetting(
                    ref config,
                    (int)Models.Datas.Enums.ProxyTypes.HTTP,
                    VgcApis.Models.Consts.Webs.LoopBackIP,
                    port
                )
            )
            {
                return empty;
            }

            // debug
            var configString = config.ToString(Formatting.None);

            return configString;
        }

        string GetRoutingTplName(JObject config, bool useV4)
        {
            var routingRules = Misc.Utils.GetKey(config, "routing.rules");
            var routingSettingsRules = Misc.Utils.GetKey(config, "routing.settings.rules");
            var hasRoutingV4 = routingRules != null && (routingRules is JArray);
            var hasRoutingV3 = routingSettingsRules != null && (routingSettingsRules is JArray);

            var isUseRoutingV4 = !hasRoutingV3 && (useV4 || hasRoutingV4);
            return isUseRoutingV4 ? "routeCnipV4" : "routeCNIP";
        }

        void ReplaceInboundSetting(ref JObject config, JObject o)
        {
            // Bug. Stream setting will mess things up.
            // Misc.Utils.MergeJson(ref config, o);

            var hasInbound = Misc.Utils.GetKey(config, "inbound") != null;
            var hasInbounds = Misc.Utils.GetKey(config, "inbounds.0") != null;
            var isUseV4 = !hasInbound && (hasInbounds || setting.isUseV4);

            if (isUseV4)
            {
                if (!hasInbounds)
                {
                    config["inbounds"] = JArray.Parse(@"[{}]");
                }
                config["inbounds"][0] = o;
            }
            else
            {
                config["inbound"] = o;
            }
        }

        #endregion
    }
}
