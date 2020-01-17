using System.Collections.Generic;

namespace VgcApis.Models.Interfaces.Lua
{
    public interface ILuaServer
    {
        List<ICoreServCtrl> GetAllServers();
        string PackSelectedServers(string orgUid, string pkgName);
        void RequireFormMainReload();

        // wont refresh form main
        void ResetIndexQuiet();

        // download (testUrl)
        long RunCustomSpeedTest(string rawConfig, string testUrl, int testTimeout);

        // download google.com
        long RunSpeedTest(string rawConfig);
        bool RunSpeedTestOnSelectedServers();

        void SortSelectedServersByLastModifiedDate();
        void SortSelectedServersBySpeedTest();
        void SortSelectedServersBySummary();

        // refresh servers' title in form main
        void UpdateAllSummary();
    }
}
