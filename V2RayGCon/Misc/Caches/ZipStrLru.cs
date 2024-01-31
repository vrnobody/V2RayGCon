using VgcApis.Libs.Infr;

namespace V2RayGCon.Misc.Caches
{
    internal static class ZipStrLru
    {
        public static readonly int capacity = 20;
        public static readonly int minSize = 96 * 1024;
        static readonly LRUCache<string, Node> cache = new LRUCache<string, Node>(capacity);

        static ZipStrLru() { }

        #region public methods
        static public bool Put(string key, string content)
        {
            if (content.Length < minSize)
            {
                return false;
            }

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
            return true;
        }

        public static bool TryRemove(string key)
        {
            key = HashOnDemand(key);
            return cache.Remove(key);
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
