namespace ProxySetter.Model.Data
{
    public class ProxySettings
    {
        // ProxyServer ProxyEnable AutoConfigURL ProxyOverride
        public string proxyAddr, pacUrl;
        public int proxyMode; // WinInet.PerConnFlags

        public ProxySettings()
        {
            proxyMode = (int)Lib.Sys.WinInet.ProxyModes.Direct;
            proxyAddr = string.Empty;
            pacUrl = string.Empty;
            // proxyOverride = string.Empty; // do not touch this
        }

        public bool IsEqualTo(ProxySettings target)
        {
            if (target.proxyMode != this.proxyMode
                || target.proxyAddr != this.proxyAddr
                || target.pacUrl != this.pacUrl)
            {
                return false;
            }
            return true;
        }
    }
}
