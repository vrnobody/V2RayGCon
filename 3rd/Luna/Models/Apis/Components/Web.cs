﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;

namespace Luna.Models.Apis.Components
{
    internal sealed class Web : VgcApis.BaseClasses.ComponentOf<LuaApis>, Interfaces.ILuaWeb
    {
        readonly VgcApis.Interfaces.Services.IWebService vgcWeb;
        readonly VgcApis.Interfaces.Services.IServersService vgcServers;
        readonly VgcApis.Interfaces.Services.IShareLinkMgrService vgcSlinkMgr;
        readonly VgcApis.Interfaces.Services.IConfigMgrService vgcCfgMgr;

        public Web(VgcApis.Interfaces.Services.IApiService api)
        {
            vgcWeb = api.GetWebService();
            vgcServers = api.GetServersService();
            vgcSlinkMgr = api.GetShareLinkMgrService();
            vgcCfgMgr = api.GetConfigMgrService();
        }

        #region ILuaWeb thinggy
        public string UnicodeDecode(string str)
        {
            return VgcApis.Misc.Utils.UnescapeUnicode(str);
        }

        public string UnicodeEncode(string str)
        {
            return VgcApis.Misc.Utils.EscapeUnicode(str);
        }

        public string UriEncode(string content) => VgcApis.Misc.Utils.UriEncode(content);

        public string UriDecode(string content) => VgcApis.Misc.Utils.UriDecode(content);

        public int GetFreeTcpPort() => VgcApis.Misc.Utils.GetFreeTcpPort();

        public long Ping(string dest) => Ping(dest, 5000);

        public long Ping(string dest, int ms)
        {
            long r = -1;
            Ping pinger = null;
            try
            {
                pinger = new Ping();
                var reply = pinger.Send(dest, ms);
                if (reply.Status == IPStatus.Success)
                {
                    r = reply.RoundtripTime;
                }
            }
            catch { }
            finally
            {
                if (pinger != null)
                {
                    pinger.Dispose();
                }
            }
            return r;
        }

        public string Post(string url, string text) => Post(url, text, 20000);

        public string Post(string url, string text, int timeout)
        {
            timeout = Math.Max(1, timeout);

            try
            {
                var t = Task.Run(async () =>
                {
                    using (var client = new HttpClient())
                    {
                        client.Timeout = TimeSpan.FromMilliseconds(timeout);
                        var content = new StringContent(text);

                        var cts = new CancellationTokenSource(timeout);
                        var token = cts.Token;
                        token.Register(() => cts.Dispose());
                        var resp = await client.PostAsync(url, content, token);
                        return await resp.Content.ReadAsStringAsync();
                    }
                });

                return t.GetAwaiter().GetResult();
            }
            catch { }
            return null;
        }

        public bool Tcping(string url, int ms) => Tcping(url, ms, -1);

        public bool Tcping(string url, int ms, int proxyPort)
        {
            var timeout = TimedDownloadTesting(url, ms, 0, proxyPort);
            return timeout > 0 && timeout <= ms;
        }

        public long TimedDownloadTesting(string url, int timeout, int kib) =>
            TimedDownloadTesting(url, timeout, kib, -1);

        public long TimedDownloadTesting(string url, int timeout, int kib, int proxyPort)
        {
            var r = VgcApis.Misc.Utils.TimedDownloadTestWorker(
                false,
                url,
                proxyPort,
                kib,
                timeout,
                null,
                null
            );
            return r.Item1;
        }

        public List<string> ExtractBase64String(string text, int minLen) =>
            VgcApis.Misc.Utils.ExtractBase64Strings(text, minLen);

        public List<string> ExtractBase64String(string text) => ExtractBase64String(text, 4);

        public int GetProxyPort() => vgcServers.GetAvailableHttpProxyPort();

        public bool Download(string url, string filename) => vgcWeb.Download(url, filename, -1, -1);

        public bool Download(string url, string filename, int millSecond) =>
            vgcWeb.Download(url, filename, -1, millSecond);

        public bool Download(string url, string filename, int proxyPort, int millSecond) =>
            vgcWeb.Download(url, filename, proxyPort, millSecond);

        public string Fetch(string url) => Fetch(url, -1);

        public string Fetch(string url, int milliSeconds) => Fetch(url, -1, milliSeconds);

        public string Fetch(string url, int proxyPort, int milliSeconds)
        {
            var host = VgcApis.Models.Consts.Webs.LoopBackIP;
            return vgcWeb.RawFetch(false, url, host, proxyPort, milliSeconds, null, null);
        }

        public string FetchWithCustomConfig(string rawConfig, string url) =>
            FetchWithCustomConfig(rawConfig, "", url, -1);

        public string FetchWithCustomConfig(
            string rawConfig,
            string coreName,
            string url,
            int timeout
        )
        {
            var title = "fetch wiht custom config";
            var text = vgcCfgMgr.FetchWithCustomConfig(rawConfig, coreName, title, url, timeout);
            return text ?? string.Empty;
        }

        public int UpdateSubscriptions() => UpdateSubscriptions(-1);

        public int UpdateSubscriptions(int proxyPort) =>
            vgcSlinkMgr.UpdateSubscriptions(false, proxyPort);

        public List<string> ExtractAllShareLinks(string text)
        {
            var formats = new VgcApis.Models.Datas.Enums.LinkTypes[]
            {
                VgcApis.Models.Datas.Enums.LinkTypes.ss,
                VgcApis.Models.Datas.Enums.LinkTypes.trojan,
                VgcApis.Models.Datas.Enums.LinkTypes.v2cfg,
                VgcApis.Models.Datas.Enums.LinkTypes.vless,
                VgcApis.Models.Datas.Enums.LinkTypes.vmess,
            };

            var r = new List<string>();
            foreach (var format in formats)
            {
                r.AddRange(vgcWeb.ExtractLinks(text, format));
            }
            return r;
        }

        public List<string> ExtractV2cfgLinks(string text) =>
            vgcWeb.ExtractLinks(text, VgcApis.Models.Datas.Enums.LinkTypes.v2cfg);

        public List<string> ExtractVmessLinks(string text) =>
            vgcWeb.ExtractLinks(text, VgcApis.Models.Datas.Enums.LinkTypes.vmess);

        public List<string> ExtractSsLinks(string text) =>
            vgcWeb.ExtractLinks(text, VgcApis.Models.Datas.Enums.LinkTypes.ss);

        public string Search(string keywords, int first, int proxyPort) =>
            vgcWeb.Search(keywords, first, proxyPort, 20 * 1000);

        public string PatchHref(string url, string href) => vgcWeb.PatchHref(url, href);

        #endregion
    }
}
