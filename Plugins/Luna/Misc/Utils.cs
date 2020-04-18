using System.Collections.Generic;

namespace Luna.Misc
{
    public static class Utils
    {
        public static List<string> LuaTableToList(NLua.LuaTable table, bool isShowKey)
        {
            var r = new List<string>();
            foreach (KeyValuePair<object, object> de in table)
            {
                var v = de.Value.ToString();
                if (isShowKey)
                {
                    v = $"{de.Key}.{v}";
                }
                r.Add(v);
            }
            return r;
        }

        public static Dictionary<string, string> LuaTableToDictionary(NLua.LuaTable data)
        {
            var r = new Dictionary<string, string>();
            foreach (KeyValuePair<object, object> de in data)
            {
                r[de.Key.ToString()] = de.Value.ToString();
            }
            return r;
        }
    }
}
