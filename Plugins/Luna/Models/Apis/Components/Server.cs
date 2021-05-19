using System.Collections.Generic;
using System.Linq;

namespace Luna.Models.Apis.Components
{
    internal sealed class Server :
        VgcApis.BaseClasses.ComponentOf<LuaApis>,
        VgcApis.Interfaces.Lua.ILuaServer
    {
        VgcApis.Interfaces.Services.IServersService vgcServers;
        VgcApis.Interfaces.Services.IConfigMgrService vgcConfigMgr;

        public Server(
             VgcApis.Interfaces.Services.IApiService api)
        {
            vgcServers = api.GetServersService();
            vgcConfigMgr = api.GetConfigMgrService();
        }

        #region balancer
        public int balancerStrategyRandom { get; } = (int)VgcApis.Models.Datas.Enums.BalancerStrategies.Random;

        public int balancerStrategyLeastPing { get; } = (int)VgcApis.Models.Datas.Enums.BalancerStrategies.LeastPing;
        #endregion

        public void UpdateAllSummary() =>
            vgcServers.UpdateAllServersSummary();

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

        public void StopAllServers() =>
            vgcServers.StopAllServers();

        public bool RunSpeedTestOnSelectedServers() =>
            vgcServers.RunSpeedTestOnSelectedServers();

        public string PackSelectedServers(string orgUid, string pkgName) =>
            vgcServers.PackSelectedServersIntoV4Package(
                orgUid, pkgName,
                VgcApis.Models.Datas.Enums.BalancerStrategies.Random);

        public string PackSelectedServers(string orgUid, string pkgName, int strategy)
        {
            var st = (VgcApis.Models.Datas.Enums.BalancerStrategies)strategy;
            return vgcServers.PackSelectedServersIntoV4Package(orgUid, pkgName, st);
        }
    }
}
