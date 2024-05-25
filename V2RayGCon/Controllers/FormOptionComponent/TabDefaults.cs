using System;
using System.Linq;
using System.Windows.Forms;
using V2RayGCon.Resources.Resx;

namespace V2RayGCon.Controllers.OptionComponent
{
    class TabDefaults : OptionComponentController
    {
        readonly Services.Settings settings;
        readonly ComboBox cboxDefInboundName = null,
            cboxDefCoreName = null,
            cboxDefSpeedtestUrl = null,
            cboxDefSpeedtestExpectedSize = null;
        private readonly CheckBox chkImportVmessShareLink;
        private readonly CheckBox chkImportVlessShareLink;
        readonly CheckBox chkSetSpeedtestIsUse = null,
            chkImportSsShareLink = null,
            chkImportTrojanShareLink = null;
        private readonly CheckBox chkImportSocksShareLink;
        readonly TextBox tboxDefImportAddr = null,
            tboxSetSpeedtestCycles = null,
            tboxSetSpeedtestTimeout = null;

        public TabDefaults(
            ComboBox cboxDefInboundName,
            TextBox tboxDefImportAddr,
            ComboBox cboxDefCoreName,
            CheckBox chkImportVmessShareLink,
            CheckBox chkImportVlessShareLink,
            CheckBox chkImportSsShareLink,
            CheckBox chkImportTrojanShareLink,
            CheckBox chkImportSocksShareLink,
            // speedtest
            CheckBox chkSetSpeedtestIsUse,
            ComboBox cboxDefSpeedtestUrl,
            TextBox tboxSetSpeedtestCycles,
            ComboBox cboxDefSpeedtestExpectedSize,
            TextBox tboxSetSpeedtestTimeout
        )
        {
            this.settings = Services.Settings.Instance;

            this.cboxDefInboundName = cboxDefInboundName;
            this.tboxDefImportAddr = tboxDefImportAddr;
            this.chkImportSsShareLink = chkImportSsShareLink;
            this.chkImportTrojanShareLink = chkImportTrojanShareLink;
            this.chkImportSocksShareLink = chkImportSocksShareLink;
            this.cboxDefCoreName = cboxDefCoreName;
            this.chkImportVmessShareLink = chkImportVmessShareLink;
            this.chkImportVlessShareLink = chkImportVlessShareLink;
            this.chkSetSpeedtestIsUse = chkSetSpeedtestIsUse;
            this.cboxDefSpeedtestUrl = cboxDefSpeedtestUrl;
            this.tboxSetSpeedtestCycles = tboxSetSpeedtestCycles;
            this.cboxDefSpeedtestExpectedSize = cboxDefSpeedtestExpectedSize;
            this.tboxSetSpeedtestTimeout = tboxSetSpeedtestTimeout;

            InitElement();
        }

        #region public method
        public override bool SaveOptions()
        {
            if (!IsOptionsChanged())
            {
                return false;
            }

            settings.CustomDefImportVlessShareLink = chkImportVlessShareLink.Checked;
            settings.CustomDefImportVmessShareLink = chkImportVmessShareLink.Checked;

            // mode
            if (
                VgcApis.Misc.Utils.TryParseAddress(
                    tboxDefImportAddr.Text,
                    out string ip,
                    out int port
                )
            )
            {
                settings.CustomDefImportHost = ip;
                settings.CustomDefImportPort = port;
            }

            settings.DefaultCoreName = GetCboxCoreNameText();
            settings.DefaultInboundName = cboxDefInboundName.Text;

            settings.CustomDefImportSocksShareLink = chkImportSocksShareLink.Checked;
            settings.CustomDefImportSsShareLink = chkImportSsShareLink.Checked;
            settings.CustomDefImportTrojanShareLink = chkImportTrojanShareLink.Checked;

            // speedtest
            settings.isUseCustomSpeedtestSettings = chkSetSpeedtestIsUse.Checked;
            settings.CustomSpeedtestUrl = cboxDefSpeedtestUrl.Text;
            settings.CustomSpeedtestCycles = VgcApis.Misc.Utils.Str2Int(
                tboxSetSpeedtestCycles.Text
            );
            settings.CustomSpeedtestExpectedSizeInKib = VgcApis.Misc.Utils.Str2Int(
                cboxDefSpeedtestExpectedSize.Text
            );
            settings.CustomSpeedtestTimeout = VgcApis.Misc.Utils.Str2Int(
                tboxSetSpeedtestTimeout.Text
            );

            settings.SaveUserSettingsNow();
            return true;
        }

        public override bool IsOptionsChanged()
        {
            var success = VgcApis.Misc.Utils.TryParseAddress(
                tboxDefImportAddr.Text,
                out string host,
                out int port
            );
            if (
                !success
                || settings.CustomDefImportVlessShareLink != chkImportVlessShareLink.Checked
                || settings.CustomDefImportVmessShareLink != chkImportVmessShareLink.Checked
                || settings.CustomDefImportHost != host
                || settings.CustomDefImportPort != port
                || settings.DefaultCoreName != GetCboxCoreNameText()
                || settings.DefaultInboundName != cboxDefInboundName.Text
                || settings.CustomDefImportSsShareLink != chkImportSsShareLink.Checked
                || settings.CustomDefImportTrojanShareLink != chkImportTrojanShareLink.Checked
                || settings.CustomDefImportSocksShareLink != chkImportSocksShareLink.Checked
                || settings.isUseCustomSpeedtestSettings != chkSetSpeedtestIsUse.Checked
                || settings.CustomSpeedtestUrl != cboxDefSpeedtestUrl.Text
                || settings.CustomSpeedtestExpectedSizeInKib
                    != VgcApis.Misc.Utils.Str2Int(cboxDefSpeedtestExpectedSize.Text)
                || settings.CustomSpeedtestCycles
                    != VgcApis.Misc.Utils.Str2Int(tboxSetSpeedtestCycles.Text)
                || settings.CustomSpeedtestTimeout
                    != VgcApis.Misc.Utils.Str2Int(tboxSetSpeedtestTimeout.Text)
            )
            {
                return true;
            }
            return false;
        }
        #endregion

        #region private method
        string GetCboxCoreNameText()
        {
            if (cboxDefCoreName.SelectedIndex < 1)
            {
                return string.Empty;
            }
            return cboxDefCoreName.Text;
        }

        void RefreshCboxInboundName(object sender, EventArgs args)
        {
            var names = settings
                .GetCustomConfigTemplates()
                .Where(inb => !inb.isInject)
                .Select(inb => inb.name)
                .ToArray();

            var items = cboxDefInboundName.Items;
            items.Clear();
            items.AddRange(names);
            VgcApis.Misc.UI.ResetComboBoxDropdownMenuWidth(cboxDefInboundName);
        }

        void RefreshCboxCoreName(object sender, EventArgs args)
        {
            var names = settings.GetCustomCoresSetting().Select(cs => cs.name).ToArray();
            var items = cboxDefCoreName.Items;
            items.Clear();
            items.Add(I18N.Default);
            items.AddRange(names);
            VgcApis.Misc.UI.ResetComboBoxDropdownMenuWidth(cboxDefCoreName);
        }

        private void InitElement()
        {
            chkImportVmessShareLink.Checked = settings.CustomDefImportVmessShareLink;
            chkImportVlessShareLink.Checked = settings.CustomDefImportVlessShareLink;
            chkImportSsShareLink.Checked = settings.CustomDefImportSsShareLink;
            chkImportTrojanShareLink.Checked = settings.CustomDefImportTrojanShareLink;
            chkImportSocksShareLink.Checked = settings.CustomDefImportSocksShareLink;

            tboxDefImportAddr.TextChanged += OnTboxImportAddrTextChanged;
            tboxDefImportAddr.Text = string.Format(
                @"{0}:{1}",
                settings.CustomDefImportHost,
                settings.CustomDefImportPort
            );

            RefreshCboxInboundName(this, EventArgs.Empty);
            VgcApis.Misc.UI.SelectComboxByText(cboxDefInboundName, settings.DefaultInboundName);
            cboxDefInboundName.DropDown += RefreshCboxInboundName;

            RefreshCboxCoreName(this, EventArgs.Empty);
            VgcApis.Misc.UI.SelectComboxByText(cboxDefCoreName, settings.DefaultCoreName);
            cboxDefCoreName.DropDown += RefreshCboxCoreName;

            // speedtest
            chkSetSpeedtestIsUse.Checked = settings.isUseCustomSpeedtestSettings;
            tboxSetSpeedtestCycles.Text = settings.CustomSpeedtestCycles.ToString();
            cboxDefSpeedtestUrl.Text = settings.CustomSpeedtestUrl;
            cboxDefSpeedtestExpectedSize.Text =
                settings.CustomSpeedtestExpectedSizeInKib.ToString();
            tboxSetSpeedtestTimeout.Text = settings.CustomSpeedtestTimeout.ToString();
        }

        void OnTboxImportAddrTextChanged(object sender, EventArgs e) =>
            VgcApis.Misc.UI.MarkInvalidAddressWithColorRed(tboxDefImportAddr);
        #endregion
    }
}
