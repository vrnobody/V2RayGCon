using System;
using System.Windows.Forms;
using V2RayGCon.Models.Datas;

namespace V2RayGCon.Views.WinForms
{
    public partial class FormCustomInboundSettings : Form
    {
        public CustomInboundSettings inbSettings;

        public FormCustomInboundSettings()
            : this(new CustomInboundSettings()) { }

        public FormCustomInboundSettings(CustomInboundSettings inbSettings)
        {
            InitializeComponent();

            VgcApis.Misc.UI.AutoSetFormIcon(this);

            this.inbSettings = inbSettings;
            this.DialogResult = DialogResult.Cancel;
        }

        private void FormCustomCoreSettings_Load(object sender, EventArgs e)
        {
            InitControls();
        }

        #region private method
        CustomInboundSettings GatherCoreSettings()
        {
            var cs = new CustomInboundSettings
            {
                name = tboxName.Text,
                template = rtboxTemplate.Text,
                format = cboxFormats.Text,
            };
            return cs;
        }

        void InitControls()
        {
            tboxName.Text = inbSettings.name;
            rtboxTemplate.Text = inbSettings.template;
            cboxFormats.Text = inbSettings.format;
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
            this.inbSettings = GatherCoreSettings();
            this.DialogResult = DialogResult.OK;
            Close();
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            var inbS = GatherCoreSettings();
            var tpl = inbS.GetFormatedTemplate("127.0.0.1", 1080);
            VgcApis.Misc.UI.MsgBoxAsync(tpl);
        }
        #endregion
    }
}
