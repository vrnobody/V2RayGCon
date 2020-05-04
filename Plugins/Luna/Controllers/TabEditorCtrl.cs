﻿using AutocompleteMenuNS;
using Luna.Resources.Langs;
using Luna.Views.WinForms;
using ScintillaNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Luna.Controllers
{
    internal sealed class TabEditorCtrl
    {
        Services.Settings settings;
        Services.FormMgr formMgr;
        Services.LuaServer luaServer;

        LuaCoreCtrl luaCoreCtrl;
        VgcApis.WinForms.FormSearch formSearch = null;
        VgcApis.UserControls.RepaintController rtboxFreezer;
        VgcApis.Libs.Sys.QueueLogger qLogger = new VgcApis.Libs.Sys.QueueLogger();

        Scintilla luaEditor = null;
        AutocompleteMenu luaAcm = null;
        private readonly FormMain formMain;

        VgcApis.Libs.Infr.Recorder history = new VgcApis.Libs.Infr.Recorder();

        #region controls
        ComboBox cboxScriptName;
        Button btnNewScript,
            btnSaveScript,
            btnRunScript,
            btnStopScript,
            btnKillScript,
            btnClearOutput;
        private readonly ComboBox cboxVarList;
        private readonly ComboBox cboxFunctionList;
        RichTextBox rtboxOutput;
        private readonly Panel pnlEditorContainer;

        #endregion

        string preScriptName = string.Empty;
        string preScriptContent = string.Empty;

        VgcApis.Libs.Tasks.Routine logUpdater;

        public TabEditorCtrl(
            FormMain formMain,
            ComboBox cboxScriptName,
            Button btnNewScript,
            Button btnSaveScript,

            ComboBox cboxVarList,
            ComboBox cboxFunctionList,

            Button btnRunScript,
            Button btnStopScript,
            Button btnKillScript,
            Button btnClearOutput,
            RichTextBox rtboxOutput,
            Panel pnlEditorContainer)
        {
            this.formMain = formMain;
            this.cboxScriptName = cboxScriptName;
            this.btnNewScript = btnNewScript;
            this.btnSaveScript = btnSaveScript;
            this.cboxVarList = cboxVarList;
            this.cboxFunctionList = cboxFunctionList;
            this.btnRunScript = btnRunScript;
            this.btnStopScript = btnStopScript;
            this.btnKillScript = btnKillScript;
            this.btnClearOutput = btnClearOutput;
            this.rtboxOutput = rtboxOutput;
            this.pnlEditorContainer = pnlEditorContainer;

            logUpdater = new VgcApis.Libs.Tasks.Routine(
               UpdateOutput,
               VgcApis.Models.Consts.Intervals.LuaPluginLogRefreshInterval);

        }

        public void Run(
          VgcApis.Interfaces.Services.IApiService api,
          Services.Settings settings,
          Services.FormMgr formMgr,
          Services.LuaServer luaServer)
        {
            this.formMgr = formMgr;
            this.settings = settings;
            this.luaServer = luaServer;
            this.luaCoreCtrl = CreateLuaCoreCtrl(settings, api);

            InitControls();
            BindEvents();
            ReloadScriptName();

            rtboxFreezer = new VgcApis.UserControls.RepaintController(rtboxOutput);
            logUpdater.Run();

#if DEBUG
            if (cboxScriptName.Items.Count > 0)
            {
                cboxScriptName.SelectedIndex = 0;
            }
#endif
        }

        #region public methods
        public void KeyBoardShortcutHandler(KeyEventArgs keyEvent)
        {
            var keyCode = keyEvent.KeyCode;
            if (keyEvent.Control)
            {
                switch (keyCode)
                {
                    case Keys.OemMinus:
                        history.Backward();
                        VgcApis.Misc.UI.Invoke(() => ScrollToLine(history.Current()));
                        break;
                    case Keys.Oemplus:
                        history.Forward();
                        VgcApis.Misc.UI.Invoke(() => ScrollToLine(history.Current()));
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
                case Keys.F12:
                    history.Add(luaEditor.CurrentLine);
                    var w = luaEditor.GetWordFromPosition(luaEditor.CurrentPosition);
                    VgcApis.Misc.UI.Invoke(() => ScrollToDefinition(w));
                    break;
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

        public bool IsChanged() =>
            luaEditor.Text != preScriptContent;

        LuaCoreCtrl CreateLuaCoreCtrl(
            Services.Settings settings,
            VgcApis.Interfaces.Services.IApiService api)
        {
            var luaApis = new Models.Apis.LuaApis(api, settings, formMgr);
            luaApis.Prepare();
            luaApis.SetRedirectLogWorker(Log);

            var coreSettings = new Models.Data.LuaCoreSetting()
            {
                isLoadClr = true,
            };
            var ctrl = new LuaCoreCtrl(true);
            ctrl.Run(settings, coreSettings, luaApis);
            return ctrl;
        }

        public void Cleanup()
        {
            logUpdater?.Dispose();
            formSearch?.Close();
            luaCoreCtrl?.AbortNow();
            qLogger?.Dispose();

            if (luaAcm != null)
            {
                luaAcm.TargetControlWrapper = null;
            }
            luaEditor?.Dispose();
        }

        public string GetCurrentEditorContent() => luaEditor.Text;

        public void SetScriptCache(string content) =>
            preScriptContent = content;

        public void SetCurrentEditorContent(string content) =>
            VgcApis.Misc.UI.Invoke(() => luaEditor.Text = content);

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

            formSearch = VgcApis.WinForms.FormSearch.CreateForm(luaEditor);
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

            VgcApis.Misc.UI.Invoke(() =>
           {
               rtboxFreezer.DisableRepaintEvent();
               rtboxOutput.Text = qLogger.GetLogAsString(true);
               VgcApis.Misc.UI.ScrollToBottom(rtboxOutput);
               rtboxFreezer.EnableRepaintEvent();
               updateOutputTimeStamp = timestamp;
           });
            bar.Remove();
        }

        string[] ExtractFunctionsFrom(string text)
        {
            var kws = new string[] { "function", "local function" };
            List<string> fns = new List<string>();

            var lines = text.Replace("\r", "").Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines)
            {
                if (line.StartsWith("function "))
                {
                    fns.Add(line);
                }
                else if (line.StartsWith("local function "))
                {
                    fns.Add(line.Substring("local ".Length));
                }
            }

            return fns.Distinct().OrderBy(x => x).ToArray();
        }

        void ScrollToFunction(string text)
        {
            foreach (var line in luaEditor.Lines)
            {
                var t = line.Text;
                if (t.StartsWith("local function ") || t.StartsWith("function "))
                {
                    if (t.Contains(text))
                    {
                        line.Goto();
                        luaEditor.FirstVisibleLine = line.Index;
                        history.Add(line.Index);
                        return;
                    }
                }
            }
        }

        string[] ExtractVariablesFrom(string text)
        {
            List<string> vs = new List<string>();
            var kw = "local ";

            var lines = text.Replace("\r", "").Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines)
            {
                if (line.StartsWith("local function "))
                {
                    continue;
                }

                if (!line.StartsWith(kw))
                {
                    continue;
                }

                var parts = line.Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length < 1 || parts[0].Length < kw.Length)
                {
                    continue;
                }

                var v = parts[0].Substring(kw.Length).Trim();
                if (v.Length > 0)
                {
                    vs.Add(v);
                }
            }

            return vs.Distinct().OrderBy(x => x).ToArray();
        }

        void ScrollToVariable(string text)
        {
            foreach (var line in luaEditor.Lines)
            {
                var t = line.Text;
                if (t.StartsWith("local ") && t.Contains(text))
                {
                    line.Goto();
                    luaEditor.FirstVisibleLine = line.Index;
                    history.Add(line.Index);
                    return;
                }
            }
        }

        void ScrollToLine(int index)
        {
            var max = luaEditor.Lines.Count;
            if (index >= max)
            {
                return;
            }
            var line = luaEditor.Lines[index];
            line.Goto();
            var text = line.Text;
            if (text.StartsWith("local function ") || text.StartsWith("function "))
            {
                luaEditor.FirstVisibleLine = line.Index;
            }
            else
            {
                line.EnsureVisible();
            }
        }

        void ScrollToDefinition(string text)
        {
            foreach (var line in luaEditor.Lines)
            {
                var t = line.Text;
                if (!t.StartsWith("local ") && !t.StartsWith("function "))
                {
                    continue;
                }

                if (t.Contains(text))
                {
                    line.Goto();
                    luaEditor.FirstVisibleLine = line.Index;
                    history.Add(line.Index);
                    return;
                }
            }
        }

        private void BindEvents()
        {
            cboxVarList.DropDownClosed += (s, a) =>
               VgcApis.Misc.UI.Invoke(() =>
               {
                   var kw = cboxVarList.Text;
                   if (string.IsNullOrWhiteSpace(kw))
                   {
                       return;
                   }
                   ScrollToVariable(kw);
                   luaEditor.Focus();
               });

            cboxVarList.DropDown += (s, a) =>
            {
                history.Add(luaEditor.CurrentLine);
                var content = luaEditor.Text;
                var list = ExtractVariablesFrom(content);
                VgcApis.Misc.UI.Invoke(() =>
                {
                    var items = cboxVarList.Items;
                    items.Clear();
                    items.AddRange(list);
                    VgcApis.Misc.UI.ResetComboBoxDropdownMenuWidth(cboxVarList);
                });
            };

            cboxFunctionList.DropDownClosed += (s, a) =>
                VgcApis.Misc.UI.Invoke(() =>
                {
                    var kw = cboxFunctionList.Text;
                    if (string.IsNullOrWhiteSpace(kw))
                    {
                        return;
                    }
                    ScrollToFunction(kw);
                    luaEditor.Focus();
                });

            cboxFunctionList.DropDown += (s, a) =>
            {
                history.Add(luaEditor.CurrentLine);
                var content = luaEditor.Text;
                var list = ExtractFunctionsFrom(content);
                VgcApis.Misc.UI.Invoke(() =>
                {
                    var items = cboxFunctionList.Items;
                    items.Clear();
                    items.AddRange(list);
                    VgcApis.Misc.UI.ResetComboBoxDropdownMenuWidth(cboxFunctionList);
                });
            };

            luaEditor.InsertCheck += Scintilla_InsertCheck;
            luaEditor.CharAdded += Scintilla_CharAdded;
            luaEditor.TextChanged += Scintilla_TextChanged;

            btnNewScript.Click += (s, a) => ClearEditor();

            btnKillScript.Click += (s, a) => luaCoreCtrl.Abort();

            btnStopScript.Click += (s, a) => luaCoreCtrl.Stop();

            btnRunScript.Click += (s, a) =>
            {
                formMain.SetOutputPanelCollapseState(false);

                var name = cboxScriptName.Text;

                luaCoreCtrl.Abort();
                luaCoreCtrl.SetScriptName(string.IsNullOrEmpty(name) ? $"({I18N.Empty})" : name);
                luaCoreCtrl.ReplaceScript(luaEditor.Text);
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
            var scriptName = cboxScriptName.Text;
            if (string.IsNullOrEmpty(scriptName))
            {
                VgcApis.Misc.UI.MsgBoxAsync(I18N.ScriptNameNotSet);
                return;
            }

            var success = SaveScript();
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
            var scriptsName = luaServer
                .GetAllLuaCoreCtrls()
                .Select(c => c.name)
                .ToArray();

            cboxScriptName.Items.Clear();
            cboxScriptName.Items.AddRange(scriptsName);
        }


        #endregion
    }
}
