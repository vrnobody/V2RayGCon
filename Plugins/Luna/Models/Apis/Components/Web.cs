using System.Collections.Generic;

namespace Luna.Models.Apis.Components
{
    public sealed class Web :
        VgcApis.BaseClasses.ComponentOf<LuaApis>,
        VgcApis.Interfaces.Lua.ILuaWeb
    {
        VgcApis.Interfaces.Services.IWebService vgcWeb;
        VgcApis.Interfaces.Services.IServersService vgcServers;
        VgcApis.Interfaces.Services.IShareLinkMgrService vgcSlinkMgr;

        public Web(VgcApis.Interfaces.Services.IApiService api)
        {
            vgcWeb = api.GetWebService();
            vgcServers = api.GetServersService();
            vgcSlinkMgr = api.GetShareLinkMgrService();
        }

        #region ILuaWeb thinggy
        public List<string> ExtractBase64String(string text) =>
            VgcApis.Misc.Utils.ExtractBase64Strings(text);

        public int GetProxyPort() =>
            vgcServers.GetAvailableHttpProxyPort();

        public string Fetch(string url) =>
            vgcWeb.Fetch(url, -1, -1);

        public int UpdateSubscriptions() =>
            vgcSlinkMgr.UpdateSubscriptions(-1);

        public int UpdateSubscriptions(int proxyPort) =>
            vgcSlinkMgr.UpdateSubscriptions(proxyPort);

        public List<string> ExtractV2cfgLinks(string text) =>
            vgcWeb.ExtractLinks(
                text, VgcApis.Models.Datas.Enums.LinkTypes.v2cfg);

        public List<string> ExtractVmessLinks(string text) =>
            vgcWeb.ExtractLinks(
                text, VgcApis.Models.Datas.Enums.LinkTypes.vmess);

        public List<string> ExtractSsLinks(string text) =>
            vgcWeb.ExtractLinks(
                text, VgcApis.Models.Datas.Enums.LinkTypes.ss);

        public string Search(string keywords, int first, int proxyPort) =>
            vgcWeb.Search(keywords, first, proxyPort, 20 * 1000);

        public string PatchHref(string url, string href) =>
            vgcWeb.PatchHref(url, href);

        public List<string> FindAllHrefs(string text) =>
            vgcWeb.FindAllHrefs(text);

        public string Fetch(string url, int proxyPort, int milliSeconds) =>
          vgcWeb.Fetch(url, proxyPort, milliSeconds);

        #endregion
    }
}
