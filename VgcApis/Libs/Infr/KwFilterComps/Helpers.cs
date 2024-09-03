using System;
using System.Collections.Generic;

namespace VgcApis.Libs.Infr.KwFilterComps
{
    internal static class Helpers
    {
        public static readonly long MiB = 1024 * 1024;

        public static Dictionary<string, TEnum> CreateEnumLookupTable<TEnum>()
            where TEnum : struct
        {
            var r = new Dictionary<string, TEnum>();
            foreach (TEnum cname in Enum.GetValues(typeof(TEnum)))
            {
                var key = cname.ToString().ToLower();
                r[key] = cname;
            }
            return r;
        }
    }
}
