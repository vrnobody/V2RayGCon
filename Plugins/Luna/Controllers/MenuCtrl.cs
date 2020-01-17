using Luna.Resources.Langs;
using System.Windows.Forms;

namespace Luna.Controllers
{
    internal sealed class MenuCtrl
    {
        TabEditorCtrl editorCtrl;
        Views.WinForms.FormMain formMain;
        Services.FormMgr formMgrService;

        public MenuCtrl(
            Services.FormMgr formMgrService,
            Views.WinForms.FormMain formMain,
            TabEditorCtrl editorCtrl,
            ToolStripMenuItem miNewWindow,
            ToolStripMenuItem miLoad,
            ToolStripMenuItem miSaveAs,
            ToolStripMenuItem miExit)
        {
            this.formMgrService = formMgrService;

            BindControls(formMain, editorCtrl);
            BindEvents(miNewWindow, miLoad, miSaveAs, miExit);
        }

        private void BindEvents(
            ToolStripMenuItem miNewWindow,
            ToolStripMenuItem miLoad,
            ToolStripMenuItem miSaveAs,
            ToolStripMenuItem miExit)
        {
            miNewWindow.Click += (s, a) =>
                formMgrService.CreateNewForm();

            // event handling
            miExit.Click += (s, a) =>
            {
                VgcApis.Libs.UI.RunInUiThread(
                    this.formMain,
                    () => this.formMain.Close());
            };

            miLoad.Click += (s, a) =>
            {
                if (editorCtrl.IsChanged()
                    && !VgcApis.Libs.UI.Confirm(I18N.DiscardUnsavedChanges))
                {
                    return;
                }

                string script = VgcApis.Libs.UI.ReadFileContentFromDialog(
                    VgcApis.Models.Consts.Files.LuaExt);

                // user cancelled.
                if (script == null)
                {
                    return;
                }

                editorCtrl.SetCurrentEditorContent(script);
            };

            miSaveAs.Click += (s, a) =>
            {
                VgcApis.Libs.UI.SaveToFile(
                    VgcApis.Models.Consts.Files.LuaExt,
                    editorCtrl.GetCurrentEditorContent());
            };
        }

        private void BindControls(Views.WinForms.FormMain formMain, TabEditorCtrl editorCtrl)
        {
            // binding
            this.editorCtrl = editorCtrl;
            this.formMain = formMain;
        }
    }
}
