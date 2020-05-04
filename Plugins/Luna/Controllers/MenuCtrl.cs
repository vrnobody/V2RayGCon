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
        private readonly ComboBox cboxScriptName;
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

            TabControl tabControlMain,
            ComboBox cboxScriptName)
        {
            this.formMgrService = formMgrService;

            this.editorCtrl = editorCtrl;
            this.miNewWindow = miNewWindow;
            this.miLoad = miLoad;
            this.miSaveAs = miSaveAs;
            this.miExit = miExit;
            this.tabControlMain = tabControlMain;
            this.cboxScriptName = cboxScriptName;
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
                cboxScriptName.Text = @"";
                editorCtrl.SetCurrentEditorContent(script);
                editorCtrl.SetScriptCache(script);
            };

            miSaveAs.Click += (s, a) =>
            {
                var script = editorCtrl.GetCurrentEditorContent();
                var err = VgcApis.Misc.UI.ShowSaveFileDialog(
                    VgcApis.Models.Consts.Files.LuaExt,
                    script,
                    out var filenaem);

                switch (err)
                {
                    case VgcApis.Models.Datas.Enums.SaveFileErrorCode.Success:
                        editorCtrl.SetScriptCache(script);
                        MessageBox.Show(I18N.Done);
                        break;
                    case VgcApis.Models.Datas.Enums.SaveFileErrorCode.Fail:
                        MessageBox.Show(I18N.WriteFileFail);
                        break;
                }
            };
        }

    }
}
