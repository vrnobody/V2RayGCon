using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Luna.Views.WinForms
{
    public partial class FormChoice : Form
    {
        readonly int MAX_TITLE_LEN = 60;
        readonly int MAX_CHOICE_LEN = 50;
        readonly int MAX_CHOICES_NUM = 18;
        private readonly string title;
        private readonly string[] choices;
        private readonly int defChoice;

        public FormChoice(string title, string[] choices, int defChoice)
        {
            InitializeComponent();
            this.title = title;
            this.choices = choices;
            this.defChoice = defChoice;
            VgcApis.Misc.UI.AutoSetFormIcon(this);
        }

        private void FormChoice_Load(object sender, EventArgs e)
        {
            InitControls();
        }

        #region public methods
        public int result = 0;
        #endregion

        #region private methods
        void SetResult()
        {
            for (int i = 0; i < radioButtons.Count; i++)
            {
                if (radioButtons[i].Checked)
                {
                    result = i + 1;
                }
            }
        }

        List<RadioButton> radioButtons = new List<RadioButton>();
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
                var control = new RadioButton
                {
                    Text = VgcApis.Misc.Utils.AutoEllipsis(choices[i], MAX_CHOICE_LEN),
                    Left = left,
                    Top = h * (i + 1) + margin,
                    AutoSize = true,
                };

                if (defChoice - 1 == i)
                {
                    control.Checked = true;
                }

                toolTip1.SetToolTip(control, choices[i]);

                Controls.Add(control);
                radioButtons.Add(control);
            }
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
