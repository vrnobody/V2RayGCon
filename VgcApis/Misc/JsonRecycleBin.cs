using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace VgcApis.Misc
{
    public static class JsonRecycleBin
    {
        static readonly TimeSpan delay = TimeSpan.FromSeconds(10);

        static ConcurrentDictionary<string, JObject> recyclebin =
            new ConcurrentDictionary<string, JObject>();

        public static void Put(string json, JObject o)
        {
            var hash = Utils.Sha256Hex(json);
            recyclebin[hash] = o;
            Task.Delay(delay).ContinueWith(_ => recyclebin.TryRemove(hash, out var _));
        }

        public static bool TryTake(string json, out JObject o)
        {
            var hash = Utils.Sha256Hex(json);
            return recyclebin.TryRemove(hash, out o);
        }
    }
}
