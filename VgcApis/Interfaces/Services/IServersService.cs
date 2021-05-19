using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace VgcApis.Interfaces.Services
{
    public interface IServersService
    {
        event EventHandler OnCoreStart, OnCoreClosing, OnCoreStop;

        int GetAvailableHttpProxyPort();
        string ReplaceOrAddNewServer(string orgUid, string newConfig);

        string ReplaceOrAddNewServer(string orgUid, string newConfig, string mark);

        void RequireFormMainReload();
        void ResetIndexQuiet();
        bool RunSpeedTestOnSelectedServers();

        void ReverseSelectedByIndex();

        void SortSelectedByLastModifiedDate();

        void SortSelectedBySpeedTest();

        void SortSelectedBySummary();

        void StopAllServers();

        void StopAllServersThen(Action lambda = null);

        void UpdateAllServersSummary();

        string PackSelectedServersIntoV4Package(
            string orgUid, string pkgName,
            VgcApis.Models.Datas.Enums.BalancerStrategies strategy);

        string PackServersIntoV4PackageUi(
            List<ICoreServCtrl> servList,
            string orgServerUid,
            string packageName,
            VgcApis.Models.Datas.Enums.BalancerStrategies strategy);

        ReadOnlyCollection<ICoreServCtrl> GetTrackableServerList();
        ReadOnlyCollection<ICoreServCtrl> GetAllServersOrderByIndex();
    }
}
