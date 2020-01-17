using System.Collections.Generic;

namespace VgcApis.Models.IServices
{
    public interface IWebService
    {
        string PatchHref(string url, string href);
        List<string> FindAllHrefs(string text);
        List<string> ExtractLinks(string text, Datas.Enum.LinkTypes linkType);

        string Search(string query, int start, int proxyPort, int timeout);
        string Fetch(string url, int proxyPort, int timeout);
    }
}
