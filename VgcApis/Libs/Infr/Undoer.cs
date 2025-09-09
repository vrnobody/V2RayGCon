namespace VgcApis.Libs.Infr
{
    public class Undoer<T>
    {
        private readonly object locker = new object();
        private readonly int capacity;
        private readonly T[] cache;
        int head = 0;
        int tail = 0;
        int cur = 0;

        public Undoer()
            : this(128) { }

        /// <summary>
        /// [2, 1024)
        /// </summary>
        public Undoer(int capacity)
        {
            this.capacity = Misc.Utils.Clamp(capacity, 2, 1024) + 1;
            this.cache = new T[this.capacity];
        }

        #region public methods
        public int Count() => (head + capacity - tail) % capacity;

        public int Position() => (cur + capacity - tail) % capacity;

        public bool Push(T text)
        {
            if (text == null)
            {
                return false;
            }
            lock (locker)
            {
                if (Position() > 0 && text.Equals(cache[cur - 1]))
                {
                    return false;
                }
                cache[cur] = text;
                cur = (cur + 1) % capacity;
                head = cur;
                if (tail == cur)
                {
                    tail = (tail + 1) % capacity;
                }
            }
            return true;
        }

        public bool TryRedo(out T content)
        {
            content = default;
            lock (locker)
            {
                var p = Position();
                var c = Count();
                if (p >= c)
                {
                    return false;
                }
                content = cache[cur];
                cur = (cur + 1) % capacity;
            }
            return true;
        }

        public bool TryUndo(out T content)
        {
            content = default;
            lock (locker)
            {
                var p = Position();
                if (p < 2)
                {
                    return false;
                }
                cur = (cur - 1 + capacity) % capacity;
                content = cache[cur - 1];
            }
            return true;
        }
        #endregion
    }
}
