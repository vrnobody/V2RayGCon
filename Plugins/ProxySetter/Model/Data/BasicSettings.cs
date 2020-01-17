using System.Windows.Forms;

namespace ProxySetter.Model.Data
{
    public class BasicSettings
    {
        public int sysProxyMode { get; set; }
        public bool isAutoUpdateSysProxy { get; set; }
        public int proxyPort { get; set; }
        public int pacServPort { get; set; }
        public bool isAlwaysStartPacServ { get; set; }
        public int pacMode { get; set; }
        public string customPacFileName { get; set; }
        public bool isUseCustomPac { get; set; }
        public int pacProtocol { get; set; }

        public bool isUseAlt { get; set; }

        public bool isUseHotkey { get; set; }

        public bool isUseShift { get; set; }

        public string hotkeyStr { get; set; }


        public BasicSettings()
        {
            sysProxyMode = (int)Enum.SystemProxyModes.None;
            isAutoUpdateSysProxy = true;
            proxyPort = 1080;
            pacServPort = 3000;
            isAlwaysStartPacServ = false;
            pacMode = (int)Enum.PacListModes.WhiteList;
            customPacFileName = string.Empty;
            isUseCustomPac = false;
            pacProtocol = (int)Enum.PacProtocols.HTTP;

            isUseHotkey = false;
            isUseAlt = false;
            isUseShift = false;
            hotkeyStr = Keys.F12.ToString();
        }
    }
}
