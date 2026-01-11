using System.Collections.Generic;
using System.Linq;

namespace Composer.Misc
{
    public static class Utils
    {
        public static List<T> GetOrdered<T>(IEnumerable<T> list)
            where T : class, VgcApis.Interfaces.IHasIndex
        {
            return list.OrderBy(item => item.GetIndex()).ToList();
        }

        public static void ResetIndex<T>(IEnumerable<T> list)
            where T : class, VgcApis.Interfaces.IHasIndex
        {
            var c = 0;
            var r = list.OrderBy(item => item.GetIndex()).ToList();
            foreach (var item in r)
            {
                item.SetIndex(++c);
            }
        }
    }
}
