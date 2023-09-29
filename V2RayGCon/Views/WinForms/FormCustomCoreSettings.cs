using System;
using System.Linq;
using System.Windows.Forms;
using V2RayGCon.Models.Datas;

namespace V2RayGCon.Views.WinForms
{
    public partial class FormCustomCoreSettings : Form
    {
        public CustomCoreSettings coreSettings;

        Services.Settings settings;

        public FormCustomCoreSettings()
            : this(new CustomCoreSettings()) { }

        public FormCustomCoreSettings(CustomCoreSettings coreSettings)
        {
            InitializeComponent();

            this.settings = Services.Settings.Instance;

            this.coreSettings = coreSettings;
            this.DialogResult = DialogResult.Cancel;

            VgcApis.Misc.UI.AutoSetFormIcon(this);
            InitComboBoxes();
        }

        private void FormCustomCoreSettings_Load(object sender, EventArgs e)
        {
            InitControls();
        }

        #region private method
        string GetCboxSpeedtestInbTplNameText()
        {
            if (cboxSpeedtestInbTplName.SelectedIndex < 1)
            {
                return string.Empty;
            }
            return cboxSpeedtestInbTplName.Text;
        }

        CustomCoreSettings GatherCoreSettings()
        {
            var cs = new CustomCoreSettings
            {
                name = tboxName.Text,
                dir = tboxDir.Text,
                setWorkingDir = chkSetWorkingDir.Checked,
                exe = tboxExe.Text,
                args = cboxArgs.Text,
                envs = tboxEnvVars.Text,
                stdInEncoding = cboxStdinEncoding.Text,
                stdOutEncoding = cboxStdoutEncoding.Text,
                configFile = cboxConfigFilename.Text,
                useFile = chkUseFile.Checked,
                useStdin = chkUseStdin.Checked,
                speedtestInboundTemplateName = GetCboxSpeedtestInbTplNameText(),
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
            tboxEnvVars.Text = coreSettings.envs;

            cboxStdinEncoding.Text = coreSettings.stdInEncoding;
            cboxStdoutEncoding.Text = coreSettings.stdOutEncoding;

            cboxConfigFilename.Text = coreSettings.configFile;
            chkUseFile.Checked = coreSettings.useFile;
            chkUseStdin.Checked = coreSettings.useStdin;

            VgcApis.Misc.UI.SelectComboxByText(
                cboxSpeedtestInbTplName,
                coreSettings.speedtestInboundTemplateName
            );
        }

        void InitComboBoxes()
        {
            var encodings = CustomCoreSettings.GetEncodings().ToArray();
            cboxStdinEncoding.Items.AddRange(encodings);
            cboxStdoutEncoding.Items.AddRange(encodings);

            var inbNames = settings.GetCustomInboundsSetting().Select(inb => inb.name).ToArray();
            cboxSpeedtestInbTplName.Items.AddRange(inbNames);

            foreach (
                var cbox in new ComboBox[]
                {
                    cboxStdinEncoding,
                    cboxStdoutEncoding,
                    cboxSpeedtestInbTplName,
                }
            )
            {
                VgcApis.Misc.UI.ResetComboBoxDropdownMenuWidth(cbox);
            }
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
