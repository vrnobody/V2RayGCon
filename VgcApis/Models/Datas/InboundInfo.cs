using System.Linq;

namespace VgcApis.Models.Datas
{
    public class InboundInfo
    {
        public string protocol;
        public string host;

        // all ports: "1080,3000,8080-8888"
        public string ports;

        // first port: 1080
        public int port;

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
            if (string.IsNullOrEmpty(ports))
            {
                return $"{protocol}://{host}:{port}";
            }
            return $"{protocol}://{host}:{ports}";
        }
    }
}
