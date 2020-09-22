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

        /* plan to delete
        public VeeConfigs(JObject config) : this(config.ToString())
        {

        }
        */

        public string ToVeeShareLink()
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
        void CopyFrom(VeeConfigs source)
        {
            name = source.name;
            description = source.description;
            proto = source.proto;
            host = source.host;
            port = source.port;
            auth1 = source.auth1;
            auth2 = source.auth2;
            method = source.method;
            useOta = source.useOta;
            useSelfSignCert = source.useSelfSignCert;
            useTls = source.useTls;
            streamType = source.streamType;
            streamParam1 = source.streamParam1;
            streamParam2 = source.streamParam2;
            streamParam3 = source.streamParam3;
        }

        #endregion

    }
}
