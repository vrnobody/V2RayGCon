using Luna.Resources.Langs;
using System.Windows.Forms;

namespace Luna.Views.WinForms
{
    internal partial class FormMain : Form
    {
        Controllers.TabGeneralCtrl genCtrl;
        Controllers.TabOptionsCtrl optionsCtrl;

        Services.LuaServer luaServer;
        Services.Settings settings;
        Services.FormMgr formMgr;

        public static FormMain CreateForm(
            Services.Settings settings,
            Services.LuaServer luaServer,
            Services.FormMgr formMgr)
        {
            FormMain r = null;
            VgcApis.Misc.UI.Invoke(() =>
            {
                r = new FormMain(settings, luaServer, formMgr);
            });
            return r;
        }

        FormMain(
            Services.Settings settings,
            Services.LuaServer luaServer,
            Services.FormMgr formMgr)
        {
            this.settings = settings;
            this.luaServer = luaServer;
            this.formMgr = formMgr;

            InitializeComponent();
            VgcApis.Misc.UI.AutoSetFormIcon(this);
            this.Text = Properties.Resources.Name + " v" + Properties.Resources.Version;
        }

        private void FormMain_Load(object sender, System.EventArgs e)
        {
            lbStatusBarMsg.Text = "";

            // TabGeneral should initialize before TabEditor.
            genCtrl = new Controllers.TabGeneralCtrl(
                flyScriptUIContainer,
                btnStopAllScript,
                btnKillAllScript,
                btnShowEditor,
                btnDeleteAllScripts,
                btnImportFromFile,
                btnExportToFile);

            optionsCtrl = new Controllers.TabOptionsCtrl(
                this,
                chkLoadClrLib,
                chkEnableCodeAnalyze,
                btnSaveOptions,
                btnExit);

            genCtrl.Run(formMgr, luaServer);
            optionsCtrl.Run(settings);

            this.FormClosing += FormClosingHandler;
            this.FormClosed += (s, a) =>
            {
                // reverse order 
                genCtrl.Cleanup();
            };
        }

        #region private methods
        private void FormClosingHandler(object sender, FormClosingEventArgs e)
        {
            if (settings.IsShutdown())
            {
                return;
            }

            if (optionsCtrl.IsChanged())
            {
                e.Cancel = !VgcApis.Misc.UI.Confirm(I18N.DiscardUnsavedChanges);
            }
        }
        #endregion

        #region UI event handlers
        private void flyScriptUIContainer_Scroll(object sender, ScrollEventArgs e)
        {
            flyScriptUIContainer.Refresh();
        }
        #endregion
    }
}
