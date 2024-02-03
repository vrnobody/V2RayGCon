using System;
using System.Collections.Generic;

namespace VgcApis.Libs.Infr
{
    // https://stackoverflow.com/questions/754233/is-it-there-any-lru-implementation-of-idictionary
    public class LRUCache<K, V>
    {
        public readonly int capacity;

        Dictionary<K, LinkedListNode<LRUCacheItem<K, V>>> cacheMap =
            new Dictionary<K, LinkedListNode<LRUCacheItem<K, V>>>();
        LinkedList<LRUCacheItem<K, V>> lruList = new LinkedList<LRUCacheItem<K, V>>();

        readonly object locker = new object();

        public LRUCache(int capacity)
        {
            if (capacity < 1)
            {
                throw new ArgumentOutOfRangeException("Capacity must larger than 1");
            }
            this.capacity = capacity;
        }

        #region public methods
        public int GetSize() => cacheMap.Count;

        public bool TryGet(K key, out V value)
        {
            lock (locker)
            {
                LinkedListNode<LRUCacheItem<K, V>> node;
                if (cacheMap.TryGetValue(key, out node))
                {
                    value = node.Value.value;
                    lruList.Remove(node);
                    lruList.AddLast(node);
                    return true;
                }
                value = default;
                return false;
            }
        }

        public void Add(K key, V val)
        {
            lock (locker)
            {
                if (cacheMap.TryGetValue(key, out var existingNode))
                {
                    lruList.Remove(existingNode);
                }
                else if (cacheMap.Count >= capacity)
                {
                    RemoveFirst();
                }

                LRUCacheItem<K, V> cacheItem = new LRUCacheItem<K, V>(key, val);
                LinkedListNode<LRUCacheItem<K, V>> node = new LinkedListNode<LRUCacheItem<K, V>>(
                    cacheItem
                );
                lruList.AddLast(node);
                // cacheMap.Add(key, node); - here's bug if try to add already existing value
                cacheMap[key] = node;
            }
        }

        public bool Remove(K key)
        {
            lock (locker)
            {
                if (cacheMap.TryGetValue(key, out var existingNode))
                {
                    cacheMap.Remove(key);
                    lruList.Remove(existingNode);
                    return true;
                }
                return false;
            }
        }
        #endregion

        #region private methods

        void RemoveFirst()
        {
            // Remove from LRUPriority
            LinkedListNode<LRUCacheItem<K, V>> node = lruList.First;
            lruList.RemoveFirst();

            // Remove from cache
            cacheMap.Remove(node.Value.key);
        }
        #endregion
    }

    class LRUCacheItem<K, V>
    {
        public LRUCacheItem(K k, V v)
        {
            key = k;
            value = v;
        }

        public K key;
        public V value;
    }
}
