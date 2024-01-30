namespace VgcApis.Models.Datas
{
    public class CoreInfo
    {
        // ---- obsolete 预计2024-06删除 ----
        public int customInbType = (int)Enums.ProxyTypes.HTTP;
        public bool isInjectImport,
            isInjectSkipCNSite;

        // ---------------------------------

        // plain text of config.json
        public string config;

        public string finalConfigCache;

        public string templates;

        public bool isAcceptInjection = true;

        public bool ignoreSendThrough = false;

        // flags
        public bool isAutoRun,
            isSelected,
            isUntrack;

        public string name,
            longName,
            shortName,
            summary,
            title,
            customMark,
            uid,
            customRemark,
            customCoreName;

        public string inbName,
            inbIp;
        public int inbPort;

        public double index;

        public long lastModifiedUtcTicks,
            lastSpeedTestUtcTicks,
            speedTestResult;

        public long totalUplinkInBytes,
            totalDownlinkInBytes;

        public string tag1,
            tag2,
            tag3;

        public CoreInfo()
        {
            customCoreName = string.Empty;

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

            customMark = string.Empty;
            customRemark = string.Empty;

            name = string.Empty;
            longName = string.Empty;
            shortName = string.Empty;
            title = string.Empty;
            summary = string.Empty;
            config = string.Empty;
            templates = string.Empty;
            uid = string.Empty;

            inbName = string.Empty;
            inbIp = Consts.Webs.LoopBackIP;
            inbPort = Consts.Webs.DefaultProxyPort;

            tag1 = string.Empty;
            tag2 = string.Empty;
            tag3 = string.Empty;
        }

        #region public methods
        public string GetFinalConfigCache()
        {
            return DeCompressOndemand(finalConfigCache);
        }

        public void SetFinalConfigCache(string config)
        {
            var c = CompressOnDemand(config);
            this.finalConfigCache = c;
        }

        public string GetConfig()
        {
            return DeCompressOndemand(config);
        }

        public void SetConfig(string config)
        {
            var c = CompressOnDemand(config);
            this.config = c;
        }
        #endregion

        #region private methods
        string DeCompressOndemand(string s)
        {
            if (Libs.Infr.ZipExtensions.IsCompressedBase64(s))
            {
                return Libs.Infr.ZipExtensions.DecompressFromBase64(s);
            }
            return s;
        }

        string CompressOnDemand(string s)
        {
            if (Misc.Utils.StrLenInBytes(s) > Consts.Libs.MinCompressStringLength)
            {
                return Libs.Infr.ZipExtensions.CompressToBase64(s);
            }
            return s;
        }
        #endregion
    }
}
