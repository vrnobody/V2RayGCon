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

        public FormMain(
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
                cboxScriptName,
                btnNewScript,
                btnSaveScript,
                btnRemoveScript,
                btnRunScript,
                btnStopScript,
                btnKillScript,
                btnClearOutput,
                rtBoxOutput,
                pnlScriptEditor,
                splitContainerTabEditor);

            editorCtrl.Run(api, settings, luaServer);

            menuCtrl = new Controllers.MenuCtrl(
                formMgr,
                this,
                editorCtrl,
                newWindowToolStripMenuItem,
                loadFileToolStripMenuItem,
                saveAsToolStripMenuItem,
                exitToolStripMenuItem);

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

        void InitSplitPanel()
        {
            splitContainerTabEditor.SplitterDistance = this.Width * 6 / 10;
            splitContainerTabEditor.Panel2Collapsed = true;
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

        private void flyScriptUIContainer_Scroll(object sender, ScrollEventArgs e)
        {
            flyScriptUIContainer.Refresh();
        }

        private void hideOutputPanelToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            splitContainerTabEditor.Panel2Collapsed = true;
        }

        private void showOutputPanelToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            splitContainerTabEditor.Panel2Collapsed = false;
        }

        #region private methods

        #endregion
    }
}
