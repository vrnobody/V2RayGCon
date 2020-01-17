using System.Collections.Generic;

namespace Statistics.Models
{
    public class UserSettings
    {
        public Dictionary<string, StatsResult> statsData;

        public UserSettings()
        {
            statsData = new Dictionary<string, StatsResult>();
        }
    }
}
