using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace V2RayGCon.Views.WinForms
{
    public partial class FormTemplateNameSelector : Form
    {
        public string result = string.Empty;
        private readonly string names;

        public FormTemplateNameSelector(string names)
        {
            InitializeComponent();

            VgcApis.Misc.UI.AutoSetFormIcon(this);
            DialogResult = DialogResult.Cancel;
            this.names = names;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            var chks = flyPanel.Controls.OfType<CheckBox>();
            var s = chks.Where(c => c.Checked).Select(c => c.Text);
            result = string.Join(", ", s);
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void FormTemplateNameSelector_Load(object sender, EventArgs e)
        {
            var table = this.names?.Replace(", ", ",")?.Split(',');

            var settings = Services.Settings.Instance;
            var names = settings.GetCustomConfigTemplates().Select(t => t.name);
            foreach (var name in names)
            {
                var chk = new CheckBox
                {
                    Text = name,
                    Checked = table != null && table.Contains(name),
                };
                flyPanel.Controls.Add(chk);
            }
        }
    }
}
