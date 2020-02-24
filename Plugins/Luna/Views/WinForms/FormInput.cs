using System;
using System.Windows.Forms;

namespace Luna.Views.WinForms
{
    public partial class FormInput : Form
    {
        readonly int MAX_TITLE_LEN = 60;
        private readonly string title;

        public FormInput(string title)
        {
            InitializeComponent();
            this.title = title;
            VgcApis.Misc.UI.AutoSetFormIcon(this);
        }

        private void FormChoice_Load(object sender, EventArgs e)
        {
            InitControls();
        }

        #region public methods
        public string result = null;
        #endregion

        #region private methods
        void SetResult()
        {
            result = rtboxInput.Text;
        }

        void InitControls()
        {
            lbTitle.Text = VgcApis.Misc.Utils.AutoEllipsis(title, MAX_TITLE_LEN);
        }
        #endregion

        #region UI event handlers
        private void btnOk_Click(object sender, EventArgs e)
        {
            SetResult();
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
        #endregion


    }
}
