using System;
using System.Windows.Forms;

namespace VgcApis.WinForms
{
    public partial class FormMultiLineInput : Form
    {
        public string Content = string.Empty;

        public FormMultiLineInput(string title, string content)
        {
            InitializeComponent();

            Misc.UI.AutoSetFormIcon(this);

            VgcApis.Misc.UI.AddContextMenu(rtboxContent);
            this.rtboxContent.Text = content;
            if (!string.IsNullOrEmpty(title))
            {
                lbTitle.Text = $"{title}";
            }

            this.DialogResult = DialogResult.Cancel;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            this.Content = rtboxContent.Text;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void FormMultiLineInput_KeyDown(object sender, KeyEventArgs e)
        {
            var kc = e.KeyCode;
            switch (kc)
            {
                case Keys.S:
                    if (e.Control)
                    {
                        btnOk.PerformClick();
                    }
                    return;
                case Keys.Escape:
                    btnCancel.PerformClick();
                    return;
            }
        }
    }
}
