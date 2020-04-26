using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Luna.Models.Apis.Components
{
    public sealed class Server :
        VgcApis.BaseClasses.ComponentOf<LuaApis>,
        VgcApis.Interfaces.Lua.ILuaServer
    {
        VgcApis.Interfaces.Services.IServersService vgcServers;
        VgcApis.Interfaces.Services.INotifierService vgcNotifier;
        VgcApis.Interfaces.Services.IConfigMgrService vgcConfigMgr;

        public Server(
             VgcApis.Interfaces.Services.IApiService api)
        {
            vgcNotifier = api.GetNotifierService();
            vgcServers = api.GetServersService();
            vgcConfigMgr = api.GetConfigMgrService();
        }

        public void UpdateAllSummary() =>
            vgcServers.UpdateAllServersSummarySync();

        public void ResetIndexes() =>
            vgcServers.ResetIndexQuiet();

        // expose for ILuaServer
        public long RunSpeedTest(string rawConfig) =>
            vgcConfigMgr.RunSpeedTest(rawConfig);

        public long RunCustomSpeedTest(string rawConfig, string testUrl, int testTimeout) =>
            vgcConfigMgr.RunCustomSpeedTest(rawConfig, testUrl, testTimeout);

        public List<VgcApis.Interfaces.ICoreServCtrl> GetAllServers() =>
            vgcServers.GetAllServersOrderByIndex().ToList();

        public void ReverseSelectedByIndex() =>
            vgcServers.ReverseSelectedByIndex();

        public void SortSelectedServersByLastModifiedDate() =>
            vgcServers.SortSelectedByLastModifiedDate();

        public void SortSelectedServersBySummary() =>
            vgcServers.SortSelectedBySummary();

        public void SortSelectedServersBySpeedTest() =>
            vgcServers.SortSelectedBySpeedTest();

        public void StopAllServers()
        {
            var evDone = new AutoResetEvent(false);
            vgcServers.StopAllServersThen(() => evDone.Set());
            vgcNotifier.BlockingWaitOne(evDone);
        }

        public bool RunSpeedTestOnSelectedServers() =>
            vgcServers.RunSpeedTestOnSelectedServers();

        public string PackSelectedServers(
            string orgUid, string pkgName) =>
            vgcServers.PackSelectedServersIntoV4Package(orgUid, pkgName);
    }
}
