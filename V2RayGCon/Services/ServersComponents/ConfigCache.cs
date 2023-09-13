using System.Collections.Concurrent;

namespace V2RayGCon.Services.ServersComponents
{
    internal class ConfigCache
    {
        ConcurrentDictionary<string, string> cache = new ConcurrentDictionary<string, string>();

        public ConfigCache() { }

        #region properties

        #endregion

        #region public methods
        public void Clear() => cache.Clear();

        public bool ContainsKey(string config)
        {
            var hash = GetHash(config);
            return cache.ContainsKey(hash);
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
            var hash = VgcApis.Misc.Utils.Sha256Hash(config);
            return VgcApis.Misc.Utils.ToHexString(hash);
        }

        #endregion

        #region protected methods

        #endregion
    }
}
