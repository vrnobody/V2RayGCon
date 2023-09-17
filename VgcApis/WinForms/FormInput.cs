using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    }
}
