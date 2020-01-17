using System;
using System.Windows.Forms;
using static VgcApis.Libs.Utils;

namespace ProxySetter.Controllers.VGCPluginComponents
{
    class TabBasicSetting : ComponentCtrl
    {
        Services.PsSettings setting;
        string oldSetting;
        Model.Data.BasicSettings basicSettings;
        Services.ServerTracker servTracker;

        ComboBox cboxBasicSysProxyMode, cboxBasicPacMode, cboxBasicPacProtocol;
        TextBox tboxBasicProxyPort, tboxBaiscPacPort, tboxBasicCustomPacPath;
        CheckBox chkBasicAutoUpdateSysProxy, chkBasicPacAlwaysOn, chkBasicUseCustomPac;


        #region hotkey winform controls
        private readonly CheckBox chkBaiscUseHotkey, chkBaiscUseAlt, chkBaiscUseShift;
        private readonly TextBox tboxBasicHotkeyStr;
        #endregion

        public TabBasicSetting(
            Services.PsSettings setting,
            Services.ServerTracker servTracker,

            ComboBox cboxBasicPacProtocol,
            ComboBox cboxBasicSysProxyMode,
            TextBox tboxBasicProxyPort,
            TextBox tboxBaiscPacPort,
            ComboBox cboxBasicPacMode,
            TextBox tboxBasicCustomPacPath,
            CheckBox chkBasicAutoUpdateSysProxy,
            CheckBox chkBasicPacAlwaysOn,
            CheckBox chkBasicUseCustomPac,
            Button btnBasicBrowseCustomPac,

            CheckBox chkBaiscUseHotkey,
            CheckBox chkBaiscUseAlt,
            CheckBox chkBaiscUseShift,
            TextBox tboxBasicHotkeyStr)
        {
            this.setting = setting;
            this.servTracker = servTracker;

            basicSettings = setting.GetBasicSetting();
            oldSetting = SerializeObject(basicSettings);

            this.cboxBasicPacProtocol = cboxBasicPacProtocol;
            this.cboxBasicSysProxyMode = cboxBasicSysProxyMode;
            this.tboxBasicProxyPort = tboxBasicProxyPort;
            this.tboxBaiscPacPort = tboxBaiscPacPort;
            this.cboxBasicPacMode = cboxBasicPacMode;
            this.tboxBasicCustomPacPath = tboxBasicCustomPacPath;
            this.chkBasicAutoUpdateSysProxy = chkBasicAutoUpdateSysProxy;
            this.chkBasicPacAlwaysOn = chkBasicPacAlwaysOn;
            this.chkBasicUseCustomPac = chkBasicUseCustomPac;
            this.chkBaiscUseHotkey = chkBaiscUseHotkey;
            this.chkBaiscUseAlt = chkBaiscUseAlt;
            this.chkBaiscUseShift = chkBaiscUseShift;
            this.tboxBasicHotkeyStr = tboxBasicHotkeyStr;

            InitControls();

            BindEvents(btnBasicBrowseCustomPac);

            servTracker.OnSysProxyChanged += OnSysProxyChangeHandler;
        }

        #region public method
        void OnSysProxyChangeHandler(object sender, EventArgs args)
        {
            basicSettings = setting.GetBasicSetting();
            oldSetting = SerializeObject(basicSettings);

            VgcApis.Libs.UI.RunInUiThread(
                chkBasicAutoUpdateSysProxy,
                () =>
                {
                    InitControls();
                });
        }

        public override bool IsOptionsChanged()
        {
            return SerializeObject(GetterSettings()) != oldSetting;
        }

        public override bool SaveOptions()
        {
            if (!IsOptionsChanged())
            {
                return false;
            }

            var bs = GetterSettings();
            oldSetting = SerializeObject(bs);
            setting.SaveBasicSetting(bs);
            return true;
        }

        public override void Cleanup()
        {
            servTracker.OnSysProxyChanged -= OnSysProxyChangeHandler;
        }
        #endregion

        #region private methods

        private void BindEvents(Button btnBasicBrowseCustomPac)
        {
            btnBasicBrowseCustomPac.Click += (s, a) =>
            {
                var filename = VgcApis.Libs.UI.ShowSelectFileDialog(
                    VgcApis.Models.Consts.Files.JsExt);

                if (!string.IsNullOrEmpty(filename))
                {
                    this.tboxBasicCustomPacPath.Text = filename;
                }
            };

            tboxBasicHotkeyStr.KeyDown += (s, a) =>
            {
                a.SuppressKeyPress = true;
                tboxBasicHotkeyStr.Text = a.KeyCode.ToString();
            };
        }

        Model.Data.BasicSettings GetterSettings()
        {
            return new Model.Data.BasicSettings
            {
                pacProtocol = Clamp(cboxBasicPacProtocol.SelectedIndex, 0, 2),
                sysProxyMode = Clamp(cboxBasicSysProxyMode.SelectedIndex, 0, 4),
                proxyPort = Str2Int(tboxBasicProxyPort.Text),
                pacServPort = Str2Int(tboxBaiscPacPort.Text),
                pacMode = Clamp(cboxBasicPacMode.SelectedIndex, 0, 2),
                customPacFileName = tboxBasicCustomPacPath.Text,
                isAutoUpdateSysProxy = chkBasicAutoUpdateSysProxy.Checked,
                isAlwaysStartPacServ = chkBasicPacAlwaysOn.Checked,
                isUseCustomPac = chkBasicUseCustomPac.Checked,

                isUseHotkey = chkBaiscUseHotkey.Checked,
                isUseAlt = chkBaiscUseAlt.Checked,
                isUseShift = chkBaiscUseShift.Checked,
                hotkeyStr = tboxBasicHotkeyStr.Text,
            };
        }

        private void InitControls()
        {
            var s = basicSettings;

            cboxBasicPacProtocol.SelectedIndex = Clamp(s.pacProtocol, 0, 2);
            cboxBasicSysProxyMode.SelectedIndex = Clamp(s.sysProxyMode, 0, 4);
            tboxBasicProxyPort.Text = s.proxyPort.ToString();
            tboxBaiscPacPort.Text = s.pacServPort.ToString();
            cboxBasicPacMode.SelectedIndex = Clamp(s.pacMode, 0, 2);
            tboxBasicCustomPacPath.Text = s.customPacFileName;
            chkBasicAutoUpdateSysProxy.Checked = s.isAutoUpdateSysProxy;
            chkBasicPacAlwaysOn.Checked = s.isAlwaysStartPacServ;
            chkBasicUseCustomPac.Checked = s.isUseCustomPac;

            chkBaiscUseHotkey.Checked = s.isUseHotkey;
            chkBaiscUseAlt.Checked = s.isUseAlt;
            chkBaiscUseShift.Checked = s.isUseShift;

            tboxBasicHotkeyStr.Text = Keys.F12.ToString();
            if (Enum.TryParse(s.hotkeyStr, out Keys hotkey))
            {
                tboxBasicHotkeyStr.Text = hotkey.ToString();
            }

        }

        #endregion
    }
}
