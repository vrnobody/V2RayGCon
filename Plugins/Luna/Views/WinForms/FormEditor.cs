using Luna.Resources.Langs;
using System.Windows.Forms;

namespace Luna.Views.WinForms
{
    internal partial class FormEditor : Form
    {
        Controllers.TabEditorCtrl editorCtrl;
        Controllers.MenuCtrl menuCtrl;

        Services.LuaServer luaServer;
        Services.Settings settings;
        Services.FormMgr formMgr;
        VgcApis.Interfaces.Services.IApiService api;

        public static FormEditor CreateForm(
            VgcApis.Interfaces.Services.IApiService api,
            Services.Settings settings,
            Services.LuaServer luaServer,
            Services.FormMgr formMgr)
        {
            FormEditor r = null;
            VgcApis.Misc.UI.Invoke(() =>
            {
                r = new FormEditor(api, settings, luaServer, formMgr);
            });
            return r;
        }

        FormEditor(
            VgcApis.Interfaces.Services.IApiService api,
            Services.Settings settings,
            Services.LuaServer luaServer,
            Services.FormMgr formMgr)
        {
            this.api = api;
            this.formMgr = formMgr;
            this.settings = settings;
            this.luaServer = luaServer;
            InitializeComponent();
            VgcApis.Misc.UI.AutoSetFormIcon(this);
            this.Text = Properties.Resources.Name + " v" + Properties.Resources.Version;
        }

        private void FormEditor_Load(object sender, System.EventArgs e)
        {

            InitSplitPanel();
            lbStatusBarMsg.Text = "";

            editorCtrl = new Controllers.TabEditorCtrl(
                this,
                cboxScriptName,
                btnNewScript,
                btnSaveScript,

                cboxVarList,
                cboxFunctionList,

                btnRunScript,
                btnStopScript,
                btnKillScript,
                btnClearOutput,
                rtBoxOutput,
                pnlScriptEditor);

            editorCtrl.Run(api, settings, formMgr, luaServer);

            menuCtrl = new Controllers.MenuCtrl(
                this,
                editorCtrl,
                newWindowToolStripMenuItem,
                showScriptManagerToolStripMenuItem,
                loadFileToolStripMenuItem,
                saveAsToolStripMenuItem,
                exitToolStripMenuItem,

                loadCLRLibraryToolStripMenuItem,
                enableCodeAnalyzeToolStripMenuItem,

                cboxScriptName);

            menuCtrl.Run(formMgr, settings);

            this.FormClosing += FormClosingHandler;
            this.FormClosed += (s, a) =>
            {
                // reverse order 
                editorCtrl.Cleanup();
            };

            this.KeyDown += (s, a) => editorCtrl?.KeyBoardShortcutHandler(a);
        }

        #region private methods
        void InitSplitPanel()
        {
            splitContainerTabEditor.SplitterDistance = this.Width * 6 / 10;
            SetOutputPanelCollapseState(true);

        }
        private void FormClosingHandler(object sender, FormClosingEventArgs e)
        {
            if (settings.IsShutdown())
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
        public void SetOutputPanelCollapseState(bool isCollapsed)
        {
            splitContainerTabEditor.Panel2Collapsed = isCollapsed;
            showOutputPanelToolStripMenuItem.Checked = !isCollapsed;
            hideOutputPanelToolStripMenuItem.Checked = isCollapsed;
        }
        #endregion


        #region UI event handlers


        private void hideOutputPanelToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            SetOutputPanelCollapseState(true);
        }

        private void showOutputPanelToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            SetOutputPanelCollapseState(false);
        }

        #endregion
    }
}
