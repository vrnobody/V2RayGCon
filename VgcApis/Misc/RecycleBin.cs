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
    public static class RecycleBin
    {
        public static readonly int minKeySize = 128 * 1024;
        public static readonly TimeSpan timeout = TimeSpan.FromSeconds(10);

        static ConcurrentDictionary<string, object> cache =
            new ConcurrentDictionary<string, object>();

        public static void Put(string key, object o)
        {
            if (key.Length < minKeySize)
            {
                return;
            }

            var hash = Utils.Sha256Hex(key);
            cache[hash] = o;
            Task.Delay(timeout).ContinueWith(_ => cache.TryRemove(hash, out var _));
        }

        public static bool TryTake<T>(string key, out T o)
            where T : class
        {
            if (key.Length <= minKeySize)
            {
                o = null;
                return false;
            }
            var hash = Utils.Sha256Hex(key);
            var ok = cache.TryRemove(hash, out var v);
            o = v as T;
            return ok && o != null;
        }
    }
}
