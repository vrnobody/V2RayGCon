using System.Collections.Generic;

namespace VgcApis.Interfaces.Lua
{
    public interface ILuaWeb
    {
        List<string> ExtractBase64String(string text);
        List<string> ExtractV2cfgLinks(string text);
        List<string> ExtractVmessLinks(string text);
        List<string> ExtractSsLinks(string text);
        string Fetch(string url);
        string Fetch(string url, int milliSeconds);
        string Fetch(string url, int proxyPort, int milliSeconds);

        // Download("http://baidu.com", "d:\index.html")
        bool Download(string url, string filename);
        bool Download(string url, string filename, int millSeconds);
        bool Download(string url, string filename, int proxyPort, int millSeconds);

        // the first running http server's port number
        int GetProxyPort();

        // patch short href to full href 
        // e.g. url = "http://baidu.com/" href = "/index.html" result = "http://baidu.com/index.html"
        string PatchHref(string url, string href);

        string Post(string url, string text);

        string Post(string url, string text, int timeout);

        // using bing.com to search sth.
        string Search(string keywords, int first, int proxyPort);

        bool Tcping(string url, int milSec);

        bool Tcping(string url, int milSec, int proxyPort);

        long TimedDownloadTesting(string url, int timeout, int kib);
        long TimedDownloadTesting(string url, int timeout, int kib, int proxyPort);

        // direct connect
        int UpdateSubscriptions();

        // through proxy
        int UpdateSubscriptions(int proxyPort);
    }
}
