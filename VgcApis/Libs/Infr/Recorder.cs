using System.Collections.Generic;
using System.Linq;

namespace VgcApis.Libs.Infr
{
    public class Recorder
    {
        readonly object locker = new object();
        private readonly int capacity;
        int startIndex = 0,
            curIndex = 0;
        List<int> datas = new List<int>() { 0 };

        public Recorder()
            : this(500) { }

        public Recorder(int capacity)
        {
            this.capacity = capacity;
        }

        public int Current()
        {
            lock (locker)
            {
                return datas[curIndex];
            }
        }

        #region public methods
        public bool Add(int content)
        {
            lock (locker)
            {
                return AddWorker(content);
            }
        }

        public bool Forward()
        {
            lock (locker)
            {
                if (curIndex >= datas.Count - 1)
                {
                    return false;
                }

                curIndex++;
                return true;
            }
        }

        public bool Backward()
        {
            lock (locker)
            {
                if (curIndex <= startIndex)
                {
                    return false;
                }
                curIndex--;
                return true;
            }
        }
        #endregion

        #region private methods

        void Trim()
        {
            var c = datas.Count;
            if (c > capacity)
            {
                startIndex = c - capacity;
            }

            if (startIndex > capacity)
            {
                curIndex -= startIndex;
                datas = datas.Skip(startIndex).ToList();
                startIndex = 0;
            }
        }

        void CutTail()
        {
            var start = datas.Count - 1;
            for (int i = start; i > curIndex; i--)
            {
                datas.RemoveAt(i);
            }
        }

        bool AddWorker(int content)
        {
            if (content < 0 || Current() == content)
            {
                return false;
            }

            CutTail();
            curIndex++;
            datas.Add(content);
            Trim();
            return true;
        }

        #endregion
    }
}
