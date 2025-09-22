using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Windows.Forms;
using System.Xml.Linq;
using Commander.Resources.Langs;
using static System.Net.Mime.MediaTypeNames;

namespace Commander.Views
{
    public partial class FormMain : Form
    {
        readonly Services.Settings settings;
        readonly Services.Server server;
        readonly string formTitle;
        Models.Data.CmderParam currentParam;
        bool isClosed = false;
        readonly VgcApis.Libs.Tasks.Routine logUpdater;
        long lastLogUpdateTimestamp = 0;

        public static FormMain CreateForm(Services.Settings setting, Services.Server server)
        {
            FormMain r = null;
            VgcApis.Misc.UI.Invoke(() =>
            {
                r = new FormMain(setting, server);
            });
            return r;
        }

        FormMain(Services.Settings settings, Services.Server server)
        {
            this.settings = settings;
            this.server = server;
            InitializeComponent();

            VgcApis.Misc.UI.AutoSetFormIcon(this);
            formTitle = Properties.Resources.Name + " v" + Properties.Resources.Version;
            logUpdater = new VgcApis.Libs.Tasks.Routine(UpdateLog, 1000);
        }

        private void FormMain_Shown(object sender, EventArgs e)
        {
            this.Text = formTitle;
            rtboxLogs.BackColor = tboxArgs.BackColor;
            rtboxLogs.ReadOnly = true;

            currentParam = settings.GetFirstCmderParam();
            UpdateCmderNames();
            UpdateUiElements();

            lsboxNames.SelectedValueChanged += (s, a) =>
            {
                var name = lsboxNames.Text;
                if (string.IsNullOrEmpty(name))
                {
                    return;
                }
                currentParam = settings.GetCmderParamByName(name) ?? new Models.Data.CmderParam();
                UpdateUiElements();
            };
            logUpdater.Restart();
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.isClosed = true;
            logUpdater.Dispose();
        }

        #region hotkeys
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case (Keys.Control | Keys.K):
                    TryFormatStdIn();
                    return true;
                case Keys.F5:
                    RunCurrentConfig();
                    return true;
                case Keys.F8:
                    ClearLogs();
                    return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }
        #endregion

        #region private methods
        void TryFormatStdIn()
        {
            try
            {
                var text = VgcApis.Misc.Utils.FormatConfig(rtboxStdInContent.Text);
                rtboxStdInContent.Text = text;
            }
            catch (Exception ex)
            {
                VgcApis.Misc.UI.MsgBox(ex.Message);
            }
        }

        void ClearLogs()
        {
            server.ClearLogs();
            // incase log updater is paused
            VgcApis.Misc.UI.Invoke(() => rtboxLogs.Text = "");
        }

        void RunCurrentConfig()
        {
            server.Start(currentParam);
        }

        void GenSubMenu(ToolStripMenuItem root, IEnumerable<string> names, Action<string> action)
        {
            var c = root.DropDownItems;
            c.Clear();
            foreach (var name in names)
            {
                var n = name;
                var mi = new ToolStripMenuItem(n);
                mi.Click += (s, a) => action?.Invoke(name);
                c.Add(mi);
            }
        }

        void UpdateLog()
        {
            var timestamp = server.GetLogTimestamp();
            if (lastLogUpdateTimestamp == timestamp)
            {
                return;
            }
            lastLogUpdateTimestamp = timestamp;
            var logs = server.GetLogs();
            VgcApis.Misc.UI.Invoke(() =>
            {
                rtboxLogs.Text = logs;
                VgcApis.Misc.UI.ScrollToBottom(rtboxLogs);
            });
        }

        void OverwriteCurrentConfig(string name)
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
                    OverwriteCurrentConfig(name);
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
                VgcApis.Misc.UI.Invoke(() => action?.Invoke(s));
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
            tboxArgs.Text = Misc.Utils.TrimComments(p.args);
            tboxEnvVar.Text = Misc.Utils.TrimComments(p.envVars);
            cboxStdInEncoding.Text = p.stdInEncoding;
            cboxStdOutEncoding.Text = p.stdOutEncoding;
            chkHideWindow.Checked = p.hideWindow;
            chkUseShell.Checked = p.useShell;
            chkWriteStdIn.Checked = p.writeToStdIn;
            rtboxStdInContent.Text = p.stdInContent;
            UpdateStdInContentBoxLater();
        }

        void UpdateCmderNames()
        {
            var names = settings.GetCmderParamNames();
            var c = lsboxNames.Items;
            c.Clear();
            c.AddRange(names.ToArray());
        }

        void UpdateStdInContentBoxLater()
        {
            // readonly will block color changing
            rtboxStdInContent.ReadOnly = false;

            var disabled = !currentParam.writeToStdIn;
            rtboxStdInContent.BackColor = disabled ? tboxArgs.BackColor : tboxExe.BackColor;
            rtboxStdInContent.ReadOnly = disabled;
        }

        void UpdateChkWriteStdin()
        {
            var disabled = chkUseShell.Checked || !chkHideWindow.Checked;
            chkWriteStdIn.Enabled = !disabled;

            if (disabled)
            {
                chkWriteStdIn.Checked = false;
            }
        }
        #endregion

        #region UI events
        private void chkWriteStdIn_CheckedChanged(object sender, EventArgs e)
        {
            currentParam.writeToStdIn = chkWriteStdIn.Checked;
            UpdateStdInContentBoxLater();
        }

        private void chkUseShell_CheckedChanged(object sender, EventArgs e)
        {
            currentParam.useShell = chkUseShell.Checked;
            UpdateChkWriteStdin();
        }

        private void chkHideWindow_CheckedChanged(object sender, EventArgs e)
        {
            currentParam.hideWindow = chkHideWindow.Checked;
            UpdateChkWriteStdin();
        }

        private void rtboxStdInContent_TextChanged(object sender, EventArgs e)
        {
            currentParam.stdInContent = rtboxStdInContent.Text;
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

        private void startToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RunCurrentConfig();
        }

        private void runToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            var cfgs = settings.GetCmderParamNames();
            startToolStripMenuItem.Enabled = cfgs.Count > 0;
            GenSubMenu(startToolStripMenuItem, cfgs, (name) => server.Start(name));

            var procs = server.GetNames();
            stopToolStripMenuItem.Enabled = procs.Count > 0;
            killToolStripMenuItem.Enabled = procs.Count > 0;
            GenSubMenu(stopToolStripMenuItem, procs, (name) => server.Stop(name));
            GenSubMenu(killToolStripMenuItem, procs, (name) => server.Kill(name));
        }

        private void pauseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var paused = pauseToolStripMenuItem.Checked;
            pauseToolStripMenuItem.Checked = !paused;
            if (paused)
            {
                logUpdater.Restart();
            }
            else
            {
                logUpdater.Stop();
            }
        }

        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (VgcApis.Misc.UI.Confirm(I18N.ConfirmClearLogs))
            {
                ClearLogs();
            }
        }

        private void saveToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            var name = currentParam.name;
            if (string.IsNullOrEmpty(name))
            {
                SaveAsNewConfig();
                return;
            }
            else
            {
                OverwriteCurrentConfig(name);
            }
        }

        private void saveAsToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SaveAsNewConfig();
        }

        private void deleteToolStripMenuItem1_Click(object sender, EventArgs e)
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
            var ok = settings.RemoveCmderParamByName(name);
            if (ok)
            {
                SetTitle("");
                UpdateCmderNames();
            }
            else
            {
                msg = string.Format(I18N.FindNoConfigWihtName, name);
                VgcApis.Misc.UI.MsgBox(msg);
            }
        }

        private void closeToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void copyToClipboardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var c = rtboxLogs.Text;
            Misc.UI.CopyToClipboardAndPrompt(c);
        }

        private void clearContentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            rtboxStdInContent.Text = "";
        }

        private void copyToClipboardToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            var c = rtboxStdInContent.Text;
            Misc.UI.CopyToClipboardAndPrompt(c);
        }

        private void pasteFromClipboardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var c = VgcApis.Misc.Utils.ReadFromClipboard();
            rtboxStdInContent.Text = c;
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currentParam = new Models.Data.CmderParam();
            UpdateUiElements();
        }

        private void btnExe_Click(object sender, EventArgs e)
        {
            var exe = VgcApis.Misc.UI.ShowSelectFileDialog(VgcApis.Models.Consts.Files.AllExt);
            if (string.IsNullOrEmpty(exe))
            {
                return;
            }
            tboxExe.Text = exe;
        }

        private void btnWrkDir_Click(object sender, EventArgs e)
        {
            var dir = VgcApis.Misc.UI.ShowSelectFolderDialog();
            if (string.IsNullOrEmpty(dir))
            {
                return;
            }
            tboxWrkDir.Text = dir;
        }

        private void btnArgs_Click(object sender, EventArgs e)
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

        private void btnEnvVars_Click(object sender, EventArgs e)
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
        #endregion
    }
}
