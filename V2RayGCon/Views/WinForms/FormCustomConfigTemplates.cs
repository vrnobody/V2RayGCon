using System;
using System.Windows.Forms;
using V2RayGCon.Models.Datas;

namespace V2RayGCon.Views.WinForms
{
    public partial class FormCustomConfigTemplates : Form
    {
        public CustomConfigTemplate inbS;

        public FormCustomConfigTemplates()
            : this(new CustomConfigTemplate()) { }

        public FormCustomConfigTemplates(CustomConfigTemplate inboundSettings)
        {
            InitializeComponent();

            VgcApis.Misc.UI.AutoSetFormIcon(this);

            this.inbS = inboundSettings;
            this.DialogResult = DialogResult.Cancel;
        }

        private void FormCustomCoreSettings_Load(object sender, EventArgs e)
        {
            InitControls();
        }

        #region private method
        void GatherCoreSettings()
        {
            inbS.name = tboxName.Text;
            inbS.template = rtboxTemplate.Text;
            inbS.jsonArrMergeOption = cboxMergeOption.Text;
            inbS.isSocks5Inbound = chkIsSocks5Inbound.Checked;
            inbS.mergeParams = tboxMergeParams.Text;
        }

        void InitControls()
        {
            tboxName.Text = inbS.name;
            rtboxTemplate.Text = inbS.template;
            chkIsSocks5Inbound.Checked = inbS.isSocks5Inbound;
            tboxMergeParams.Text = inbS.mergeParams;

            var opts = CustomConfigTemplate.GetJsonArrayMergeOptions();
            cboxMergeOption.Items.AddRange(opts.ToArray());
            var def = inbS.GetJsonArrMergeOption();
            VgcApis.Misc.UI.SelectComboxByText(cboxMergeOption, def);
        }

        #endregion

        #region bind hotkey
        protected override bool ProcessCmdKey(ref Message msg, Keys keyCode)
        {
            switch (keyCode)
            {
                case (Keys.Control | Keys.K):
                    btnFormat.PerformClick();
                    break;
                default:
                    return base.ProcessCmdKey(ref msg, keyCode);
            }
            return true;
        }
        #endregion

        #region public methods
        #endregion

        #region UI event handler
        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            GatherCoreSettings();
            this.DialogResult = DialogResult.OK;
            Close();
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            GatherCoreSettings();
            var tpl = inbS.GetFormatedTemplate("127.0.0.1", 1080);
            VgcApis.Misc.UI.MsgBoxAsync(tpl);
        }

        private void btnFormat_Click(object sender, EventArgs e)
        {
            var src = "%port%";
            var rep = "\"%port%\"";
            var txt = rtboxTemplate.Text?.Replace(src, rep);
            try
            {
                txt = VgcApis.Misc.Utils.FormatConfig(txt)?.Replace(rep, src);
                if (!string.IsNullOrEmpty(txt))
                {
                    rtboxTemplate.Text = txt;
                }
            }
            catch (Exception ex)
            {
                VgcApis.Misc.UI.MsgBox(ex.Message);
            }
        }

        private void cboxMergeOption_TextChanged(object sender, EventArgs e)
        {
            var enabled = cboxMergeOption.Text == CustomConfigTemplate.MergeOptionOutbound;
            tboxMergeParams.Enabled = enabled;
        }
        #endregion
    }
}
