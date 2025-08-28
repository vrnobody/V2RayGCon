using System;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;
using Commander.Resources.Langs;

namespace Commander.Views
{
    public partial class FormMain : Form
    {
        readonly Services.Settings settings;
        readonly string formTitle;
        Models.Data.CmderParam currentParam;
        bool isClosed = false;

        public static FormMain CreateForm(Services.Settings setting)
        {
            FormMain r = null;
            VgcApis.Misc.UI.Invoke(() =>
            {
                r = new FormMain(setting);
            });
            return r;
        }

        FormMain(Services.Settings settings)
        {
            this.settings = settings;
            InitializeComponent();

            VgcApis.Misc.UI.AutoSetFormIcon(this);
            formTitle = Properties.Resources.Name + " v" + Properties.Resources.Version;
            this.Text = formTitle;
            rtboxLogs.BackColor = tboxArgs.BackColor;
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            currentParam = settings.GetCurrentCmderParam();
            UpdateCmderNames();
            UpdateUiElements();

            lsboxNames.SelectedValueChanged += (s, a) =>
            {
                var name = lsboxNames.Text;
                if (string.IsNullOrEmpty(name))
                {
                    return;
                }
                currentParam = settings.GetCmderParamByName(name);
                UpdateUiElements();
            };
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.isClosed = true;
        }

        #region public methods
        #endregion

        #region private methods
        void SaveCurrentConfig(string name)
        {
            settings.SaveCmderParam(currentParam);
            SetTitle(name);
            UpdateCmderNames();
        }

        private void SaveAsNewConfig()
        {
            var onOk = WrapUserInputHandler(
                (name) =>
                {
                    currentParam.name = name;
                    SaveCurrentConfig(name);
                }
            );
            VgcApis.Misc.UI.GetUserInput(I18N.NewConfigName, onOk);
        }

        Action<string> WrapUserInputHandler(Action<string> action)
        {
            return (s) =>
            {
                if (this.isClosed)
                {
                    return;
                }
                action?.Invoke(s);
            };
        }

        void SetTitle(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                this.Text = formTitle;
            }
            else
            {
                this.Text = $"{formTitle} - {text}";
            }
        }

        void UpdateUiElements()
        {
            var p = currentParam;
            SetTitle(p.name);
            tboxExe.Text = p.exe;
            tboxWrkDir.Text = p.wrkDir;
            tboxArgs.Text = Misc.Utils.ReplaceNewLines(p.args);
            tboxEnvVar.Text = Misc.Utils.ReplaceNewLines(p.envVars);
            cboxStdInEncoding.Text = p.stdInEncoding;
            cboxStdOutEncoding.Text = p.stdOutEncoding;
            chkHideWindow.Checked = p.hideWindow;
            chkWriteStdIn.Checked = p.writeToStdIn;
            rtboxStdInContent.Text = p.stdInContent;
            UpdateStdInContentBackgroundColor();
        }

        void UpdateCmderNames()
        {
            var names = settings.GetCmderParamNames();
            var c = lsboxNames.Items;
            c.Clear();
            c.AddRange(names.ToArray());
        }

        void UpdateStdInContentBackgroundColor()
        {
            rtboxStdInContent.BackColor = currentParam.writeToStdIn
                ? tboxExe.BackColor
                : tboxArgs.BackColor;
        }
        #endregion

        #region UI events

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void chkWriteStdIn_CheckedChanged(object sender, EventArgs e)
        {
            currentParam.writeToStdIn = chkWriteStdIn.Checked;
            UpdateStdInContentBackgroundColor();
        }

        private void pasteToSTDINToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var c = VgcApis.Misc.Utils.ReadFromClipboard();
            rtboxStdInContent.Text = c;
        }

        private void copySTDINToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var c = rtboxStdInContent.Text;
            Misc.UI.CopyToClipboardAndPrompt(c);
        }

        private void copyLogsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var c = rtboxLogs.Text;
            Misc.UI.CopyToClipboardAndPrompt(c);
        }

        private void lbExe_Click(object sender, EventArgs e)
        {
            var exe = VgcApis.Misc.UI.ShowSelectFileDialog(VgcApis.Models.Consts.Files.AllExt);
            if (string.IsNullOrEmpty(exe))
            {
                return;
            }
            tboxExe.Text = exe;
        }

        private void lbWrkDir_Click(object sender, EventArgs e)
        {
            var dir = VgcApis.Misc.UI.ShowSelectFolderDialog();
            if (string.IsNullOrEmpty(dir))
            {
                return;
            }
            tboxWrkDir.Text = dir;
        }

        private void chkHideWindow_CheckedChanged(object sender, EventArgs e)
        {
            currentParam.hideWindow = chkHideWindow.Checked;
        }

        private void rtboxStdInContent_TextChanged(object sender, EventArgs e)
        {
            currentParam.stdInContent = rtboxStdInContent.Text;
        }

        private void lbArgs_Click(object sender, EventArgs e)
        {
            var onOk = WrapUserInputHandler(
                (s) =>
                {
                    currentParam.args = s;
                    UpdateUiElements();
                }
            );
            VgcApis.Misc.UI.GetUserMultiLineInput(I18N.ModifyArgs, currentParam.args, onOk);
        }

        private void lbEnvVar_Click(object sender, EventArgs e)
        {
            var onOk = WrapUserInputHandler(
                (s) =>
                {
                    currentParam.envVars = s;
                    UpdateUiElements();
                }
            );
            VgcApis.Misc.UI.GetUserMultiLineInput(I18N.ModifyEnvVars, currentParam.envVars, onOk);
        }

        private void tboxExe_TextChanged(object sender, EventArgs e)
        {
            currentParam.exe = tboxExe.Text;
        }

        private void tboxWrkDir_TextChanged(object sender, EventArgs e)
        {
            currentParam.wrkDir = tboxWrkDir.Text;
        }

        private void cboxStdInEncoding_TextChanged(object sender, EventArgs e)
        {
            currentParam.stdInEncoding = cboxStdInEncoding.Text;
        }

        private void cboxStdOutEncoding_TextChanged(object sender, EventArgs e)
        {
            currentParam.stdOutEncoding = cboxStdOutEncoding.Text;
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveAsNewConfig();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var name = currentParam.name;
            if (string.IsNullOrEmpty(name))
            {
                SaveAsNewConfig();
                return;
            }
            else
            {
                SaveCurrentConfig(name);
            }
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var name = currentParam.name;
            if (string.IsNullOrEmpty(name))
            {
                VgcApis.Misc.UI.MsgBox(I18N.ConfigNameIsEmpty);
                return;
            }

            var msg = string.Format(I18N.ConfirmDeleteConfig, name);
            if (!VgcApis.Misc.UI.Confirm(msg))
            {
                return;
            }
            settings.RemoveCmderParamByName(name);
            SetTitle("");
            UpdateCmderNames();
        }

        #endregion
    }
}
