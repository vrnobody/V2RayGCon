using System.Collections.Generic;
using V2RayGCon.Resources.Resx;

namespace V2RayGCon.Models.Datas
{
    class Table
    {
        public static readonly Dictionary<Enums.Cultures, string> Cultures = new Dictionary<
            Enums.Cultures,
            string
        >
        {
            { Enums.Cultures.auto, "auto" },
            { Enums.Cultures.enUS, "en" },
            { Enums.Cultures.zhCN, "cn" },
        };
    }
}
