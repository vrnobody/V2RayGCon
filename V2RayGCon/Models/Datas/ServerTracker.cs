using System.Collections.Generic;

namespace V2RayGCon.Models.Datas
{
    public class ServerTracker
    {
        public bool isTrackerOn;
        public List<string> serverList;
        public List<string> uids;
        public string curServer;

        public ServerTracker()
        {
            isTrackerOn = true;
            serverList = new List<string>();
            uids = new List<string>();
            curServer = string.Empty;
        }
    }
}
