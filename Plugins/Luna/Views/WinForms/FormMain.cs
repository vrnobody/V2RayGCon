using Luna.Resources.Langs;
using System.Windows.Forms;

namespace Luna.Views.WinForms
{
    public partial class FormMain : Form
    {
        Controllers.TabEditorCtrl editorCtrl;
        Controllers.TabGeneralCtrl genCtrl;
        Controllers.MenuCtrl menuCtrl;

        Services.LuaServer luaServer;
        Services.Settings settings;
        Services.FormMgr formMgr;
        VgcApis.Interfaces.Services.IApiService api;

        public static FormMain CreateForm(
            VgcApis.Interfaces.Services.IApiService api,
            Services.Settings settings,
            Services.LuaServer luaServer,
            Services.FormMgr formMgr)
        {
            FormMain r = null;
            VgcApis.Misc.UI.Invoke(() =>
            {
                r = new FormMain(api, settings, luaServer, formMgr);
            });
            return r;
        }

        FormMain(
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

        private void FormMain_Load(object sender, System.EventArgs e)
        {
            chkEnableClrSupports.Checked = settings.isEnableClrSupports;
#if DEBUG
            tabControl1.SelectTab(1);
#endif

            InitSplitPanel();
            lbStatusBarMsg.Text = "";

            // TabGeneral should initialize before TabEditor.
            genCtrl = new Controllers.TabGeneralCtrl(
                flyScriptUIContainer,
                btnStopAllScript,
                btnKillAllScript,
                btnDeleteAllScripts,
                btnImportFromFile,
                btnExportToFile);

            genCtrl.Run(settings, luaServer);

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
                formMgr,
                this,
                editorCtrl,
                newWindowToolStripMenuItem,
                loadFileToolStripMenuItem,
                saveAsToolStripMenuItem,
                exitToolStripMenuItem,
                tabControl1,
                cboxScriptName);

            menuCtrl.Run();

            this.FormClosing += FormClosingHandler;
            this.FormClosed += (s, a) =>
            {
                // reverse order 
                editorCtrl.Cleanup();
                genCtrl.Cleanup();
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
        private void flyScriptUIContainer_Scroll(object sender, ScrollEventArgs e)
        {
            flyScriptUIContainer.Refresh();
        }

        private void hideOutputPanelToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            SetOutputPanelCollapseState(true);
        }

        private void showOutputPanelToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            SetOutputPanelCollapseState(false);
        }
        private void chkEnableClrSupports_CheckedChanged(object sender, System.EventArgs e)
        {
            settings.isEnableClrSupports = chkEnableClrSupports.Checked;
        }
        #endregion


    }
}
