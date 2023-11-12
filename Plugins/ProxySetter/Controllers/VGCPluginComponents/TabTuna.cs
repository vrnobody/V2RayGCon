using ProxySetter.Resources.Langs;
using ProxySetter.Services;
using System;
using System.Linq;
using System.Windows.Forms;

namespace ProxySetter.Controllers.VGCPluginComponents
{
    class TabTuna : ComponentCtrl
    {
        private readonly PsSettings settings;
        private readonly TunaServer tunaServer;

        #region controls
        private readonly Label lbStatus;
        private readonly TextBox tboxProxy;
        private readonly TextBox tboxNicIp;
        private readonly TextBox tboxTunIp;
        private readonly TextBox tboxDns;
        private readonly TextBox tboxExe;
        private readonly TextBox tboxTunName;
        private readonly CheckBox chkAutoGenArgs;
        private readonly CheckBox chkDebug;
        private readonly RichTextBox rtboxStartup;
        private readonly Button btnExe;
        private readonly Button btnDetect;
        private readonly Button btnStart;
        private readonly Button btnStop;
        #endregion

        public TabTuna(
            PsSettings settings,
            TunaServer tunaServer,
            Label lbStatus,
            TextBox tboxProxy,
            TextBox tboxNicIp,
            TextBox tboxTunIp,
            TextBox tboxDns,
            TextBox tboxExe,
            TextBox tboxTunName,
            CheckBox chkAutoGenArgs,
            CheckBox chkDebug,
            RichTextBox rtboxStartup,
            Button btnExe,
            Button btnDetect,
            Button btnStart,
            Button btnStop
        )
        {
            this.settings = settings;
            this.tunaServer = tunaServer;
            this.lbStatus = lbStatus;
            this.tboxProxy = tboxProxy;
            this.tboxNicIp = tboxNicIp;
            this.tboxTunIp = tboxTunIp;
            this.tboxDns = tboxDns;
            this.tboxExe = tboxExe;
            this.tboxTunName = tboxTunName;
            this.chkAutoGenArgs = chkAutoGenArgs;
            this.chkDebug = chkDebug;
            this.rtboxStartup = rtboxStartup;
            this.btnExe = btnExe;
            this.btnDetect = btnDetect;
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
            var enable = !chkAutoGenArgs.Checked;
            tboxNicIp.Enabled = enable;
            rtboxStartup.Enabled = enable;
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
                nicIp = tboxNicIp.Text,
                tunIp = tboxTunIp.Text,
                startupScript = rtboxStartup.Text,
                dns = tboxDns.Text,
                exe = tboxExe.Text,
                tunName = tboxTunName.Text,
                isDebug = chkDebug.Checked,
                autoGenArgs = chkAutoGenArgs.Checked,
            };
        }

        void InitControls()
        {
            var ts = settings.GetTunaSettings();
            tboxProxy.Text = ts.proxy;
            tboxNicIp.Text = ts.nicIp;
            tboxTunIp.Text = ts.tunIp;
            tboxDns.Text = ts.dns;
            tboxExe.Text = ts.exe;
            tboxTunName.Text = ts.tunName;
            chkAutoGenArgs.Checked = ts.autoGenArgs;
            chkDebug.Checked = ts.isDebug;

            rtboxStartup.Text = ts.startupScript;

            OnStatusChange(tunaServer.isRunning);
            ToggleControlEnableState();
        }
        #endregion
    }
}
