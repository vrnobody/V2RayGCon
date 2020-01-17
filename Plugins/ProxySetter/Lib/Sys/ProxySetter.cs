using static VgcApis.Libs.Utils;

namespace ProxySetter.Lib.Sys
{
    // https://stackoverflow.com/questions/197725/programmatically-set-browser-proxy-settings-in-c-sharp
    public class ProxySetter
    {
        #region public method
        static string LoopBackIP = VgcApis.Models.Consts.Webs.LoopBackIP;
        public static void SetPacProxy(string pacUrl)
        {
            var url = pacUrl + "?&t=" + RandomHex(8);
            WinInet.SetProxySettings(new Model.Data.ProxySettings
            {
                proxyMode = (int)WinInet.ProxyModes.PAC,
                pacUrl = url ?? "",
            });
        }

        public static void ClearSysProxy() =>
            UpdateProxySettingOnDemand(new Model.Data.ProxySettings());

        public static void SetGlobalProxy(int port)
        {
            var proxyUrl = string.Format(
                "{0}:{1}",
                LoopBackIP,
                Clamp(port, 0, 65536));

            var proxySetting = new Model.Data.ProxySettings
            {
                proxyMode = (int)WinInet.ProxyModes.Proxy,
                proxyAddr = proxyUrl,
            };
            UpdateProxySettingOnDemand(proxySetting);
        }

        public static void UpdateProxySettingOnDemand(Model.Data.ProxySettings proxySetting)
        {
            if (!proxySetting.IsEqualTo(GetProxySetting()))
            {
                WinInet.SetProxySettings(proxySetting);
            }
        }

        public static Model.Data.ProxySettings GetProxySetting() =>
            WinInet.GetProxySettings();

        #endregion
    }
}
