using System;
using System.Threading;
using System.Windows.Forms;

namespace NeoLuna.Views.WinForms
{
    public partial class FormInput : Form, Interfaces.IWinFormControl<string>
    {
        readonly int MAX_TITLE_LEN = 60;
        readonly int MAX_LINE_NUM = 25;
        private readonly AutoResetEvent done;
        private readonly string title;
        private readonly string content;
        private readonly int lines;

        public FormInput(AutoResetEvent done, string title, string content, int lines)
        {
            InitializeComponent();
            this.done = done;
            this.title = title;
            this.content = content;
            this.lines = lines;
            VgcApis.Misc.UI.AutoSetFormIcon(this);
            AutosetFormHeight();
        }

        private void FormInput_Load(object sender, EventArgs e)
        {
            InitControls();
            this.FormClosed += (s, a) => done.Set();
        }

        #region public methods
        string result = null;

        public string GetResult() => result;
        #endregion

        #region private methods
        void AutosetFormHeight()
        {
            var h = VgcApis.Misc.Utils.Clamp(lines, 1, MAX_LINE_NUM + 1);
            var dh = (h - 1) * rtboxInput.ClientRectangle.Height;
            Height = Height + dh;
        }

        void SetResult()
        {
            result = rtboxInput.Text;
        }

        void InitControls()
        {
            lbTitle.Text = VgcApis.Misc.Utils.AutoEllipsis(title, MAX_TITLE_LEN);
            rtboxInput.Text = content ?? string.Empty;
            toolTip1.SetToolTip(lbTitle, title);
            VgcApis.Misc.UI.AddContextMenu(rtboxInput);
        }
        #endregion

        #region UI event handlers
        private void btnOk_Click(object sender, EventArgs e)
        {
            SetResult();
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void FormInput_KeyDown(object sender, KeyEventArgs e)
        {
            var kc = e.KeyCode;
            switch (kc)
            {
                case Keys.Escape:
                    Close();
                    return;
            }
        }
        #endregion
    }
}
