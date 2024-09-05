using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Newtonsoft.Json.Linq;
using V2RayGCon.Resources.Resx;
using VgcApis.Models.Consts;
using static VgcApis.Models.Datas.Enums;

namespace V2RayGCon.Services
{
    #region latency test result
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

        static readonly long TIMEOUT = Core.SpeedtestTimeout;

        readonly ConcurrentQueue<VgcApis.Interfaces.ICoreServCtrl> latencyTestingQueue =
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
            var core = new Libs.V2Ray.Core(setting) { title = title };
            try
            {
                var sci = CreateSpeedTestConfig(coreName, rawConfig, port);
                core.SetCustomCoreName(coreName);
                core.RestartCoreIgnoreError(sci.config);
                if (core.WaitUntilReady())
                {
                    var host = Webs.LoopBackIP;
                    text = sci.isSocks5
                        ? VgcApis.Misc.Utils.FetchWorker(true, url, host, port, timeout, null, null)
                        : VgcApis.Misc.Utils.Fetch(url, port, timeout);
                }
                core.StopCore();
            }
            catch
            {
                return string.Empty;
            }
            finally
            {
                core.Dispose();
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
            latencyTestingQueue.Enqueue(coreServ);
            Interlocked.Increment(ref setting.SpeedtestCounter);
            WakeupLatencyTester();
        }

        public void Run(Settings setting)
        {
            this.setting = setting;
        }

        public JObject GenV4ChainConfig(List<VgcApis.Interfaces.ICoreServCtrl> servList)
        {
            var package = Misc.Caches.Jsons.LoadPackage("chainV4Tpl");
            var outbounds = package["outbounds"] as JArray;

            var counter = 0;
            JObject prev = null;
            for (var i = 0; i < servList.Count; i++)
            {
                var s = servList[i];
                var config = s.GetConfiger().GetConfig();
                var json = VgcApis.Misc.Utils.ParseJObject(config);
                var parts = VgcApis.Misc.Utils.ExtractOutboundsFromConfig(json);

                foreach (var item in parts)
                {
                    if (!(item is JObject p))
                    {
                        continue;
                    }

                    var prefix = Config.ServsPkgTagPrefix;
                    var tag = $"{prefix}{counter++:d6}";
                    p["tag"] = tag;
                    if (prev != null)
                    {
                        prev["proxySettings"] = JObject.Parse(@"{tag: '',transportLayer: true}");
                        prev["proxySettings"]["tag"] = tag;
                        outbounds.Add(prev);
                    }
                    prev = p;
                }
            }
            outbounds.Add(prev);
            return package;
        }

        public string GenV4BalancerConfig(
            List<VgcApis.Interfaces.ICoreServCtrl> servList,
            string interval,
            string url,
            BalancerStrategies strategy
        )
        {
            var tagPrefix = Config.ServsPkgTagPrefix;
            var placeHolder = $"vgc-outbounds-{Guid.NewGuid()}";

            // create basic config without outbounds
            var tpl = Misc.Caches.Jsons.LoadPackage("pkgV4Tpl");
            tpl["routing"]["balancers"][0]["selector"] = JArray.Parse($"['{tagPrefix}']");
            InjectBalacerStrategy(ref tpl, interval, url, strategy);
            tpl["outbounds"] = new JArray { placeHolder };
            var pkg = VgcApis.Misc.Utils.FormatConfig(tpl);

            // parse outbounds
            var outbounds = new List<string>();
            var padding = Config.OutboundsLeftPadding;
            var counter = 0;
            for (var i = 0; i < servList.Count; i++)
            {
                var coreServ = servList[i];
                var config = coreServ.GetConfiger().GetConfig();
                var outbs = ExtractOutboundsFromConfig(config, ref counter, tagPrefix, padding);
                outbounds.AddRange(outbs);
            }

            var r = VgcApis.Misc.Utils.InjectOutboundsIntoBasicConfig(pkg, placeHolder, outbounds);
            VgcApis.Misc.Utils.CollectOnHighPressure(r.Length);
            return r;
        }
        #endregion

        #region private methods

        List<string> ExtractOutboundsFromConfig(
            string config,
            ref int counter,
            string prefix,
            string padding
        )
        {
            var r = new List<string>();
            var json = VgcApis.Misc.Utils.ParseJObject(config);
            var outbounds = VgcApis.Misc.Utils.ExtractOutboundsFromConfig(json);
            foreach (var item in outbounds)
            {
                if (!(item is JObject outbound))
                {
                    continue;
                }

                outbound["tag"] = $"{prefix}{counter++:d6}";
                var c = VgcApis.Misc.Utils.FormatConfig(outbound, padding);
                r.Add(c);
            }
            return r;
        }

        void InjectBalacerStrategy(
            ref JObject config,
            string interval,
            string url,
            BalancerStrategies strategy
        )
        {
            switch (strategy)
            {
                case BalancerStrategies.RoundRobin:
                    try
                    {
                        config["routing"]["balancers"][0]["strategy"] = JObject.Parse(
                            "{type:'roundRobin'}"
                        );
                    }
                    catch { }
                    break;
                case BalancerStrategies.LeastLoad:
                    try
                    {
                        config["routing"]["balancers"][0]["strategy"] = JObject.Parse(
                            "{type:'leastLoad'}"
                        );
                        var tplKey = "burstObservatory";
                        var burstCfg = Misc.Caches.Jsons.LoadPackage(tplKey);
                        var prefix = Config.ServsPkgTagPrefix;
                        burstCfg["subjectSelector"] = JArray.Parse($"['{prefix}']");
                        if (!string.IsNullOrWhiteSpace(interval))
                        {
                            burstCfg["pingConfig"]["interval"] = interval;
                        }
                        if (!string.IsNullOrWhiteSpace(url))
                        {
                            burstCfg["pingConfig"]["destination"] = url;
                        }
                        config[tplKey] = burstCfg;
                    }
                    catch { }
                    break;
                case BalancerStrategies.LeastPing:
                    try
                    {
                        var prefix = Config.ServsPkgTagPrefix;
                        config["observatory"] = JObject.Parse($"{{subjectSelector:['{prefix}']}}");
                        if (!string.IsNullOrWhiteSpace(interval))
                        {
                            config["observatory"]["probeInterval"] = interval;
                        }
                        if (!string.IsNullOrWhiteSpace(url))
                        {
                            config["observatory"]["probeURL"] = url;
                        }
                        config["routing"]["balancers"][0]["strategy"] = JObject.Parse(
                            "{type:'leastPing'}"
                        );
                    }
                    catch { }
                    break;
                default:
                    break;
            }
        }

        int GetDefaultTimeout()
        {
            var customTimeout = setting.CustomSpeedtestTimeout;
            if (customTimeout > 0)
            {
                return customTimeout;
            }
            return Intervals.DefaultSpeedTestTimeout;
        }

        string GetDefaultSpeedtestUrl() =>
            setting.isUseCustomSpeedtestSettings ? setting.CustomSpeedtestUrl : Webs.GoogleDotCom;

        LatencyTestResult DoSpeedTest(
            string rawConfig,
            string title,
            string coreName,
            string testUrl,
            int testTimeout,
            Action<string> logDeliever
        )
        {
            var result = new LatencyTestResult(Core.SpeedtestAbort, 0);
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
                    var r = VgcApis.Misc.Utils.TimedDownloadTestWorker(
                        sci.isSocks5,
                        testUrl,
                        port,
                        expectedSizeInKib,
                        testTimeout,
                        null,
                        null
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
            core.Dispose();
            return new LatencyTestResult(latency, len);
        }

        void WakeupLatencyTester()
        {
            if (!setting.SpeedTestPool.TryTakeOne())
            {
                return;
            }

            if (latencyTestingQueue.Count > 0 && latencyTestingQueue.TryDequeue(out var coreServ))
            {
                if (coreServ != null)
                {
                    VgcApis.Misc.Utils.RunInBackground(() =>
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
            var nodeLog = VgcApis.Misc.Utils.GetKey(json, "log");
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

        void DoLatencyTestOnCore(VgcApis.Interfaces.ICoreServCtrl coreServ)
        {
            long avgDelay = 0;
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
            var text = "";

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

                // abort
                if (curDelay <= 0)
                {
                    continue;
                }

                text = curDelay == TIMEOUT ? I18N.Timeout : $"{curDelay}ms";
                coreStates.SetStatus(text);
                coreLogger.Log($"{I18N.CurSpeedtestResult}{text}");

                if (curDelay != TIMEOUT)
                {
                    avgDelay = VgcApis.Misc.Utils.SpeedtestMean(
                        avgDelay,
                        curDelay,
                        Config.CustomSpeedtestMeanWeight
                    );
                }
            }

            // all speedtest timeouted
            if (avgDelay <= 0 && !setting.isSpeedtestCancelled)
            {
                avgDelay = TIMEOUT;
            }
            text = avgDelay == TIMEOUT ? I18N.Timeout : $"{avgDelay}ms";
            coreStates.SetStatus(avgDelay > 0 ? text : "");
            coreLogger.Log($"{I18N.AvgSpeedtestResult}{text}");
            coreStates.SetSpeedTestResult(avgDelay);

            coreCtrl.ReleaseSpeedTestLock();
        }
        #endregion
    }
}
