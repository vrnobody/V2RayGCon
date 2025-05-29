using System;
using System.Windows.Forms;

namespace VgcApis.WinForms
{
    public partial class FormInput : Form
    {
        public string Content = string.Empty;

        public FormInput(string title)
        {
            InitializeComponent();

            Misc.UI.AutoSetFormIcon(this);
            if (!string.IsNullOrEmpty(title))
            {
                lbTitle.Text = $"{title}";
            }
            this.DialogResult = DialogResult.Cancel;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            this.Content = tboxContent.Text;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void tboxContent_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    btnOk.PerformClick();
                    break;
                case Keys.Escape:
                    btnCancel.PerformClick();
                    break;
            }
        }
    }
}
