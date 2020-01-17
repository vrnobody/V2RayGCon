using System;

namespace VgcApis.Models.Datas
{
    public class StatsSample
    {
        public long stamp { get; set; }
        public int statsUplink { get; set; }
        public int statsDownlink { get; set; }

        public StatsSample(string upLink, string downLink)
            : this(Libs.Utils.Str2Int(upLink),
                  Libs.Utils.Str2Int(downLink))
        { }

        public StatsSample(int upLink, int downLink)
        {
            stamp = DateTime.Now.Ticks;
            statsUplink = upLink;
            statsDownlink = downLink;
        }

        public StatsSample() : this(0, 0)
        { }
    }
}
