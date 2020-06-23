namespace VgcApis.Models.Datas
{
    public class CoreInfo
    {
        // private variables will not be serialized

        // plain text of config.json
        public string config;

        // flags
        public bool
            isAutoRun,
            isInjectImport,
            isSelected,
            isInjectSkipCNSite,
            isUntrack;

        public string name, longName, shortName, summary, title, inbIp, customMark, uid, customRemark;

        public int customInbType, inbPort;

        public double index;

        public long lastModifiedUtcTicks, lastSpeedTestUtcTicks, speedTestResult;

        public long totalUplinkInBytes, totalDownlinkInBytes;

        public CoreInfo()
        {
            lastModifiedUtcTicks = System.DateTime.UtcNow.Ticks;
            lastSpeedTestUtcTicks = System.DateTime.UtcNow.Ticks;

            speedTestResult = -1;

            totalUplinkInBytes = 0;
            totalDownlinkInBytes = 0;

            // new server will displays at the bottom
            index = double.MaxValue;

            isSelected = false;
            isUntrack = false;

            isAutoRun = false;
            isInjectImport = false;

            customMark = string.Empty;
            customRemark = string.Empty;

            name = string.Empty;
            longName = string.Empty;
            shortName = string.Empty;
            title = string.Empty;
            summary = string.Empty;
            config = string.Empty;
            uid = string.Empty;


            customInbType = (int)Enums.ProxyTypes.HTTP;
            inbIp = Consts.Webs.LoopBackIP;
            inbPort = Consts.Webs.DefaultProxyPort;
        }

        public void ClearCachedString()
        {
            shortName = string.Empty;
            longName = string.Empty;
            title = string.Empty;
        }
    }
}
