using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace VgcApis.Libs.Infr
{
    // https://stackoverflow.com/questions/754233/is-it-there-any-lru-implementation-of-idictionary
    public class LRUCache<K, V>
    {
        private int capacity;
        private Dictionary<K, LinkedListNode<LRUCacheItem<K, V>>> cacheMap =
            new Dictionary<K, LinkedListNode<LRUCacheItem<K, V>>>();
        private LinkedList<LRUCacheItem<K, V>> lruList = new LinkedList<LRUCacheItem<K, V>>();

        public LRUCache(int capacity)
        {
            if (capacity < 2)
            {
                throw new ArgumentOutOfRangeException("Capacity must bigger than 1");
            }
            this.capacity = capacity;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public bool TryGet(K key, out V value)
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

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Add(K key, V val)
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

        [MethodImpl(MethodImplOptions.Synchronized)]
        public bool Remove(K key)
        {
            if (cacheMap.TryGetValue(key, out var existingNode))
            {
                cacheMap.Remove(key);
                lruList.Remove(existingNode);
                return true;
            }
            return false;
        }

        private void RemoveFirst()
        {
            // Remove from LRUPriority
            LinkedListNode<LRUCacheItem<K, V>> node = lruList.First;
            lruList.RemoveFirst();

            // Remove from cache
            cacheMap.Remove(node.Value.key);
        }
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
