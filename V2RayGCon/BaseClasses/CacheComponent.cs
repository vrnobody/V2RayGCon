﻿using System.Collections.Generic;
using System.Linq;

namespace V2RayGCon.BaseClasses
{
    public class CacheComponent<TKey, TValue> : ICacheComponent<TKey, TValue>
    {
        public object writeLock;
        public Dictionary<TKey, Models.Datas.LockValuePair<TValue>> data;

        public CacheComponent()
        {
            writeLock = new object();
            Clear();
        }

        #region public method
        public void TrimHalf(int maxSize)
        {
            lock (writeLock)
            {
                var count = data.Keys.Count;
                if (count < maxSize)
                {
                    return;
                }

                var keys = VgcApis.Misc.Utils.Shuffle(data.Keys).Take(count / 2).ToList();
                for (var i = 0; i < keys.Count; i++)
                {
                    if (data.ContainsKey(keys[i]))
                    {
                        data.Remove(keys[i]);
                    }
                }
            }
        }

        public bool ContainsKey(TKey key)
        {
            return data.ContainsKey(key);
        }

        public int Count
        {
            get => data.Count;
        }

        public TKey[] Keys
        {
            get => data.Keys.ToArray();
        }

        public void Remove(List<TKey> keys)
        {
            lock (writeLock)
            {
                foreach (var key in keys)
                {
                    if (data.ContainsKey(key))
                    {
                        lock (data[key].rwLock)
                        {
                            data.Remove(key);
                        }
                    }
                }
            }
        }

        public void Clear()
        {
            lock (writeLock)
            {
                data = new Dictionary<TKey, Models.Datas.LockValuePair<TValue>>();
            }
        }

        public TValue this[TKey key]
        {
            get
            {
                lock (data[key].rwLock)
                {
                    return data[key].content;
                }
            }
            set
            {
                lock (writeLock)
                {
                    if (!data.ContainsKey(key))
                    {
                        data[key] = new Models.Datas.LockValuePair<TValue>();
                    }
                    data[key].content = value;
                }
            }
        }
        #endregion

        #region private method
        #endregion
    }
}
