using System;

namespace DyFetch.Comps
{
    internal static class Utils
    {
        public static void DisposeObject(IDisposable o)
        {
            try
            {
                o?.Dispose();
            }
            catch { }
        }
    }
}
