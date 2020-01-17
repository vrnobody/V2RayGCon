using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProxySetter.Lib.Sys;

namespace ProxySetterTests
{
    [TestClass]
    public class WinInetTests
    {
        [TestMethod]
        public void GeneralTests()
        {
            // for debug only 
            // WinInetDebugWorker();
        }

        void WinInetDebugWorker()
        {
            var orgSettings = WinInet.GetProxySettings();
            ProxySetter.Model.Data.ProxySettings curSettings;
            bool success;

            // set proxy server
            var proxySetting = new ProxySetter.Model.Data.ProxySettings
            {
                proxyMode = (int)WinInet.ProxyModes.Proxy,
                proxyAddr = "192.168.1.1:1234",
            };

            success = WinInet.SetProxySettings(proxySetting);
            Assert.AreEqual(true, success);
            curSettings = WinInet.GetProxySettings();
            Assert.AreEqual(true, proxySetting.IsEqualTo(curSettings));

            // set pac proxy
            var pacSetting = new ProxySetter.Model.Data.ProxySettings
            {
                proxyMode = (int)WinInet.ProxyModes.PAC,
                pacUrl = "http://localhost/pac/a.pac",
            };

            success = WinInet.SetProxySettings(pacSetting);
            Assert.AreEqual(true, success);
            curSettings = WinInet.GetProxySettings();
            Assert.AreEqual(true, pacSetting.IsEqualTo(curSettings));

            WinInet.SetProxySettings(orgSettings);
            curSettings = WinInet.GetProxySettings();
            Assert.AreEqual(true, orgSettings.IsEqualTo(curSettings));
        }

    }
}
