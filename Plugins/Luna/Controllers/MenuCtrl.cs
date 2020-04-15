using Luna.Resources.Langs;
using System.Windows.Forms;

namespace Luna.Controllers
{
    internal sealed class MenuCtrl
    {
        TabEditorCtrl editorCtrl;
        private readonly ToolStripMenuItem miNewWindow;
        private readonly ToolStripMenuItem miLoad;
        private readonly ToolStripMenuItem miSaveAs;
        private readonly ToolStripMenuItem miExit;
        private readonly TabControl tabControlMain;
        Views.WinForms.FormMain formMain;
        Services.FormMgr formMgrService;

        public MenuCtrl(
            Services.FormMgr formMgrService,
            Views.WinForms.FormMain formMain,
            TabEditorCtrl editorCtrl,
            ToolStripMenuItem miNewWindow,
            ToolStripMenuItem miLoad,
            ToolStripMenuItem miSaveAs,
            ToolStripMenuItem miExit,

            TabControl tabControlMain)
        {
            this.formMgrService = formMgrService;

            this.editorCtrl = editorCtrl;
            this.miNewWindow = miNewWindow;
            this.miLoad = miLoad;
            this.miSaveAs = miSaveAs;
            this.miExit = miExit;
            this.tabControlMain = tabControlMain;
            this.formMain = formMain;
        }

        public void Run()
        {
            BindEvents();
        }

        private void BindEvents()
        {
            miNewWindow.Click += (s, a) =>
                formMgrService.CreateNewForm();

            // event handling
            miExit.Click += (s, a) =>
                VgcApis.Misc.UI.CloseFormIgnoreError(formMain);

            miLoad.Click += (s, a) =>
            {
                if (editorCtrl.IsChanged()
                    && !VgcApis.Misc.UI.Confirm(I18N.DiscardUnsavedChanges))
                {
                    return;
                }

                string script = VgcApis.Misc.UI.ReadFileContentFromDialog(
                    VgcApis.Models.Consts.Files.LuaExt);

                // user cancelled.
                if (script == null)
                {
                    return;
                }

                tabControlMain.SelectTab(1);
                editorCtrl.SetCurrentEditorContent(script);
            };

            miSaveAs.Click += (s, a) =>
            {
                VgcApis.Misc.UI.SaveToFile(
                    VgcApis.Models.Consts.Files.LuaExt,
                    editorCtrl.GetCurrentEditorContent());
            };
        }

    }
}
