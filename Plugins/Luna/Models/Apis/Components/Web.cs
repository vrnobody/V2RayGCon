using System.Collections.Generic;

namespace Luna.Models.Apis.Components
{
    internal sealed class Web :
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
        public bool Tcping(string url, int milSec) =>
            Tcping(url, milSec, -1);

        public bool Tcping(string url, int milSec, int proxyPort)
        {
            var timeout = TimedDownloadTesting(url, milSec, 0, proxyPort);
            return timeout > 0 && timeout <= milSec;
        }

        public long TimedDownloadTesting(string url, int timeout, int kib) =>
            TimedDownloadTesting(url, timeout, kib, -1);

        public long TimedDownloadTesting(string url, int timeout, int kib, int proxyPort)
        {
            try
            {
                return VgcApis.Misc.Utils.TimedDownloadTest(url, proxyPort, kib, timeout);
            }
            catch { }
            return -1;
        }

        public List<string> ExtractBase64String(string text) =>
            VgcApis.Misc.Utils.ExtractBase64Strings(text);

        public int GetProxyPort() =>
            vgcServers.GetAvailableHttpProxyPort();

        public bool Download(string url, string filename) =>
            vgcWeb.Download(url, filename, -1, -1);

        public bool Download(string url, string filename, int millSecond) =>
            vgcWeb.Download(url, filename, -1, millSecond);

        public bool Download(string url, string filename, int proxyPort, int millSecond) =>
            vgcWeb.Download(url, filename, proxyPort, millSecond);

        public string Fetch(string url) => vgcWeb.Fetch(url, -1, -1);

        public string Fetch(string url, int milliSeconds) =>
            vgcWeb.Fetch(url, -1, milliSeconds);

        public string Fetch(string url, int proxyPort, int milliSeconds) =>
            vgcWeb.Fetch(url, proxyPort, milliSeconds);

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



        #endregion
    }
}
