using System.Windows.Forms;
using VgcApis.Interfaces;

namespace V2RayGCon.Views.WinForms
{
    public partial class FormModifyServerSettings : Form
    {
        #region Sigleton
        static FormModifyServerSettings _instant;
        public static void ShowForm(ICoreServCtrl coreServ)
        {
            if (_instant == null || _instant.IsDisposed)
            {
                _instant = new FormModifyServerSettings();
            }
            _instant.InitControls(coreServ);
            _instant.Show();
            _instant.Activate();
        }
        #endregion

        private ICoreServCtrl coreServ;
        VgcApis.Models.Datas.CoreServSettings orgCoreServSettings;
        Services.Servers servers;

        public FormModifyServerSettings()
        {
            InitializeComponent();
            VgcApis.Misc.UI.AutoSetFormIcon(this);
            servers = Services.Servers.Instance;
        }

        private void FormModifyServerSettings_Load(object sender, System.EventArgs e)
        {

        }

        VgcApis.Models.Datas.CoreServSettings GetterSettings()
        {
            var result = new VgcApis.Models.Datas.CoreServSettings();
            result.serverName = tboxServerName.Text;
            result.serverDescription = tboxDescription.Text;
            result.inboundMode = cboxInboundMode.SelectedIndex;
            result.inboundAddress = cboxInboundAddress.Text;
            result.mark = cboxMark.Text;
            result.isAutorun = chkAutoRun.Checked;
            result.isBypassCnSite = chkBypassCnSite.Checked;
            result.isGlobalImport = chkGlobalImport.Checked;
            result.isUntrack = chkUntrack.Checked;
            return result;
        }

        void UpdateControls(VgcApis.Models.Datas.CoreServSettings coreServSettings)
        {
            var s = coreServSettings;
            tboxServerName.Text = s.serverName;
            tboxDescription.Text = s.serverDescription;
            cboxInboundMode.SelectedIndex = s.inboundMode;
            cboxInboundAddress.Text = s.inboundAddress;
            cboxMark.Text = s.mark;
            chkAutoRun.Checked = s.isAutorun;
            chkBypassCnSite.Checked = s.isBypassCnSite;
            chkGlobalImport.Checked = s.isGlobalImport;
            chkUntrack.Checked = s.isUntrack;
        }

        void InitControls(ICoreServCtrl coreServ)
        {
            this.coreServ = coreServ;
            orgCoreServSettings = new VgcApis.Models.Datas.CoreServSettings(coreServ);
            var marks = servers.GetMarkList();

            VgcApis.Misc.UI.RunInUiThread(this, () =>
            {
                this.Text = coreServ.GetCoreStates().GetTitle();
                cboxMark.Items.Clear();
                foreach (var mark in marks)
                {
                    cboxMark.Items.Add(mark);
                }
                Misc.UI.ResetComboBoxDropdownMenuWidth(cboxMark);
                UpdateControls(orgCoreServSettings);
            });
        }

        private void cboxInboundAddress_TextChanged(object sender, System.EventArgs e)
        {
            VgcApis.Misc.UI.MarkInvalidAddressWithColorRed(cboxInboundAddress);
        }

        private void btnSave_Click(object sender, System.EventArgs e)
        {
            var curSettings = GetterSettings();
            if (!curSettings.Equals(orgCoreServSettings))
            {
                coreServ.UpdateCoreSettings(curSettings);
                servers.UpdateMarkList();
            }
            Close();
        }

        private void cboxInboundMode_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            var idx = cboxInboundMode.SelectedIndex;
            cboxInboundAddress.Enabled = idx == 1 || idx == 2;
        }
    }
}
