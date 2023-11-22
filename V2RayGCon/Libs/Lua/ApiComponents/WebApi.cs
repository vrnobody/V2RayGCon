using System.Collections.Generic;

namespace V2RayGCon.Libs.Lua.ApiComponents
{
    public sealed class WebApi
        : VgcApis.BaseClasses.Disposable,
            VgcApis.Interfaces.Services.IWebService
    {
        public string PatchHref(string url, string href) => Misc.Utils.PatchHref(url, href);

        public List<string> ExtractLinks(
            string text,
            VgcApis.Models.Datas.Enums.LinkTypes linkType
        ) => Misc.Utils.ExtractLinks(text, linkType);

        public string Search(string keywords, int first, int proxyPort, int timeout)
        {
            var url = Misc.Utils.GenSearchUrl(keywords, first);
            return Misc.Utils.Fetch(url, proxyPort, timeout);
        }

        public string RawFetch(
            bool isSocks5,
            string url,
            string host,
            int proxyPort,
            int timeout,
            string username,
            string password
        ) => Misc.Utils.FetchWorker(isSocks5, url, host, proxyPort, timeout, username, password);

        public bool Download(string url, string filename, int proxyPort, int timeout)
        {
            var host = VgcApis.Models.Consts.Webs.LoopBackIP;
            return Misc.Utils.DownloadFileWorker(url, filename, host, proxyPort, timeout);
        }
    }
}
