namespace VgcApis.Libs.Tasks
{
    public class Bar
    {
        bool isBlocking = false;
        readonly object locker = new object();

        public Bar() { }

        public bool IsBlocking() => isBlocking;

        public bool Remove()
        {
            lock (locker)
            {
                if (!isBlocking)
                {
                    return false;
                }

                isBlocking = false;
                return true;
            }
        }

        public bool Install()
        {
            lock (locker)
            {
                if (isBlocking)
                {
                    return false;
                }

                isBlocking = true;
                return true;
            }
        }
    }
}
