using System;
using System.Net;
using System.Windows.Forms;

namespace V2RayGCon.Controller.OptionComponent
{
    class TabDefaults : OptionComponentController
    {
        Service.Setting setting;

        ComboBox cboxDefImportMode = null;
        CheckBox chkSetSpeedtestIsUse = null,
            chkImportSsShareLink = null,
            chkImportIsFold = null,
            chkImportBypassCnSite = null,
            chkImportInjectGlobalImport = null;

        TextBox tboxDefImportAddr = null,
            tboxSetSpeedtestUrl = null,
            tboxSetSpeedtestCycles = null,
            tboxSetSpeedtestExpectedSize = null,
            tboxSetSpeedtestTimeout = null;

        public TabDefaults(
            ComboBox cboxDefImportMode,
            TextBox tboxDefImportAddr,
            CheckBox chkImportSsShareLink,
            CheckBox chkImportIsFold,
            CheckBox chkImportBypassCnSite,
            CheckBox chkImportInjectGlobalImport,

            CheckBox chkSetSpeedtestIsUse,
            TextBox tboxSetSpeedtestUrl,
            TextBox tboxSetSpeedtestCycles,
            TextBox tboxSetSpeedtestExpectedSize,
            TextBox tboxSetSpeedtestTimeout)
        {
            this.setting = Service.Setting.Instance;

            // Do not put these lines of code into InitElement.
            this.cboxDefImportMode = cboxDefImportMode;
            this.tboxDefImportAddr = tboxDefImportAddr;
            this.chkImportSsShareLink = chkImportSsShareLink;
            this.chkImportIsFold = chkImportIsFold;
            this.chkImportBypassCnSite = chkImportBypassCnSite;
            this.chkImportInjectGlobalImport = chkImportInjectGlobalImport;

            this.chkSetSpeedtestIsUse = chkSetSpeedtestIsUse;
            this.tboxSetSpeedtestUrl = tboxSetSpeedtestUrl;
            this.tboxSetSpeedtestCycles = tboxSetSpeedtestCycles;
            this.tboxSetSpeedtestExpectedSize = tboxSetSpeedtestExpectedSize;
            this.tboxSetSpeedtestTimeout = tboxSetSpeedtestTimeout;

            InitElement();
        }

        private void InitElement()
        {
            // mode
            chkImportBypassCnSite.Checked = setting.CustomDefImportBypassCnSite;
            chkImportInjectGlobalImport.Checked = setting.CustomDefImportGlobalImport;
            chkImportIsFold.Checked = setting.CustomDefImportIsFold;
            chkImportSsShareLink.Checked = setting.CustomDefImportSsShareLink;
            cboxDefImportMode.SelectedIndex = setting.CustomDefImportMode;
            tboxDefImportAddr.TextChanged += OnTboxImportAddrTextChanged;
            tboxDefImportAddr.Text = string.Format(
                @"{0}:{1}",
                setting.CustomDefImportIp,
                setting.CustomDefImportPort);

            // speedtest
            chkSetSpeedtestIsUse.Checked = setting.isUseCustomSpeedtestSettings;
            tboxSetSpeedtestCycles.Text = setting.CustomSpeedtestCycles.ToString();
            tboxSetSpeedtestUrl.Text = setting.CustomSpeedtestUrl;
            tboxSetSpeedtestExpectedSize.Text = setting.CustomSpeedtestExpectedSizeInKib.ToString();
            tboxSetSpeedtestTimeout.Text = setting.CustomSpeedtestTimeout.ToString();
        }

        #region public method
        public override bool SaveOptions()
        {
            if (!IsOptionsChanged())
            {
                return false;
            }

            // mode
            if (VgcApis.Libs.Utils.TryParseIPAddr(tboxDefImportAddr.Text, out string ip, out int port))
            {
                setting.CustomDefImportIp = ip;
                setting.CustomDefImportPort = port;
            }
            setting.CustomDefImportMode = cboxDefImportMode.SelectedIndex;
            setting.CustomDefImportIsFold = chkImportIsFold.Checked;
            setting.CustomDefImportSsShareLink = chkImportSsShareLink.Checked;
            setting.CustomDefImportGlobalImport = chkImportInjectGlobalImport.Checked;
            setting.CustomDefImportBypassCnSite = chkImportBypassCnSite.Checked;

            // speedtest
            setting.isUseCustomSpeedtestSettings = chkSetSpeedtestIsUse.Checked;
            setting.CustomSpeedtestUrl = tboxSetSpeedtestUrl.Text;
            setting.CustomSpeedtestCycles = VgcApis.Libs.Utils.Str2Int(tboxSetSpeedtestCycles.Text);
            setting.CustomSpeedtestExpectedSizeInKib = VgcApis.Libs.Utils.Str2Int(tboxSetSpeedtestExpectedSize.Text);
            setting.CustomSpeedtestTimeout = VgcApis.Libs.Utils.Str2Int(tboxSetSpeedtestTimeout.Text);

            setting.SaveUserSettingsNow();
            return true;
        }

        public override bool IsOptionsChanged()
        {
            var success = VgcApis.Libs.Utils.TryParseIPAddr(tboxDefImportAddr.Text, out string ip, out int port);
            if (!success
                || setting.CustomDefImportGlobalImport != chkImportInjectGlobalImport.Checked
                || setting.CustomDefImportBypassCnSite != chkImportBypassCnSite.Checked
                || setting.CustomDefImportIsFold != chkImportIsFold.Checked
                || setting.CustomDefImportSsShareLink != chkImportSsShareLink.Checked
                || setting.CustomDefImportIp != ip
                || setting.CustomDefImportPort != port
                || setting.CustomDefImportMode != cboxDefImportMode.SelectedIndex

                || setting.isUseCustomSpeedtestSettings != chkSetSpeedtestIsUse.Checked
                || setting.CustomSpeedtestUrl != tboxSetSpeedtestUrl.Text
                || setting.CustomSpeedtestExpectedSizeInKib != VgcApis.Libs.Utils.Str2Int(tboxSetSpeedtestExpectedSize.Text)
                || setting.CustomSpeedtestCycles != VgcApis.Libs.Utils.Str2Int(tboxSetSpeedtestCycles.Text)
                || setting.CustomSpeedtestTimeout != VgcApis.Libs.Utils.Str2Int(tboxSetSpeedtestTimeout.Text))
            {
                return true;
            }
            return false;
        }
        #endregion

        #region private method
        void OnTboxImportAddrTextChanged(object sender, EventArgs e) =>
            VgcApis.Libs.UI.TryParseAddressFromTextBox(
                tboxDefImportAddr, out string ip, out int port);
        #endregion
    }
}
