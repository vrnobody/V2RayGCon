namespace VgcApis.Models.Datas
{
    public class InboundInfo
    {
        public string protocol;
        public string host;
        public int port;

        public InboundInfo(string protocol, string host, int port)
        {
            this.protocol = protocol;
            this.host = host;
            this.port = port;
        }

        public override string ToString()
        {
            return $"{protocol}://{host}:{port}";
        }
    }
}
