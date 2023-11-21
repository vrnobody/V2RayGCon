using System.Collections.Generic;

namespace VgcApis.Interfaces.Services
{
    public interface IWebService
    {
        string PatchHref(string url, string href);

        List<string> ExtractLinks(string text, Models.Datas.Enums.LinkTypes linkType);

        string Search(string query, int start, int proxyPort, int timeout);

        string FetchSocks5(
            string url,
            string host,
            int proxyPort,
            int timeout,
            string username,
            string password
        );

        string Fetch(string url, int proxyPort, int timeout);

        string Fetch(string url, string host, int proxyPort, int timeout);

        bool Download(string url, string filename, int proxyPort, int timeout);
    }
}
