using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using Commander.Services;
using VgcApis.Models.Composer;

namespace V2RayGCon.Views.WinForms
{
    public partial class FormShareLinkTypesSelector : Form
    {
        private readonly Services.Settings settings;

        public FormShareLinkTypesSelector()
        {
            InitializeComponent();

            VgcApis.Misc.UI.AutoSetFormIcon(this);

            settings = Services.Settings.Instance;
            InitControls();
        }

        #region private methods
        void InitControls()
        {
            chkVless.Checked = settings.CustomDefImportVlessShareLink;
            chkTrojan.Checked = settings.CustomDefImportTrojanShareLink;
            chkMob.Checked = settings.CustomDefImportMobShareLink;
            chkVmess.Checked = settings.CustomDefImportVmessShareLink;
            chkShadowsocks.Checked = settings.CustomDefImportSsShareLink;
            chkSocks.Checked = settings.CustomDefImportSocksShareLink;
        }

        #endregion

        #region UI events
        private void btnSave_Click(object sender, EventArgs e)
        {
            settings.CustomDefImportVlessShareLink = chkVless.Checked;
            settings.CustomDefImportTrojanShareLink = chkTrojan.Checked;
            settings.CustomDefImportMobShareLink = chkMob.Checked;
            settings.CustomDefImportVmessShareLink = chkVmess.Checked;
            settings.CustomDefImportSsShareLink = chkShadowsocks.Checked;
            settings.CustomDefImportSocksShareLink = chkSocks.Checked;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        #endregion
    }
}
