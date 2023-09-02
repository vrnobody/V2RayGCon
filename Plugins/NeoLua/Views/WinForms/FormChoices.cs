using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;

namespace NeoLuna.Views.WinForms
{
    public partial class FormChoices : Form, Interfaces.IWinFormControl<List<int>>
    {
        readonly int MAX_TITLE_LEN = 60;
        readonly int MAX_CHOICE_LEN = 50;
        readonly int MAX_CHOICES_NUM = 18;
        private readonly AutoResetEvent done;
        private readonly string title;
        private readonly string[] choices;

        public FormChoices(AutoResetEvent done, string title, string[] choices)
        {
            InitializeComponent();
            this.done = done;
            this.title = title;
            this.choices = choices;
            VgcApis.Misc.UI.AutoSetFormIcon(this);
        }

        private void FormChoice_Load(object sender, EventArgs e)
        {
            InitControls();
            checkBoxes[0].Focus();
            this.FormClosed += (s, a) => done.Set();
        }

        #region public methods
        List<int> results = null;

        public List<int> GetResult() => results;
        #endregion

        #region private methods
        void SetResult()
        {
            results = new List<int>();
            for (int i = 0; i < checkBoxes.Count; i++)
            {
                if (checkBoxes[i].Checked)
                {
                    results.Add(i + 1);
                }
            }
        }

        List<CheckBox> checkBoxes = new List<CheckBox>();

        void InitControls()
        {
            lbTitle.Text = VgcApis.Misc.Utils.AutoEllipsis(title, MAX_TITLE_LEN);
            toolTip1.SetToolTip(lbTitle, title);

            var margin = lbTitle.Top;
            var left = lbTitle.Left * 2;
            var h = lbTitle.Height + margin;

            var num = Math.Min(choices.Length, MAX_CHOICES_NUM);
            var clientRectHeight = h * (num + 1) + btnOk.Height + margin * 2;
            Height = Height - (ClientRectangle.Height - clientRectHeight);

            btnOk.Top = clientRectHeight - margin - btnOk.Height;
            btnCancel.Top = btnOk.Top;

            for (int i = 0; i < num; i++)
            {
                var control = new CheckBox
                {
                    Text = VgcApis.Misc.Utils.AutoEllipsis(choices[i], MAX_CHOICE_LEN),
                    Left = left,
                    Top = h * (i + 1) + margin,
                    AutoSize = true,
                };
                toolTip1.SetToolTip(control, choices[i]);

                control.KeyDown += (s, a) =>
                {
                    if (a.KeyCode == Keys.Enter)
                    {
                        VgcApis.Misc.UI.Invoke(btnOk.PerformClick);
                    }
                };

                Controls.Add(control);
                checkBoxes.Add(control);
            }
        }
        #endregion

        #region UI event handlers
        private void btnOk_Click(object sender, EventArgs e)
        {
            SetResult();
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void FormChoices_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                VgcApis.Misc.UI.CloseFormIgnoreError(this);
            }
        }
        #endregion
    }
}
