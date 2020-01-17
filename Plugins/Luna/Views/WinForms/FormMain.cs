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
        VgcApis.Models.IServices.IApiService api;

        public FormMain(
            VgcApis.Models.IServices.IApiService api,
            Services.Settings settings,
            Services.LuaServer luaServer,
            Services.FormMgr formMgr)
        {
            this.api = api;
            this.formMgr = formMgr;
            this.settings = settings;
            this.luaServer = luaServer;
            InitializeComponent();
            VgcApis.Libs.UI.AutoSetFormIcon(this);
            this.Text = Properties.Resources.Name + " v" + Properties.Resources.Version;
        }

        private void FormMain_Load(object sender, System.EventArgs e)
        {
            FixSplitPanelWidth();
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
                pnlScriptEditor);

            editorCtrl.Run(api, settings, luaServer);

            menuCtrl = new Controllers.MenuCtrl(
                formMgr,
                this,
                editorCtrl,
                newWindowToolStripMenuItem,
                loadFileToolStripMenuItem,
                saveAsToolStripMenuItem,
                exitToolStripMenuItem);

            this.FormClosing += FormClosingHandler;
            this.FormClosed += (s, a) =>
            {
                // reverse order 
                editorCtrl.Cleanup();
                genCtrl.Cleanup();
            };

            this.KeyDown += (s, a) => editorCtrl?.KeyBoardShortcutHandler(a);
        }

        //protected override bool ProcessCmdKey(ref Message msg, Keys keyCode)
        //{

        //    editorCtrl?.KeyBoardShortcutHandler(keyCode);
        //    return base.ProcessCmdKey(ref msg, keyCode);
        //}

        void FixSplitPanelWidth()
        {
            splitContainerTabEditor.SplitterDistance = this.Width * 6 / 10;
        }

        private void FormClosingHandler(object sender, FormClosingEventArgs e)
        {
            if (settings.IsShutdown())
            {
                return;
            }

            if (editorCtrl.IsChanged())
            {
                e.Cancel = !VgcApis.Libs.UI.Confirm(I18N.DiscardUnsavedChanges);
            }
        }

    }
}
