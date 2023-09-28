using System;
using System.Windows.Forms;
using V2RayGCon.Resources.Resx;

namespace V2RayGCon.Views.WinForms
{
    public partial class FormSimpleConfiger : Form
    {
        public string shareLink;

        public FormSimpleConfiger()
        {
            InitializeComponent();
            VgcApis.Misc.UI.AutoSetFormIcon(this);
            this.DialogResult = DialogResult.Cancel;
            this.SimpleConfigerUI1.ToggleTextBoxName(false);
        }

        private void FormSimpleEditor_Load(object sender, EventArgs e) { }

        public void LoadConfig(string config)
        {
            VgcApis.Misc.UI.Invoke(() => SimpleConfigerUI1.FromCoreConfig(config));
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
            this.shareLink = this.SimpleConfigerUI1.ToVeeShareLink();
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        #endregion
    }
}
