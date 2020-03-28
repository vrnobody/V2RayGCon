using System.Collections.Generic;

namespace VgcApis.Interfaces.Lua
{
    public interface ILuaMisc
    {
        #region system
        string GetOsVersion();

        string GetOsReleaseInfo();
        int SetWallpaper(string filename);
        string GetAppDir();
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
        string PredefinedFunctions();

        void Print(params object[] contents);

        void Sleep(int milliseconds);

        string Replace(string text, string oldStr, string newStr);

        string RandomHex(int len);

        string NewGuid();

        string GetImageResolution(string filename);

        #endregion


        #region file
        string PickRandomLine(string filename);
        #endregion

        #region UI thing

        // 2MiB char max
        string Input(string title);

        // 25 lines max
        string Input(string title, int lines);

        string Input(string title, string content, int lines);

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
