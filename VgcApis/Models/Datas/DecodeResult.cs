using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VgcApis.Models.Datas
{
    public class DecodeResult
    {
        public string name;
        public string config;

        public DecodeResult(string name, string config)
        {
            this.config = config;
            this.name = name;
        }

        public DecodeResult()
            : this(string.Empty, string.Empty) { }
    }
}
