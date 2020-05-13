using Luna.Resources.Langs;
using System.Windows.Forms;

namespace Luna.Controllers
{
    internal sealed class MenuCtrl
    {
        Services.FormMgr formMgrService;

        Views.WinForms.FormEditor formEditor;
        TabEditorCtrl editorCtrl;
        private readonly ToolStripMenuItem miNewWindow;
        private readonly ToolStripMenuItem miShowMgr;
        private readonly ToolStripMenuItem miLoad;
        private readonly ToolStripMenuItem miSaveAs;
        private readonly ToolStripMenuItem miExit;
        private readonly ToolStripMenuItem miLoadClrLib;
        private readonly ToolStripMenuItem miEanbleCodeAnalyze;
        private readonly ComboBox cboxScriptName;

        public MenuCtrl(
            Views.WinForms.FormEditor formEditor,

            TabEditorCtrl editorCtrl,
            ToolStripMenuItem miNewWindow,
            ToolStripMenuItem miShowMgr,
            ToolStripMenuItem miLoad,
            ToolStripMenuItem miSaveAs,
            ToolStripMenuItem miExit,

            ToolStripMenuItem miLoadClrLib,
            ToolStripMenuItem miEanbleCodeAnalyze,

            ComboBox cboxScriptName)
        {
            this.editorCtrl = editorCtrl;
            this.miNewWindow = miNewWindow;
            this.miShowMgr = miShowMgr;
            this.miLoad = miLoad;
            this.miSaveAs = miSaveAs;
            this.miExit = miExit;
            this.miLoadClrLib = miLoadClrLib;
            this.miEanbleCodeAnalyze = miEanbleCodeAnalyze;
            this.cboxScriptName = cboxScriptName;
            this.formEditor = formEditor;
        }

        public void Run(
            Services.FormMgr formMgrService,
            Services.Settings settings)
        {
            this.formMgrService = formMgrService;
            miLoadClrLib.Checked = settings.isLoadClrLib;
            miEanbleCodeAnalyze.Checked = settings.isEnableCodeAnalyze;

            BindEvents();
        }

        private void BindEvents()
        {
            miShowMgr.Click += (s, a) => formMgrService.ShowFormMain();

            miLoadClrLib.Click += (s, a) =>
            {
                var enable = !miLoadClrLib.Checked;
                miLoadClrLib.Checked = enable;
                editorCtrl.isLoadClrLib = enable;
            };

            miEanbleCodeAnalyze.Click += (s, a) =>
            {
                var enable = !miEanbleCodeAnalyze.Checked;
                miEanbleCodeAnalyze.Checked = enable;
                editorCtrl.SetIsEnableCodeAnalyze(enable);
            };

            miNewWindow.Click += (s, a) =>
                formMgrService.CreateNewEditor();

            // event handling
            miExit.Click += (s, a) =>
                VgcApis.Misc.UI.CloseFormIgnoreError(formEditor);

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
