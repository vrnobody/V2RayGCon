using System;
using System.Windows.Forms;
using V2RayGCon.Resources.Resx;
using V2RayGCon.Services;

namespace V2RayGCon.Views.WinForms
{
    public partial class FormTextConfigEditor : Form
    {
        Controllers.FormTextConfigEditorCtrl ctrl;
        private readonly bool isReadonly;
        private readonly string formTitle;
        private readonly Settings setting;

        FormSimpleConfiger simpleConfiger = null;

        FormTextConfigEditor(bool isReadonly)
        {
            this.isReadonly = isReadonly;
            setting = Settings.Instance;

            InitializeComponent();
            this.formTitle = this.Text;

            VgcApis.Misc.UI.AutoSetFormIcon(this);
        }

        private void FormTextConfigEditor_Load(object sender, EventArgs e)
        {
            setting.RestoreFormRect(this);

            InitFormController();

            this.FormClosing += (s, a) =>
            {
                if (ctrl.IsConfigChanged())
                {
                    a.Cancel = !VgcApis.Misc.UI.Confirm(I18N.ConfirmCloseWinWithoutSave);
                }
            };

            this.FormClosed += (s, a) =>
            {
                simpleConfiger?.Close();
                ctrl.Cleanup();
                setting.SaveFormRect(this);
            };
        }

        #region static methods
        public static FormTextConfigEditor ShowEmptyConfig() => ShowConfig("", "", false);

        public static FormTextConfigEditor ShowConfig(string title, string config, bool isReadonly)
        {
            FormTextConfigEditor f = null;
            VgcApis.Misc.UI.Invoke(() =>
            {
                f = new FormTextConfigEditor(isReadonly);
                f.Show();
                f.ctrl.LoadConfig(config);
                f.SetTitle(title);
            });
            return f;
        }

        public static FormTextConfigEditor ShowServer(string uid)
        {
            FormTextConfigEditor f = null;
            VgcApis.Misc.UI.Invoke(() =>
            {
                f = new FormTextConfigEditor(false);
                f.Show();
                f.ctrl.LoadConfigByUid(uid);
            });
            return f;
        }

        public void SetTitle(string title)
        {
            VgcApis.Misc.UI.Invoke(() =>
            {
                if (string.IsNullOrEmpty(title))
                {
                    this.Text = formTitle;
                }
                else
                {
                    this.Text = string.Format("{0} - {1}", formTitle, title);
                }
            });
        }
        #endregion

        #region private methods
        private void InitFormController()
        {
            var editor = new Controllers.FormTextConfigEditorComponent.Editor();
            var mu = new Controllers.FormTextConfigEditorComponent.MenuUpdater();

            ctrl = new Controllers.FormTextConfigEditorCtrl(this);
            ctrl.Plug(editor).Plug(mu);

            // plug之后才可以init
            editor.Init(pnlEditor, isReadonly, toolStripCboxNavigtion);

            mu.Init(
                serverToolStripMenuItem,
                overwriteServerToolStripMenuItem,
                loadConfigToolStripMenuItem
            );

            ctrl.Init();
        }
        #endregion

        #region bind hotkey
        protected override bool ProcessCmdKey(ref Message msg, Keys keyCode)
        {
            switch (keyCode)
            {
                case (Keys.Control | Keys.K):
                    ctrl.GetEditor().Format();
                    break;
                case (Keys.Control | Keys.F):
                case (Keys.Control | Keys.H):
                    ctrl.GetEditor().ShowSearchBox();
                    break;
                case (Keys.Control | Keys.OemOpenBrackets):
                    ctrl.GetEditor().ZoomOut();
                    break;
                case (Keys.Control | Keys.Oem6):
                    ctrl.GetEditor().ZoomIn();
                    break;
            }
            return base.ProcessCmdKey(ref msg, keyCode);
        }
        #endregion

        #region UI
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void searchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ctrl.GetEditor().ShowSearchBox();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowEmptyConfig();
        }

        private void addNewServerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (VgcApis.Misc.UI.Confirm(I18N.AddNewServer))
            {
                ctrl.AddNewServer();
            }
        }

        private void overwriteCurrentServerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (VgcApis.Misc.UI.Confirm(I18N.ConfirmSaveCurConfig))
            {
                ctrl.OverwriteCurServer();
            }
        }

        private void loadFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ctrl.IsConfigChanged() && !VgcApis.Misc.UI.Confirm(I18N.ConfirmLoadNewServer))
            {
                return;
            }

            var tuple = VgcApis.Misc.UI.ReadFileFromDialog(VgcApis.Models.Consts.Files.AllExt);

            var json = tuple.Item1;
            var filename = tuple.Item2;

            // user cancelled.
            if (json == null)
            {
                return;
            }

            ctrl.LoadConfig(json);
            SetTitle(filename);
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var filename = ctrl.SaveToFile();
            if (!string.IsNullOrEmpty(filename))
            {
                SetTitle(filename);
            }
        }

        private void formatToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ctrl.GetEditor().Format();
        }

        private void simpleConfigerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (simpleConfiger == null)
            {
                simpleConfiger = new FormSimpleConfiger();
                simpleConfiger.FormClosed += (s, a) =>
                {
                    if (simpleConfiger.DialogResult == DialogResult.OK)
                    {
                        var shareLink = simpleConfiger.shareLink;
                        var r = ShareLinkMgr.Instance.DecodeShareLinkToConfig(shareLink);
                        ctrl.GetEditor().content = r.config;
                    }
                    simpleConfiger = null;
                };
                simpleConfiger.LoadConfig(ctrl.GetEditor().content);
                simpleConfiger.Show();
            }
            else
            {
                simpleConfiger.Activate();
            }
        }
        #endregion
    }
}
