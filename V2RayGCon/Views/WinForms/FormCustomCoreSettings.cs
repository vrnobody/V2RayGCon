using System;
using System.Windows.Forms;
using V2RayGCon.Models.Datas;

namespace V2RayGCon.Views.WinForms
{
    public partial class FormCustomCoreSettings : Form
    {
        public CustomCoreSettings coreSettings;

        public FormCustomCoreSettings()
            : this(new CustomCoreSettings()) { }

        public FormCustomCoreSettings(CustomCoreSettings coreSettings)
        {
            InitializeComponent();
            InitEncodingComboBox();

            VgcApis.Misc.UI.AutoSetFormIcon(this);

            this.coreSettings = coreSettings;
            this.DialogResult = DialogResult.Cancel;
        }

        private void FormCustomCoreSettings_Load(object sender, EventArgs e)
        {
            InitControls();
        }

        #region private method
        CustomCoreSettings GatherCoreSettings()
        {
            var cs = new CustomCoreSettings();
            cs.name = tboxName.Text;
            cs.dir = tboxDir.Text;
            cs.isWorkingDir = chkSetWorkingDir.Checked;
            cs.exe = tboxExe.Text;
            cs.args = tboxArgs.Text;
            cs.stdInEncoding = cboxStdinEncoding.Text;
            cs.stdOutEncoding = cboxStdoutEncoding.Text;
            cs.configFilename = tboxConfigFilename.Text;
            cs.useConfigFile = chkUseFile.Checked;
            cs.useStdInToPassConfig = chkUseStdin.Checked;
            cs.protocols = tboxProtocols.Text;
            cs.isBindToConfigProtocol = chkBindConfig.Checked;
            cs.isBindToShareLinkProtocol = chkBindSharelink.Checked;
            return cs;
        }

        void InitControls()
        {
            tboxName.Text = coreSettings.name;
            tboxDir.Text = coreSettings.dir;
            chkSetWorkingDir.Checked = coreSettings.isWorkingDir;
            tboxExe.Text = coreSettings.exe;
            tboxArgs.Text = coreSettings.args;
            cboxStdinEncoding.Text = coreSettings.stdInEncoding;
            cboxStdoutEncoding.Text = coreSettings.stdOutEncoding;

            tboxConfigFilename.Text = coreSettings.configFilename;
            chkUseFile.Checked = coreSettings.useConfigFile;
            chkUseStdin.Checked = coreSettings.useStdInToPassConfig;

            tboxProtocols.Text = coreSettings.protocols;
            chkBindConfig.Checked = coreSettings.isBindToConfigProtocol;
            chkBindSharelink.Checked = coreSettings.isBindToShareLinkProtocol;
        }

        void InitEncodingComboBox()
        {
            var encodings = CustomCoreSettings.GetEncodings().ToArray();

            cboxStdinEncoding.Items.AddRange(encodings);
            cboxStdoutEncoding.Items.AddRange(encodings);
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
            this.coreSettings = GatherCoreSettings();
            this.DialogResult = DialogResult.OK;
            Close();
        }

        private void btnDir_Click(object sender, EventArgs e)
        {
            var folder = VgcApis.Misc.UI.ShowSelectFolderDialog();
            if (!string.IsNullOrEmpty(folder))
            {
                tboxDir.Text = folder;
            }
        }

        #endregion
    }
}
