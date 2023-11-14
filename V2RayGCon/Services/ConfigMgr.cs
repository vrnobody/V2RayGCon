using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using V2RayGCon.Resources.Resx;

namespace V2RayGCon.Services
{
    #region test result
    class LatencyTestResult
    {
        public long latency;
        public long size;

        public LatencyTestResult(long latency, long size)
        {
            this.latency = latency;
            this.size = size;
        }

        public LatencyTestResult() { }
    }
    #endregion

    public sealed class ConfigMgr
        : BaseClasses.SingletonService<ConfigMgr>,
            VgcApis.Interfaces.Services.IConfigMgrService
    {
        Settings setting;
        Cache cache;

        static readonly long TIMEOUT = VgcApis.Models.Consts.Core.SpeedtestTimeout;

        readonly ConcurrentQueue<VgcApis.Interfaces.ICoreServCtrl> coreServs =
            new ConcurrentQueue<VgcApis.Interfaces.ICoreServCtrl>();

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
                var sci = CreateSpeedTestConfig(coreName, rawConfig, port);
                var core = new Libs.V2Ray.Core(setting) { title = title };
                core.SetCustomCoreName(coreName);
                core.RestartCoreIgnoreError(sci.config);
                if (core.WaitUntilReady())
                {
                    text = Misc.Utils.Fetch(url, port, timeout, sci.isSocks5);
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
        )
        {
            Interlocked.Increment(ref setting.SpeedtestCounter);
            setting.SpeedTestPool.WaitOne();
            var result = DoSpeedTest(
                rawConfig,
                "Custom speed-test",
                coreName,
                testUrl,
                testTimeout,
                null
            );
            setting.SpeedTestPool.ReturnOne();
            Interlocked.Decrement(ref setting.SpeedtestCounter);
            WakeupLatencyTester();
            return result.latency;
        }

        public long RunSpeedTest(string rawConfig)
        {
            Interlocked.Increment(ref setting.SpeedtestCounter);
            setting.SpeedTestPool.WaitOne();
            var url = GetDefaultSpeedtestUrl();
            var r = DoSpeedTest(
                rawConfig,
                "Default speed-test",
                "",
                url,
                GetDefaultTimeout(),
                null
            );
            setting.SpeedTestPool.ReturnOne();
            Interlocked.Decrement(ref setting.SpeedtestCounter);
            WakeupLatencyTester();
            return r.latency;
        }

        public void AddToSpeedTestQueue(VgcApis.Interfaces.ICoreServCtrl coreServ)
        {
            coreServs.Enqueue(coreServ);
            Interlocked.Increment(ref setting.SpeedtestCounter);
            WakeupLatencyTester();
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

        JObject GenV4BalancerConfig(List<VgcApis.Interfaces.ICoreServCtrl> servList)
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

        LatencyTestResult DoSpeedTest(
            string rawConfig,
            string title,
            string coreName,
            string testUrl,
            int testTimeout,
            Action<string> logDeliever
        )
        {
            var result = new LatencyTestResult(VgcApis.Models.Consts.Core.SpeedtestAbort, 0);
            if (!setting.isSpeedtestCancelled)
            {
                var port = VgcApis.Misc.Utils.GetFreeTcpPort();
                var sci = CreateSpeedTestConfig(coreName, rawConfig, port);
                result = SpeedTestCore(
                    sci,
                    title,
                    coreName,
                    testUrl,
                    testTimeout,
                    port,
                    logDeliever
                );
            }
            return result;
        }

        LatencyTestResult SpeedTestCore(
            Models.Datas.SpeedTestConfigInfos sci,
            string title,
            string coreName,
            string testUrl,
            int testTimeout,
            int port,
            Action<string> log
        )
        {
            log($"{I18N.SpeedtestPortNum}{port}");
            if (string.IsNullOrEmpty(sci.config))
            {
                log(I18N.DecodeImportFail);
                return new LatencyTestResult(TIMEOUT, 0);
            }

            var core = new Libs.V2Ray.Core(setting) { title = title };
            core.SetCustomCoreName(coreName);
            if (log != null)
            {
                core.OnLog += log;
            }

            long latency = TIMEOUT;
            long len = 0;
            try
            {
                core.RestartCoreIgnoreError(sci.config);
                if (core.WaitUntilReady())
                {
                    var expectedSizeInKib = setting.isUseCustomSpeedtestSettings
                        ? setting.CustomSpeedtestExpectedSizeInKib
                        : -1;
                    var r = VgcApis.Misc.Utils.TimedDownloadTest(
                        sci.isSocks5,
                        testUrl,
                        port,
                        expectedSizeInKib,
                        testTimeout
                    );
                    latency = r.Item1;
                    len = r.Item2;
                }
                core.StopCore();
            }
            catch { }
            if (log != null)
            {
                core.OnLog -= log;
            }
            return new LatencyTestResult(latency, len);
        }

        void WakeupLatencyTester()
        {
            if (!setting.SpeedTestPool.TryTakeOne())
            {
                return;
            }

            if (coreServs.Count > 0 && coreServs.TryDequeue(out var coreServ))
            {
                if (coreServ != null)
                {
                    VgcApis.Misc.Utils.RunInBgSlim(() =>
                    {
                        DoLatencyTestOnCore(coreServ);
                        setting.SpeedTestPool.ReturnOne();
                        Interlocked.Decrement(ref setting.SpeedtestCounter);
                        WakeupLatencyTester();
                    });
                    // 预防有线程意外终止，补个线程
                    WakeupLatencyTester();
                    return;
                }
                else
                {
                    // ???
                    Interlocked.Decrement(ref setting.SpeedtestCounter);
                }
            }
            setting.SpeedTestPool.ReturnOne();
        }

        Models.Datas.SpeedTestConfigInfos CreateSpeedTestConfig(
            string coreName,
            string rawConfig,
            int port
        )
        {
            var empty = new Models.Datas.SpeedTestConfigInfos(false, string.Empty);
            if (port <= 0)
            {
                return empty;
            }

            var coreS = setting.GetCustomCoresSetting().FirstOrDefault(cs => cs.name == coreName);
            var inbS = setting
                .GetCustomConfigTemplates()
                .FirstOrDefault(inb => inb.name == coreS?.speedtestConfigTemplateName);

            if (inbS != null)
            {
                return new Models.Datas.SpeedTestConfigInfos(
                    inbS.isSocks5Inbound,
                    inbS.MergeToConfig(rawConfig, port)
                );
            }

            var json = VgcApis.Misc.Utils.ParseJObject(rawConfig);
            if (json == null)
            {
                return empty;
            }

            json["inbounds"] = VgcApis.Misc.Utils.GenHttpInbound(port);

            // inject log config
            var nodeLog = Misc.Utils.GetKey(json, "log");
            if (nodeLog != null && nodeLog is JObject)
            {
                nodeLog["loglevel"] = "warning";
            }
            else
            {
                json["log"] = JToken.Parse("{'loglevel': 'warning'}");
            }

            // debug
            var r = VgcApis.Misc.Utils.FormatConfig(json);
            return new Models.Datas.SpeedTestConfigInfos(false, r);
        }

        void ShowCurrentSpeedtestResult(
            VgcApis.Interfaces.CoreCtrlComponents.ICoreStates coreStates,
            VgcApis.Interfaces.CoreCtrlComponents.ILogger coreLogger,
            string prefix,
            long delay
        )
        {
            if (delay <= 0)
            {
                delay = TIMEOUT;
            }
            var text = delay == TIMEOUT ? I18N.Timeout : $"{delay}ms";
            coreStates.SetStatus(text);
            coreStates.SetSpeedTestResult(delay);
            coreLogger.Log($"{prefix}{text}");
        }

        void DoLatencyTestOnCore(VgcApis.Interfaces.ICoreServCtrl coreServ)
        {
            long avgDelay = -1;
            long curDelay = TIMEOUT;
            var cycles = Math.Max(
                1,
                setting.isUseCustomSpeedtestSettings ? setting.CustomSpeedtestCycles : 1
            );

            var coreCtrl = coreServ.GetCoreCtrl();
            var coreStates = coreServ.GetCoreStates();
            var coreLogger = coreServ.GetLogger();

            var coreName = coreCtrl.GetCustomCoreName();
            var config = coreServ.GetConfiger().GetFinalConfig();
            var url = GetDefaultSpeedtestUrl();

            for (int i = 0; i < cycles && !setting.isSpeedtestCancelled; i++)
            {
                var sr = DoSpeedTest(
                    config,
                    coreStates.GetTitle(),
                    coreName,
                    url,
                    GetDefaultTimeout(),
                    coreLogger.Log
                );
                curDelay = sr.latency;
                coreStates.AddStatSample(new VgcApis.Models.Datas.StatsSample(0, sr.size));
                ShowCurrentSpeedtestResult(
                    coreStates,
                    coreLogger,
                    I18N.CurSpeedtestResult,
                    curDelay
                );
                if (curDelay == TIMEOUT)
                {
                    continue;
                }

                avgDelay = VgcApis.Misc.Utils.SpeedtestMean(
                    avgDelay,
                    curDelay,
                    VgcApis.Models.Consts.Config.CustomSpeedtestMeanWeight
                );
            }

            // all speedtest timeout
            if (avgDelay <= 0)
            {
                avgDelay = TIMEOUT;
            }
            ShowCurrentSpeedtestResult(coreStates, coreLogger, I18N.AvgSpeedtestResult, avgDelay);
            coreCtrl.ReleaseSpeedTestLock();
        }
        #endregion
    }
}
