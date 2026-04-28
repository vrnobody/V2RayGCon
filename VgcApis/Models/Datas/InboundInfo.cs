using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Newtonsoft.Json;

namespace VgcApis.Models.Datas
{
    public class InboundInfo
    {
        static readonly List<string> caches = new List<string>();

        [DefaultValue("http")]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string protocol = "http";

        [DefaultValue("127.0.0.1")]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string host = Consts.Webs.LoopBackIP;

        // all ports: "1080,3000,8080-8888"
        [DefaultValue("8080")]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string ports = "8080";

        // first port: 1080
        [DefaultValue(8080)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int port = Consts.Webs.DefaultProxyPort;

        #region props

        public int GetPort() => port;

        public string GetProtocol() => protocol;

        public string GetHost() => host;

        #endregion


        public InboundInfo(string protocol, string host, string ports)
        {
            this.protocol = RefFromCache(protocol);
            this.host = RefFromCache(host);
            this.ports = RefFromCache(ports);

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
            return RefFromCache(s);
        }

        #region private methods
        string RefFromCache(string content)
        {
            if (string.IsNullOrEmpty(content))
            {
                return "";
            }

            lock (caches)
            {
                var idx = caches.IndexOf(content);
                if (idx < 0)
                {
                    caches.Add(content);
                    if (caches.Count > 500)
                    {
                        while (caches.Count > 250)
                        {
                            caches.RemoveAt(0);
                        }
                    }
                    return content;
                }
                return caches[idx];
            }
        }
        #endregion
    }
}
