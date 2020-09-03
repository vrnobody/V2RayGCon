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

        object EncodeToBytes(VeeShareLinks.BasicSettings bs, string proto)
        {
            if (VeeShareLinks.Vmess0a.IsEncoderFor(proto))
            {
                return new VeeShareLinks.Vmess0a(bs);
            }
            else if (VeeShareLinks.Vless4a.IsEncoderFor(proto))
            {
                return new VeeShareLinks.Vless4a(bs);
            }
            else if (VeeShareLinks.Ss1b.IsEncoderFor(proto))
            {
                return new VeeShareLinks.Ss1b(bs);
            }
            else if (VeeShareLinks.Socks2a.IsEncoderFor(proto))
            {
                return new VeeShareLinks.Socks2a(bs);
            }
            else if (VeeShareLinks.Http3a.IsEncoderFor(proto))
            {
                return new VeeShareLinks.Http3a(bs);
            }
            return null;
        }

        byte[] GenVeeShareLink()
        {
            var bs = GetBasicSettings();
            if (VeeShareLinks.Vmess0a.IsEncoderFor(proto))
            {
                var vmess = new VeeShareLinks.Vmess0a(bs);
                vmess.uuid = Guid.Parse(auth1);
                return vmess.ToBytes();
            }
            else if (VeeShareLinks.Vless4a.IsEncoderFor(proto))
            {
                var vless = new VeeShareLinks.Vless4a(bs);
                vless.uuid = Guid.Parse(auth1);
                return vless.ToBytes();
            }
            else if (VeeShareLinks.Ss1b.IsEncoderFor(proto))
            {
                var ss = new VeeShareLinks.Ss1b(bs);
                ss.isUseOta = useOta;
                ss.password = auth1;
                ss.method = method;
                return ss.ToBytes();
            }
            else if (VeeShareLinks.Socks2a.IsEncoderFor(proto))
            {
                var socks = new VeeShareLinks.Socks2a(bs);
                socks.userName = auth1;
                socks.userPassword = auth2;
                return socks.ToBytes();
            }
            else if (VeeShareLinks.Http3a.IsEncoderFor(proto))
            {
                var http = new VeeShareLinks.Http3a(bs);
                http.userName = auth1;
                http.userPassword = auth2;
                return http.ToBytes();
            }

            return null;
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

        object DecodeBytes(byte[] bytes)
        {
            var ver = VgcApis.Libs.Streams.BitStream.ReadVersion(bytes);
            if (VeeShareLinks.Vmess0a.IsDecoderFor(ver))
            {
                return new VeeShareLinks.Vmess0a(bytes);
            }
            else if (VeeShareLinks.Vless4a.IsDecoderFor(ver))
            {
                return new VeeShareLinks.Vless4a(bytes);
            }
            else if (VeeShareLinks.Ss1b.IsDecoderFor(ver))
            {
                return new VeeShareLinks.Ss1b(bytes);
            }
            else if (VeeShareLinks.Socks2a.IsDecoderFor(ver))
            {
                return new VeeShareLinks.Socks2a(bytes);
            }
            else if (VeeShareLinks.Http3a.IsDecoderFor(ver))
            {
                return new VeeShareLinks.Http3a(bytes);
            }

            return null;
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

            var linkModel = DecodeBytes(bytes);
            if (linkModel == null)
            {
                return;
            }

            ParseBasicSettings(linkModel as VeeShareLinks.BasicSettings);
            switch (linkModel)
            {
                case VeeShareLinks.Vmess0a vmess:
                    this.proto = "vmess";
                    this.auth1 = vmess.uuid.ToString();
                    break;
                case VeeShareLinks.Vless4a vless:
                    this.proto = "vless";
                    this.auth1 = vless.uuid.ToString();
                    break;
                case VeeShareLinks.Ss1b ss:
                    this.proto = "shadowsocks";
                    this.useOta = ss.isUseOta;
                    this.auth1 = ss.password;
                    this.method = ss.method;
                    break;
                case VeeShareLinks.Http3a http:
                    this.proto = "http";
                    this.auth1 = http.userName;
                    this.auth2 = http.userPassword;
                    break;
                case VeeShareLinks.Socks2a socks:
                    this.proto = "socks";
                    this.auth1 = socks.userName;
                    this.auth2 = socks.userPassword;
                    break;
                default:
                    break;
            }
        }



        #endregion

    }
}
