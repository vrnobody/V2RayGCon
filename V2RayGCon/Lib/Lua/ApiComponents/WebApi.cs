using System.Collections.Generic;

namespace V2RayGCon.Lib.Lua.ApiComponents
{
    public sealed class WebApi :
        VgcApis.Models.BaseClasses.Disposable,
        VgcApis.Models.IServices.IWebService
    {
        public string PatchHref(string url, string href) =>
            Utils.PatchHref(url, href);

        public List<string> FindAllHrefs(string text) =>
            Lib.Utils.FindAllHrefs(text);

        public List<string> ExtractLinks(
            string text,
            VgcApis.Models.Datas.Enum.LinkTypes linkType) =>
            Lib.Utils.ExtractLinks(text, linkType);

        public string Search(string keywords, int first, int proxyPort, int timeout)
        {
            var url = Lib.Utils.GenSearchUrl(keywords, first);
            return Fetch(url, proxyPort, timeout);
        }

        public string Fetch(string url, int proxyPort, int timeout) =>
          Utils.Fetch(url, proxyPort, timeout);
    }
}
