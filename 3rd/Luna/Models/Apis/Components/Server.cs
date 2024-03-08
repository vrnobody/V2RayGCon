using System.Collections.Generic;
using System.Linq;
using NLua;
using VgcApis.Interfaces;

namespace Luna.Models.Apis.Components
{
    internal sealed class Server : VgcApis.BaseClasses.ComponentOf<LuaApis>, Interfaces.ILuaServer
    {
        readonly VgcApis.Interfaces.Services.IServersService vgcServers;
        readonly VgcApis.Interfaces.Services.IConfigMgrService vgcConfigMgr;

        public Server(VgcApis.Interfaces.Services.IApiService api)
        {
            vgcServers = api.GetServersService();
            vgcConfigMgr = api.GetConfigMgrService();
        }

        #region balancer
        public int BalancerStrategyRandom { get; } =
            (int)VgcApis.Models.Datas.Enums.BalancerStrategies.Random;

        public int BalancerStrategyLeastPing { get; } =
            (int)VgcApis.Models.Datas.Enums.BalancerStrategies.LeastPing;

        public int BalancerStrategyRoundRobin { get; } =
            (int)VgcApis.Models.Datas.Enums.BalancerStrategies.RoundRobin;

        public int BalancerStrategyLeastLoad { get; } =
            (int)VgcApis.Models.Datas.Enums.BalancerStrategies.LeastLoad;
        #endregion

        public int Count() => vgcServers.Count();

        public int CountSelected() => vgcServers.CountSelected();

        public bool Add(string config) => Add(config, "");

        public bool Add(string config, string mark) => Add(config, string.Empty, mark);

        public bool Add(string config, string name, string mark)
        {
            var uid = vgcServers.AddServer(name, config, mark, false);
            return !string.IsNullOrEmpty(uid);
        }

        public bool DeleteServerByConfig(string config) =>
            vgcServers.DeleteServerByConfig(config, true);

        public ICoreServCtrl GetServerByConfig(string config)
        {
            return vgcServers.GetServerByConfig(config);
        }

        public ICoreServCtrl GetServerByIndex(int index)
        {
            return vgcServers.GetServerByIndex(index);
        }

        public ICoreServCtrl GetServerByUid(string uid)
        {
            return vgcServers.GetServerByUid(uid);
        }

        public bool DeleteServerByUid(string uid)
        {
            return vgcServers.DeleteServerByUid(uid);
        }

        public int DeleteServerByUids(LuaTable uids)
        {
            var list = global::Luna.Misc.Utils.LuaTableToList(uids, false);
            return vgcServers.DeleteServerByUids(list);
        }

        public void UpdateAllSummary() => vgcServers.UpdateAllServersSummary();

        public void ResetIndexes() => vgcServers.ResetIndexQuiet();

        // expose for ILuaServer
        public long RunSpeedTest(string rawConfig) => vgcConfigMgr.RunSpeedTest(rawConfig);

        public long RunCustomSpeedTest(
            string rawConfig,
            string coreName,
            string testUrl,
            int testTimeout
        ) => vgcConfigMgr.RunCustomSpeedTest(rawConfig, coreName, testUrl, testTimeout);

        public List<ICoreServCtrl> GetAllServers() => vgcServers.GetAllServersOrderByIndex();

        public List<ICoreServCtrl> GetServersByUids(LuaTable uids)
        {
            var list = global::Luna.Misc.Utils.LuaTableToList(uids, false);
            return vgcServers.GetServersByUids(list);
        }

        public void ReverseSelectedByIndex() => vgcServers.ReverseSelectedByIndex();

        public void SortSelectedServersByLastModifiedDate() =>
            vgcServers.SortSelectedByLastModifiedDate();

        public void SortSelectedServersBySummary() => vgcServers.SortSelectedBySummary();

        public void SortSelectedServersBySpeedTest() => vgcServers.SortSelectedBySpeedTest();

        public void SortSelectedByDownloadTotal() => vgcServers.SortSelectedByDownloadTotal();

        public void SortSelectedByUploadTotal() => vgcServers.SortSelectedByUploadTotal();

        public void ReverseServersByIndex(LuaTable uids)
        {
            var list = global::Luna.Misc.Utils.LuaTableToList(uids, false);
            vgcServers.ReverseServersByIndex(list);
        }

        public void SortServersByLastModifiedDate(LuaTable uids)
        {
            var list = global::Luna.Misc.Utils.LuaTableToList(uids, false);
            vgcServers.SortServersByLastModifiedDate(list);
        }

        public void SortServersBySpeedTest(LuaTable uids)
        {
            var list = global::Luna.Misc.Utils.LuaTableToList(uids, false);
            vgcServers.SortServersBySpeedTest(list);
        }

        public void SortServersBySummary(LuaTable uids)
        {
            var list = global::Luna.Misc.Utils.LuaTableToList(uids, false);
            vgcServers.SortServersBySummary(list);
        }

        public void SortServersByDownloadTotal(LuaTable uids)
        {
            var list = global::Luna.Misc.Utils.LuaTableToList(uids, false);
            vgcServers.SortServersByDownloadTotal(list);
        }

        public void SortServersByUploadTotal(LuaTable uids)
        {
            var list = global::Luna.Misc.Utils.LuaTableToList(uids, false);
            vgcServers.SortServersByUploadTotal(list);
        }

        public void StopAllServers() => vgcServers.StopAllServers();

        public bool RunSpeedTestByUids(LuaTable uids)
        {
            var list = global::Luna.Misc.Utils.LuaTableToList(uids, false);
            return vgcServers.RunSpeedTestBgQuiet(list);
        }

        public bool RunSpeedTestOnSelectedServers() => vgcServers.RunSpeedTestOnSelectedServers();

        public bool RunSpeedTestOnSelectedServersBgQuiet() =>
            vgcServers.RunSpeedTestOnSelectedServersBgQuiet();

        public void StopSpeedTest() => vgcServers.StopSpeedTest();

        public bool IsRunningSpeedTest() => vgcServers.IsRunningSpeedTest();

        public string PackSelectedServers(string orgUid, string pkgName) =>
            PackSelectedServers(
                orgUid,
                pkgName,
                (int)VgcApis.Models.Datas.Enums.BalancerStrategies.Random
            );

        public string PackSelectedServers(string orgUid, string pkgName, int strategy)
        {
            return PackSelectedServers(orgUid, pkgName, strategy, string.Empty, string.Empty);
        }

        public string PackSelectedServers(
            string orgUid,
            string pkgName,
            int strategy,
            string interval,
            string url
        )
        {
            var st = (VgcApis.Models.Datas.Enums.BalancerStrategies)strategy;
            return vgcServers.PackSelectedServersV4(
                orgUid,
                pkgName,
                interval,
                url,
                st,
                VgcApis.Models.Datas.Enums.PackageTypes.Balancer
            );
        }

        public string PackServersWithUids(
            LuaTable uids,
            string orgUid,
            string pkgName,
            int strategy,
            string interval,
            string url
        )
        {
            var st = (VgcApis.Models.Datas.Enums.BalancerStrategies)strategy;
            var list = global::Luna.Misc.Utils.LuaTableToList(uids, false);
            return vgcServers.PackServersWithUidsV4(
                list,
                orgUid,
                pkgName,
                interval,
                url,
                st,
                VgcApis.Models.Datas.Enums.PackageTypes.Balancer
            );
        }

        public string ChainServersWithUids(LuaTable uids, string orgUid, string pkgName)
        {
            var list = global::Luna.Misc.Utils.LuaTableToList(uids, false);
            return vgcServers.PackServersWithUidsV4(
                list,
                orgUid,
                pkgName,
                string.Empty,
                string.Empty,
                VgcApis.Models.Datas.Enums.BalancerStrategies.Random,
                VgcApis.Models.Datas.Enums.PackageTypes.Chain
            );
        }

        public string ChainSelectedServers(string orgUid, string pkgName) =>
            vgcServers.PackSelectedServersV4(
                orgUid,
                pkgName,
                string.Empty,
                string.Empty,
                VgcApis.Models.Datas.Enums.BalancerStrategies.Random,
                VgcApis.Models.Datas.Enums.PackageTypes.Chain
            );

        #region wrap interface
        public IWrappedCoreServCtrl GetWrappedServerByIndex(int index) =>
            GetServerByIndex(index)?.Wrap();

        public IWrappedCoreServCtrl GetWrappedServerByUid(string uid) =>
            GetServerByUid(uid)?.Wrap();

        public IWrappedCoreServCtrl GetWrappedServerByConfig(string config) =>
            GetServerByConfig(config)?.Wrap();

        public List<IWrappedCoreServCtrl> GetWrappedServersByUids(LuaTable uids) =>
            GetServersByUids(uids).Select(c => c?.Wrap()).Where(c => c != null).ToList();
        #endregion
    }
}
