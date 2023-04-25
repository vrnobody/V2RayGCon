namespace V2RayGCon.Models.Datas
{
    public class VeeConfigsWithReality : VeeConfigs
    {
        //public string name = string.Empty;
        //public string description = string.Empty;
        //public string proto = string.Empty;
        //public string host = string.Empty;
        //public int port = 0;
        //public string auth1 = string.Empty;
        //public string auth2 = string.Empty;

        //public string streamType = string.Empty;
        //public string streamParam1 = string.Empty;
        //public string streamParam2 = string.Empty;
        //public string streamParam3 = string.Empty;

        //public string tlsType = @"none";
        //public string tlsServName = @"";
        //public bool useSelfSignCert = false;

        // reality.publicKey
        public string tlsParam1 = string.Empty;

        // reality.shortId
        public string tlsParam2 = string.Empty;

        // reality.spiderX
        public string tlsParam3 = string.Empty;

        // tls.fingerprint
        public string tlsFingerPrint = string.Empty;

        // tls.alpn (not support yet)
        public string tlsAlpn = string.Empty;


        public VeeConfigsWithReality() { }

        public VeeConfigsWithReality(VeeConfigs veeConfigs) : this()
        {
            base.CopyFrom(veeConfigs);
        }

        public VeeConfigsWithReality(string config) : this()
        {
            try
            {
                var slinkMgr = Services.ShareLinkMgr.Instance;
                var vee = slinkMgr.EncodeConfigToShareLink(config, VgcApis.Models.Datas.Enums.LinkTypes.v);
                if (vee == null)
                {
                    return;
                }
                var veeCodec = slinkMgr.GetCodec<Services.ShareLinkComponents.VeeDecoder>();
                var vc = veeCodec.VeeLink2VeeConfig(vee);
                if (vc != null)
                {
                    CopyFrom(vc);
                }
            }
            catch { }
        }

        public new string ToVeeShareLink()
        {
            try
            {
                var slinkMgr = Services.ShareLinkMgr.Instance;
                var veeCodec = slinkMgr.GetCodec<Services.ShareLinkComponents.VeeDecoder>();
                return veeCodec.VeeConfig2VeeLink(this);
            }
            catch { }
            return "";
        }

        #region private methods
        protected void CopyFrom(VeeConfigsWithReality source)
        {
            base.CopyFrom(source);

            tlsAlpn = source.tlsAlpn;
            tlsFingerPrint = source.tlsFingerPrint;
            tlsParam1 = source.tlsParam1;
            tlsParam2 = source.tlsParam2;
            tlsParam3 = source.tlsParam3;
        }

        #endregion

    }
}
