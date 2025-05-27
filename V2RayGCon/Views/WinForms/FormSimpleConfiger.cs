using System;
using System.Windows.Forms;
using V2RayGCon.Resources.Resx;
using V2RayGCon.Services;
using static ScintillaNET.Style;

namespace V2RayGCon.Views.WinForms
{
    public partial class FormSimpleConfiger : Form
    {
        public string config;

        public FormSimpleConfiger()
        {
            InitializeComponent();
            VgcApis.Misc.UI.AutoSetFormIcon(this);
            VgcApis.Misc.UI.AddTagToFormTitle(this);
            this.DialogResult = DialogResult.Cancel;
        }

        private void FormSimpleEditor_Load(object sender, EventArgs e) { }

        public void LoadConfig(string name, string config)
        {
            VgcApis.Misc.UI.Invoke(() =>
            {
                SimpleConfigerUI1.SetTitle(name);
                SimpleConfigerUI1.FromCoreConfig(config);
            });
        }

        #region private methods
        void ShowResult(string result)
        {
            if (result == null)
            {
                VgcApis.Misc.UI.MsgBox(I18N.DecodeFail);
                return;
            }
            this.config = result;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        #endregion

        #region UI events
        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnServer_Click(object sender, EventArgs e)
        {
            var meta = this.SimpleConfigerUI1.ToShareLinkMetaData();
            var r = ShareLinkMgr.Instance.ToServerConfig(meta);
            ShowResult(r);
        }

        #endregion

        private void btnClient_Click(object sender, EventArgs e)
        {
            var meta = this.SimpleConfigerUI1.ToShareLinkMetaData();
            var shareLink = meta.ToShareLink();
            var r = ShareLinkMgr.Instance.DecodeShareLinkToConfig(shareLink)?.config;
            ShowResult(r);
        }
    }
}
