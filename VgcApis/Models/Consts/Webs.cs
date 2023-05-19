namespace VgcApis.Models.Consts
{
    public static class Webs
    {
        public static string Nobody3uVideoUrl = @"https://www.youtube.com/watch?v=BA7fdSkp8ds";

        public static string UserAgentKey = @"User-Agent";
        public static string ChromeUserAgent = @"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/113.0.0.0 Safari/537.36";

        // this var will be modified by V2RayGCon.Services.Settings.
        public static string CustomUserAgent = ChromeUserAgent;

        // this var will be modified by V2RayGCon.Services.Settings.
        public static string UserAgent = $"{UserAgentKey}: {ChromeUserAgent}";

        public static int CheckForUpdateDelay = 15 * 1000;

        public static string ReleaseDownloadUrlTpl =
            @"https://github.com/vrnobody/V2RayGCon/releases/download/{0}/V2RayGCon.zip";

        public static string LoopBackIP = System.Net.IPAddress.Loopback.ToString();
        public static int DefaultProxyPort = 8080;

        public const string FakeRequestUrl = @"http://localhost:3000/pac/?&t=abc1234";
        public const string GoogleDotCom = @"https://www.google.com";

        public const string BingDotCom = @"https://www.bing.com";

        // https://www.bing.com/search?q=vmess&first=21
        public const string SearchUrlPrefix = BingDotCom + @"/search?q=";
        public const string SearchPagePrefix = @"&first=";


    }
}
