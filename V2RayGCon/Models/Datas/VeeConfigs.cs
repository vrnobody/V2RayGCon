using Newtonsoft.Json.Linq;
using System;

namespace V2RayGCon.Models.Datas
{
    public class VeeConfigs
    {
        public string name = string.Empty;
        public string description = string.Empty;
        public string proto = string.Empty;
        public string host = string.Empty;
        public int port = 0;
        public string auth1 = string.Empty;
        public string auth2 = string.Empty;
        public string method = string.Empty;
        public bool useOta = false;
        public bool useSelfSignCert = false;
        public bool useTls = false;
        public string streamType = string.Empty;
        public string streamParam1 = string.Empty;
        public string streamParam2 = string.Empty;
        public string streamParam3 = string.Empty;


        public VeeConfigs() { }

        public VeeConfigs(string config) : this()
        {
            try
            {
                ParseConfig(config);
            }
            catch { }
        }

        public VeeConfigs(JObject config) : this(config.ToString())
        {

        }

        public string ToVeeShareLink()
        {
            try
            {
                var bytes = GenVeeShareLink();
                return Services.ShareLinkComponents.VeeDecoder.Bytes2VeeLink(bytes);
            }
            catch { }
            return "";
        }

        #region private methods


        byte[] GenVeeShareLink()
        {
            var bs = GetBasicSettings();
            switch (proto)
            {
                case "vmess":
                    var vmess = new VeeShareLinks.Vmess0a(bs);
                    vmess.uuid = Guid.Parse(auth1);
                    return vmess.ToBytes();
                case "vless":
                    var vless = new VeeShareLinks.Vless4a(bs);
                    vless.uuid = Guid.Parse(auth1);
                    return vless.ToBytes();
                case "http":
                    var http = new VeeShareLinks.Http3a(bs);
                    http.userName = auth1;
                    http.userPassword = auth2;
                    return http.ToBytes();
                case "socks":
                    var socks = new VeeShareLinks.Socks2a(bs);
                    socks.userName = auth1;
                    socks.userPassword = auth2;
                    return socks.ToBytes();
                case "shadowsocks":
                    var ss = GetSsConfig();
                    return ss.ToBytes();
                default:
                    break;
            }
            return null;
        }

        VeeShareLinks.Ss1a GetSsConfig()
        {
            var ss = new VeeShareLinks.Ss1a();
            ss.alias = name;
            ss.description = description;
            ss.address = host;
            ss.port = port;

            ss.isUseTls = useTls;
            ss.streamType = streamType;
            ss.streamParam1 = streamParam1;
            ss.streamParam2 = streamParam2;
            ss.streamParam3 = streamParam3;

            ss.isUseOta = useOta;
            ss.password = auth1;
            ss.method = method;
            return ss;
        }

        VeeShareLinks.BasicSettings GetBasicSettings()
        {
            var bs = new VeeShareLinks.BasicSettings();
            bs.alias = name;
            bs.description = description;

            bs.address = host;
            bs.port = port;

            bs.isUseTls = useTls;
            bs.isSecTls = !useSelfSignCert;
            bs.streamType = streamType;

            bs.streamParam1 = streamParam1;
            bs.streamParam2 = streamParam2;
            bs.streamParam3 = streamParam3;
            return bs;
        }

        void ParseBasicSettings(VeeShareLinks.BasicSettings bs)
        {
            name = bs.alias;
            description = bs.description;

            host = bs.address;
            port = bs.port;

            useTls = bs.isUseTls;
            useSelfSignCert = !bs.isSecTls;
            streamType = bs.streamType;

            streamParam1 = bs.streamParam1;
            streamParam2 = bs.streamParam2;
            streamParam3 = bs.streamParam3;
        }

        void ParseConfig(string config)
        {
            var slinkMgr = Services.ShareLinkMgr.Instance;

            var vee = slinkMgr.EncodeConfigToShareLink(config, VgcApis.Models.Datas.Enums.LinkTypes.v);
            if (vee == null)
            {
                return;
            }

            var bytes = Services.ShareLinkComponents.VeeDecoder.VeeLink2Bytes(vee);
            var lv = VgcApis.Libs.Streams.BitStream.ReadVersion(bytes);

            if (lv == VeeShareLinks.Vmess0a.SupportedVersion())
            {
                this.proto = "vmess";
                var vmess = new VeeShareLinks.Vmess0a(bytes);
                ParseBasicSettings(vmess);
                this.auth1 = vmess.uuid.ToString();
            }
            else if (lv == VeeShareLinks.Vless4a.SupportedVersion())
            {
                this.proto = "vless";
                var vless = new VeeShareLinks.Vless4a(bytes);
                ParseBasicSettings(vless);
                this.auth1 = vless.uuid.ToString();
            }
            else if (lv == VeeShareLinks.Ss1a.SupportedVersion())
            {
                this.proto = "shadowsocks";
                var ss = new VeeShareLinks.Ss1a(bytes);
                ParseSsConfig(ss);
            }
            else if (lv == VeeShareLinks.Http3a.SupportedVersion())
            {
                this.proto = "http";
                var http = new VeeShareLinks.Http3a(bytes);
                ParseBasicSettings(http);
                this.auth1 = http.userName;
                this.auth2 = http.userPassword;
            }
            else if (lv == VeeShareLinks.Socks2a.SupportedVersion())
            {
                this.proto = "socks";
                var socks = new VeeShareLinks.Socks2a(bytes);
                ParseBasicSettings(socks);
                this.auth1 = socks.userName;
                this.auth2 = socks.userPassword;
            }
        }

        void ParseSsConfig(VeeShareLinks.Ss1a bs)
        {

            name = bs.alias;
            description = bs.description;

            host = bs.address;
            port = bs.port;

            this.useTls = bs.isUseTls;
            this.useSelfSignCert = false;
            this.streamType = bs.streamType;

            this.streamParam1 = bs.streamParam1;
            this.streamParam2 = bs.streamParam2;
            this.streamParam3 = bs.streamParam3;
            this.useOta = bs.isUseOta;
            this.auth1 = bs.password;
            this.method = bs.method;
        }

        #endregion

    }
}
