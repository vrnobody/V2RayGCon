using System.Collections.Generic;

namespace VgcApis.Interfaces.Lua
{
    public interface ILuaServer
    {
        int balancerStrategyRandom { get; }

        int balancerStrategyLeastPing { get; }

        List<ICoreServCtrl> GetAllServers();
        string PackSelectedServers(string orgUid, string pkgName);

        string PackSelectedServers(string orgUid, string pkgName, int strategy);

        // wont refresh form main
        void ResetIndexes();

        // download (testUrl)
        long RunCustomSpeedTest(string rawConfig, string testUrl, int testTimeout);

        // download google.com
        long RunSpeedTest(string rawConfig);
        bool RunSpeedTestOnSelectedServers();

        void ReverseSelectedByIndex();
        void SortSelectedServersByLastModifiedDate();
        void SortSelectedServersBySpeedTest();
        void SortSelectedServersBySummary();

        void StopAllServers();

        // refresh servers' title in form main
        void UpdateAllSummary();

    }
}
