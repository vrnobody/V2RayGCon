using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commander.Misc
{
    internal static class Utils
    {
        public static string ReplaceNewLines(string text)
        {
            return (text ?? "").Replace("\r", "").Replace("\n", " ");
        }
    }
}
