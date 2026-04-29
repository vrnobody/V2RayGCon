using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Newtonsoft.Json;

namespace VgcApis.Models.Datas
{
    public class InboundInfo
    {
        #region props

        string _protocol = "http";

        [DefaultValue("http")]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string protocol
        {
            get { return _protocol; }
            private set { _protocol = Libs.Infr.StringRefCache.Ref(value); }
        }

        string _host = Consts.Webs.LoopBackIP;

        [DefaultValue("127.0.0.1")]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string host
        {
            get { return _host; }
            private set { _host = Libs.Infr.StringRefCache.Ref(value); }
        }

        // all ports: "1080,3000,8080-8888"
        string _ports = "8080";

        [DefaultValue("8080")]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string ports
        {
            get { return _ports; }
            private set { _ports = Libs.Infr.StringRefCache.Ref(value); }
        }

        // first port: 1080
        int _port = Consts.Webs.DefaultProxyPort;

        [DefaultValue(8080)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int port
        {
            get { return _port; }
            private set { _port = value; }
        }

        #endregion
        // json serializer requires this ctor!!
        public InboundInfo() { }

        public InboundInfo(string protocol, string host, string ports)
        {
            this.protocol = protocol;
            this.host = host;
            this.ports = ports;

            if (!string.IsNullOrEmpty(ports))
            {
                var first = this
                    .ports.Split(
                        new char[] { ',', ' ', '-' },
                        System.StringSplitOptions.RemoveEmptyEntries
                    )
                    .FirstOrDefault();
                this.port = Misc.Utils.Str2Int(first);
            }
        }

        public override string ToString()
        {
            var s = string.IsNullOrEmpty(ports)
                ? $"{protocol}://{host}:{port}"
                : $"{protocol}://{host}:{ports}";
            return Libs.Infr.StringRefCache.Ref(s);
        }

        #region private methods

        #endregion
    }
}
