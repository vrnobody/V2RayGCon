using NeoLuna.Resources.Langs;
using NeoLuna.Views.WinForms;
using ScintillaNET;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace NeoLuna.Controllers.FormEditorCtrl
{
    internal sealed class ButtonCtrl
    {
        Services.Settings settings;
        Services.LuaServer luaServer;

        private readonly FormEditor formEditor;
        VgcApis.WinForms.FormSearch formSearch = null;

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
            RichTextBox rtboxOutput
        )
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
                VgcApis.Models.Consts.Intervals.LuaPluginLogRefreshInterval
            );
        }

        public void Run(Services.FormMgrSvc formMgr)
        {
            this.settings = formMgr.settings;
            this.luaServer = formMgr.luaServer;

            // this.luaCoreCtrl = CreateLuaCoreCtrl(settings, api);

            isLoadClrLib = false;
            this.luaCoreCtrl = Misc.Utils.CreateLuaCoreCtrl(formMgr, Log);

            BindEvents();
            ReloadScriptName();

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
                    case Keys.OemOpenBrackets:
                        editor.ZoomOut();
                        break;
                    case Keys.Oem6:
                        editor.ZoomIn();
                        break;
                    case Keys.G:
                        tboxGoto.Focus();
                        tboxGoto.SelectAll();
                        break;
                    case Keys.F:
                    case Keys.H:
                        ShowFormSearchHandler(this, EventArgs.Empty);
                        break;
                    case Keys.S:
                        SaveScript(false);
                        break;
                    case Keys.N:
                        ClearEditorHandler(this, EventArgs.Empty);
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

        public bool IsChanged() => editor.Text != preScriptContent;

        public void Cleanup()
        {
            ReleaseEvents();
            VgcApis.Misc.UI.CloseFormIgnoreError(formSearch);
            luaCoreCtrl?.AbortNow();
            logUpdater?.Dispose();
            qLogger?.Dispose();
        }

        public string GetCurrentEditorContent() => editor.Text;

        public void SetScriptCache(string content) => preScriptContent = content;

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
            editor.Margins[0].Width =
                editor.TextWidth(Style.LineNumber, new string('9', maxLineNumberCharLength + 1))
                + padding;
            this.maxLineNumberCharLength = maxLineNumberCharLength;
        }

        string Scintilla_TrimText(string text) =>
            text?.Replace("\t", "")?.Replace("\r", "")?.Replace("\n", "")?.Trim()?.ToLower();

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
            if (
                text.StartsWith("function")
                || text.StartsWith("local function")
                || text.EndsWith("do")
                || text.EndsWith("then")
                || text.EndsWith("else")
                || text == "repeat"
                || Regex.IsMatch(text, "{\\s*$")
            )
            {
                indent = indent + 4;
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
            if (text == "}" || text.StartsWith("until") || text == "else" || text == "end")
            {
                var indent = editor.Lines[curLine - 1].Indentation - 4;
                editor.Lines[curLine].Indentation = Math.Max(0, indent);
            }
        }

        #endregion

        #region private methods
        void Invoke(Action action) => VgcApis.Misc.UI.Invoke(action);

        void ShowFormSearchHandler(object sender, EventArgs args)
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

        void UpdateOutput()
        {
            var timestamp = qLogger.GetTimestamp();
            if (updateOutputTimeStamp == timestamp)
            {
                return;
            }

            var logs = qLogger.GetLogAsString(true);
            updateOutputTimeStamp = timestamp;

            VgcApis.Misc.UI.UpdateRichTextBox(rtboxOutput, logs);
        }

        void GotoLineHandler(object sender, EventArgs args)
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

        void SelectAllHandler(object sender, EventArgs args)
        {
            tboxGoto.SelectAll();
        }

        void GotoBoxKeyDownHandler(object sender, KeyEventArgs args)
        {
            if (args.KeyCode == Keys.Enter)
            {
                Invoke(() => GotoLineHandler(this, EventArgs.Empty));
            }
        }

        private void ReleaseEvents()
        {
            btnShowSearchBox.Click -= ShowFormSearchHandler;
            btnGoto.Click -= GotoLineHandler;
            tboxGoto.Click -= SelectAllHandler;
            tboxGoto.KeyDown -= GotoBoxKeyDownHandler;
            editor.InsertCheck -= Scintilla_InsertCheck;
            editor.CharAdded -= Scintilla_CharAdded;
            editor.TextChanged -= Scintilla_TextChanged;
            editor.MouseClick -= Scintilla_MouseClicked;
            editor.DoubleClick -= Scintilla_DoubleClick;
            btnNewScript.Click -= ClearEditorHandler;

            btnKillLuaCore.Click -= OnBtnKillLuaCoreClickHandler;

            btnStopLuaCore.Click -= OnBtnStopLuaCoreClickHandler;

            btnRunScript.Click -= OnBtnRunScriptClickHandler;

            btnClearOutput.Click -= OnBtnClearOutputClickHandler;

            btnSaveScript.Click -= OnBtnSaveScriptClickHandler;

            cboxScriptName.DropDown -= OnCboxScriptNameDropDownHandler;

            cboxScriptName.SelectedValueChanged -= CboxScriptNameChangedHandler;
        }

        void OnBtnRunScriptClickHandler(object sender, EventArgs args)
        {
            formEditor.SetOutputPanelCollapseState(false);

            var name = cboxScriptName.Text;
            var script = editor.Text;

            luaCoreCtrl.name = string.IsNullOrEmpty(name) ? $"({I18N.Empty})" : name;
            Misc.Utils.DoString(luaCoreCtrl, script, isLoadClrLib);
        }

        private void BindEvents()
        {
            VgcApis.Misc.Utils.BindEditorDragDropEvent(editor);

            btnShowSearchBox.Click += ShowFormSearchHandler;

            btnGoto.Click += GotoLineHandler;

            tboxGoto.Click += SelectAllHandler;

            tboxGoto.KeyDown += GotoBoxKeyDownHandler;

            editor.InsertCheck += Scintilla_InsertCheck;
            editor.CharAdded += Scintilla_CharAdded;
            editor.TextChanged += Scintilla_TextChanged;
            editor.MouseClick += Scintilla_MouseClicked;
            editor.DoubleClick += Scintilla_DoubleClick;

            btnNewScript.Click += ClearEditorHandler;

            btnKillLuaCore.Click += OnBtnKillLuaCoreClickHandler;

            btnStopLuaCore.Click += OnBtnStopLuaCoreClickHandler;

            btnRunScript.Click += OnBtnRunScriptClickHandler;

            btnClearOutput.Click += OnBtnClearOutputClickHandler;

            btnSaveScript.Click += OnBtnSaveScriptClickHandler;

            cboxScriptName.DropDown += OnCboxScriptNameDropDownHandler;

            cboxScriptName.SelectedValueChanged += CboxScriptNameChangedHandler;
        }

        void OnBtnStopLuaCoreClickHandler(object sender, EventArgs args)
        {
            luaCoreCtrl.Stop();
        }

        void OnBtnKillLuaCoreClickHandler(object sender, EventArgs args)
        {
            luaCoreCtrl.Abort();
        }

        void OnCboxScriptNameDropDownHandler(object sender, EventArgs args)
        {
            ReloadScriptName();
        }

        void OnBtnClearOutputClickHandler(object sender, EventArgs args)
        {
            qLogger?.Clear();
        }

        void OnBtnSaveScriptClickHandler(object sender, EventArgs args)
        {
            SaveScript(true);
        }

        private void SaveScript(bool showResult)
        {
            var name = cboxScriptName.Text;
            if (string.IsNullOrWhiteSpace(name) && string.IsNullOrWhiteSpace(curFileName))
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

        private void ClearEditorHandler(object sender, EventArgs args)
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
            preScriptContent =
                settings.GetLuaCoreSettings().Where(s => s.name == name).FirstOrDefault()?.script
                ?? string.Empty;

            editor.Text = preScriptContent;
            SetCurFileName("");
        }

        void ReloadScriptName()
        {
            var scriptsName = luaServer.GetAllLuaCoreCtrls().Select(c => c.name).ToArray();

            Invoke(() =>
            {
                cboxScriptName.Items.Clear();
                cboxScriptName.Items.AddRange(scriptsName);
            });
        }

        #endregion
    }
}
