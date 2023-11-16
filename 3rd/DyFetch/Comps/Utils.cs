using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
