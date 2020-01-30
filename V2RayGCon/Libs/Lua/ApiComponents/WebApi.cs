using System.Collections.Generic;

namespace V2RayGCon.Libs.Lua.ApiComponents
{
    public sealed class WebApi :
        VgcApis.BaseClasses.Disposable,
        VgcApis.Interfaces.Services.IWebService
    {
        public string PatchHref(string url, string href) =>
            Misc.Utils.PatchHref(url, href);

        public List<string> FindAllHrefs(string text) =>
            Misc.Utils.FindAllHrefs(text);

        public List<string> ExtractLinks(
            string text,
            VgcApis.Models.Datas.Enums.LinkTypes linkType) =>
            Misc.Utils.ExtractLinks(text, linkType);

        public string Search(string keywords, int first, int proxyPort, int timeout)
        {
            var url = Misc.Utils.GenSearchUrl(keywords, first);
            return Fetch(url, proxyPort, timeout);
        }

        public string Fetch(string url, int proxyPort, int timeout) =>
          Misc.Utils.Fetch(url, proxyPort, timeout);
    }
}
