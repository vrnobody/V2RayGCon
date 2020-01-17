using System.Collections.Generic;

namespace VgcApis.Models.Interfaces.Lua
{
    public interface ILuaWeb
    {
        List<string> ExtractBase64String(string text);
        List<string> ExtractV2cfgLinks(string text);
        List<string> ExtractVmessLinks(string text);
        List<string> ExtractSsLinks(string text);
        string Fetch(string url);
        string Fetch(string url, int proxyPort, int milliSeconds);
        List<string> FindAllHrefs(string text);

        // the first running http server's port number
        int GetProxyPort();

        // patch short href to full href 
        // e.g. url = "http://baidu.com/" href = "/index.html" result = "http://baidu.com/index.html"
        string PatchHref(string url, string href);

        // using bing.com to search sth.
        string Search(string keywords, int first, int proxyPort);

        // direct connect
        int UpdateSubscriptions();

        // through proxy
        int UpdateSubscriptions(int proxyPort);
    }
}
