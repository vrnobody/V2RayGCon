using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Luna.Models.Apis.SysCmpos
{
    public class SnapCache : VgcApis.BaseClasses.Disposable
    {
        readonly static ConcurrentDictionary<string, ConcurrentDictionary<string, object>> cache = new ConcurrentDictionary<string, ConcurrentDictionary<string, object>>();
        string token = null;
        readonly object locker = new object();

        public SnapCache()
        { }

        #region properties

        #endregion

        #region public methods
        public string GetToken()
        {
            lock (locker)
            {
                if (string.IsNullOrEmpty(token))
                {
                    var handle = Guid.NewGuid().ToString();
                    var dict = new ConcurrentDictionary<string, object>();
                    if (cache.TryAdd(handle, dict))
                    {
                        token = handle;
                    }
                }
            }
            return token;
        }

        public bool Set(string token, string key, object value)
        {
            if (!string.IsNullOrEmpty(token)
                && !string.IsNullOrEmpty(key)
                && cache.TryGetValue(token, out var dict))
            {
                dict.AddOrUpdate(key, value, (k, v) => value);
                return true;
            }
            return false;
        }

        public object Get(string token, string key)
        {
            if (!string.IsNullOrEmpty(token)
                && !string.IsNullOrEmpty(key)
                && cache.TryGetValue(token, out var dict)
                && dict != null
                && dict.TryGetValue(key, out var value))
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
        protected override void Cleanup()
        {
            if (!string.IsNullOrEmpty(token))
            {
                cache.TryRemove(token, out _);
            }
        }
        #endregion
    }
}