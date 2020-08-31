using System.Threading;

namespace VgcApis.Libs.Tasks
{
    public class Bar
    {
        readonly SemaphoreSlim mlocker = new SemaphoreSlim(1);

        public Bar() { }

        public void Remove() => mlocker.Release();

        public bool Install() => mlocker.Wait(0);
    }
}
