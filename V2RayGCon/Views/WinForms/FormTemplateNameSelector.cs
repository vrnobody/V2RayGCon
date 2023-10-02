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

        #region private methods
        List<string> selected = new List<string>();

        string[] Split(string str)
        {
            return str?.Replace(", ", ",")
                    ?.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                ?? new string[0];
        }

        string GatherResult()
        {
            return string.Join(",", selected);
        }

        bool suppressEvent = false;

        void CheckBoxChanged(CheckBox cbox)
        {
            if (suppressEvent)
            {
                return;
            }
            var name = cbox.Text;
            selected.Remove(name);
            if (cbox.Checked)
            {
                selected.Add(name);
            }
            tboxNames.Text = GatherResult();
        }
        #endregion

        #region UI events
        private void btnOk_Click(object sender, EventArgs e)
        {
            result = GatherResult();
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void FormTemplateNameSelector_Load(object sender, EventArgs e)
        {
            var table = Split(this.names);
            selected.AddRange(table);
            tboxNames.Text = GatherResult();

            var settings = Services.Settings.Instance;
            var names = settings.GetCustomConfigTemplates().Select(t => t.name);
            foreach (var name in names)
            {
                var chk = new CheckBox
                {
                    Text = name,
                    Checked = table != null && table.Contains(name),
                };
                chk.CheckedChanged += (s, a) => CheckBoxChanged(s as CheckBox);
                flyPanel.Controls.Add(chk);
            }
        }

        private void tboxNames_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
            {
                return;
            }

            e.SuppressKeyPress = true;

            suppressEvent = true;
            selected.Clear();
            selected.AddRange(Split(tboxNames.Text));

            var chks = flyPanel.Controls.OfType<CheckBox>();
            foreach (var chk in chks)
            {
                chk.Checked = selected.Contains(chk.Text);
            }
            suppressEvent = false;
        }
        #endregion
    }
}
