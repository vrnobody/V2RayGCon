using System.Collections.Generic;

namespace VgcApis.Interfaces.Lua
{
    public interface ILuaMisc
    {
        int SetWallpaper(string filename);

        string GetImageResolution(string filename);

        string PickRandomLine(string filename);

        string RandomHex(int len);

        string NewGuid();

        string GetSubscriptionConfig();

        // max content length is 2MiB
        string Input(string title);

        // max choices number is 6
        List<int> Choices(string title, params string[] choices);

        // max choices number is 6
        int Choice(string title, params string[] choices);

        bool Confirm(string content);

        void Alert(string content);

        // sort server panel by index
        void RefreshFormMain();

        // share among all scripts
        string ReadLocalStorage(string key);

        // share among all scripts
        void WriteLocalStorage(string key, string value);

        // remove a key from local storage
        bool RemoveLocalStorage(string key);

        // get all keys of local storage
        List<string> LocalStorageKeys();

        #region encode decode
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

        string GetAppDir();

        // GetLinkBody("vmess://abcdefg") == "abcdefg"
        string GetLinkBody(string link);

        long GetTimeoutValue();

        string PredefinedFunctions();
        void Print(params object[] contents);
        string ScanQrcode();
        void Sleep(int milliseconds);
    }
}
