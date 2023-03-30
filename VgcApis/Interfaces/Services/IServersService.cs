using System;
using System.Collections.Generic;

namespace VgcApis.Interfaces.Services
{
    public interface IServersService
    {
        event EventHandler OnCoreStart, OnCoreClosing, OnCoreStop;

        int Count();

        void DeleteServerByConfig(string config);
        void DeleteServerByUids(List<string> uids);

        int GetAvailableHttpProxyPort();
        string ReplaceOrAddNewServer(string orgUid, string newConfig);

        string ReplaceOrAddNewServer(string orgUid, string newConfig, string mark);

        void RequireFormMainReload();
        void ResetIndexQuiet();

        void RestartOneServerByUid(string uid);

        bool RunSpeedTestOnSelectedServers();

        void ReverseSelectedByIndex();

        void SortSelectedByLastModifiedDate();

        void SortSelectedBySpeedTest();

        void SortSelectedBySummary();


        void StopAllServers();

        void StopAllServersThen(Action lambda = null);

        void UpdateAllServersSummary();

        string PackServersWithUidsV4(
            List<string> uids,
            string orgUid, string pkgName,
            string interval, string url,
            VgcApis.Models.Datas.Enums.BalancerStrategies strategy,
            VgcApis.Models.Datas.Enums.PackageTypes packageType);

        string PackSelectedServersV4(
              string orgUid, string pkgName,
              string interval, string url,
              VgcApis.Models.Datas.Enums.BalancerStrategies strategy,
              VgcApis.Models.Datas.Enums.PackageTypes packageType);

        string PackServersV4Ui(
            List<VgcApis.Interfaces.ICoreServCtrl> servList,
            string orgUid,
            string packageName,
            string interval,
            string url,
            VgcApis.Models.Datas.Enums.BalancerStrategies strategy,
            VgcApis.Models.Datas.Enums.PackageTypes packageType);

        List<ICoreServCtrl> GetTrackableServerList();
        List<ICoreServCtrl> GetAllServersOrderByIndex();
        List<ICoreServCtrl> GetSelectedServers(bool descending = false);
    }
}
