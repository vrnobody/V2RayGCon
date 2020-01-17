namespace ProxySetter.Model.Data
{

    public class CustomPacHeader
    {
        public string protocol, mode, customWhite, customBlack, ip;
        public int port;

        public CustomPacHeader(
            PacUrlParams urlParam,
            string customWhiteList,
            string customBlackList)

            : this(urlParam.isSocks,
                urlParam.isWhiteList,
                urlParam.ip,
                urlParam.port,
                customWhiteList,
                customBlackList)
        { }

        public CustomPacHeader(
            bool isSocks,
            bool isWhiteList,
            string ip,
            int port,
            string customWhiteList,
            string customBlackList)
        {
            this.protocol = isSocks ? "socks" : "http";
            this.mode = isWhiteList ? "white" : "black";
            this.customWhite = customWhiteList;
            this.customBlack = customBlackList;
            this.ip = ip;
            this.port = port;
        }

    }
}
