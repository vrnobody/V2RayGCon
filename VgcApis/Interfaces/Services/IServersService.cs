using System;
using System.Collections.Generic;

namespace VgcApis.Interfaces.Services
{
    public interface IServersService
    {
        event EventHandler OnCoreStart,
            OnCoreClosing,
            OnCoreStop;

        int Count();

        int CountSelected();

        bool DeleteServerByConfig(string config, bool isQuiet);

        bool DeleteServerByUid(string uid);

        int DeleteServerByUids(List<string> uids);

        List<Models.Datas.InboundInfo> GetAllActiveInboundsInfo();

        bool GetAvailableProxyInfo(out bool isSocks5, out int port);

        int GetAvailableHttpProxyPort();
        int GetAvailableSocksProxyPort();

        string AddServer(string name, string config, string mark, bool quiet);

        string ReplaceOrAddNewServer(string orgUid, string newName, string newConfig, string mark);

        void RequireFormMainReload();
        void ResetIndexQuiet();

        void RestartOneServerByUid(string uid);

        bool RunSpeedTestOnSelectedServers();

        bool RunSpeedTestOnSelectedServersBgQuiet();

        bool RunSpeedTestBgQuiet(List<string> uids);

        void StopSpeedTest();

        bool IsRunningSpeedTest();

        void ReverseSelectedByIndex();

        void SortSelectedByLastModifiedDate();

        void SortSelectedBySpeedTest();

        void SortSelectedBySummary();

        void SortSelectedByDownloadTotal();

        void SortSelectedByUploadTotal();

        void ReverseServersByIndex(List<string> uids);

        void SortServersByLastModifiedDate(List<string> uids);

        void SortServersBySpeedTest(List<string> uids);

        void SortServersBySummary(List<string> uids);

        void SortServersByDownloadTotal(List<string> uids);

        void SortServersByUploadTotal(List<string> uids);

        void StopAllServers();

        void StopAllServersThen(Action lambda = null);

        void UpdateAllServersSummary();

        string PackServersToString(List<string> uids);

        string PackServersWithUidsV4(
            List<string> uids,
            string orgUid,
            string pkgName,
            string interval,
            string url,
            Models.Datas.Enums.BalancerStrategies strategy,
            Models.Datas.Enums.PackageTypes packageType
        );

        string PackSelectedServersV4(
            string orgUid,
            string pkgName,
            string interval,
            string url,
            Models.Datas.Enums.BalancerStrategies strategy,
            Models.Datas.Enums.PackageTypes packageType
        );

        string PackServersV4Ui(
            List<ICoreServCtrl> servList,
            string orgUid,
            string packageName,
            string interval,
            string url,
            Models.Datas.Enums.BalancerStrategies strategy,
            Models.Datas.Enums.PackageTypes packageType
        );

        List<ICoreServCtrl> GetTrackableServerList();

        IReadOnlyCollection<ICoreServCtrl> GetAllServersOrderByIndex();

        ICoreServCtrl GetServerByConfig(string config);

        ICoreServCtrl GetServerByIndex(int index);

        ICoreServCtrl GetServerByUid(string uid);

        List<ICoreServCtrl> GetServersByUids(IEnumerable<string> uids);

        List<ICoreServCtrl> GetSelectedServers();
    }
}
