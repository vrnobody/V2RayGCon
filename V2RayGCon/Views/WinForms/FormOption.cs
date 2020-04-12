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
        static public FormOption GetForm() => auxSiForm.GetForm();
        static public void ShowForm() => auxSiForm.ShowForm();
        #endregion

        Controllers.FormOptionCtrl optionCtrl;

        public FormOption()
        {
            InitializeComponent();

            VgcApis.Misc.UI.AutoSetFormIcon(this);
        }

        private void FormOption_Load(object sender, System.EventArgs e)
        {
            // throw new System.ArgumentException("for debug");

            this.optionCtrl = InitOptionCtrl();

            this.FormClosing += (s, a) =>
            {
                if (!this.optionCtrl.IsOptionsSaved())
                {
                    a.Cancel = !Misc.UI.Confirm(I18N.ConfirmCloseWinWithoutSave);
                }
            };

            this.FormClosed += (s, a) =>
            {
                Services.Settings.Instance.LazyGC();
            };
        }

        #region public method

        #endregion

        #region private method

        private Controllers.FormOptionCtrl InitOptionCtrl()
        {
            var ctrl = new Controllers.FormOptionCtrl();

            ctrl.Plug(
                new Controllers.OptionComponent.TabMultiConf(
                    flyMultiConfPanel,
                    btnMultiConfAdd));

            ctrl.Plug(
                new Controllers.OptionComponent.TabImport(
                    flyImportPanel,
                    btnImportAdd));

            ctrl.Plug(
                new Controllers.OptionComponent.Subscription(
                    flySubsUrlContainer,
                    btnAddSubsUrl,
                    btnUpdateViaSubscription,
                    chkSubsIsUseProxy,
                    chkSubsIsAutoPatch,
                    btnSubsUseAll,
                    btnSubsInvertSelection));

            ctrl.Plug(
                new Controllers.OptionComponent.TabPlugin(
                    flyPluginsItemsContainer));

            ctrl.Plug(
                new Controllers.OptionComponent.TabSetting(
                    cboxSettingLanguage,
                    cboxSettingPageSize,
                    chkSetServAutotrack,
                    tboxSettingsMaxCoreNum,
                    cboxSettingsRandomSelectServerLatency,
                    chkSetSysPortable,
                    chkSetUseV4,
                    chkSetServStatistics,
                    chkSetUpgradeUseProxy,
                    chkSetCheckWhenStart));

            ctrl.Plug(
                new Controllers.OptionComponent.TabDefaults(

                    // def import share link mode
                    cboxDefImportMode,
                    tboxDefImportAddr,
                    chkDefImportSsShareLink,
                    chkDefImportBypassCnSite,
                    chkDefImportInjectGlobalImport,

                    // speedtest 
                    chkDefSpeedtestIsUse,
                    cboxDefSpeedTestUrl,
                    tboxDefSpeedtestCycles,
                    cboxDefSpeedTestExpectedSize,
                    tboxDefSpeedtestTimeout)
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
            this.optionCtrl.SaveAllOptions();
            MessageBox.Show(I18N.Done);
        }

        private void btnBakBackup_Click(object sender, System.EventArgs e)
        {
            optionCtrl.BackupOptions();
        }

        private void btnBakRestore_Click(object sender, System.EventArgs e)
        {
            optionCtrl.RestoreOptions();
        }

        private void flySubsUrlContainer_Scroll(object sender, ScrollEventArgs e)
        {
            flySubsUrlContainer.Refresh();
        }

        private void flyImportPanel_Scroll(object sender, ScrollEventArgs e)
        {
            flyImportPanel.Refresh();
        }

        private void flyPluginsItemsContainer_Scroll(object sender, ScrollEventArgs e)
        {
            flyPluginsItemsContainer.Refresh();
        }
        #endregion

    }
}
