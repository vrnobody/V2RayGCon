using Luna.Resources.Langs;
using System.Windows.Forms;

namespace Luna.Views.WinForms
{
    internal partial class FormMain : Form
    {
        Controllers.FormMainCtrl.TabGeneralCtrl genCtrl;

        Services.LuaServer luaServer;
        Services.Settings settings;
        Services.FormMgrSvc formMgr;

        public static FormMain CreateForm(
            Services.Settings settings,
            Services.LuaServer luaServer,
            Services.FormMgrSvc formMgr
        )
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
            Services.FormMgrSvc formMgr
        )
        {
            this.settings = settings;
            this.luaServer = luaServer;
            this.formMgr = formMgr;

            InitializeComponent();
            VgcApis.Misc.UI.AutoSetFormIcon(this);
            this.Text = string.Format(I18N.LunaScrMgr, Properties.Resources.Version);
        }

        private void FormMain_Load(object sender, System.EventArgs e)
        {
            lbStatusBarMsg.Text = "";

            // TabGeneral should initialize before TabEditor.
            genCtrl = new Controllers.FormMainCtrl.TabGeneralCtrl(
                flyScriptUIContainer,
                btnStopAllScript,
                btnKillAllScript,
                btnDeleteAllScripts,
                btnImportFromFile,
                btnExportToFile
            );

            genCtrl.Run(luaServer, formMgr);

            this.FormClosed += (s, a) =>
            {
                // reverse order
                genCtrl.Cleanup();
            };
        }

        #region private methods

        #endregion

        #region UI event handlers
        private void flyScriptUIContainer_Scroll(object sender, ScrollEventArgs e)
        {
            flyScriptUIContainer.Refresh();
        }

        private void showEditorToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            formMgr.ShowOrCreateFirstEditor();
        }

        private void closeToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }

        private void btnOpenEditor_Click(object sender, System.EventArgs e)
        {
            formMgr.CreateNewEditor();
        }
        #endregion
    }
}
