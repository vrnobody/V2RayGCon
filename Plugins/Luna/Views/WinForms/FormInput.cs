using System;
using System.Windows.Forms;

namespace Luna.Views.WinForms
{
    public partial class FormInput : Form
    {
        readonly int MAX_TITLE_LEN = 60;
        readonly int MAX_LINE_NUM = 25;

        private readonly string title;
        private readonly string content;
        private readonly int lines;


        public FormInput(string title, string content, int lines)
        {
            InitializeComponent();
            this.title = title;
            this.content = content;
            this.lines = lines;
            VgcApis.Misc.UI.AutoSetFormIcon(this);
            AutosetFormHeight();
        }

        private void FormChoice_Load(object sender, EventArgs e)
        {
            InitControls();
        }

        #region public methods
        public string result = null;
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
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
        #endregion


    }
}
