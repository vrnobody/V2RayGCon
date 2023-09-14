using Newtonsoft.Json.Linq;

namespace V2RayGCon.Models.Datas
{
    public class SharelinkMetadata
    {
        public string name = string.Empty;
        public string description = string.Empty;
        public string proto = string.Empty;
        public string host = string.Empty;
        public int port = 0;
        public string auth1 = string.Empty;
        public string auth2 = string.Empty;

        public string streamType = string.Empty;
        public string streamParam1 = string.Empty;
        public string streamParam2 = string.Empty;
        public string streamParam3 = string.Empty;

        public string tlsType = @"none";
        public string tlsServName = @"";
        public bool useSelfSignCert = false;

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

        public SharelinkMetadata() { }

        public SharelinkMetadata(string config)
            : this()
        {
            try
            {
                var json = JObject.Parse(config);
                var src = Services.ShareLinkComponents.Comm.ExtractFromJsonConfig(json);
                CopyFrom(src);
            }
            catch { }
        }

        #region private methods
        protected void CopyFrom(SharelinkMetadata source)
        {
            name = source.name;
            description = source.description;
            proto = source.proto;
            host = source.host;
            port = source.port;
            auth1 = source.auth1;
            auth2 = source.auth2;

            streamType = source.streamType;
            streamParam1 = source.streamParam1;
            streamParam2 = source.streamParam2;
            streamParam3 = source.streamParam3;

            tlsType = source.tlsType;
            useSelfSignCert = source.useSelfSignCert;
            tlsServName = source.tlsServName;

            tlsAlpn = source.tlsAlpn;
            tlsFingerPrint = source.tlsFingerPrint;
            tlsParam1 = source.tlsParam1;
            tlsParam2 = source.tlsParam2;
            tlsParam3 = source.tlsParam3;
        }

        #endregion
    }
}
