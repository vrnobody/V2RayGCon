using System.Windows.Forms;
using ProxySetter.Resources.Langs;
using ProxySetter.Services;

namespace ProxySetter.Controllers.VGCPluginComponents
{
    class TabTuna : ComponentCtrl
    {
        private readonly PsSettings settings;
        private readonly TunaServer tunaServer;

        #region controls
        private readonly Label lbStatus;
        private readonly TextBox tboxProxy;
        private readonly TextBox tboxNicIpv4;
        private readonly TextBox tboxTunIpv4;
        private readonly TextBox tboxTunIpv6;
        private readonly RichTextBox rtboxDns;
        private readonly TextBox tboxExe;
        private readonly TextBox tboxTunName;
        private readonly CheckBox chkAutoGenArgs;
        private readonly CheckBox chkDebug;
        private readonly CheckBox chkModifySendThrough;
        private readonly CheckBox chkEnableIpv6;
        private readonly RichTextBox rtboxStartup;
        private readonly Button btnExe;
        private readonly Button btnDetect;
        private readonly CheckBox chkIsAutorun;
        private readonly Button btnStart;
        private readonly Button btnStop;
        #endregion

        public TabTuna(
            PsSettings settings,
            TunaServer tunaServer,
            Label lbStatus,
            TextBox tboxProxy,
            TextBox tboxNicIpv4,
            TextBox tboxTunIpv4,
            TextBox tboxTunIpv6,
            RichTextBox rtboxDns,
            TextBox tboxExe,
            TextBox tboxTunName,
            CheckBox chkAutoGenArgs,
            CheckBox chkEnableIpv6,
            CheckBox chkModifySendThrough,
            CheckBox chkDebug,
            RichTextBox rtboxStartup,
            Button btnExe,
            Button btnDetect,
            CheckBox chkIsAutorun,
            Button btnStart,
            Button btnStop
        )
        {
            this.settings = settings;
            this.tunaServer = tunaServer;

            this.lbStatus = lbStatus;

            this.tboxProxy = tboxProxy;
            this.tboxNicIpv4 = tboxNicIpv4;
            this.tboxTunIpv4 = tboxTunIpv4;
            this.tboxTunIpv6 = tboxTunIpv6;
            this.rtboxDns = rtboxDns;
            this.tboxExe = tboxExe;
            this.tboxTunName = tboxTunName;
            this.chkAutoGenArgs = chkAutoGenArgs;
            this.chkDebug = chkDebug;
            this.chkEnableIpv6 = chkEnableIpv6;
            this.chkModifySendThrough = chkModifySendThrough;

            this.rtboxStartup = rtboxStartup;
            this.btnExe = btnExe;
            this.btnDetect = btnDetect;
            this.chkIsAutorun = chkIsAutorun;
            this.btnStart = btnStart;
            this.btnStop = btnStop;

            InitControls();
            BindEvents();
            if (string.IsNullOrEmpty(rtboxStartup.Text))
            {
                GenerateStartScript();
            }
        }

        #region public method

        public override void Cleanup()
        {
            tunaServer.onChanged -= OnStatusChange;
        }

        public override bool IsOptionsChanged()
        {
            var src = settings.GetTunaSettings();
            var ts = GatherSettings();
            return !ts.EqualsTo(src);
        }

        public override bool SaveOptions()
        {
            var ts = GatherSettings();
            settings.SaveTunaSettings(ts);
            return true;
        }
        #endregion

        #region private method
        void ToggleControlEnableState()
        {
            var isAuto = chkAutoGenArgs.Checked;
            tboxNicIpv4.Enabled = !isAuto;
            rtboxStartup.Enabled = !isAuto;
            tboxTunIpv6.Enabled = chkEnableIpv6.Checked;
        }

        void OnStatusChange(bool isRunning)
        {
            var status = isRunning ? I18N.TunaIsRunning : I18N.TunaStopped;

            VgcApis.Misc.UI.Invoke(() =>
            {
                lbStatus.Text = status;
                btnStart.Enabled = !isRunning;
                btnStop.Enabled = isRunning;
            });
        }

        void BindEvents()
        {
            chkAutoGenArgs.CheckedChanged += (s, a) => ToggleControlEnableState();
            chkEnableIpv6.CheckedChanged += (s, a) => ToggleControlEnableState();

            tunaServer.onChanged += OnStatusChange;

            this.btnDetect.Click += (s, a) =>
            {
                if (GenerateStartScript())
                {
                    VgcApis.Misc.UI.MsgBox(I18N.Done);
                }
            };

            btnExe.Click += (s, a) =>
            {
                var file = VgcApis.Misc.UI.ShowSelectFileDialog(VgcApis.Models.Consts.Files.ExeExt);
                tboxExe.Text = file;
            };

            btnStart.Click += (s, a) =>
            {
                SaveOptions();
                tunaServer.Start();
            };
            btnStop.Click += (s, a) => tunaServer.Stop();
        }

        bool GenerateStartScript()
        {
            var ts = GatherSettings();
            var ok = tunaServer.UpdateTunaSettings(ts);
            if (ok)
            {
                settings.SaveTunaSettings(ts);
                InitControls();
            }
            else
            {
                VgcApis.Misc.UI.MsgBox(I18N.DetectRouteConfigFailed);
            }
            return ok;
        }

        Model.Data.TunaSettings GatherSettings()
        {
            return new Model.Data.TunaSettings()
            {
                proxy = tboxProxy.Text,
                nicIpv4 = tboxNicIpv4.Text,
                tunIpv4 = tboxTunIpv4.Text,
                tunIpv6 = tboxTunIpv6.Text,
                startupScript = rtboxStartup.Text,
                dns = rtboxDns.Text,
                exe = tboxExe.Text,
                tunName = tboxTunName.Text,
                isDebug = chkDebug.Checked,
                isEnableIpv6 = chkEnableIpv6.Checked,
                isModifySendThrough = chkModifySendThrough.Checked,
                autoGenArgs = chkAutoGenArgs.Checked,
                isAutorun = chkIsAutorun.Checked,
            };
        }

        void InitControls()
        {
            var ts = settings.GetTunaSettings();
            tboxProxy.Text = ts.proxy;
            tboxNicIpv4.Text = ts.nicIpv4;
            tboxTunIpv4.Text = ts.tunIpv4;
            tboxTunIpv6.Text = ts.tunIpv6;
            rtboxDns.Text = ts.dns;
            tboxExe.Text = ts.exe;
            tboxTunName.Text = ts.tunName;

            chkAutoGenArgs.Checked = ts.autoGenArgs;
            chkEnableIpv6.Checked = ts.isEnableIpv6;
            chkModifySendThrough.Checked = ts.isModifySendThrough;
            chkDebug.Checked = ts.isDebug;
            chkIsAutorun.Checked = ts.isAutorun;

            rtboxStartup.Text = ts.startupScript;

            OnStatusChange(tunaServer.isRunning);
            ToggleControlEnableState();
        }
        #endregion
    }
}
