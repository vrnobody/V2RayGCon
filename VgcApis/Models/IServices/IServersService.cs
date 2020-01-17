using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace VgcApis.Models.IServices
{
    public interface IServersService
    {
        event EventHandler OnCoreStart, OnCoreClosing, OnCoreStop;

        int GetAvailableHttpProxyPort();
        string ReplaceOrAddNewServer(string orgUid, string newConfig);
        void RequireFormMainReload();
        void ResetIndexQuiet();
        bool RunSpeedTestOnSelectedServers();

        void SortSelectedByLastModifiedDate();

        void SortSelectedBySpeedTest();

        void SortSelectedBySummary();
        void UpdateAllServersSummarySync();

        string PackSelectedServersIntoV4Package(
            string orgUid, string pkgName);

        string PackServersIntoV4Package(
            List<Interfaces.ICoreServCtrl> servList,
            string orgServerUid,
            string packageName);

        ReadOnlyCollection<Interfaces.ICoreServCtrl> GetTrackableServerList();
        ReadOnlyCollection<Interfaces.ICoreServCtrl> GetAllServersOrderByIndex();
    }
}
