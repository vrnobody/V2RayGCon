using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoLuna.Models.Data
{
    public class ServerSelectorDTO
    {
        public string tag = "";
        public string filter = "";
        public Dictionary<string, string> uids = new Dictionary<string, string>();

        public VgcApis.Models.Composer.Selector ToSelector()
        {
            var r = new VgcApis.Models.Composer.Selector() { tag = tag, filter = filter };
            foreach (var kv in uids)
            {
                r.uids.Add(kv.Value.ToString());
            }
            return r;
        }
    }
}
