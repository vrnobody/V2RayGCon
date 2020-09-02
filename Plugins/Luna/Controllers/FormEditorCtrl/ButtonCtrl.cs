using Luna.Resources.Langs;
using Luna.Views.WinForms;
using ScintillaNET;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Luna.Controllers.FormEditorCtrl
{
    internal sealed class ButtonCtrl
    {
        Services.Settings settings;
        Services.FormMgrSvc formMgr;
        Services.LuaServer luaServer;

        private readonly FormEditor formEditor;
        VgcApis.WinForms.FormSearch formSearch = null;

        VgcApis.UserControls.RepaintController rtboxFreezer;
        VgcApis.Libs.Sys.QueueLogger qLogger = new VgcApis.Libs.Sys.QueueLogger();

        Scintilla editor = null;
        LuaCoreCtrl luaCoreCtrl;

        #region controls
        ComboBox cboxScriptName;
        Button btnNewScript,
            btnSaveScript,
            btnRunScript,
            btnStopLuaCore,
            btnKillLuaCore,
            btnClearOutput;
        private readonly Button btnShowSearchBox;
        private readonly Button btnGoto;
        private readonly TextBox tboxGoto;
        RichTextBox rtboxOutput;

        #endregion

        string preScriptName = string.Empty;
        string preScriptContent = string.Empty;

        VgcApis.Libs.Tasks.Routine logUpdater;

        public ButtonCtrl(
            FormEditor formEditor,
            Scintilla editor,
            ComboBox cboxScriptName,
            Button btnNewScript,
            Button btnSaveScript,

            Button btnRunScript,
            Button btnStopScript,
            Button btnKillScript,
            Button btnClearOutput,

            Button btnShowSearchBox,
            Button btnGoto,
            TextBox tboxGoto,

            RichTextBox rtboxOutput)
        {
            this.formEditor = formEditor;
            this.cboxScriptName = cboxScriptName;
            this.btnNewScript = btnNewScript;
            this.btnSaveScript = btnSaveScript;
            this.btnRunScript = btnRunScript;
            this.btnStopLuaCore = btnStopScript;
            this.btnKillLuaCore = btnKillScript;
            this.btnClearOutput = btnClearOutput;
            this.btnShowSearchBox = btnShowSearchBox;
            this.btnGoto = btnGoto;
            this.tboxGoto = tboxGoto;
            this.rtboxOutput = rtboxOutput;
            this.editor = editor;

            logUpdater = new VgcApis.Libs.Tasks.Routine(
                UpdateOutput,
                VgcApis.Models.Consts.Intervals.LuaPluginLogRefreshInterval);

        }

        public void Run(
            VgcApis.Interfaces.Services.IApiService api,
            Services.Settings settings,
            Services.FormMgrSvc formMgr,
            Services.LuaServer luaServer)
        {
            this.formMgr = formMgr;
            this.settings = settings;
            this.luaServer = luaServer;

            this.luaCoreCtrl = CreateLuaCoreCtrl(settings, api);


            isLoadClrLib = false;

            BindEvents();
            ReloadScriptName();

            rtboxFreezer = new VgcApis.UserControls.RepaintController(rtboxOutput);
            logUpdater.Run();

            /*
            #if DEBUG
                        if (cboxScriptName.Items.Count > 0)
                        {
                            cboxScriptName.SelectedIndex = 0;
                        }
            #endif
            */
        }

        #region public methods
        public void LoadScript(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                cboxScriptName.Text = name;
                CboxScriptNameChangedHandler(this, EventArgs.Empty);
            }
        }

        public bool isLoadClrLib;

        public void KeyBoardShortcutHandler(KeyEventArgs keyEvent)
        {
            var keyCode = keyEvent.KeyCode;
            if (keyEvent.Control)
            {
                switch (keyCode)
                {
                    case Keys.OemMinus:
                        editor.ZoomOut();
                        break;
                    case Keys.Oemplus:
                        editor.ZoomIn();
                        break;
                    case Keys.G:
                        tboxGoto.Focus();
                        tboxGoto.SelectAll();
                        break;
                    case Keys.F:
                        ShowFormSearch();
                        break;
                    case Keys.S:
                        OnBtnSaveScriptClickHandler(false);
                        break;
                    case Keys.N:
                        ClearEditor();
                        break;
                }
                return;
            }

            switch (keyCode)
            {
                case Keys.Escape:
                    VgcApis.Misc.UI.CloseFormIgnoreError(formSearch);
                    break;
                case Keys.F2:
                    formSearch?.SearchPrevious();
                    break;
                case Keys.F3:
                    formSearch?.SearchNext();
                    break;
                case Keys.F4:
                    formSearch?.SearchFirst(true);
                    break;
                case Keys.F5:
                    btnRunScript.PerformClick();
                    break;
                case Keys.F6:
                    btnStopLuaCore.PerformClick();
                    break;
                case Keys.F7:
                    btnKillLuaCore.PerformClick();
                    break;
                case Keys.F8:
                    btnClearOutput.PerformClick();
                    break;
            }
        }

        public bool IsChanged() =>
            editor.Text != preScriptContent;

        LuaCoreCtrl CreateLuaCoreCtrl(
           Services.Settings settings,
           VgcApis.Interfaces.Services.IApiService api)
        {
            var luaApis = new Models.Apis.LuaApis(api, settings, formMgr);
            luaApis.Prepare();
            luaApis.SetRedirectLogWorker(Log);

            var coreSettings = new Models.Data.LuaCoreSetting()
            {
                isLoadClr = isLoadClrLib,
            };
            var ctrl = new LuaCoreCtrl(true);
            ctrl.Run(settings, coreSettings, luaApis);
            return ctrl;
        }

        public void Cleanup()
        {
            VgcApis.Misc.UI.CloseFormIgnoreError(formSearch);

            luaCoreCtrl?.AbortNow();
            logUpdater?.Dispose();
            qLogger?.Dispose();
        }

        public string GetCurrentEditorContent() => editor.Text;

        public void SetScriptCache(string content) =>
            preScriptContent = content;

        string curFileName = null;
        public void SetCurFileName(string filename)
        {
            this.curFileName = filename;
            formEditor.SetTitleTail(filename);
        }

        public void SetCurrentEditorContent(string content) =>
            VgcApis.Misc.UI.Invoke(() => editor.Text = content);

        #endregion

        #region Scintilla


        void Scintilla_DoubleClick(object sender, EventArgs e)
        {
            var keyword = VgcApis.Misc.Utils.GetWordFromCurPos(editor);

            if (string.IsNullOrEmpty(keyword) || keyword.Length < 2)
            {
                return;
            }

            var orgFlag = editor.SearchFlags;

            editor.SearchFlags = SearchFlags.MatchCase | SearchFlags.WholeWord;
            editor.TargetStart = 0;
            editor.TargetEnd = editor.TextLength;

            while (editor.SearchInTarget(keyword) != -1)
            {
                var end = editor.TargetEnd;
                var start = editor.TargetStart;
                editor.IndicatorFillRange(start, end - start);
                editor.TargetStart = end;
                editor.TargetEnd = editor.TextLength;
            }

            editor.TargetStart = 0;
            editor.TargetEnd = 0;
            editor.SearchFlags = orgFlag;
        }

        private void Scintilla_MouseClicked(object sender, EventArgs e)
        {
            editor.IndicatorClearRange(0, editor.TextLength);
        }

        private int maxLineNumberCharLength;
        private void Scintilla_TextChanged(object sender, EventArgs e)
        {
            // Did the number of characters in the line number display change?
            // i.e. nnn VS nn, or nnnn VS nn, etc...
            var maxLineNumberCharLength = editor.Lines.Count.ToString().Length;
            if (maxLineNumberCharLength == this.maxLineNumberCharLength)
                return;

            // Calculate the width required to display the last line number
            // and include some padding for good measure.
            const int padding = 2;
            editor.Margins[0].Width = editor.TextWidth(Style.LineNumber, new string('9', maxLineNumberCharLength + 1)) + padding;
            this.maxLineNumberCharLength = maxLineNumberCharLength;
        }

        string Scintilla_TrimText(string text) =>
            text?.Replace("\t", "")
                ?.Replace("\r", "")
                ?.Replace("\n", "")
                ?.Trim()
                ?.ToLower();

        string GetCurrentText(int curPos)
        {
            var line = editor.Lines[editor.CurrentLine];
            var text = line.Text;
            var len = Math.Min(curPos - line.Position, text.Length);
            if (len <= 0)
            {
                return string.Empty;
            }
            return text.Substring(0, len);
        }

        private void Scintilla_InsertCheck(object sender, InsertCheckEventArgs e)
        {
            if (!e.Text.EndsWith("\n"))
            {
                return;
            }

            var indent = editor.Lines[editor.CurrentLine].Indentation;
            var text = Scintilla_TrimText(GetCurrentText(e.Position));
            if (text.StartsWith("function")
                || text.StartsWith("local function")
                || text.EndsWith("do")
                || text.EndsWith("then")
                || text.EndsWith("else")
                || text == "repeat"
                || Regex.IsMatch(text, "{\\s*$"))
            {
                indent += 4;
            }

            e.Text = e.Text + new string(' ', Math.Max(0, indent));
        }

        private void Scintilla_CharAdded(object sender, CharAddedEventArgs e)
        {
            int curLine = editor.CurrentLine;
            if (curLine < 2)
            {
                return;
            }

            string text = Scintilla_TrimText(editor.Lines[curLine].Text);
            if (text == "}"
                || text.StartsWith("until")
                || text == "else"
                || text == "end")
            {
                var indent = editor.Lines[curLine - 1].Indentation - 4;
                editor.Lines[curLine].Indentation = Math.Max(0, indent);
            }
        }

        #endregion

        #region private methods
        void Invoke(Action action) => VgcApis.Misc.UI.Invoke(action);

        void ShowFormSearch()
        {
            if (formSearch != null)
            {
                formSearch.Activate();
                return;
            }

            formSearch = VgcApis.WinForms.FormSearch.CreateForm(editor);
            formSearch.FormClosed += (s, a) => formSearch = null;
        }

        void Log(string content) => qLogger.Log(content);

        long updateOutputTimeStamp = 0;
        VgcApis.Libs.Tasks.Bar bar = new VgcApis.Libs.Tasks.Bar();

        void UpdateOutput()
        {
            if (!bar.Install())
            {
                return;
            }

            var timestamp = qLogger.GetTimestamp();
            if (updateOutputTimeStamp == timestamp)
            {
                bar.Remove();
                return;
            }

            VgcApis.Misc.UI.InvokeThen(
                () =>
                {
                    rtboxFreezer.DisableRepaintEvent();
                    rtboxOutput.Text = qLogger.GetLogAsString(true);
                    VgcApis.Misc.UI.ScrollToBottom(rtboxOutput);
                    rtboxFreezer.EnableRepaintEvent();
                    updateOutputTimeStamp = timestamp;
                }, () => bar.Remove());
        }

        void GotoLine()
        {
            if (int.TryParse(tboxGoto.Text, out int lineNumber))
            {
                var line = editor.Lines[lineNumber - 1];
                var linesOnScreen = editor.LinesOnScreen - 2; // Fudge factor            
                var top = line.Index - (linesOnScreen / 2);
                line.Goto();
                editor.FirstVisibleLine = Math.Max(0, top);
                editor.Focus();
            }
        }

        private void BindEvents()
        {
            VgcApis.Misc.Utils.BindEditorDragDropEvent(editor);

            btnShowSearchBox.Click += (s, a) => ShowFormSearch();

            btnGoto.Click += (s, a) => GotoLine();

            tboxGoto.Click += (s, a) => tboxGoto.SelectAll();

            tboxGoto.KeyDown += (s, a) =>
            {
                if (a.KeyCode == Keys.Enter)
                {
                    Invoke(GotoLine);
                }
            };

            editor.InsertCheck += Scintilla_InsertCheck;
            editor.CharAdded += Scintilla_CharAdded;
            editor.TextChanged += Scintilla_TextChanged;
            editor.MouseClick += Scintilla_MouseClicked;
            editor.DoubleClick += Scintilla_DoubleClick;

            btnNewScript.Click += (s, a) => ClearEditor();

            btnKillLuaCore.Click += (s, a) => luaCoreCtrl.Abort();

            btnStopLuaCore.Click += (s, a) => luaCoreCtrl.Stop();

            btnRunScript.Click += (s, a) =>
            {
                formEditor.SetOutputPanelCollapseState(false);

                var name = cboxScriptName.Text;

                luaCoreCtrl.Abort();
                luaCoreCtrl.SetScriptName(string.IsNullOrEmpty(name) ? $"({I18N.Empty})" : name);
                luaCoreCtrl.ReplaceScript(editor.Text);
                luaCoreCtrl.isLoadClr = isLoadClrLib;
                luaCoreCtrl.Start();
            };

            btnClearOutput.Click += (s, a) =>
            {
                qLogger?.Reset();
            };


            btnSaveScript.Click += (s, a) => OnBtnSaveScriptClickHandler(true);

            cboxScriptName.DropDown += (s, a) => ReloadScriptName();

            cboxScriptName.SelectedValueChanged += CboxScriptNameChangedHandler;
        }

        private void OnBtnSaveScriptClickHandler(bool showResult)
        {
            var name = cboxScriptName.Text;
            if (string.IsNullOrWhiteSpace(name)
                && string.IsNullOrWhiteSpace(curFileName))
            {
                VgcApis.Misc.UI.MsgBoxAsync(I18N.ScriptNameNotSet);
                return;
            }

            var success = false;
            var curScript = editor.Text;

            if (!string.IsNullOrEmpty(name))
            {
                success = luaServer.AddOrReplaceScript(name, curScript);
                SetCurFileName("");
            }
            else
            {
                // save to file
                try
                {
                    File.WriteAllText(curFileName, curScript);
                    success = true;
                }
                catch { }
            }

            if (success)
            {
                preScriptContent = curScript;
            }

            if (showResult)
            {
                VgcApis.Misc.UI.MsgBoxAsync(success ? I18N.Done : I18N.Fail);
            }
        }

        private void ClearEditor()
        {
            if (IsChanged() && !VgcApis.Misc.UI.Confirm(I18N.DiscardUnsavedChanges))
            {
                return;
            }
            preScriptName = "";
            preScriptContent = "";
            editor.Text = "";
            cboxScriptName.Text = "";
        }

        void CboxScriptNameChangedHandler(object sender, EventArgs args)
        {
            var name = cboxScriptName.Text;

            if (name == preScriptName)
            {
                return;
            }

            if (IsChanged() && !VgcApis.Misc.UI.Confirm(I18N.DiscardUnsavedChanges))
            {
                cboxScriptName.Text = preScriptName;
                return;
            }

            preScriptName = name;
            preScriptContent = settings.GetLuaCoreSettings()
                .Where(s => s.name == name)
                .FirstOrDefault()
                ?.script
                ?? string.Empty;

            editor.Text = preScriptContent;
            SetCurFileName("");
        }

        void ReloadScriptName()
        {
            var scriptsName = luaServer
                .GetAllLuaCoreCtrls()
                .Select(c => c.name)
                .ToArray();

            Invoke(() =>
            {
                cboxScriptName.Items.Clear();
                cboxScriptName.Items.AddRange(scriptsName);
            });
        }


        #endregion
    }
}
