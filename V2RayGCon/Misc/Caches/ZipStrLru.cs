using System.Web.UI.WebControls;
using VgcApis.Libs.Infr;

namespace V2RayGCon.Misc.Caches
{
    internal static class ZipStrLru
    {
        // only final config editor will use this cache
        // not very useful 2024-01-31
        public static readonly int capacity = 20;

#if DEBUG
        public static readonly int minSize = 2 * 1024;
#else
        public static readonly int minSize = 128 * 1024;
#endif
        static readonly LRUCache<string, Node> cache = new LRUCache<string, Node>(capacity);

        static ZipStrLru() { }

        #region public methods
        static public bool Put(string key, string content)
        {
            if (content.Length < minSize)
            {
                return false;
            }

            VgcApis.Misc.Logger.Debug($"ZipStrLru cache: {key} {content.Length / 1024} KiB");
            var zipped = false;
            if (!ZipExtensions.IsCompressedBase64(content))
            {
                zipped = true;
                content = ZipExtensions.CompressToBase64(content);
            }

            key = HashOnDemand(key);
            cache.Add(key, new Node(zipped, content));
            return true;
        }

        public static bool TryGet(string key, out string content)
        {
            key = HashOnDemand(key);
            if (!cache.TryGet(key, out var node))
            {
                content = "";
                return false;
            }
            if (node.isCompressed)
            {
                content = ZipExtensions.DecompressFromBase64(node.content);
            }
            else
            {
                content = node.content;
            }
            VgcApis.Misc.Logger.Debug($"ZipStrLru reuse cache: {key}");
            return true;
        }

        public static bool TryRemove(string key)
        {
            key = HashOnDemand(key);
            if (cache.Remove(key))
            {
                VgcApis.Misc.Logger.Debug($"ZipStrLru remove cache : {key}");
                return true;
            }
            return false;
        }
        #endregion

        #region private methods
        static string HashOnDemand(string key)
        {
            if (!string.IsNullOrEmpty(key) && key.Length > 1024)
            {
                return VgcApis.Misc.Utils.Sha256Hex(key);
            }
            return key ?? "";
        }

        struct Node
        {
            public readonly bool isCompressed;
            public readonly string content;

            public Node(bool isCompressed, string content)
            {
                this.isCompressed = isCompressed;
                this.content = content;
            }
        }
        #endregion
    }
}
