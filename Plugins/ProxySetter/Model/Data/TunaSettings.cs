namespace ProxySetter.Model.Data
{
    class TunaSettings
    {
        public string tunName = "wintun";
        public string proxy = "";

        public string nicIpv4 = "";
        public string tunIpv4 = "";
        public string tunIpv6 = "";

        public string startupScript = "";
        public string dns = "8.8.8.8\n2001:4860:4860::8888";
        public string exe = "3rd/tun2socks/tun2socks.exe";
        public bool autoGenArgs = false;
        public bool isDebug = false;
        public bool isEnableIpv6 = false;
        public bool isModifySendThrough = true;

        public TunaSettings() { }

        public bool EqualsTo(TunaSettings o)
        {
            if (
                o == null
                || tunName != o.tunName
                || proxy != o.proxy
                || nicIpv4 != o.nicIpv4
                || tunIpv4 != o.tunIpv4
                || tunIpv6 != o.tunIpv6
                || startupScript != o.startupScript
                || dns != o.dns
                || exe != o.exe
                || autoGenArgs != o.autoGenArgs
                || isDebug != o.isDebug
                || isEnableIpv6 != o.isEnableIpv6
                || isModifySendThrough != o.isModifySendThrough
            )
            {
                return false;
            }
            return true;
        }
    }
}
