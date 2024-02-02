using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace VgcApis.Misc
{
    public static class RecycleBin
    {
#if DEBUG
        public static readonly int minSize = 1 * 1024;
#else
        public static readonly int minSize = 4 * 1024;
#endif

        public static readonly int capacity = 30;

        static readonly BlockingCollection<HashNode> rmQueue = new BlockingCollection<HashNode>();
        public static readonly TimeSpan timeout = TimeSpan.FromSeconds(10);

        static Libs.Infr.LRUCache<string, object> cache = new Libs.Infr.LRUCache<string, object>(
            capacity
        );

        static RecycleBin()
        {
            Task.Delay(500).ContinueWith(_ => Recycle()).ConfigureAwait(false);
        }

        #region public methods
        public static void Put(string key, object o)
        {
            // disable this function
            // return;

            if (string.IsNullOrEmpty(key) || key.Length < minSize)
            {
                return;
            }
            var hash = Utils.Sha256Hex(key);
            cache.Add(hash, o);
            rmQueue.Add(new HashNode(hash, timeout));
        }

        public static JObject Parse(string config)
        {
            if (TryTake<JObject>(config, out var json))
            {
                return json;
            }
            Logger.Debug($"recyclebin parse new json {config.Length / 1024} KiB");
            return Utils.ParseJObject(config);
        }

        public static bool TryTake<T>(string config, out T o)
            where T : class
        {
            var hash = Utils.Sha256Hex(config);
            var ok = cache.TryGet(hash, out var v);
            cache.Remove(hash);
            o = v as T;
            if (ok)
            {
                Logger.Debug($"recyclebin reuse json {config.Length / 1024} KiB");
            }
            return ok && o != null;
        }
        #endregion

        #region private methods
        static void Recycle()
        {
            var node = rmQueue.Take();
            var diff = node.expired.Subtract(DateTime.Now).TotalMilliseconds;
            var delay = (int)(Math.Max(diff, 0));

            if (delay > 800)
            {
                Task.Delay(delay)
                    .ContinueWith(_ =>
                    {
                        if (cache.Remove(node.hash))
                        {
                            Logger.Debug($"recyclebin drop slow");
                        }
                        Recycle();
                    })
                    .ConfigureAwait(false);
                return;
            }

            if (cache.Remove(node.hash))
            {
                Logger.Debug($"recyclebin drop fast");
            }
            Recycle();
        }
        #endregion
    }

    struct HashNode
    {
        public readonly string hash;
        public readonly DateTime expired;

        public HashNode(string hash, TimeSpan timeout)
        {
            this.hash = hash;
            this.expired = DateTime.Now.Add(timeout);
        }
    }
}
