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
            var k = Md5(key);
            return base.TryGet(k, out value);
        }

        public new void Add(string key, V val)
        {
            var k = Md5(key);
            base.Add(k, val);
        }

        public new bool Remove(string key)
        {
            var k = Md5(key);
            return base.Remove(k);
        }

        private string Md5(string str)
        {
            var b = Misc.Utils.Md5Hash(str);
            return Misc.Utils.ToHexString(b);
        }
    }
}
