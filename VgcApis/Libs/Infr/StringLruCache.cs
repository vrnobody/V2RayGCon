using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using VgcApis.Misc;

namespace VgcApis.Libs.Infr
{
    public class StringLruCache<V>
    {
        public readonly TimeSpan timeout;
        public readonly int capacity;

        readonly BlockingCollection<Node> rmQueue = new BlockingCollection<Node>();
        readonly bool autoRemove = false;
        readonly LRUCache<string, V> cache;

        public StringLruCache()
            : this(30, TimeSpan.MinValue) { }

        public StringLruCache(int capacity, TimeSpan timeout)
        {
            this.capacity = capacity;
            cache = new LRUCache<string, V>(capacity);
            this.timeout = timeout;
            if (timeout > TimeSpan.Zero)
            {
                autoRemove = true;
                Task.Delay(500).ContinueWith(_ => CollectMem()).ConfigureAwait(false);
            }
        }

        #region public methods
        public bool TryGet(string key, out V value)
        {
            var k = Utils.Md5Hex(key);
            return cache.TryGet(k, out value);
        }

        public void Add(string key, V val)
        {
            var k = Utils.Md5Hex(key);
            if (autoRemove)
            {
                rmQueue.Add(new Node(k, timeout));
            }
            cache.Add(k, val);
        }

        public bool Remove(string key)
        {
            var k = Utils.Md5Hex(key);
            return cache.Remove(k);
        }
        #endregion

        #region private methods
        void CollectMem()
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
                            Logger.Debug($"drop slow");
                        }
                        CollectMem();
                    })
                    .ConfigureAwait(false);
                return;
            }

            if (cache.Remove(node.hash))
            {
                Logger.Debug($"drop fast");
            }
            CollectMem();
        }
        #endregion
    }

    struct Node
    {
        public readonly string hash;
        public readonly DateTime expired;

        public Node(string hash, TimeSpan timeout)
        {
            this.hash = hash;
            this.expired = DateTime.Now.Add(timeout);
        }
    }
}
