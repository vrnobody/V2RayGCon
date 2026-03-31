namespace VgcApis.Libs.Infr
{
    public class Undoer<T>
    {
        private readonly object locker = new object();
        private readonly int size;
        private readonly T[] cache;
        int head = 0;
        int tail = 0;
        int cur = 0;

        public Undoer()
            : this(128) { }

        public Undoer(int capacity)
        {
            var MAX_SIZE = int.MaxValue - 1;
            if (capacity < 2 || capacity >= MAX_SIZE)
            {
                throw new System.ArgumentOutOfRangeException(
                    $"capacity must between 2 and {MAX_SIZE}"
                );
            }

            this.size = capacity + 1;
            this.cache = new T[this.size];
        }

        #region public methods
        public int Count() => Normalize(head + size - tail);

        public int Position() => Normalize(cur + size - tail);

        public bool Push(T text)
        {
            if (text == null)
            {
                return false;
            }
            lock (locker)
            {
                var prevIdx = Normalize(cur - 1);
                if (Position() > 0 && text.Equals(cache[prevIdx]))
                {
                    return false;
                }
                cache[cur] = text;
                cur = Normalize(cur + 1);
                head = cur;
                if (tail == cur)
                {
                    tail = Normalize(tail + 1);
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
                cur = Normalize(cur + 1);
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
                cur = Normalize(cur - 1);
                content = cache[Normalize(cur - 1)];
            }
            return true;
        }
        #endregion

        #region private methods
        int Normalize(int index)
        {
            return (index + size) % size;
        }
        #endregion
    }
}
