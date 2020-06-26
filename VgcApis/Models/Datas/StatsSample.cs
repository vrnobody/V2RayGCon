using System;

namespace VgcApis.Models.Datas
{
    public class StatsSample
    {
        public long stamp { get; set; }
        public long statsUplink { get; set; }
        public long statsDownlink { get; set; }

        public StatsSample(string upLink, string downLink)
            : this(Misc.Utils.Str2Int(upLink), Misc.Utils.Str2Int(downLink))
        { }

        public StatsSample(long upLink, long downLink)
        {
            stamp = DateTime.Now.Ticks;
            statsUplink = upLink;
            statsDownlink = downLink;
        }

        public StatsSample() : this(0, 0)
        { }
    }
}
