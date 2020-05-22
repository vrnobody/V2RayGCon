using Luna.Resources.Langs;
using Luna.Services;
using System.Windows.Forms;

namespace Luna.Controllers.FormEditorCtrl
{
    internal sealed class MenuCtrl
    {
        Views.WinForms.FormEditor formEditor;
        ButtonCtrl editorCtrl;
        private readonly ToolStripMenuItem miNewWindow;
        private readonly ToolStripMenuItem miShowMgr;
        private readonly ToolStripMenuItem miLoad;
        private readonly ToolStripMenuItem miSaveAs;
        private readonly ToolStripMenuItem miExit;
        private readonly ToolStripMenuItem miLoadClrLib;

        private readonly ToolStripStatusLabel smiLbClrLib;
        private readonly ComboBox cboxScriptName;

        public MenuCtrl(
            Views.WinForms.FormEditor formEditor,

            ButtonCtrl editorCtrl,
            ToolStripMenuItem miNewWindow,
            ToolStripMenuItem miShowMgr,
            ToolStripMenuItem miLoad,
            ToolStripMenuItem miSaveAs,
            ToolStripMenuItem miExit,

            ToolStripMenuItem miLoadClrLib,


            ToolStripStatusLabel smiLbClrLib,


            ComboBox cboxScriptName)
        {
            this.editorCtrl = editorCtrl;
            this.miNewWindow = miNewWindow;
            this.miShowMgr = miShowMgr;
            this.miLoad = miLoad;
            this.miSaveAs = miSaveAs;
            this.miExit = miExit;
            this.miLoadClrLib = miLoadClrLib;

            this.smiLbClrLib = smiLbClrLib;

            this.cboxScriptName = cboxScriptName;
            this.formEditor = formEditor;
        }

        public void Run(
            Services.FormMgrSvc formMgrService)
        {
            InitControls();
            BindEvents(formMgrService);
        }

        #region private method
        private void InitControls()
        {
            miLoadClrLib.Checked = false;
            smiLbClrLib.Enabled = false;
        }

        private void BindEvents(FormMgrSvc formMgrService)
        {
            miShowMgr.Click += (s, a) => formMgrService.ShowFormMain();

            miLoadClrLib.Click += (s, a) =>
            {
                var enable = !miLoadClrLib.Checked;
                miLoadClrLib.Checked = enable;
                smiLbClrLib.Enabled = enable;
                editorCtrl.isLoadClrLib = enable;
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

                var cf = VgcApis.Misc.UI.ReadFileFromDialog(VgcApis.Models.Consts.Files.LuaExt);
                var script = cf.Item1;
                var filename = cf.Item2;

                if (script == null)
                {
                    return;
                }

                cboxScriptName.Text = @"";
                editorCtrl.SetCurFileName(filename);
                editorCtrl.SetCurrentEditorContent(script);
                editorCtrl.SetScriptCache(script);
            };

            miSaveAs.Click += (s, a) =>
            {
                var script = editorCtrl.GetCurrentEditorContent();
                var err = VgcApis.Misc.UI.ShowSaveFileDialog(
                    VgcApis.Models.Consts.Files.LuaExt,
                    script,
                    out var filename);

                switch (err)
                {
                    case VgcApis.Models.Datas.Enums.SaveFileErrorCode.Success:
                        if (string.IsNullOrEmpty(cboxScriptName.Text))
                        {
                            editorCtrl.SetCurFileName(filename);
                            editorCtrl.SetScriptCache(script);
                        }
                        VgcApis.Misc.UI.MsgBoxAsync(I18N.Done);
                        break;
                    case VgcApis.Models.Datas.Enums.SaveFileErrorCode.Fail:
                        VgcApis.Misc.UI.MsgBoxAsync(I18N.WriteFileFail);
                        break;
                }
            };
        }
        #endregion
    }
}
