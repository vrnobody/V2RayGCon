using AutocompleteMenuNS;
using Luna.Resources.Langs;
using ScintillaNET;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Luna.Controllers
{
    internal sealed class TabEditorCtrl
    {
        Services.Settings settings;
        Services.LuaServer luaServer;

        LuaCoreCtrl luaCoreCtrl;
        VgcApis.WinForms.FormSearch formSearch = null;
        VgcApis.Libs.Views.RepaintCtrl repaintCtrl;
        VgcApis.Libs.Sys.QueueLogger qLogger = new VgcApis.Libs.Sys.QueueLogger();

        Scintilla luaEditor = null;
        AutocompleteMenu luaAcm = null;

        #region controls
        ComboBox cboxScriptName;
        Button btnNewScript,
            btnSaveScript,
            btnDeleteScript,
            btnRunScript,
            btnStopScript,
            btnKillScript,
            btnClearOutput;

        RichTextBox rtboxOutput;
        Panel pnlEditorContainer;
        #endregion

        string preScriptName = string.Empty;
        string preScriptContent = string.Empty;

        VgcApis.Libs.Tasks.Routine logUpdater;

        public TabEditorCtrl(
            ComboBox cboxScriptName,
            Button btnNewScript,
            Button btnSaveScript,
            Button btnDeleteScript,
            Button btnRunScript,
            Button btnStopScript,
            Button btnKillScript,
            Button btnClearOutput,
            RichTextBox rtboxOutput,
            Panel pnlEditorContainer,
            SplitContainer splitContainer)
        {
            this.cboxScriptName = cboxScriptName;
            this.btnNewScript = btnNewScript;
            this.btnSaveScript = btnSaveScript;
            this.btnDeleteScript = btnDeleteScript;
            this.btnRunScript = btnRunScript;
            this.btnStopScript = btnStopScript;
            this.btnKillScript = btnKillScript;
            this.btnClearOutput = btnClearOutput;
            this.rtboxOutput = rtboxOutput;
            this.pnlEditorContainer = pnlEditorContainer;
            this.splitContainer = splitContainer;

            logUpdater = new VgcApis.Libs.Tasks.Routine(
               UpdateOutput,
               VgcApis.Models.Consts.Intervals.LuaPluginLogRefreshInterval);

        }

        public void Run(
          VgcApis.Interfaces.Services.IApiService api,
          Services.Settings settings,
          Services.LuaServer luaServer)
        {
            this.settings = settings;
            this.luaServer = luaServer;
            this.luaCoreCtrl = CreateLuaCoreCtrl(settings, api);

            InitControls();
            BindEvents();

            ReloadScriptName();

            repaintCtrl = new VgcApis.Libs.Views.RepaintCtrl(rtboxOutput);
            logUpdater.Run();
        }

        #region public methods
        public void KeyBoardShortcutHandler(KeyEventArgs keyEvent)
        {
            var keyCode = keyEvent.KeyCode;
            if (keyEvent.Control)
            {
                switch (keyCode)
                {
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
                case Keys.F5:
                    btnRunScript.PerformClick();
                    break;
                case Keys.F6:
                    btnStopScript.PerformClick();
                    break;
                case Keys.F7:
                    btnKillScript.PerformClick();
                    break;
                case Keys.F8:
                    btnClearOutput.PerformClick();
                    break;
            }
        }

        public bool IsChanged()
        {
            var script = luaEditor.Text;
            if (script == preScriptContent)
            {
                return false;
            }
            return true;
        }

        LuaCoreCtrl CreateLuaCoreCtrl(
            Services.Settings settings,
            VgcApis.Interfaces.Services.IApiService api)
        {
            var luaApis = new Models.Apis.LuaApis(settings, api);
            luaApis.Prepare();
            luaApis.SetRedirectLogWorker(Log);

            var coreSettings = new Models.Data.LuaCoreSetting();

            var ctrl = new LuaCoreCtrl();
            ctrl.Run(settings, coreSettings, luaApis);
            return ctrl;
        }

        public void Cleanup()
        {
            logUpdater?.Dispose();
            formSearch?.Close();
            luaCoreCtrl?.Kill();
            qLogger?.Dispose();

            if (luaAcm != null)
            {
                luaAcm.TargetControlWrapper = null;
            }
            luaEditor?.Dispose();
        }

        public string GetCurrentEditorContent() => luaEditor.Text;

        public void SetCurrentEditorContent(string content) =>
            VgcApis.Misc.UI.RunInUiThread(
                luaEditor, () => luaEditor.Text = content);

        public bool SaveScript()
        {
            var scriptName = cboxScriptName.Text;
            var content = luaEditor.Text;
            var success = luaServer.AddOrReplaceScript(scriptName, content);

            if (success)
            {
                preScriptContent = content;
            }

            return success;
        }
        #endregion

        #region Scintilla
        private int maxLineNumberCharLength;
        private void Scintilla_TextChanged(object sender, EventArgs e)
        {
            // Did the number of characters in the line number display change?
            // i.e. nnn VS nn, or nnnn VS nn, etc...
            var maxLineNumberCharLength = luaEditor.Lines.Count.ToString().Length;
            if (maxLineNumberCharLength == this.maxLineNumberCharLength)
                return;

            // Calculate the width required to display the last line number
            // and include some padding for good measure.
            const int padding = 2;
            luaEditor.Margins[0].Width = luaEditor.TextWidth(Style.LineNumber, new string('9', maxLineNumberCharLength + 1)) + padding;
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
            var line = luaEditor.Lines[luaEditor.CurrentLine];
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

            var indent = luaEditor.Lines[luaEditor.CurrentLine].Indentation;
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
            int curLine = luaEditor.CurrentLine;
            if (curLine < 2)
            {
                return;
            }

            string text = Scintilla_TrimText(luaEditor.Lines[curLine].Text);
            if (text == "}"
                || text.StartsWith("until")
                || text == "else"
                || text == "end")
            {
                var indent = luaEditor.Lines[curLine - 1].Indentation - 4;
                luaEditor.Lines[curLine].Indentation = Math.Max(0, indent);
            }
        }

        #endregion

        #region private methods
        void ShowFormSearch()
        {
            if (formSearch != null)
            {
                formSearch.Activate();
                return;
            }

            formSearch = new VgcApis.WinForms.FormSearch(luaEditor);
            formSearch.FormClosed += (s, a) => formSearch = null;
        }

        void Log(string content) => qLogger.Log(content);

        long updateOutputTimeStamp = 0;
        VgcApis.Libs.Tasks.Bar bar = new VgcApis.Libs.Tasks.Bar();
        private readonly SplitContainer splitContainer;

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

            VgcApis.Misc.UI.RunInUiThread(rtboxOutput, () =>
            {
                // form maybe closed
                try
                {
                    repaintCtrl.Disable();
                    rtboxOutput.Text = qLogger.GetLogAsString(true);
                    VgcApis.Misc.UI.ScrollToBottom(rtboxOutput);
                    repaintCtrl.Enable();
                    updateOutputTimeStamp = timestamp;
                }
                catch { }
                finally
                {
                    bar.Remove();
                }
            });
        }

        private void BindEvents()
        {
            luaEditor.InsertCheck += Scintilla_InsertCheck;
            luaEditor.CharAdded += Scintilla_CharAdded;
            luaEditor.TextChanged += Scintilla_TextChanged;

            btnNewScript.Click += (s, a) => ClearEditor();

            btnKillScript.Click += (s, a) => luaCoreCtrl.Kill();

            btnStopScript.Click += (s, a) => luaCoreCtrl.Stop();

            btnRunScript.Click += (s, a) =>
            {
                splitContainer.Panel2Collapsed = false;

                var name = cboxScriptName.Text;

                luaCoreCtrl.Kill();

                luaCoreCtrl.SetScriptName(
                    string.IsNullOrEmpty(name)
                    ? $"({I18N.Empty})" : name);

                luaCoreCtrl.ReplaceScript(luaEditor.Text);
                luaCoreCtrl.Start();
            };

            btnClearOutput.Click += (s, a) =>
            {
                qLogger?.Reset();
            };

            btnDeleteScript.Click += (s, a) =>
            {
                var scriptName = cboxScriptName.Text;
                if (string.IsNullOrEmpty(scriptName)
                || !VgcApis.Misc.UI.Confirm(I18N.ConfirmRemoveScript))
                {
                    return;
                }

                if (!luaServer.RemoveScriptByName(scriptName))
                {
                    VgcApis.Misc.UI.MsgBoxAsync("", I18N.ScriptNotFound);
                }
            };

            btnSaveScript.Click += (s, a) => OnBtnSaveScriptClickHandler(true);

            cboxScriptName.DropDown += (s, a) => ReloadScriptName();

            cboxScriptName.SelectedValueChanged += CboxScriptNameChangedHandler;

        }

        private void OnBtnSaveScriptClickHandler(bool showResult)
        {
            var scriptName = cboxScriptName.Text;
            if (string.IsNullOrEmpty(scriptName))
            {
                VgcApis.Misc.UI.MsgBoxAsync("", I18N.ScriptNameNotSet);
                return;
            }

            var success = SaveScript();
            if (showResult)
            {
                VgcApis.Misc.UI.MsgBoxAsync("", success ? I18N.Done : I18N.Fail);
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
            luaEditor.Text = "";
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
            preScriptContent = LoadScriptByName(name);
            luaEditor.Text = preScriptContent;
        }

        string LoadScriptByName(string name) =>
            settings.GetLuaCoreSettings()
                .Where(s => s.name == name)
                .FirstOrDefault()
                ?.script
                ?? string.Empty;

        void InitControls()
        {
            // script editor
            luaEditor = Misc.UI.CreateLuaEditor(pnlEditorContainer);
            luaAcm = settings.AttachSnippetsTo(luaEditor);
        }

        void ReloadScriptName()
        {
            cboxScriptName.Items.Clear();

            var cores = settings
                .GetLuaCoreSettings()
                .OrderBy(c => c.name)
                .ToList();

            foreach (var coreState in cores)
            {
                cboxScriptName.Items.Add(coreState.name);
            }
        }


        #endregion
    }
}
