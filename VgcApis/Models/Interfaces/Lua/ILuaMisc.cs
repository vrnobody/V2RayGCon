using System.Collections.Generic;

namespace VgcApis.Models.Interfaces.Lua
{
    public interface ILuaMisc
    {
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

        string PredefinedFunctions();
        void Print(params object[] contents);
        string ScanQrcode();
        void Sleep(int milliseconds);
    }
}
