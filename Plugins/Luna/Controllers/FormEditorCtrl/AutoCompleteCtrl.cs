using AutocompleteMenuNS;
using Luna.Libs.LuaSnippet;
using Luna.Services;
using Newtonsoft.Json.Linq;
using ScintillaNET;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Luna.Controllers.FormEditorCtrl
{
    internal sealed class AutoCompleteCtrl
    {
        private readonly Scintilla editor;
        private readonly ComboBox cboxVarList;
        private readonly ComboBox cboxFunctionList;
        private readonly ToolStripMenuItem miEanbleCodeAnalyzeEx;
        private readonly ToolStripStatusLabel smiLbCodeanalyze;

        Services.AstServer astServer;
        AutocompleteMenu luaAcm = null;
        BestMatchSnippets bestMatchSnippets = null;

        VgcApis.Libs.Infr.Recorder history = new VgcApis.Libs.Infr.Recorder();
        VgcApis.Libs.Tasks.LazyGuy lazyAnalyser;


        public AutoCompleteCtrl(
            AstServer astServer,

            Scintilla editor,
            ComboBox cboxVarList,
            ComboBox cboxFunctionList,

            ToolStripMenuItem miEanbleCodeAnalyzeEx,
            ToolStripStatusLabel smiLbCodeanalyze)
        {
            this.astServer = astServer;
            this.editor = editor;
            this.cboxVarList = cboxVarList;
            this.cboxFunctionList = cboxFunctionList;
            this.miEanbleCodeAnalyzeEx = miEanbleCodeAnalyzeEx;
            this.smiLbCodeanalyze = smiLbCodeanalyze;
        }

        bool isEnableCodeAnalyzeEx;

        public void Run()
        {
            lazyAnalyser = new VgcApis.Libs.Tasks.LazyGuy(
                AnalizeScriptWorker,
                VgcApis.Models.Consts.Intervals.AnalizeLuaScriptDelay,
                3000);

            InitControls();
            BindEvents();

            EnableCodeAnalyzeEx(false);
            AstServerOnFileChangedHandler();
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
                        ScrollLineToTheMiddle(history.Current());
                        break;
                    case Keys.Oemplus:
                        history.Forward();
                        ScrollLineToTheMiddle(history.Current());
                        break;
                }
                return;
            }

            switch (keyCode)
            {
                case Keys.F12:
                    history.Add(editor.CurrentLine);
                    var w = VgcApis.Misc.Utils.GetWordFromCurPos(editor);
                    ScrollToDefinition(w);
                    break;
            }

        }


        public void Cleanup()
        {
            ReleaseEvents();
            lazyAnalyser?.Dispose();

            luaAcm.SetAutocompleteMenu(editor, null);
            luaAcm.Dispose();
            luaAcm = null;
            bestMatchSnippets?.Cleanup();
        }
        #endregion


        #region file watching

        void AstServerOnFileChangedHandler()
        {
            var modules = astServer.GetRequireModuleNames();
            var ms = modules.Select(m => new LuaImportClrSnippets(m)).ToList();
            bestMatchSnippets?.UpdateRequireModuleSnippets(ms);
        }

        #endregion

        #region code analyzing

        void BuildSnippets(JObject ast, List<MatchItemBase> snippets)
        {
            if (ast == null)
            {
                return;
            }

            if (ast[Services.AstServer.KEY_VARS] is JObject)
            {
                foreach (var kv in ast[Services.AstServer.KEY_VARS] as JObject)
                {
                    snippets.Add(new LuaKeywordSnippets(kv.Key));
                }
            }

            if (ast[Services.AstServer.KEY_FUNCTION] is JObject)
            {
                foreach (var kv in ast[Services.AstServer.KEY_FUNCTION] as JObject)
                {
                    var ps = (kv.Value as JObject)[Services.AstServer.KEY_PARAMS] as JArray;
                    var sps = string.Join(", ", ps);
                    snippets.Add(new LuaFuncSnippets($"{kv.Key}({sps})"));
                }
            }

            if (ast[Services.AstServer.KEY_SUB_FUNCS] is JObject)
            {
                foreach (var kv in ast[Services.AstServer.KEY_SUB_FUNCS] as JObject)
                {
                    var ps = (kv.Value as JObject)[Services.AstServer.KEY_PARAMS] as JArray;
                    var sps = string.Join(", ", ps);
                    snippets.Add(new LuaSubFuncSnippets($"{kv.Key}({sps})", "."));
                }
            }

            if (ast[Services.AstServer.KEY_METHODS] is JObject)
            {
                foreach (var kv in ast[Services.AstServer.KEY_METHODS] as JObject)
                {
                    var ps = (kv.Value as JObject)[Services.AstServer.KEY_PARAMS] as JArray;
                    var sps = string.Join(", ", ps);
                    snippets.Add(new LuaSubFuncSnippets($"{kv.Key}({sps})", ":"));
                }
            }

            BuildModuleSnippets(ast, snippets);
        }

        private void BuildModuleSnippets(JObject ast, List<MatchItemBase> snippets)
        {
            if (!(ast[Services.AstServer.KEY_MODULES] is JObject))
            {
                return;
            }

            foreach (var kv in ast[Services.AstServer.KEY_MODULES] as JObject)
            {
                var mn = kv.Value.ToString();
                var mAst = astServer.AnalyzeModule(mn, isEnableCodeAnalyzeEx);
                if (mAst == null)
                {
                    continue;
                }

                BuildOneModuleSnippets(kv.Key, mAst, snippets);
            }
        }

        private void BuildOneModuleSnippets(string varName, JObject ast, List<MatchItemBase> snippets)
        {
            var fds = new Dictionary<string, string>() {
                { Services.AstServer.KEY_FUNCTION,"." },
                { Services.AstServer.KEY_METHODS ,":" },
            };

            snippets.Add(new LuaKeywordSnippets(varName));

            foreach (var kv in ast)
            {
                if (kv.Value is JArray && kv.Key == Services.AstServer.KEY_PROPERTY)
                {
                    foreach (string prop in kv.Value as JArray)
                    {
                        var snp = new LuaKeywordSnippets($"{varName}.{prop}");
                        snippets.Add(snp);
                    }
                    continue;
                }

                if (kv.Value is JObject && fds.Keys.Contains(kv.Key))
                {
                    foreach (var skv in kv.Value as JObject)
                    {
                        var ps = string.Join(", ", skv.Value as JArray);
                        var sep = fds[kv.Key];
                        snippets.Add(new LuaSubFuncSnippets($"{varName}{sep}{skv.Key}({ps})", sep));
                    }
                }
            }
        }

        string GetAllTextExceptCurLine()
        {
            StringBuilder sb = new StringBuilder();
            int curLine = editor.CurrentLine;
            var lines = editor.Lines;

            for (int i = 0; i < lines.Count; i++)
            {
                if (i == curLine)
                {
                    sb.Append("\n");
                }
                else
                {
                    sb.Append(lines[i].Text);
                }
            }
            return sb.ToString();
        }

        JObject currentCodeAst = null;
        void AnalizeScriptWorker()
        {
            List<string> srcs = new List<string>();

            VgcApis.Misc.UI.Invoke(() =>
            {
                srcs.Add(editor.Text);
                srcs.Add(GetAllTextExceptCurLine());
            });

            JObject ast = null;
            string key = null;
            foreach (var src in srcs)
            {
                key = src;
                ast = astServer.AnalyzeCode(src);
                if (ast != null)
                {
                    break;
                }
            }

            if (ast != null)
            {
                currentCodeAst = ast;
            }

            // var debug = ast.ToString();

            var snps = new List<MatchItemBase>();
            BuildSnippets(ast, snps);
            bestMatchSnippets?.UpdateCustomScriptSnippets(snps);
        }

        void AnalyzeScriptLater(object sender, EventArgs e)
        {
            lazyAnalyser?.Throttle();
        }

        #endregion


        #region private methods
        void EnableCodeAnalyzeEx(bool isEnable)
        {
            isEnableCodeAnalyzeEx = isEnable;
            AnalyzeScriptLater(this, EventArgs.Empty);
            Invoke(() =>
            {
                miEanbleCodeAnalyzeEx.Checked = isEnable;
                smiLbCodeanalyze.Enabled = isEnable;
            });

        }

        void Invoke(Action action) => VgcApis.Misc.UI.Invoke(action);

        string RemoveLocalPrefix(string text, bool trimFunctionPrefix)
        {
            if (string.IsNullOrEmpty(text))
            {
                return text;
            }

            string[] kws = new string[] { "local ", "function " };
            if (text.StartsWith(kws[0]))
            {
                text = text.Substring(kws[0].Length);
            }

            if (trimFunctionPrefix && text.StartsWith(kws[1]))
            {
                text = text.Substring(kws[1].Length);
            }
            return text;
        }

        bool ScrollToFunction(string text)
        {
            var funcs = funcDefTable;
            if (funcs.ContainsKey(text))
            {
                ScrollToLine(funcs[text]);
                return true;
            }

            return FallbackScrollToFunction(text);
        }

        bool FallbackScrollToFunction(string text)
        {
            text = text?.Replace(":", ".")?.Replace(" ", "")
                ?.Replace("['", ".")
                ?.Replace("[\"", ".")
                ?.Replace("\"]", "")
                ?.Replace("']", "")
                ?.Split('(')?.FirstOrDefault();

            if (string.IsNullOrWhiteSpace(text))
            {
                return false;
            }

            foreach (var line in editor.Lines)
            {
                var t = line.Text;
                if (string.IsNullOrWhiteSpace(t) || !t.Contains("function"))
                {
                    continue;
                }

                var trimed = RemoveLocalPrefix(t, true)
                    ?.Replace(" ", "")
                    ?.Replace(":", ".")
                    ?.Replace("[\"", ".")
                    ?.Replace("['", ".")
                    ?.Replace("\"]", "")
                    ?.Replace("']", "")
                    ?.Replace("=function(", "(")
                    ?.Split('(')
                    ?.FirstOrDefault();

                if (trimed == text)
                {
                    ScrollToLine(line.Index);
                    return true;
                }
            }

            return false;
        }

        void ScrollToLine(int index)
        {
            index = Math.Min(Math.Max(0, index), editor.Lines.Count);
            history.Add(index);

            var line = editor.Lines[index];
            line.Goto();
            editor.FirstVisibleLine = line.Index;
        }

        void ScrollLineToTheMiddle(int index)
        {
            history.Add(index);
            editor.Lines[index].Goto();
            var first = index - editor.LinesOnScreen / 2 + 1;
            editor.FirstVisibleLine = first;
        }

        void ScrollToDefinition(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return;
            }

            if (ScrollToFunction(text) || ScrollToVariable(text))
            {
                return;
            }

            foreach (var line in editor.Lines)
            {
                var t = line.Text?.Trim();

                if (string.IsNullOrWhiteSpace(t) || !t.StartsWith("local "))
                {
                    continue;
                }

                var trimed = RemoveLocalPrefix(t, true);
                var first = trimed?.Split(new char[] { ' ', '.', '=', ',', '\r', '\n', '(' })
                    ?.FirstOrDefault(s => s == text);

                if (first != null)
                {
                    line.Goto();
                    line.EnsureVisible();
                    // ScrollToLine(line.Index);
                    return;
                }
            }
        }

        AutocompleteMenu CreateAcm(Scintilla editor)
        {

            bestMatchSnippets = astServer?.CreateBestMatchSnippet(editor);

            var imageList = new ImageList();
            imageList.Images.AddRange(new Image[] {
                Properties.Resources.KeyDown_16x,
                Properties.Resources.Method_16x,
                Properties.Resources.Class_16x,
            });

            var acm = new AutocompleteMenu()
            {
                SearchPattern = VgcApis.Models.Consts.Patterns.LuaSnippetSearchPattern,
                MaximumSize = new Size(300, 200),
                ToolTipDuration = 20000,
                ImageList = imageList,
            };

            acm.TargetControlWrapper = new ScintillaWrapper(editor);
            acm.SetAutocompleteItems(bestMatchSnippets);
            return acm;
        }

        private void InitControls()
        {
            luaAcm = CreateAcm(editor);
            miEanbleCodeAnalyzeEx.Checked = false;
            smiLbCodeanalyze.Enabled = false;
        }

        void ReleaseEvents()
        {
            astServer.OnFileChanged -= AstServerOnFileChangedHandler;
            var editor = this.editor;
            if (editor == null)
            {
                return;
            }

            editor.TextChanged -= AnalyzeScriptLater;
            editor.Click -= AddToHistory;
        }

        void AddToHistory(object sender, EventArgs args)
        {
            var editor = this.editor;
            if (editor != null)
            {
                history.Add(editor.CurrentLine);
            }
        }

        void BindEvents()
        {

            astServer.OnFileChanged += AstServerOnFileChangedHandler;

            editor.TextChanged += AnalyzeScriptLater;
            editor.Click += AddToHistory;

            miEanbleCodeAnalyzeEx.Click += (s, a) =>
            {
                var enable = !miEanbleCodeAnalyzeEx.Checked;
                EnableCodeAnalyzeEx(enable);
            };

            cboxVarList.DropDownClosed += (s, a) =>
                VgcApis.Misc.UI.Invoke(() =>
                {
                    var kw = cboxVarList.Text;
                    ScrollToVariable(kw);
                    editor.Focus();
                });

            cboxVarList.DropDown += OnCboxVarListDropDownHandler;

            cboxFunctionList.DropDownClosed += (s, a) =>
                VgcApis.Misc.UI.Invoke(() =>
                {
                    var kw = cboxFunctionList.Text;
                    ScrollToFunction(kw);
                    editor.Focus();
                });

            cboxFunctionList.DropDown += OnCboxFunctionListDropDownHandler;

        }

        Dictionary<string, int> funcDefTable = new Dictionary<string, int>();

        private void OnCboxFunctionListDropDownHandler(object sender, EventArgs args)
        {
            history.Add(editor.CurrentLine);

            var ast = currentCodeAst;
            var debug = ast?.ToString();

            string[] keys = new string[] {
                Services.AstServer.KEY_FUNCTION,
                Services.AstServer.KEY_SUB_FUNCS,
                Services.AstServer.KEY_METHODS,
            };

            Dictionary<string, int> funcs = new Dictionary<string, int>();

            foreach (var key in keys)
            {
                if (ast != null && ast[key] is JObject)
                {
                    foreach (var kv in ast[key] as JObject)
                    {
                        var ps = (kv.Value as JObject)[Services.AstServer.KEY_PARAMS] as JArray;
                        var luaLineNumber = (kv.Value as JObject)[Services.AstServer.KEY_LINE_NUM].Value<int>();
                        var sps = string.Join(", ", ps);
                        var fn = $"{kv.Key}({sps})";
                        funcs.Add(fn, luaLineNumber - 1);
                    }
                }
            }
            funcDefTable = funcs;

            VgcApis.Misc.UI.Invoke(() =>
            {
                var items = cboxFunctionList.Items;
                items.Clear();
                items.AddRange(funcs.Keys.OrderBy(x => x).ToArray());
                VgcApis.Misc.UI.ResetComboBoxDropdownMenuWidth(cboxFunctionList);
            });
        }

        bool ScrollToVariable(string v)
        {
            if (string.IsNullOrWhiteSpace(v))
            {
                return false;
            }

            var ast = currentCodeAst;
            if (ast != null && ast[Services.AstServer.KEY_VARS] is JObject)
            {
                foreach (var kv in ast[Services.AstServer.KEY_VARS] as JObject)
                {
                    if (kv.Key == v)
                    {
                        // index starts from 1 in lua
                        ScrollToLine((int)kv.Value - 1);
                        return true;
                    }
                }
            }

            // fallback
            foreach (var line in editor.Lines)
            {
                var text = line.Text;
                if (text.Contains(v))
                {
                    var trimed = RemoveLocalPrefix(text, false);
                    var first = trimed?.Split(new char[] { ' ', '=', ',', '\r', '\n' }).FirstOrDefault();
                    if (first == v)
                    {
                        ScrollToLine(line.Index);
                        return true;
                    }
                }
            }
            return false;
        }

        private void OnCboxVarListDropDownHandler(object sender, EventArgs args)
        {
            history.Add(editor.CurrentLine);
            var ast = currentCodeAst;
            List<string> vars = new List<string>();
            if (ast != null && ast[Services.AstServer.KEY_VARS] is JObject)
            {
                foreach (var kv in ast[Services.AstServer.KEY_VARS] as JObject)
                {
                    vars.Add(kv.Key);
                }
            }

            VgcApis.Misc.UI.Invoke(() =>
            {
                var items = cboxVarList.Items;
                items.Clear();
                items.AddRange(vars.OrderBy(x => x).ToArray());
                VgcApis.Misc.UI.ResetComboBoxDropdownMenuWidth(cboxVarList);
            });
        }

        #endregion

    }
}
