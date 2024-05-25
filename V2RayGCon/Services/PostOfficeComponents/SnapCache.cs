using System;
using System.Collections.Concurrent;

namespace V2RayGCon.Services.PostOfficeComponents
{
    public class SnapCache
    {
        static readonly ConcurrentDictionary<string, ConcurrentDictionary<string, object>> cache =
            new ConcurrentDictionary<string, ConcurrentDictionary<string, object>>();

        public SnapCache() { }

        #region properties

        #endregion

        #region public methods
        public bool RemoveCache(string token)
        {
            if (!string.IsNullOrEmpty(token))
            {
                return cache.TryRemove(token, out var _);
            }
            return false;
        }

        public bool CreateCache(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return false;
            }
            var dict = new ConcurrentDictionary<string, object>();
            cache.TryAdd(token, dict);
            return true;
        }

        public string ApplyNewCache()
        {
            var token = Guid.NewGuid().ToString();
            var dict = new ConcurrentDictionary<string, object>();
            if (cache.TryAdd(token, dict))
            {
                return token;
            }
            return null;
        }

        public bool Set(string token, string key, object value)
        {
            if (
                !string.IsNullOrEmpty(token)
                && !string.IsNullOrEmpty(key)
                && cache.TryGetValue(token, out var dict)
            )
            {
                dict.AddOrUpdate(key, value, (k, v) => value);
                return true;
            }
            return false;
        }

        public object Get(string token, string key)
        {
            if (
                !string.IsNullOrEmpty(token)
                && !string.IsNullOrEmpty(key)
                && cache.TryGetValue(token, out var dict)
                && dict != null
                && dict.TryGetValue(key, out var value)
            )
            {
                return value;
            }
            return null;
        }

        #endregion

        #region private methods

        #endregion

        #region protected methods
        #endregion

        #region protected override
        #endregion
    }
}
