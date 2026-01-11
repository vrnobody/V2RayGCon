using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VgcApis.Models.Datas.Composer
{
    public class Selector
    {
        public string tag = "";
        public string filter = "";
        public List<string> uids = new List<string>();
    }

    public class Options
    {
        public string skelecton = "";
        public bool isAppend = true;
        public List<Selector> selectors = new List<Selector>();
    }
}
