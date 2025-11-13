using System.Collections.Generic;

namespace V2RayGCon.Models.Datas
{
    public class ServerTracker
    {
        public bool isTrackerOn;
        public List<string> uids;

        public ServerTracker()
        {
            isTrackerOn = true;
            uids = new List<string>();
        }
    }
}
