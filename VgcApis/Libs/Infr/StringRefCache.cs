using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VgcApis.Libs.Infr
{
    public static class StringRefCache
    {
        static readonly int CACHE_SIZE = 500;

        static readonly List<string> caches = new List<string>();

        static StringRefCache() { }

        #region public methods
        /// <summary>
        /// This cache is design to store short string.
        /// Do not use on large string block!
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string Ref(string content)
        {
            if (string.IsNullOrEmpty(content))
            {
                return "";
            }

            lock (caches)
            {
                var idx = caches.IndexOf(content);
                if (idx < 0)
                {
                    caches.Add(content);
                    if (caches.Count > CACHE_SIZE)
                    {
                        var keep = CACHE_SIZE * 2 / 3;
                        while (caches.Count > keep)
                        {
                            caches.RemoveAt(0);
                        }
                    }
                    return content;
                }
                return caches[idx];
            }
        }
        #endregion
    }
}
