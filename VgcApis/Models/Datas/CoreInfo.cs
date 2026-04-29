using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;

namespace VgcApis.Models.Datas
{
    public class CoreInfo
    {
        static readonly string DefaultZstdDictTag = Libs.Infr
            .ZipExtensions
            .ZSTD_DICT_TAG_CONFIG_JSON_V1;

        static readonly byte[] ZstdEmptyConfig = new byte[0];

        // plain text of config.json
        [DefaultValue("")]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string config = "";

        string _zstdDictTag = "";

        [DefaultValue("")]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string zstdDictTag
        {
            get { return _zstdDictTag; }
            private set { _zstdDictTag = Libs.Infr.StringRefCache.Ref(value); }
        }

        public byte[] zstdConfig = ZstdEmptyConfig;

        string _templates = "";

        [DefaultValue("")]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string templates
        {
            get { return _templates; }
            set { _templates = Libs.Infr.StringRefCache.Ref(value); }
        }

        string _customCoreName = "";

        [DefaultValue("")]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string customCoreName
        {
            get { return _customCoreName; }
            set { _customCoreName = Libs.Infr.StringRefCache.Ref(value); }
        }

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

        string _inbName = "http";

        [DefaultValue("http")]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string inbName
        {
            get { return _inbName; }
            set { _inbName = Libs.Infr.StringRefCache.Ref(value); }
        }

        string _inbIp = Consts.Webs.LoopBackIP;

        [DefaultValue("127.0.0.1")]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string inbIp
        {
            get { return _inbIp; }
            set { _inbIp = Libs.Infr.StringRefCache.Ref(value); }
        }

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

        string _customMark = "";

        [DefaultValue("")]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string customMark
        {
            get { return _customMark; }
            set { _customMark = Libs.Infr.StringRefCache.Ref(value); }
        }

        [DefaultValue("")]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string customRemark = "",
            tag1 = "",
            tag2 = "",
            tag3 = "";

        public List<InboundInfo> inboundsInfoCache = null;

        // json serializer requires this ctor!!
        public CoreInfo() { }

        #region public methods
        public string GetConfig()
        {
            return DeCompressOndemand(config);
        }

        public bool SetRawConfig(string config)
        {
            if (config == this.config)
            {
                return false;
            }
            this.config = config ?? "";
            this.zstdDictTag = "";
            this.zstdConfig = new byte[0];
            return true;
        }

        public void SetConfig(string config)
        {
            var len = Misc.Utils.StrLenInBytes(config);
            if (len >= Consts.Libs.MaxCompressConfigLength)
            {
                this.config = config;
                this.zstdDictTag = "";
                this.zstdConfig = ZstdEmptyConfig;
            }
            else
            {
                this.zstdConfig = Libs.Infr.ZipExtensions.ZstdDictToBytes(
                    DefaultZstdDictTag,
                    config
                );
                this.zstdDictTag = DefaultZstdDictTag;
                this.config = "";
            }
        }
        #endregion

        #region private methods
        string DeCompressOndemand(string s)
        {
            if (!string.IsNullOrEmpty(zstdDictTag))
            {
                var c = Libs.Infr.ZipExtensions.ZstdDictFromBytes(zstdDictTag, zstdConfig);
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
