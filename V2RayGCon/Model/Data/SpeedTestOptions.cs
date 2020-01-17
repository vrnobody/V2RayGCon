using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace V2RayGCon.Model.Data
{
    public class SpeedTestOptions
    {
        // FormOption->Defaults->Speedtest
        public string Url { get; set; }
        public bool IsUse { get; set; }
        public int Cycles { get; set; }
        public int ExpectedSize { get; set; }
        public int Timeout { get; set; }

        public SpeedTestOptions()
        {
            Url = VgcApis.Models.Consts.Webs.GoogleDotCom;
            IsUse = false;
            Cycles = 3;
            ExpectedSize = 0;
            Timeout = VgcApis.Models.Consts.Intervals.SpeedTestTimeout;
        }

        public bool Equals(SpeedTestOptions target)
        {
            if (target == null
                || !Url.Equals(target.Url)
                || IsUse != target.IsUse
                || Cycles != target.Cycles
                || ExpectedSize != target.ExpectedSize
                || Timeout != target.Timeout)
            {
                return false;
            }
            return true;
        }
    }
}
