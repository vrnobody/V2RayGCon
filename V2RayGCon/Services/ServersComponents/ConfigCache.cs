using System.Collections.Concurrent;
using System.Linq;

namespace V2RayGCon.Services.ServersComponents
{
    internal class ConfigCache
    {
        readonly ConcurrentDictionary<string, string> cache =
            new ConcurrentDictionary<string, string>();

        public ConfigCache() { }

        #region public methods
        public void Clear() => cache.Clear();

        public bool ContainsKey(string config)
        {
            var hash = GetHash(config);
            return cache.ContainsKey(hash);
        }

        public bool TryUpdate(string uid, string newConfig)
        {
            var key = cache.FirstOrDefault(kv => kv.Value == uid).Key;
            if (!string.IsNullOrEmpty(key))
            {
                cache.TryRemove(key, out _);
            }

            var hash = GetHash(newConfig);
            return cache.TryAdd(hash, uid);
        }

        public bool TryAdd(string config, string uid)
        {
            if (string.IsNullOrEmpty(config))
            {
                return false;
            }
            var hash = GetHash(config);
            return cache.TryAdd(hash, uid);
        }

        public bool TryRemove(string config, out string uid)
        {
            var hash = GetHash(config);
            return cache.TryRemove(hash, out uid);
        }

        public bool TryGetValue(string config, out string uid)
        {
            var hash = GetHash(config);
            return cache.TryGetValue(hash, out uid);
        }
        #endregion

        #region private methods
        string GetHash(string config)
        {
            if (string.IsNullOrEmpty(config))
            {
                return string.Empty;
            }
            return VgcApis.Misc.Utils.Sha256Hex(config);
        }

        #endregion

        #region protected methods

        #endregion
    }
}
