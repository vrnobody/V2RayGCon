namespace VgcApis.Libs.Infr
{
    public class Undoer
    {
        private readonly int capacity;
        private readonly string[] cache;
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
            this.capacity = Misc.Utils.Clamp(capacity, 2, 1024);
            this.cache = new string[capacity];
        }

        #region private methods
        void Forward()
        {
            if (cur == tail)
            {
                tail = (tail + 1) % capacity;
            }
            cur = (cur + 1) % capacity;
            head = cur;
        }

        #endregion

        #region public methods
        public int Count() => (this.head + capacity - this.tail) % capacity;

        public int Position() => (cur + capacity - tail) % capacity;

        public void Push(string text)
        {
            Forward();
            cache[cur] = text;
        }

        public bool TryRedo(out string content)
        {
            content = "";
            if (Position() >= Count())
            {
                return false;
            }
            cur = (cur + 1) % capacity;
            content = cache[cur];
            return true;
        }

        public bool TryUndo(out string content)
        {
            content = "";
            if (Position() < 1)
            {
                return false;
            }
            cur = (cur - 1 + capacity) % capacity;
            content = cache[cur];
            return true;
        }
        #endregion
    }
}
