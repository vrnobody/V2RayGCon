using System.Collections.Generic;

namespace Luna.Models.Apis.Components
{
    public sealed class Web :
        VgcApis.Models.BaseClasses.ComponentOf<LuaApis>,
        VgcApis.Models.Interfaces.Lua.ILuaWeb
    {
        VgcApis.Models.IServices.IWebService vgcWeb;
        VgcApis.Models.IServices.IServersService vgcServers;
        VgcApis.Models.IServices.IShareLinkMgrService vgcSlinkMgr;

        public Web(VgcApis.Models.IServices.IApiService api)
        {
            vgcWeb = api.GetWebService();
            vgcServers = api.GetServersService();
            vgcSlinkMgr = api.GetShareLinkMgrService();
        }

        #region ILuaWeb thinggy
        public List<string> ExtractBase64String(string text) =>
            VgcApis.Libs.Utils.ExtractBase64Strings(text);

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
                text, VgcApis.Models.Datas.Enum.LinkTypes.v2cfg);

        public List<string> ExtractVmessLinks(string text) =>
            vgcWeb.ExtractLinks(
                text, VgcApis.Models.Datas.Enum.LinkTypes.vmess);

        public List<string> ExtractSsLinks(string text) =>
            vgcWeb.ExtractLinks(
                text, VgcApis.Models.Datas.Enum.LinkTypes.ss);

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
