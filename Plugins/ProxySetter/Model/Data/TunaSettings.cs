namespace ProxySetter.Model.Data
{
    class TunaSettings
    {
        public string tunName = "wintun";
        public string proxy = "";
        public string nicIp = "";
        public string tunIp = "";
        public string startupScript = "";
        public string dns = "8.8.8.8";
        public string exe = "3rd/tun2socks/tun2socks.exe";
        public bool autoGenArgs = false;
        public bool isDebug = false;

        public TunaSettings() { }

        public bool EqualsTo(TunaSettings o)
        {
            if (
                o == null
                || isDebug != o.isDebug
                || tunName != o.tunName
                || proxy != o.proxy
                || nicIp != o.nicIp
                || tunIp != o.tunIp
                || startupScript != o.startupScript
                || dns != o.dns
                || exe != o.exe
                || autoGenArgs != o.autoGenArgs
            )
            {
                return false;
            }
            return true;
        }
    }
}
