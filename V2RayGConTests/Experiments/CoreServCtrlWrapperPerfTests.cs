using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace V2RayGCon.Test.Experiments
{
    [TestClass]
    public class CoreServCtrlWrapperPerfTests
    {
#if DEBUG
        static readonly Services.Cache cache = Services.Cache.Instance;
        static readonly Services.Settings setting = Services.Settings.Instance;
        static readonly Services.ConfigMgr configMgr = Services.ConfigMgr.Instance;
        static readonly Services.Servers servers = Services.Servers.Instance;

        VgcApis.Interfaces.ICoreServCtrl CreateCoreServCtrl()
        {
            var ci = new VgcApis.Models.Datas.CoreInfo()
            {
                uid = Guid.NewGuid().ToString(),
                name = VgcApis.Misc.Utils.RandomHex(16),
            };
            var csv = new Controllers.CoreServerCtrl(ci);
            csv.Run(cache, setting, configMgr, servers);
            return csv;
        }

        void Log(Stopwatch sw, int len, string tag)
        {
            var total = sw.ElapsedMilliseconds;
            var avg = 1.0 * total * 1000 / len;
            avg = Math.Floor(avg * 1000) / 1000;
            Console.WriteLine($"[{tag}] total: {total}ms avg: {avg}ns");
        }

        [TestMethod]
        public void GetNamePerfTest()
        {
            var len = 1_000_000;

            len = 10;

            var coreServ = CreateCoreServCtrl();
            var sw = new Stopwatch();
            var names = new List<string>();
            sw.Restart();
            for (var i = 0; i < len; i++)
            {
                var name = coreServ.GetCoreStates().GetName();
                names.Add(name);
            }
            sw.Stop();
            Log(sw, len, "Avg");
        }

        [TestMethod]
        public void WrapPerfTest()
        {
            var len = 1_000_000; // about 1 minute

            len = 10;

            var sw = new Stopwatch();
            var coreServs = new List<VgcApis.Interfaces.ICoreServCtrl>();
            for (var i = 0; i < len; i++)
            {
                var csv = CreateCoreServCtrl();
                coreServs.Add(csv);
            }

            var wServs = new List<VgcApis.Interfaces.IWrappedCoreServCtrl>();
            sw.Restart();
            for (var i = 0; i < len; i++)
            {
                var ws = Controllers.CoreServerComponent.Wrapper.Wrap(coreServs[i]);
                wServs.Add(ws);
            }
            sw.Stop();
            Log(sw, len, "Wrap");

            var names = new List<string>();
            sw.Restart();
            for (var i = 0; i < len; i++)
            {
                var name = coreServs[i].GetCoreStates().GetName();
                names.Add(name);
            }
            sw.Stop();
            Log(sw, len, "CoreServ.GetName");

            names = new List<string>();
            sw.Restart();
            for (var i = 0; i < len; i++)
            {
                var name = wServs[i].GetName();
                names.Add(name);
            }
            sw.Stop();
            Log(sw, len, "wServ.GetName");

            names = new List<string>();
            sw.Restart();
            for (var i = 0; i < len; i++)
            {
                var name = wServs[i].GetName();
                names.Add(name);
            }
            sw.Stop();
            Log(sw, len, "2nd times");
        }
#endif
    }
}
