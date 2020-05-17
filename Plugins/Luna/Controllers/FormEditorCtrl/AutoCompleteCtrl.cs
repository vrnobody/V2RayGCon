using AutocompleteMenuNS;
using Luna.Libs.LuaSnippet;
using Luna.Services;
using Moq;
using Newtonsoft.Json.Linq;
using NLua;
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
        #region constants
        string KEY_PARAMS = "params";
        string KEY_PROPERTY = "props";
        string KEY_FUNCTION = "funcs";
        string KEY_VARS = "vars";
        string KEY_MODULES = "modules";
        string KEY_LINE_NUM = "line";  // line number
        string KEY_METHODS = "methods";
        string KEY_SUB_FUNCS = "subs";
        #endregion

        private readonly Scintilla editor;
        private readonly ComboBox cboxVarList;
        private readonly ComboBox cboxFunctionList;
        private readonly ToolStripMenuItem miEanbleCodeAnalyze;
        private readonly ToolStripStatusLabel smiLbCodeanalyze;

        AutocompleteMenu luaAcm = null;
        BestMatchSnippets bestMatchSnippets;

        VgcApis.Libs.Infr.Recorder history = new VgcApis.Libs.Infr.Recorder();
        VgcApis.Libs.Tasks.LazyGuy lazyAnalyser;

        ConcurrentQueue<string> hotCacheKeys = new ConcurrentQueue<string>();
        ConcurrentDictionary<string, JObject> astCodeCache = new ConcurrentDictionary<string, JObject>();
        ConcurrentDictionary<string, JObject> astModuleCache = new ConcurrentDictionary<string, JObject>();

        public AutoCompleteCtrl(
            Scintilla editor,
            ComboBox cboxVarList,
            ComboBox cboxFunctionList,

            ToolStripMenuItem miEanbleCodeAnalyze,
            ToolStripStatusLabel smiLbCodeanalyze)
        {
            this.editor = editor;
            this.cboxVarList = cboxVarList;
            this.cboxFunctionList = cboxFunctionList;
            this.miEanbleCodeAnalyze = miEanbleCodeAnalyze;
            this.smiLbCodeanalyze = smiLbCodeanalyze;
        }

        bool isEnableCodeAnalyze;

        Settings settings;
        public void Run(Settings settings)
        {
            this.settings = settings;
            lazyAnalyser = new VgcApis.Libs.Tasks.LazyGuy(AnalizeScriptWorker, 1000, 3000);

            InitControls(settings);
            BindEvents();

            SetIsEnableCodeAnalyze(settings.isEnableAdvanceAutoComplete);
            UpdateLuaRequireModuleNameSnippets();
        }

        #region public methods

        public void KeyBoardShortcutHandler(KeyEventArgs keyEvent)
        {

            var keyCode = keyEvent.KeyCode;
            if (keyEvent.Control)
            {
                switch (keyCode)
                {
                    case Keys.OemOpenBrackets:
                        history.Backward();
                        Invoke(() => ScrollToLine(history.Current()));
                        break;
                    case Keys.Oem6:
                        history.Forward();
                        Invoke(() => ScrollToLine(history.Current()));
                        break;
                }

                switch (keyCode)
                {
                    case Keys.F12:
                        history.Add(editor.CurrentLine);
                        Invoke(() =>
                        {
                            var w = editor.GetWordFromPosition(editor.CurrentPosition);
                            VgcApis.Misc.UI.Invoke(() => ScrollToDefinition(w));
                        });
                        break;
                }
            }
        }


        public void Cleanup()
        {
            StopFileSystemWatcher();
            lazyAnalyser?.Dispose();

            if (luaAcm != null)
            {
                luaAcm.TargetControlWrapper = null;
            }

        }
        #endregion


        #region file watching
        FileSystemWatcher fsWatcher;
        void StopFileSystemWatcher()
        {
            if (fsWatcher == null)
            {
                return;
            }
            fsWatcher.EnableRaisingEvents = false;
            fsWatcher.Dispose();
            fsWatcher = null;
        }

        FileSystemWatcher CreateFileSystemWatcher(string relativeFileName)
        {
            if (!Directory.Exists(relativeFileName))
            {
                return null;
            }

            var fsw = new FileSystemWatcher
            {
                IncludeSubdirectories = true,
                Path = relativeFileName,
                Filter = "*.lua",
            };

            fsw.Changed += FileSystemEventHandler;
            fsw.Created += FileSystemEventHandler;
            fsw.Deleted += FileSystemEventHandler;
            fsw.Renamed += FileSystemEventHandler;

            fsw.EnableRaisingEvents = true;

            return fsw;
        }

        void UpdateLuaRequireModuleNameSnippets()
        {
            try
            {
                List<LuaImportClrSnippets> snps = new List<LuaImportClrSnippets>();
                string[] fileArray = Directory.GetFiles(@"lua", "*.lua", SearchOption.AllDirectories);
                foreach (var file in fileArray)
                {
                    if (!string.IsNullOrEmpty(file) || !file.ToLower().EndsWith(".lua"))
                    {
                        var mn = file.Replace("\\", ".")
                            .Replace("/", ".")
                            .Substring(0, file.Length - ".lua".Length);

                        var scr = $"require('{mn}')";
                        snps.Add(new LuaImportClrSnippets(scr));
                    }
                }
                bestMatchSnippets.UpdateRequireModuleSnippets(snps);
            }
            catch { }
        }

        void FileSystemEventHandler(object sender, FileSystemEventArgs e)
        {
            UpdateLuaRequireModuleNameSnippets();
            var mn = VgcApis.Misc.Utils.GetLuaModuleName(e.FullPath);
            if (string.IsNullOrWhiteSpace(mn))
            {
                return;
            }
            astModuleCache.TryRemove(mn, out _);
        }


        #endregion

        #region code analyzing

        void BuildSnippets(JObject ast, List<MatchItemBase> snippets)
        {
            if (ast == null)
            {
                return;
            }

            if (ast[KEY_VARS] is JObject)
            {
                foreach (var kv in ast[KEY_VARS] as JObject)
                {
                    snippets.Add(new LuaKeywordSnippets(kv.Key));
                }
            }

            if (ast[KEY_FUNCTION] is JObject)
            {
                foreach (var kv in ast[KEY_FUNCTION] as JObject)
                {
                    var ps = (kv.Value as JObject)[KEY_PARAMS] as JArray;
                    var sps = string.Join(", ", ps);
                    snippets.Add(new LuaFuncSnippets($"{kv.Key}({sps})"));
                }
            }

            if (ast[KEY_SUB_FUNCS] is JObject)
            {
                foreach (var kv in ast[KEY_SUB_FUNCS] as JObject)
                {
                    var ps = (kv.Value as JObject)[KEY_PARAMS] as JArray;
                    var sps = string.Join(", ", ps);
                    snippets.Add(new LuaSubFuncSnippets($"{kv.Key}({sps})", "."));
                }
            }

            if (ast[KEY_METHODS] is JObject)
            {
                foreach (var kv in ast[KEY_METHODS] as JObject)
                {
                    var ps = (kv.Value as JObject)[KEY_PARAMS] as JArray;
                    var sps = string.Join(", ", ps);
                    snippets.Add(new LuaSubFuncSnippets($"{kv.Key}({sps})", ":"));
                }
            }

            BuildModuleSnippets(ast, snippets);
        }

        private void BuildModuleSnippets(JObject ast, List<MatchItemBase> snippets)
        {
            if (!(ast[KEY_MODULES] is JObject))
            {
                return;
            }

            foreach (var kv in ast[KEY_MODULES] as JObject)
            {
                var mn = kv.Value.ToString();
                if (!astModuleCache.TryGetValue(mn, out var amc))
                {
                    CheckCacheSize(astModuleCache);
                    var fn = mn.Replace('.', Path.DirectorySeparatorChar) + ".lua";
                    amc = AnalyzeOneModule(fn);
                    if (amc != null)
                    {
                        astModuleCache.TryAdd(mn, amc);
                    }
                }

                if (amc == null)
                {
                    continue;
                }

                BuildOneModuleSnippets(kv.Key, amc, snippets);
            }
        }

        private void BuildOneModuleSnippets(string varName, JObject ast, List<MatchItemBase> snippets)
        {
            var fds = new Dictionary<string, string>() {
                { KEY_FUNCTION,"." },
                { KEY_METHODS ,":" },
            };

            snippets.Add(new LuaKeywordSnippets(varName));

            foreach (var kv in ast)
            {
                if (kv.Value is JArray && kv.Key == KEY_PROPERTY)
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

        JObject AnalyzeOneModule(string filename)
        {
            try
            {
                var code = File.ReadAllText(filename);
                var mode = isEnableCodeAnalyze ? AnalyzeModes.ModuleEx : AnalyzeModes.Module;
                return Analyze(code, mode);
            }
            catch { }
            return null;
        }

        void CheckCacheSize<TKey, TValue>(ConcurrentDictionary<TKey, TValue> cache)
        {
            try
            {
                var keep = 100;
                var keys = cache.Keys.ToList();
                if (keys.Count > keep * 2)
                {
                    var cut = keys.Count - keep;
                    for (int i = 0; i < cut; i++)
                    {
                        cache.TryRemove(keys[i], out _);
                    }
                }
            }
            catch { }
        }

        void AddAstCodeCache(string key, JObject value)
        {
            while (hotCacheKeys.Count > 20)
            {
                hotCacheKeys.TryDequeue(out _);
            }

            if (astCodeCache.Count > 300)
            {
                var keys = astCodeCache.Keys;
                var filterd = keys
                    .Where(k => !hotCacheKeys.Contains(k))
                    .Skip(150 - hotCacheKeys.Count)
                    .ToList();
                foreach (var k in filterd)
                {
                    astCodeCache.TryRemove(k, out _);
                }
            }

            astCodeCache.TryAdd(key, value);
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
                if (astCodeCache.TryGetValue(src, out var cachedAst))
                {
                    currentCodeAst = cachedAst;
                    hotCacheKeys.Enqueue(src);
                    key = src;
                    ast = cachedAst;
                    break;
                }
            }

            if (ast == null)
            {
                var st = srcs.Select(s =>
                    {
                        var t = Analyze(s, AnalyzeModes.SourceCode);
                        AddAstCodeCache(s, t);
                        return new Tuple<string, JObject>(s, t);
                    })
                   .OrderByDescending(tp => ((tp.Item2?.ToString()) ?? "").Length)
                   .First();
                key = st.Item1;
                ast = st.Item2;
            }

            if (ast != null)
            {
                currentCodeAst = ast;
            }

            // var debug = ast.ToString();

            var snps = new List<MatchItemBase>();
            BuildSnippets(ast, snps);
            bestMatchSnippets.UpdateCustomScriptSnippets(snps);
        }

        Lua CreateAnalyser()
        {
            Lua anz = new Lua()
            {
                UseTraceback = true,
            };

            anz.State.Encoding = Encoding.UTF8;

            // phony
            anz["Misc"] = new Mock<VgcApis.Interfaces.Lua.ILuaMisc>().Object;
            anz["Signal"] = new Mock<VgcApis.Interfaces.Lua.ILuaSignal>().Object;
            anz["Sys"] = new Mock<VgcApis.Interfaces.Lua.ILuaSys>().Object;
            anz["Server"] = new Mock<VgcApis.Interfaces.Lua.ILuaServer>().Object;
            anz["Web"] = new Mock<VgcApis.Interfaces.Lua.ILuaWeb>().Object;

            anz.DoString(Resources.Files.Datas.LuaPredefinedFunctions);

            return anz;
        }


        void AnalyzeScriptLater(object sender, EventArgs e)
        {
            lazyAnalyser?.Deadline();
        }


        enum AnalyzeModes
        {
            SourceCode,
            Module,
            ModuleEx
        }

        JObject Analyze(string code, AnalyzeModes analyzeMode)
        {
            try
            {
                Lua state = CreateAnalyser();
                state["code"] = code;

                var fn = "analyzeCode";
                if (analyzeMode == AnalyzeModes.Module)
                {
                    fn = "analyzeModule";
                }
                else if (analyzeMode == AnalyzeModes.ModuleEx)
                {
                    fn = "analyzeModuleEx";
                }

                string tpl = @"local analyzer = require('lua.libs.luacheck.analyzer').new();"
                    + @"return analyzer.{0}(code)";

                var script = string.Format(tpl, fn);
                string r = state.DoString(script)[0] as string;

                return JObject.Parse(r);
            }
            catch { }
            return null;
        }
        #endregion


        #region private methods
        void SetIsEnableCodeAnalyze(bool isEnable)
        {
            isEnableCodeAnalyze = isEnable;
            astModuleCache.Clear();
            AnalyzeScriptLater(this, EventArgs.Empty);

            Invoke(() =>
            {
                miEanbleCodeAnalyze.Checked = isEnable;
                smiLbCodeanalyze.Enabled = isEnable;
            });

        }

        void Invoke(Action action) => VgcApis.Misc.UI.Invoke(action);

        void ScrollToFunction(string text)
        {
            text = text?.Replace(".", "")?.Split('(')?.FirstOrDefault();

            if (string.IsNullOrWhiteSpace(text))
            {
                return;
            }

            foreach (var line in editor.Lines)
            {
                var t = line.Text;
                if (t.StartsWith("local function ") || t.StartsWith("function "))
                {
                    if (t.Replace(".", "").Contains(text))
                    {
                        ScrollToLine(line.Index);
                        return;
                    }
                }
            }
        }

        void ScrollToLine(int index)
        {
            index = Math.Min(Math.Max(0, index), editor.Lines.Count);
            history.Add(index);

            var line = editor.Lines[index];
            line.Goto();
            editor.FirstVisibleLine = line.Index;
        }

        void ScrollToDefinition(string text)
        {
            foreach (var line in editor.Lines)
            {
                var t = line.Text;
                if (!t.StartsWith("local ") && !t.StartsWith("function "))
                {
                    continue;
                }

                if (t.Contains(text))
                {
                    ScrollToLine(line.Index);
                    return;
                }
            }
        }

        AutocompleteMenu CreateAcm(Scintilla editor)
        {

            bestMatchSnippets = settings?.CreateBestMatchSnippet(editor);

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

        private void InitControls(Settings settings)
        {
            luaAcm = CreateAcm(editor);
            miEanbleCodeAnalyze.Checked = settings.isEnableAdvanceAutoComplete;
            smiLbCodeanalyze.Enabled = settings.isEnableAdvanceAutoComplete;
        }

        void BindEvents()
        {
            fsWatcher = CreateFileSystemWatcher(@"lua");

            editor.TextChanged += AnalyzeScriptLater;
            editor.Click += (s, a) => history.Add(editor.CurrentLine);

            miEanbleCodeAnalyze.Click += (s, a) =>
            {
                var enable = !miEanbleCodeAnalyze.Checked;
                SetIsEnableCodeAnalyze(enable);
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

        private void OnCboxFunctionListDropDownHandler(object sender, EventArgs args)
        {
            history.Add(editor.CurrentLine);

            var ast = currentCodeAst;
            var debug = ast?.ToString();

            string[] keys = new string[] {
                KEY_FUNCTION,
                KEY_SUB_FUNCS,
                KEY_METHODS,
            };

            List<string> funcs = new List<string>();
            foreach (var key in keys)
            {
                if (ast != null && ast[key] is JObject)
                {
                    foreach (var kv in ast[key] as JObject)
                    {
                        var ps = (kv.Value as JObject)[KEY_PARAMS] as JArray;
                        var sps = string.Join(", ", ps);
                        funcs.Add($"{kv.Key}({sps})");
                    }
                }
            }

            VgcApis.Misc.UI.Invoke(() =>
            {
                var items = cboxFunctionList.Items;
                items.Clear();
                items.AddRange(funcs.OrderBy(x => x).ToArray());
                VgcApis.Misc.UI.ResetComboBoxDropdownMenuWidth(cboxFunctionList);
            });
        }

        void ScrollToVariable(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return;
            }

            var ast = currentCodeAst;
            if (ast != null && ast[KEY_VARS] is JObject)
            {
                foreach (var kv in ast[KEY_VARS] as JObject)
                {
                    if (kv.Key == text)
                    {
                        // lua index starts from 1
                        ScrollToLine((int)kv.Value - 1);
                        return;
                    }
                }
            }

            // fallback
            foreach (var line in editor.Lines)
            {
                var t = line.Text;
                if (t.StartsWith("local ") && t.Contains(text))
                {
                    ScrollToLine(line.Index);
                    return;
                }
            }
        }

        private void OnCboxVarListDropDownHandler(object sender, EventArgs args)
        {
            history.Add(editor.CurrentLine);
            var ast = currentCodeAst;
            List<string> vars = new List<string>();
            if (ast != null && ast["vars"] is JObject)
            {
                foreach (var kv in ast[KEY_VARS] as JObject)
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
