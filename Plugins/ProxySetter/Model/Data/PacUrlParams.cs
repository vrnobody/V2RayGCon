namespace ProxySetter.Model.Data
{
    public class PacUrlParams
    {
        public string ip, mime;
        public int port;
        public bool isSocks, isWhiteList, isDebug;

        // (string ip, int port, bool isSocks, bool isWhiteList)
        public PacUrlParams()
        {
            mime = "html";  // html,js,pac default pac
            ip = VgcApis.Models.Consts.Webs.LoopBackIP;
            isSocks = false;
            isWhiteList = true;
            isDebug = false;
            port = 1080;
        }

        #region public method
        #endregion
    }
}
