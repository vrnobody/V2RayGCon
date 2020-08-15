using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

namespace Luna.Views.WinForms
{
    public partial class FormChoice :
        Form,
        VgcApis.Interfaces.Lua.IWinFormControl<int>
    {
        readonly int MAX_TITLE_LEN = 60;
        readonly int MAX_CHOICE_LEN = 50;
        readonly int MAX_CHOICES_NUM = 18;

        int result = -1;
        private readonly AutoResetEvent done;
        private readonly string title;
        private readonly string[] choices;
        private readonly int defChoice;

        public FormChoice(
            AutoResetEvent done,
            string title, string[] choices, int defChoice)
        {
            InitializeComponent();
            this.done = done;
            this.title = title;
            this.choices = choices;
            this.defChoice = defChoice;
            VgcApis.Misc.UI.AutoSetFormIcon(this);
        }

        private void FormChoice_Load(object sender, EventArgs e)
        {
            InitControls();
            this.FormClosed += (s, a) => done.Set();
        }

        private void FormChoice_Shown(object sender, EventArgs e)
        {
            Choose(this.defChoice - 1);
        }

        #region public methods
        public int GetResult() => result;

        #endregion

        #region private methods
        bool Choose(int idx)
        {
            var len = radioButtons.Count;
            if (idx >= len || idx < 0)
            {
                return false;
            }

            var btn = radioButtons[idx];
            btn.Checked = true;
            VgcApis.Misc.UI.Invoke(() =>
            {
                btnOk.Focus();
            });
            return true;
        }

        void SetResult()
        {
            for (int i = 0; i < radioButtons.Count; i++)
            {
                if (radioButtons[i].Checked)
                {
                    result = i + 1;
                    return;
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
                RadioButton control = CreateOneChoiceCtrl(margin, left, h, i);

                Controls.Add(control);
                radioButtons.Add(control);
            }
        }

        private RadioButton CreateOneChoiceCtrl(int margin, int left, int h, int i)
        {
            var control = new RadioButton
            {
                Text = VgcApis.Misc.Utils.AutoEllipsis(choices[i], MAX_CHOICE_LEN),
                Left = left,
                Top = h * (i + 1) + margin,
                AutoSize = true,
            };

            MethodInfo m = typeof(RadioButton).GetMethod("SetStyle", BindingFlags.Instance | BindingFlags.NonPublic);
            if (m != null)
            {
                m.Invoke(control, new object[] { ControlStyles.StandardClick | ControlStyles.StandardDoubleClick, true });
            }

            control.MouseDoubleClick += (s, a) => VgcApis.Misc.UI.Invoke(btnOk.PerformClick);

            toolTip1.SetToolTip(control, choices[i]);

            return control;
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

        private void FormChoice_KeyDown(object sender, KeyEventArgs e)
        {
            var kc = e.KeyCode;
            switch (kc)
            {
                case Keys.Escape:
                    VgcApis.Misc.UI.CloseFormIgnoreError(this);
                    return;
                case Keys.Enter:
                    // VgcApis.Misc.UI.Invoke(btnOk.PerformClick);
                    return;
                case Keys.D0:
                    Choose(9);
                    return;
            }

            if (kc >= Keys.D1 && kc <= Keys.D9)
            {
                Choose(kc - Keys.D1);
                return;
            }
        }


        #endregion


    }
}
