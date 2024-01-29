namespace VgcApis.Libs.Infr
{
    public class StringLruCache<V> : LRUCache<string, V>
    {
        public StringLruCache()
            : this(30) { }

        public StringLruCache(int capacity)
            : base(capacity) { }

        public new bool TryGet(string key, out V value)
        {
            var k = Misc.Utils.Md5Hex(key);
            return base.TryGet(k, out value);
        }

        public new void Add(string key, V val)
        {
            var k = Misc.Utils.Md5Hex(key);
            base.Add(k, val);
        }

        public new bool Remove(string key)
        {
            var k = Misc.Utils.Md5Hex(key);
            return base.Remove(k);
        }
    }
}
