using System.Collections.Generic;

namespace VgcApis.Models.Datas
{
    public class CoreInfo
    {
        static readonly string defZConfigVersion = "v1";
        static readonly byte[] zConfigEmpty = new byte[0];

        // plain text of config.json
        public string config;

        public string zConfigVer = "";
        public byte[] zConfigBytes = zConfigEmpty;

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

        public List<InboundInfo> inboundsInfoCache = null;

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
        public string GetConfig()
        {
            return DeCompressOndemand(config);
        }

        public void SetConfig(string config)
        {
            var len = Misc.Utils.StrLenInBytes(config);
            if (len >= Consts.Libs.MaxCompressConfigLength)
            {
                this.config = config;
                this.zConfigVer = "";
                this.zConfigBytes = zConfigEmpty;
            }
            else
            {
                this.zConfigVer = defZConfigVersion;
                this.zConfigBytes = Libs.Infr.ZipExtensions.ZstdDictToBytes(
                    defZConfigVersion,
                    config
                );
                this.config = "";
            }
        }
        #endregion

        #region private methods
        string DeCompressOndemand(string s)
        {
            if (!string.IsNullOrEmpty(zConfigVer))
            {
                var c = Libs.Infr.ZipExtensions.ZstdDictFromBytes(zConfigVer, zConfigBytes);
                this.config = "";
                return c;
            }
            else if (Libs.Infr.ZipExtensions.IsZstdBase64(s))
            {
                return Libs.Infr.ZipExtensions.ZstdFromBase64(s);
            }
            else if (Libs.Infr.ZipExtensions.IsCompressedBase64(s))
            {
                return Libs.Infr.ZipExtensions.DecompressFromBase64(s);
            }
            return s;
        }

        #endregion
    }
}
