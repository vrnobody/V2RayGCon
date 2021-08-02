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

        ReadOnlyCollection<ICoreServCtrl> GetTrackableServerList();
        ReadOnlyCollection<ICoreServCtrl> GetAllServersOrderByIndex();
    }
}
