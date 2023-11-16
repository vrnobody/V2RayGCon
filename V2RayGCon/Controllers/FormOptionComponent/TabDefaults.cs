using System;
using System.Linq;
using System.Windows.Forms;
using V2RayGCon.Resources.Resx;

namespace V2RayGCon.Controllers.OptionComponent
{
    class TabDefaults : OptionComponentController
    {
        readonly Services.Settings settings;
        private readonly ToolTip toolTip;
        readonly ComboBox cboxDefInboundName = null,
            cboxDefCoreName = null,
            cboxDefSpeedtestUrl = null,
            cboxDefSpeedtestExpectedSize = null;

        readonly CheckBox chkSetSpeedtestIsUse = null,
            chkImportSsShareLink = null,
            chkImportTrojanShareLink = null,
            chkDefVmessDecodeTemplateEnabled = null;

        readonly TextBox tboxDefImportAddr = null,
            tboxSetSpeedtestCycles = null,
            tboxSetSpeedtestTimeout = null,
            tboxDefVmessDecodeTemplateUrl = null;

        public TabDefaults(
            ToolTip toolTip,
            ComboBox cboxDefInboundName,
            TextBox tboxDefImportAddr,
            ComboBox cboxDefCoreName,
            CheckBox chkImportSsShareLink,
            CheckBox chkImportTrojanShareLink,
            CheckBox chkSetSpeedtestIsUse,
            ComboBox cboxDefSpeedtestUrl,
            TextBox tboxSetSpeedtestCycles,
            ComboBox cboxDefSpeedtestExpectedSize,
            TextBox tboxSetSpeedtestTimeout,
            TextBox tboxDefVmessDecodeTemplateUrl,
            CheckBox chkDefVmessDecodeTemplateEnabled
        )
        {
            this.settings = Services.Settings.Instance;

            this.tboxDefVmessDecodeTemplateUrl = tboxDefVmessDecodeTemplateUrl;
            this.chkDefVmessDecodeTemplateEnabled = chkDefVmessDecodeTemplateEnabled;
            this.toolTip = toolTip;
            this.cboxDefInboundName = cboxDefInboundName;
            this.tboxDefImportAddr = tboxDefImportAddr;
            this.chkImportSsShareLink = chkImportSsShareLink;
            this.chkImportTrojanShareLink = chkImportTrojanShareLink;
            this.cboxDefCoreName = cboxDefCoreName;
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

            settings.CustomVmessDecodeTemplateEnabled = chkDefVmessDecodeTemplateEnabled.Checked;
            settings.CustomVmessDecodeTemplateUrl = tboxDefVmessDecodeTemplateUrl.Text;

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
                || settings.CustomVmessDecodeTemplateUrl != tboxDefVmessDecodeTemplateUrl.Text
                || settings.CustomVmessDecodeTemplateEnabled
                    != chkDefVmessDecodeTemplateEnabled.Checked
                || settings.CustomDefImportHost != host
                || settings.CustomDefImportPort != port
                || settings.DefaultCoreName != GetCboxCoreNameText()
                || settings.DefaultInboundName != cboxDefInboundName.Text
                || settings.CustomDefImportSsShareLink != chkImportSsShareLink.Checked
                || settings.CustomDefImportTrojanShareLink != chkImportTrojanShareLink.Checked
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

        void UpdateTooltip(ComboBox cbox)
        {
            toolTip.SetToolTip(cbox, cbox.Text);
        }

        private void InitElement()
        {
            // import
            tboxDefVmessDecodeTemplateUrl.Text = settings.CustomVmessDecodeTemplateUrl;
            chkDefVmessDecodeTemplateEnabled.Checked = settings.CustomVmessDecodeTemplateEnabled;

            chkImportSsShareLink.Checked = settings.CustomDefImportSsShareLink;
            chkImportTrojanShareLink.Checked = settings.CustomDefImportTrojanShareLink;

            tboxDefImportAddr.TextChanged += OnTboxImportAddrTextChanged;
            tboxDefImportAddr.Text = string.Format(
                @"{0}:{1}",
                settings.CustomDefImportHost,
                settings.CustomDefImportPort
            );

            RefreshCboxInboundName(this, EventArgs.Empty);
            cboxDefInboundName.SelectedIndexChanged += (s, a) => UpdateTooltip(cboxDefInboundName);
            VgcApis.Misc.UI.SelectComboxByText(cboxDefInboundName, settings.DefaultInboundName);
            cboxDefInboundName.DropDown += RefreshCboxInboundName;

            RefreshCboxCoreName(this, EventArgs.Empty);
            cboxDefCoreName.SelectedIndexChanged += (s, a) => UpdateTooltip(cboxDefCoreName);
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
