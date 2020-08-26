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
            string orgUid, string pkgName);

        string PackServersIntoV4Package(
            List<ICoreServCtrl> servList,
            string orgServerUid,
            string packageName);

        ReadOnlyCollection<ICoreServCtrl> GetTrackableServerList();
        ReadOnlyCollection<ICoreServCtrl> GetAllServersOrderByIndex();
    }
}
