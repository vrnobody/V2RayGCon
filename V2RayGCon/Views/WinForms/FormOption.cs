using System.Diagnostics;
using System.Windows.Forms;
using V2RayGCon.Resources.Resx;

namespace V2RayGCon.Views.WinForms
{
    public partial class FormOption : Form
    {
        #region Sigleton
        static readonly VgcApis.BaseClasses.AuxSiWinForm<FormOption> auxSiForm =
            new VgcApis.BaseClasses.AuxSiWinForm<FormOption>();

        public static FormOption GetForm() => auxSiForm.GetForm();

        public static void ShowForm() => auxSiForm.ShowForm();
        #endregion

        Controllers.FormOptionCtrl optionCtrl;

        public FormOption()
        {
            InitializeComponent();

            VgcApis.Misc.UI.AutoSetFormIcon(this);
            VgcApis.Misc.UI.AddTagToFormTitle(this);
        }

        private void FormOption_Load(object sender, System.EventArgs e)
        {
            // throw new System.ArgumentException("for debug");

            this.optionCtrl = InitOptionCtrl();

            this.FormClosing += (s, a) =>
            {
                if (!this.optionCtrl.IsOptionsSaved())
                {
                    a.Cancel = !VgcApis.Misc.UI.Confirm(I18N.ConfirmCloseWinWithoutSave);
                    return;
                }
            };

            this.FormClosed += (s, a) =>
            {
                optionCtrl.Cleanup();
            };
        }

        #region private method

        private Controllers.FormOptionCtrl InitOptionCtrl()
        {
            var ctrl = new Controllers.FormOptionCtrl();

            ctrl.Plug(
                new Controllers.OptionComponent.TabCustomCoreSettings(
                    flyCoresSetting,
                    btnCoresSettingAdd
                )
            );

            ctrl.Plug(
                new Controllers.OptionComponent.TabCustomInboundSettings(
                    flyCustomTemplates,
                    btnCustomTemplatesAdd
                )
            );

            ctrl.Plug(
                new Controllers.OptionComponent.Subscription(
                    flySubsUrlContainer,
                    btnAddSubsUrl,
                    btnUpdateViaSubscription,
                    chkSubsIsUseProxy,
                    chkSubsIsAutoPatch,
                    btnSubsUseAll,
                    btnSubsInvertSelection
                )
            );

            ctrl.Plug(
                new Controllers.OptionComponent.TabPlugin(
                    btnRefreshPluginsPanel,
                    chkIsLoad3rdPartyPlugins,
                    flyPluginsItemsContainer
                )
            );

            ctrl.Plug(
                new Controllers.OptionComponent.TabSetting(
                    cboxCustomUserAgent,
                    chkIsUseCustomUserAgent,
                    tboxSystrayLeftClickCommand,
                    chkIsEnableSystrayLeftClickCommand,
                    cboxSettingLanguage,
                    cboxSettingPageSize,
                    chkSetServAutotrack,
                    tboxSettingsMaxCoreNum,
                    cboxSettingsRandomSelectServerLatency,
                    chkSetSysPortable,
                    chkSetSelfSignedCert,
                    cboxSettingsUtlsFingerprint,
                    chkSettingsEnableUtlsFingerprint,
                    chkSetServStatistics,
                    chkSetUpgradeUseProxy,
                    chkSetCheckVgcUpdateWhenStart,
                    chkSetCheckV2RayCoreUpdateWhenStart,
                    btnSetBrowseDebugFile,
                    tboxSetDebugFilePath,
                    chkSetEnableDebugFile
                )
            );

            ctrl.Plug(
                new Controllers.OptionComponent.TabDefaults(
                    // def import share link mode
                    cboxDefImportInbName,
                    tboxDefImportAddr,
                    cboxDefImportCoreName,
                    // decoders
                    chkDefImportVmessShareLink,
                    chkDefImportVlessShareLink,
                    chkDefImportSsShareLink,
                    chkDefImportTrojanShareLink,
                    chkDefImportSocksShareLink,
                    // speedtest
                    chkDefSpeedtestIsUse,
                    cboxDefSpeedTestUrl,
                    tboxDefSpeedtestCycles,
                    cboxDefSpeedTestExpectedSize,
                    tboxDefSpeedtestTimeout
                )
            );

            return ctrl;
        }

        #endregion

        #region UI event
        private void btnSetOpenStartupFolder_Click(object sender, System.EventArgs e)
        {
            Process.Start(@"shell:startup");
        }

        private void btnOptionExit_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }

        private void btnOptionSave_Click(object sender, System.EventArgs e)
        {
            btnOptionSave.Enabled = false;
            btnOptionExit.Enabled = false;

            this.optionCtrl.SaveAllOptions();
            VgcApis.Misc.UI.MsgBox(I18N.Done);

            btnOptionSave.Enabled = true;
            btnOptionExit.Enabled = true;
        }
        #endregion
    }
}
