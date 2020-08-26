using Newtonsoft.Json.Linq;
using NLua;
using System.Collections.Generic;

namespace VgcApis.Interfaces.Lua
{
    public interface ILuaMisc
    {
        #region Json
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
        #endregion

        #region vgc.Forms
        void Invoke(LuaFunction func);

        void ShowFormJsonEditor(string config);
        void ShowFormServerSettings(ICoreServCtrl coreServ);
        void ShowFormSimpleEditor(ICoreServCtrl coreServ);

        void ShowFormOption();

        void ShowFormLunaMgr();
        void ShowFormLunaEditor();

        void ShowFormMain();

        void ShowFormLog();

        #endregion

        #region vgc
        // timeout = long.MaxValue
        long GetTimeoutValue();

        string ScanQrcode();
        string GetSubscriptionConfig();

        // share among all scripts
        string ReadLocalStorage(string key);

        // share among all scripts
        void WriteLocalStorage(string key, string value);

        // remove a key from local storage
        bool RemoveLocalStorage(string key);

        // get all keys of local storage
        List<string> LocalStorageKeys();
        #endregion

        #region utils
        string GetAppDir();

        string PredefinedFunctions();

        void Print(params object[] contents);

        void Sleep(int milSec);

        string Replace(string text, string oldStr, string newStr);

        string RandomHex(int len);

        string NewGuid();

        #endregion

        #region UI thing
        string BrowseFolder();

        string BrowseFile();

        string BrowseFile(string extends);

        // 2MiB char max
        string Input(string title);

        // 25 lines max
        string Input(string title, int lines);

        string Input(string title, string content, int lines);

        string ShowData(string title, NLua.LuaTable columns, NLua.LuaTable rows);
        string ShowData(string title, NLua.LuaTable columns, NLua.LuaTable rows, int defColumn);

        // 18 choices max
        List<int> Choices(string title, params string[] choices);

        List<int> Choices(string title, NLua.LuaTable choices);

        List<int> Choices(string title, NLua.LuaTable choices, bool isShowKey);

        int Choice(string title, params string[] choices);

        int Choice(string title, NLua.LuaTable choices);

        int Choice(string title, NLua.LuaTable choices, bool isShowKey);

        int Choice(string title, NLua.LuaTable choices, bool isShowKey, int selected);

        bool Confirm(string content);

        void Alert(string content);

        // sort server panel by index
        void RefreshFormMain();

        #endregion

        #region encode decode
        // GetLinkBody("vmess://abcdefg") == "abcdefg"
        string GetLinkBody(string link);

        // v2cfg://(b64Str)
        string AddV2cfgPrefix(string b64Str);

        // v://(b64Str)
        string AddVeePrefix(string b64Str);

        // vmess://(b64Str)
        string AddVmessPrefix(string b64Str);

        string Base64Encode(string text);
        string Base64Decode(string b64Str);

        string Config2V2cfg(string config);
        string Config2VeeLink(string config);
        string Config2VmessLink(string config);

        string ShareLink2ConfigString(string shareLink);

        // links = "vmess://... ss://...  (...)"
        int ImportLinks(string links, string mark);
        #endregion
    }
}
