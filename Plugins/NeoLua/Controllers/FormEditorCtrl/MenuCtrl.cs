using NeoLuna.Resources.Langs;
using NeoLuna.Services;
using System;
using System.Windows.Forms;

namespace NeoLuna.Controllers.FormEditorCtrl
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

        FormMgrSvc formMgrService;

        public void Run(
            Services.FormMgrSvc formMgrService,
            Models.Data.LuaCoreSetting initialCoreSettings)
        {
            this.formMgrService = formMgrService;
            InitControls();
            BindEvents();

            if (initialCoreSettings != null)
            {
                var name = initialCoreSettings.name;
                if (!string.IsNullOrEmpty(name))
                {
                    editorCtrl.LoadScript(name);
                }

                var enabled = initialCoreSettings.isLoadClr;
                UpdateClrControlsEanbledState(enabled);
                editorCtrl.isLoadClrLib = enabled;
            }
        }

        public void Cleanup()
        {
            ReleaseEvents();
        }

        #region private method
        private void InitControls()
        {
            miLoadClrLib.Checked = false;
            smiLbClrLib.Enabled = false;
        }
        void ReleaseEvents()
        {
            miShowMgr.Click -= OnMiShowMgrClickHandler;
            miLoadClrLib.Click -= OnMiLoadClrLibClickHandler;
            miNewWindow.Click -= OnMiNewWindowClickHandler;
            miExit.Click -= OnMiExitClickHandler;
            miLoad.Click -= OnMiLoadClickHandler;
            miSaveAs.Click -= OnMiSaveAslickHandler;
        }

        void OnMiShowMgrClickHandler(object sender, EventArgs args)
        {
            formMgrService.ShowFormMain();
        }

        void OnMiLoadClrLibClickHandler(object sender, EventArgs args)
        {
            var enable = !miLoadClrLib.Checked;
            UpdateClrControlsEanbledState(enable);
        }

        void OnMiNewWindowClickHandler(object sender, EventArgs args)
        {
            formMgrService.CreateNewEditor();
        }

        void OnMiExitClickHandler(object sender, EventArgs args)
        {
            VgcApis.Misc.UI.CloseFormIgnoreError(formEditor);
        }

        void OnMiLoadClickHandler(object sender, EventArgs args)
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
        }

        void OnMiSaveAslickHandler(object sender, EventArgs args)
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
        }

        private void BindEvents()
        {
            miShowMgr.Click += OnMiShowMgrClickHandler;
            miLoadClrLib.Click += OnMiLoadClrLibClickHandler;
            miNewWindow.Click += OnMiNewWindowClickHandler;
            miExit.Click += OnMiExitClickHandler;
            miLoad.Click += OnMiLoadClickHandler;
            miSaveAs.Click += OnMiSaveAslickHandler;
        }

        private void UpdateClrControlsEanbledState(bool enable)
        {
            miLoadClrLib.Checked = enable;
            smiLbClrLib.Enabled = enable;
            editorCtrl.isLoadClrLib = enable;
        }
        #endregion
    }
}
