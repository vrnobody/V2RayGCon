using System.Collections.Concurrent;
using System.Linq;

namespace V2RayGCon.Services.ServersComponents
{
    internal class ConfigCache
    {
        readonly ConcurrentDictionary<string, VgcApis.Interfaces.ICoreServCtrl> cache =
            new ConcurrentDictionary<string, VgcApis.Interfaces.ICoreServCtrl>();

        public ConfigCache() { }

        #region public methods
        public void Clear() => cache.Clear();

        public bool ContainsKey(string config)
        {
            // forbid empty config!
            if (string.IsNullOrEmpty(config))
            {
                return true;
            }
            var hash = GetHash(config);
            return cache.ContainsKey(hash);
        }

        public bool TryUpdate(VgcApis.Interfaces.ICoreServCtrl coreServ, string newConfig)
        {
            var key = cache.FirstOrDefault(kv => kv.Value == coreServ).Key;
            if (!string.IsNullOrEmpty(key))
            {
                cache.TryRemove(key, out _);
            }

            var hash = GetHash(newConfig);
            return cache.TryAdd(hash, coreServ);
        }

        public bool TryAdd(string config, VgcApis.Interfaces.ICoreServCtrl coreServ)
        {
            if (string.IsNullOrEmpty(config))
            {
                return false;
            }
            var hash = GetHash(config);
            return cache.TryAdd(hash, coreServ);
        }

        public bool TryRemove(string config, out VgcApis.Interfaces.ICoreServCtrl coreServ)
        {
            var hash = GetHash(config);
            return cache.TryRemove(hash, out coreServ);
        }

        public bool TryGetValue(string config, out VgcApis.Interfaces.ICoreServCtrl coreServ)
        {
            var hash = GetHash(config);
            return cache.TryGetValue(hash, out coreServ);
        }
        #endregion

        #region private methods
        string GetHash(string config)
        {
            if (string.IsNullOrEmpty(config))
            {
                return string.Empty;
            }
            var bytes = VgcApis.Misc.Utils.Md5Hash(config);
            var b64 = VgcApis.Misc.Utils.Base64EncodeBytes(bytes);
            return b64;
        }

        #endregion
    }
}
