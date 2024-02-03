using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace VgcApis.Misc
{
    public static class RecycleBin
    {
        static readonly BlockingCollection<RecycleBinHashNode> rmQueue =
            new BlockingCollection<RecycleBinHashNode>();
        public static readonly TimeSpan timeout = TimeSpan.FromSeconds(10);

        static ConcurrentDictionary<string, object> cache =
            new ConcurrentDictionary<string, object>();

        static RecycleBin()
        {
            Task.Delay(500).ContinueWith(_ => Recycle()).ConfigureAwait(false);
        }

        #region public methods
        public static int GetSize() => cache.Count;

        public static int GetRecycleQueueLength() => rmQueue.Count;

        public static void Put(string key, object o)
        {
            var hash = Utils.Sha256Hex(key);
            cache.AddOrUpdate(hash, o, (_, __) => o);
            rmQueue.Add(new RecycleBinHashNode(hash, timeout));
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
            var ok = cache.TryRemove(hash, out var v);
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
                        if (cache.TryRemove(node.hash, out var _))
                        {
                            Logger.Debug($"recyclebin drop slow");
                        }
                        Recycle();
                    })
                    .ConfigureAwait(false);
                return;
            }

            if (cache.TryRemove(node.hash, out var _))
            {
                Logger.Debug($"recyclebin drop fast");
            }
            Recycle();
        }
        #endregion
    }

    struct RecycleBinHashNode
    {
        public readonly string hash;
        public readonly DateTime expired;

        public RecycleBinHashNode(string hash, TimeSpan timeout)
        {
            this.hash = hash;
            this.expired = DateTime.Now.Add(timeout);
        }
    }
}
