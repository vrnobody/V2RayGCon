using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;

namespace VgcApis.Models.Datas
{
    public class CoreInfo
    {
        static readonly string defZConfigVersion = Libs.Infr
            .ZipExtensions
            .ZSTD_DICT_TAG_CORE_INFO_V1;
        static readonly byte[] zConfigEmpty = new byte[0];

        // plain text of config.json
        [DefaultValue("")]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string config = "";

        [DefaultValue("")]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string zConfigVer = "";

        public byte[] zConfigBytes = zConfigEmpty;

        [DefaultValue("")]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string templates = "",
            customCoreName = "";

        [DefaultValue(true)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool isAcceptInjection = true;

        // flags
        [DefaultValue(false)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool isAutoRun = false,
            isSelected = false,
            isUntrack = false,
            ignoreSendThrough = false;

        [DefaultValue("")]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string name = "",
            longName = "",
            shortName = "",
            summary = "",
            title = "",
            uid = "";

        [DefaultValue("http")]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string inbName = "http";

        [DefaultValue("127.0.0.1")]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string inbIp = Consts.Webs.LoopBackIP;

        [DefaultValue(8080)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int inbPort = Consts.Webs.DefaultProxyPort;

        // new server in the bottom
        public double index = double.MaxValue;

        public long lastModifiedUtcTicks = System.DateTime.UtcNow.Ticks,
            lastSpeedTestUtcTicks = System.DateTime.UtcNow.Ticks;

        [DefaultValue(-1)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public long speedTestResult = -1;

        [DefaultValue(0)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public long totalUplinkInBytes = 0,
            totalDownlinkInBytes = 0;

        [DefaultValue("")]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string customMark = "",
            customRemark = "",
            tag1 = "",
            tag2 = "",
            tag3 = "";

        public List<InboundInfo> inboundsInfoCache = null;

        public CoreInfo() { }

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
                this.zConfigBytes = Libs.Infr.ZipExtensions.ZstdDictToBytes(
                    defZConfigVersion,
                    config
                );
                this.zConfigVer = defZConfigVersion;
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
