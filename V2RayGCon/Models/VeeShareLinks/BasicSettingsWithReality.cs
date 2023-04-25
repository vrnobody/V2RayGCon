using System.Collections.Generic;

namespace V2RayGCon.Models.VeeShareLinks
{
    public class BasicSettingsWithReality : BasicSettings
    {
        // max string length is 256 bytes

        //public string alias, description;
        //public string address;
        //public int port; // 16 bit each

        //public bool isUseTls, isSecTls;
        //public string streamType, streamParam1, streamParam2, streamParam3;

        //// patch
        //public string tlsType, tlsServName;

        // reality.publicKey
        public string tlsParam1 = string.Empty;

        // reality.shortId
        public string tlsParam2 = string.Empty;

        // reality.spiderX
        public string tlsParam3 = string.Empty;

        // tls.fingerprint
        public string tlsFingerPrint = string.Empty;
        public string tlsAlpn = string.Empty;


        public BasicSettingsWithReality(BasicSettings source) : base()
        {
            CopyFrom(source);
        }

        public BasicSettingsWithReality() : base()
        {

            tlsParam1 = string.Empty;
            tlsParam2 = string.Empty;
            tlsParam3 = string.Empty;

            tlsFingerPrint = string.Empty;
            tlsAlpn = string.Empty;
        }

        #region protected 
        // for string compression
        protected new readonly List<string> strTable = new List<string>{
            "ws", "tcp", "kcp", "h2", "quic",
            "none", "srtp", "utp", "wechat-video",
            "dtls", "wireguard",
            "xtls-rprx-direct", "xtls-rprx-splice",
            "tls", "xtls", "reality",
            "auto", "aes-128-gcm", "chacha20-poly1305",
            "gun", "multi", "guna",
            "chrome", "firefox", "safari", "ios", "android", "edge", "360", "qq", "random", "randomized",
            "",
        };

        #endregion

        #region public methods

        #endregion
        public virtual void CopyFromVeeConfig(Datas.VeeConfigsWithReality vcr)
        {
            base.CopyFromVeeConfig(vcr);
            this.tlsParam1 = vcr.tlsParam1;
            this.tlsParam2 = vcr.tlsParam2;
            this.tlsParam3 = vcr.tlsParam3;
            this.tlsFingerPrint = vcr.tlsFingerPrint;
            this.tlsAlpn = vcr.tlsAlpn;
        }

        public new Datas.VeeConfigsWithReality ToVeeConfigs()
        {
            var vc = base.ToVeeConfigs();
            var vcr = new Datas.VeeConfigsWithReality(vc);
            vcr.tlsParam1 = tlsParam1;
            vcr.tlsParam2 = tlsParam2;
            vcr.tlsParam3 = tlsParam3;
            vcr.tlsFingerPrint = tlsFingerPrint;
            vcr.tlsAlpn = tlsAlpn;

            return vcr;
        }

        public void CopyFrom(BasicSettingsWithReality source)
        {
            if (source == null)
            {
                return;
            }

            base.CopyFrom(source);
            this.tlsParam1 = source.tlsParam1;
            this.tlsParam2 = source.tlsParam2;
            this.tlsParam3 = source.tlsParam3;
            this.tlsFingerPrint = source.tlsFingerPrint;
            this.tlsAlpn = source.tlsAlpn;

        }

        public bool EqTo(BasicSettingsWithReality target)
        {
            if (!base.EqTo(target)
                || tlsParam1 != target.tlsParam1
                || tlsParam2 != target.tlsParam2
                || tlsParam3 != target.tlsParam3
                || tlsFingerPrint != target.tlsFingerPrint
                || tlsAlpn != target.tlsAlpn)
            {
                return false;
            }
            return true;
        }
    }
}
