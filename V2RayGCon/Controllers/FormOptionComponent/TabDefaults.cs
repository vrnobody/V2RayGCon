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

        readonly CheckBox chkSetSpeedtestIsUse = null;

        readonly TextBox tboxDefImportAddr = null,
            tboxSetSpeedtestCycles = null,
            tboxSetSpeedtestTimeout = null;
        private readonly RichTextBox rtboxCustomFilterKeywords;

        public TabDefaults(
            ComboBox cboxDefInboundName,
            TextBox tboxDefImportAddr,
            ComboBox cboxDefCoreName,
            // speedtest
            CheckBox chkSetSpeedtestIsUse,
            ComboBox cboxDefSpeedtestUrl,
            TextBox tboxSetSpeedtestCycles,
            ComboBox cboxDefSpeedtestExpectedSize,
            TextBox tboxSetSpeedtestTimeout,
            // filter
            RichTextBox rtboxCustomFilterKeywords
        )
        {
            this.settings = Services.Settings.Instance;

            this.cboxDefInboundName = cboxDefInboundName;
            this.tboxDefImportAddr = tboxDefImportAddr;
            this.cboxDefCoreName = cboxDefCoreName;

            this.chkSetSpeedtestIsUse = chkSetSpeedtestIsUse;
            this.cboxDefSpeedtestUrl = cboxDefSpeedtestUrl;
            this.tboxSetSpeedtestCycles = tboxSetSpeedtestCycles;
            this.cboxDefSpeedtestExpectedSize = cboxDefSpeedtestExpectedSize;
            this.tboxSetSpeedtestTimeout = tboxSetSpeedtestTimeout;
            this.rtboxCustomFilterKeywords = rtboxCustomFilterKeywords;

            InitControls();
        }

        #region public method
        public override bool SaveOptions()
        {
            if (!IsOptionsChanged())
            {
                return false;
            }

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

            settings.CustomFilterKeywords = rtboxCustomFilterKeywords.Text;

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
                || settings.CustomDefImportHost != host
                || settings.CustomDefImportPort != port
                || settings.DefaultCoreName != GetCboxCoreNameText()
                || settings.DefaultInboundName != cboxDefInboundName.Text
                || settings.isUseCustomSpeedtestSettings != chkSetSpeedtestIsUse.Checked
                || settings.CustomSpeedtestUrl != cboxDefSpeedtestUrl.Text
                || settings.CustomSpeedtestExpectedSizeInKib
                    != VgcApis.Misc.Utils.Str2Int(cboxDefSpeedtestExpectedSize.Text)
                || settings.CustomSpeedtestCycles
                    != VgcApis.Misc.Utils.Str2Int(tboxSetSpeedtestCycles.Text)
                || settings.CustomSpeedtestTimeout
                    != VgcApis.Misc.Utils.Str2Int(tboxSetSpeedtestTimeout.Text)
                || settings.CustomFilterKeywords != rtboxCustomFilterKeywords.Text
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

        private void InitControls()
        {
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

            // filter
            rtboxCustomFilterKeywords.Text = settings.CustomFilterKeywords;
        }

        void OnTboxImportAddrTextChanged(object sender, EventArgs e) =>
            VgcApis.Misc.UI.MarkInvalidAddressWithColorRed(tboxDefImportAddr);
        #endregion
    }
}
