using System.Threading;

namespace VgcApis.Libs.Tasks
{
    public class Bar
    {
        int guard = 0;

        public Bar() { }

        public void Remove()
        {
            guard = 0;
        }

        public bool Install()
        {
            if (guard != 0)
            {
                return false;
            }
            return Interlocked.Exchange(ref guard, 1) == 0;
        }
    }
}
