namespace VgcApis.Interfaces.Services
{
    public interface IConfigMgrService
    {
        long RunSpeedTest(string rawConfig);
        long RunCustomSpeedTest(string rawConfig, string coreName, string testUrl, int testTimeout);

        string FetchWithCustomConfig(
            string rawConfig,
            string coreName,
            string title,
            string url,
            int timeout
        );
    }
}
