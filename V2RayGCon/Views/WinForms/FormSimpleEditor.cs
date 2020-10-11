using System;
using System.Windows.Forms;
using V2RayGCon.Resources.Resx;

namespace V2RayGCon.Views.WinForms
{
    public partial class FormSimpleEditor : Form
    {

        #region Sigleton
        static FormSimpleEditor _instant;

        public static FormSimpleEditor GetForm()
        {
            if (_instant == null || _instant.IsDisposed)
            {
                VgcApis.Misc.UI.Invoke(
                    () =>
                    {
                        _instant = new FormSimpleEditor();
                        _instant.Show();
                    });
            }
            return _instant;
        }
        #endregion

        Services.Settings setting;
        Services.ShareLinkMgr slinkMgr;

        VgcApis.Interfaces.ICoreServCtrl coreServ = null;

        FormSimpleEditor()
        {
            InitializeComponent();

            setting = Services.Settings.Instance;
            slinkMgr = Services.ShareLinkMgr.Instance;

            VgcApis.Misc.UI.AutoSetFormIcon(this);

            this.FormClosed += (s, a) =>
            {
                setting.LazyGC();
            };
        }

        private void FormSimpleEditor_Load(object sender, EventArgs e)
        {
        }

        public void LoadCoreServer(VgcApis.Interfaces.ICoreServCtrl coreServ)
        {
            this.coreServ = coreServ;
            VgcApis.Misc.UI.Invoke(() =>
            {
                if (this.coreServ == null)
                {
                    lbTitle.Visible = false;
                    linkConfigEditor.Visible = true;
                    return;
                }

                var title = coreServ.GetCoreStates().GetTitle();
                lbTitle.Text = title;
                lbTitle.Visible = true;
                linkConfigEditor.Visible = false;
                var config = coreServ.GetConfiger().GetConfig();
                this.veeConfigerUI1.FromCoreConfig(config);
            });
        }

        #region private methods

        #endregion

        #region UI events
        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            var vee = this.veeConfigerUI1.ToVeeShareLink();

            if (coreServ == null)
            {
                slinkMgr.ImportLinkWithOutV2cfgLinks(vee);
                this.Close();
                return;
            }

            if (VgcApis.Misc.UI.Confirm(I18N.ConfirmClearOrgServCfg))
            {
                var server = Services.Servers.Instance;
                var config = slinkMgr.DecodeShareLinkToConfig(vee);
                var uid = coreServ.GetCoreStates().GetUid();
                server.ReplaceOrAddNewServer(uid, config);
                this.Close();
            }
        }
        private void linkConfigEditor_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            FormConfiger.ShowEmptyConfig();
        }

        #endregion





    }
}
