namespace VgcApis.Interfaces.Services
{
    public interface IConfigMgrService
    {
        long RunSpeedTest(string rawConfig);
        long RunCustomSpeedTest(string rawConfig, string testUrl, int testTimeout);

        string FetchWithCustomConfig(string rawConfig, string title, string url, int timeout);
    }
}
