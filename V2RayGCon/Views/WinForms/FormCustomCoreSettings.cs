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
            var cs = new CustomCoreSettings
            {
                name = tboxName.Text,
                dir = tboxDir.Text,
                setWorkingDir = chkSetWorkingDir.Checked,
                exe = tboxExe.Text,
                args = cboxArgs.Text,
                stdInEncoding = cboxStdinEncoding.Text,
                stdOutEncoding = cboxStdoutEncoding.Text,
                configFile = cboxConfigFilename.Text,
                useFile = chkUseFile.Checked,
                useStdin = chkUseStdin.Checked
            };
            return cs;
        }

        void InitControls()
        {
            tboxName.Text = coreSettings.name;
            tboxDir.Text = coreSettings.dir;
            chkSetWorkingDir.Checked = coreSettings.setWorkingDir;
            tboxExe.Text = coreSettings.exe;
            cboxArgs.Text = coreSettings.args;
            cboxStdinEncoding.Text = coreSettings.stdInEncoding;
            cboxStdoutEncoding.Text = coreSettings.stdOutEncoding;

            cboxConfigFilename.Text = coreSettings.configFile;
            chkUseFile.Checked = coreSettings.useFile;
            chkUseStdin.Checked = coreSettings.useStdin;
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
