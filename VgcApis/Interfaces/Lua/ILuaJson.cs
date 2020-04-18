using Newtonsoft.Json.Linq;

namespace VgcApis.Interfaces.Lua
{
    public interface ILuaJson
    {
        /*
         Json中的path是以点号连接的key串，数字会被解释为列表的序号。

         假设有以下一个json对象：
         var jobj = ParseJObject(@"{'a':[1,2,3],'1':[1,2,3]}");
         var s1 = GetString(jobj, "a.1")  // s1 = "2";
         var s2 = GetString(jobj, "1.2")  // 报错，因为1解释为列表序号，但jobj不是列表
         */

        // lua 不能识别参数类型, 所以不可以用 SetValue(int v) SetValue(bool v)的重载形式来简化
        bool TrySetDoubleValue(JToken json, string path, double value);
        bool TrySetIntValue(JToken json, string path, int value);
        bool TrySetBoolValue(JToken json, string path, bool value);
        bool TrySetStringValue(JToken json, string path, string value);

        string GetString(JToken json, string path);
        bool GetBool(JToken json, string path);
        int GetInt(JToken json, string path);
        double GetDouble(JToken json, string path);

        void CombineWithRoutingInFront(JObject body, JObject mixin);
        void CombineWithRoutingInTheEnd(JObject body, JObject mixin);
        JToken GetKey(JToken json, string path);
        string GetProtocol(JObject config);
        string JTokenToString(JToken jtoken);
        void Merge(JObject body, JObject mixin);
        JToken ParseJToken(string json);
        JArray ParseJArray(string json);
        JObject ParseJObject(string json);
        void Replace(JToken node, JToken value);
        JArray ToJArray(JToken jtoken);
        JObject ToJObject(JToken jtoken);
        void Union(JObject body, JObject mixin);
    }
}
