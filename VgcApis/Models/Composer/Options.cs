using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VgcApis.Models.Composer
{
    public class Options
    {
        public string skelecton = "";
        public bool isAppend = true;
        public bool isProxyChain = false;
        public List<Selector> selectors = new List<Selector>();
    }
}
