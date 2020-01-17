using System.Collections.Generic;

namespace V2RayGCon.Model.Data
{
    public class ServerTracker
    {
        public bool isTrackerOn;
        public List<string> serverList;
        public string curServer;

        public ServerTracker()
        {
            isTrackerOn = true;
            serverList = new List<string>();
            curServer = string.Empty;
        }
    }
}
