using NLua;
using System;
using System.Collections.Generic;
using System.Linq;
using VgcApis.Interfaces;

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
        public int BalancerStrategyRandom { get; } = (int)VgcApis.Models.Datas.Enums.BalancerStrategies.Random;

        public int BalancerStrategyLeastPing { get; } = (int)VgcApis.Models.Datas.Enums.BalancerStrategies.LeastPing;
        #endregion

        public int Count() =>
            vgcServers.Count();

        public int CountSelected() =>
            vgcServers.CountSelected();

        public bool Add(string config) => Add(config, "");

        public bool Add(string config, string mark)
        {
            return vgcServers.AddServer(config, mark);
        }

        public bool DeleteServerByConfig(string config) =>
            vgcServers.DeleteServerByConfig(config, true);

        public ICoreServ GetServByIndex(int index)
        {
            return CoreServWrapper.Wrap<ICoreServ>(GetServerByIndex(index));
        }

        public ICoreServ GetServByUid(string uid)
        {
            return CoreServWrapper.Wrap<ICoreServ>(GetServerByUid(uid));
        }

        public List<ICoreServ> GetServsByUids(LuaTable uids)
        {
            var servs = GetServersByUids(uids)
                .Select(s => CoreServWrapper.Wrap<ICoreServ>(s))
                .ToList();
            return servs;
        }

        public ICoreServ GetServByConfig(string config)
        {
            return CoreServWrapper.Wrap<ICoreServ>(GetServerByConfig(config));
        }

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

        /// <summary>
        /// 危险操作！！
        /// </summary>
        /// <param name="uids"></param>
        public int DeleteServerByUids(LuaTable uids)
        {
            var list = global::Luna.Misc.Utils.LuaTableToList(uids, false);
            return vgcServers.DeleteServerByUids(list);
        }

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
            vgcServers.GetAllServersOrderByIndex();

        public List<VgcApis.Interfaces.ICoreServ> GetAllServs()
        {
            var servs = vgcServers.GetAllServersOrderByIndex();
            var coreServs = servs
                .Select(s => CoreServWrapper.Wrap<VgcApis.Interfaces.ICoreServ>(s))
                .ToList();
            return coreServs;
        }

        public List<VgcApis.Interfaces.ICoreServCtrl> GetServersByUids(LuaTable uids)
        {
            var list = global::Luna.Misc.Utils.LuaTableToList(uids, false);
            return vgcServers.GetServersByUidsOrderByIndex(list);
        }

        public void ReverseSelectedByIndex() =>
            vgcServers.ReverseSelectedByIndex();

        public void SortSelectedServersByLastModifiedDate() =>
            vgcServers.SortSelectedByLastModifiedDate();

        public void SortSelectedServersBySummary() =>
            vgcServers.SortSelectedBySummary();

        public void SortSelectedServersBySpeedTest() =>
            vgcServers.SortSelectedBySpeedTest();

        public void SortSelectedByDownloadTotal() =>
            vgcServers.SortSelectedByDownloadTotal();

        public void SortSelectedByUploadTotal() =>
            vgcServers.SortSelectedByUploadTotal();

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

        public void StopAllServers() =>
            vgcServers.StopAllServers();

        public bool RunSpeedTestByUids(LuaTable uids)
        {
            var list = global::Luna.Misc.Utils.LuaTableToList(uids, false);
            return vgcServers.RunSpeedTestBgQuiet(list);
        }

        public bool RunSpeedTestOnSelectedServers() =>
            vgcServers.RunSpeedTestOnSelectedServers();

        public bool RunSpeedTestOnSelectedServersBgQuiet() =>
            vgcServers.RunSpeedTestOnSelectedServersBgQuiet();

        public void StopSpeedTest() =>
            vgcServers.StopSpeedTest();

        public bool IsRunningSpeedTest() =>
            vgcServers.IsRunningSpeedTest();

        public string PackSelectedServers(string orgUid, string pkgName) =>
            PackSelectedServers(
                orgUid, pkgName,
                (int)VgcApis.Models.Datas.Enums.BalancerStrategies.Random);

        public string PackSelectedServers(string orgUid, string pkgName, int strategy)
        {
            return PackSelectedServers(
                orgUid,
                pkgName,
                strategy,
                string.Empty,
                string.Empty);
        }

        public string PackSelectedServers(
            string orgUid, string pkgName, int strategy,
            string interval, string url)
        {
            var st = (VgcApis.Models.Datas.Enums.BalancerStrategies)strategy;
            return vgcServers.PackSelectedServersV4(
                orgUid,
                pkgName,
                interval,
                url,
                st,
                VgcApis.Models.Datas.Enums.PackageTypes.Balancer);
        }

        public string PackServersWithUids(
            LuaTable uids,
            string orgUid, string pkgName, int strategy,
            string interval, string url)
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
                VgcApis.Models.Datas.Enums.PackageTypes.Balancer);
        }

        public string ChainServersWithUids(
            LuaTable uids, string orgUid, string pkgName)
        {
            var list = global::Luna.Misc.Utils.LuaTableToList(uids, false);
            return vgcServers.PackServersWithUidsV4(
                list,
                orgUid,
                pkgName,
                string.Empty,
                string.Empty,
                VgcApis.Models.Datas.Enums.BalancerStrategies.Random,
                VgcApis.Models.Datas.Enums.PackageTypes.Chain);
        }


        public string ChainSelectedServers(string orgUid, string pkgName) =>
            vgcServers.PackSelectedServersV4(
                orgUid,
                pkgName,
                string.Empty,
                string.Empty,
                VgcApis.Models.Datas.Enums.BalancerStrategies.Random,
                VgcApis.Models.Datas.Enums.PackageTypes.Chain);

    }
}
