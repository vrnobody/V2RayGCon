using NeoLuna.Models.Data;
using NeoLuna.Resources.Langs;
using System.Windows.Forms;

namespace NeoLuna.Views.WinForms
{
    internal partial class FormEditor : Form
    {
        Controllers.FormEditorCtrl.ButtonCtrl editorCtrl;
        Controllers.FormEditorCtrl.AutoCompleteCtrl acmCtrl;
        Controllers.FormEditorCtrl.MenuCtrl menuCtrl;
        readonly Services.Settings settings;
        readonly Services.FormMgrSvc formMgr;

        private readonly LuaCoreSetting initialCoreSettings;
        readonly ScintillaNET.Scintilla editor;
        readonly string title = "";

        public static FormEditor CreateForm(
            Services.FormMgrSvc formMgr,
            LuaCoreSetting initialCoreSettings
        )
        {
            FormEditor r = null;
            VgcApis.Misc.UI.Invoke(() =>
            {
                r = new FormEditor(formMgr, initialCoreSettings);
            });
            return r;
        }

        FormEditor(Services.FormMgrSvc formMgr, LuaCoreSetting initialCoreSettings)
        {
            this.initialCoreSettings = initialCoreSettings;

            this.formMgr = formMgr;
            this.settings = formMgr.settings;

            InitializeComponent();
            VgcApis.Misc.UI.AutoSetFormIcon(this);
            title = string.Format(I18N.LunaScrEditor, Properties.Resources.Version);

            editor = Misc.UI.CreateLuaEditor(pnlScriptEditor);

            this.Text = title;
        }

        private void FormEditor_Load(object sender, System.EventArgs e)
        {
            InitSplitPanel();

            lbStatusBarMsg.Text = "";

            editorCtrl = new Controllers.FormEditorCtrl.ButtonCtrl(
                this,
                editor,
                cboxScriptName,
                btnNewScript,
                btnSaveScript,
                btnRunScript,
                btnStopScript,
                btnKillScript,
                btnClearOutput,
                btnShowFormSearch,
                btnGotoLine,
                tboxQuickSearch,
                rtBoxOutput
            );

            acmCtrl = new Controllers.FormEditorCtrl.AutoCompleteCtrl(
                formMgr.astServer,
                editor,
                cboxVarList,
                cboxFunctionList,
                enableCodeAnalyzeToolStripMenuItem,
                toolStripStatusCodeAnalyze
            );

            acmCtrl.Run();

            editorCtrl.Run(formMgr);

            menuCtrl = new Controllers.FormEditorCtrl.MenuCtrl(
                this,
                editorCtrl,
                newWindowToolStripMenuItem,
                showScriptManagerToolStripMenuItem,
                loadFileToolStripMenuItem,
                saveAsToolStripMenuItem,
                exitToolStripMenuItem,
                loadCLRLibraryToolStripMenuItem,
                toolStripStatusClrLib,
                cboxScriptName
            );

            menuCtrl.Run(formMgr, initialCoreSettings);

            this.FormClosing += FormClosingHandler;
            this.FormClosed += (s, a) =>
            {
                this.KeyDown -= KeyDownHandler;
                acmCtrl.Cleanup();
                editorCtrl.Cleanup();
                menuCtrl.Cleanup();
            };

            this.KeyDown += KeyDownHandler;
        }

        #region private methods
        void KeyDownHandler(object sender, KeyEventArgs a)
        {
            VgcApis.Misc.Utils.RunInBackground(
                () =>
                    VgcApis.Misc.UI.Invoke(() =>
                    {
                        editorCtrl?.KeyBoardShortcutHandler(a);
                        acmCtrl?.KeyBoardShortcutHandler(a);
                    })
            );
        }

        void InitSplitPanel()
        {
            splitContainerTabEditor.SplitterDistance = this.Width * 6 / 10;
            SetOutputPanelCollapseState(true);
        }

        private void FormClosingHandler(object sender, FormClosingEventArgs e)
        {
            if (settings.IsClosing())
            {
                return;
            }

            if (editorCtrl.IsChanged())
            {
                e.Cancel = !VgcApis.Misc.UI.Confirm(I18N.DiscardUnsavedChanges);
            }
        }
        #endregion

        #region public methods
        public void SetTitleTail(string text)
        {
            var t = $"{title} {text}";
            if (t == this.Text)
            {
                return;
            }

            var len = t.Length;
            var max = 100;
            if (len > max)
            {
                var head = 60;
                var tail = max - head;
                t = t.Substring(0, head) + " ... " + t.Substring(len - tail, tail);
            }

            VgcApis.Misc.UI.Invoke(() =>
            {
                this.Text = t;

                // tooltip would not work in form title
                // this.toolTip1.SetToolTip(this, text ?? "");
            });
        }

        public void SetOutputPanelCollapseState(bool isCollapsed)
        {
            splitContainerTabEditor.Panel2Collapsed = isCollapsed;
            outputPanelToolStripMenuItem.Checked = !isCollapsed;
        }
        #endregion


        #region UI event handlers
        private void outputPanelToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            var isCollapsed = !splitContainerTabEditor.Panel2Collapsed;
            SetOutputPanelCollapseState(isCollapsed);
        }

        #endregion
    }
}
