using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using NDesk.Options;

namespace V2RayGCon.Models.Datas
{
    internal class CmdArgs
    {
        public string userSettings,
            userSettingsBak;
        public string tag = null;
        public bool help = false;

        readonly OptionSet opts;

        public CmdArgs(string[] args)
        {
            var us = "";
            opts = new OptionSet()
            {
                { "t|tag=", "tag shows in systray and form title", v => tag = v ?? "" },
                { "s|settings=", "path of userSettings.json", v => us = v ?? "" },
                { "h|help", "show this help information", v => help = v != null }
            };

            try
            {
                opts.Parse(args);
                if (string.IsNullOrEmpty(us))
                {
                    us = Properties.Resources.PortableUserSettingsFilename;
                }
                userSettings = Path.GetFullPath(us);
                userSettingsBak = VgcApis.Misc.Utils.ReplaceFileExtention(userSettings, ".bak");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        #region properties

        #endregion

        #region public methods
        public string GetHelpMessage()
        {
            var nv = Misc.Utils.GetAppNameAndVer();
            var n = VgcApis.Misc.Utils.GetAppName();
            var sb = new StringBuilder();
            sb.AppendLine($"{nv}");
            sb.AppendLine("");
            sb.AppendLine("Usage:");
            sb.AppendLine($"{n}.exe --settings=\"c:\\vgc\\userSettings.json\" --tag=\"2\"");
            sb.AppendLine("");
            sb.AppendLine("Arguments:");

            using (TextWriter writer = new StringWriter(sb))
            {
                opts?.WriteOptionDescriptions(writer);
            }

            return sb.ToString();
        }
        #endregion

        #region private methods

        #endregion

        #region protected methods

        #endregion
    }
}
