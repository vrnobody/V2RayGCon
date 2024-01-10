using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using ScintillaNET;

namespace V2RayGCon.Controllers.FormTextConfigEditorComponent
{
    internal class Editor : CompBase
    {
        private bool isReadonly;

        Scintilla editor = null;

        public Editor() { }

        #region properties
        private string _content;

        public string content
        {
            get { return _content; }
            set
            {
                if (isReadonly)
                {
                    editor.ReadOnly = false;
                }
                SetField(ref _content, value ?? "");
                UpdateLexer();
                if (isReadonly)
                {
                    editor.ReadOnly = true;
                }
            }
        }

        #endregion

        #region public methods
        public void Init(Panel panel, bool isReadonly, ToolStripComboBox cboxNavigation)
        {
            this.cboxNavigation = cboxNavigation;
            CreateEditor(panel, isReadonly);
            this.isReadonly = isReadonly;
            AttachEditorEvents();
        }

        public void Format()
        {
            var ty = VgcApis.Misc.Utils.DetectConfigType(content);
            SaveCurrentPosition();
            switch (ty)
            {
                case VgcApis.Models.Datas.Enums.ConfigType.json:
                    var config = VgcApis.Misc.Utils.FormatConfig(content);
                    if (!string.IsNullOrEmpty(config))
                    {
                        content = config;
                        RestorePreviousPosition();
                    }
                    break;
                case VgcApis.Models.Datas.Enums.ConfigType.yaml:
                default:
                    break;
            }
        }

        public void ZoomIn() => editor.ZoomIn();

        public void ZoomOut() => editor.ZoomOut();

        VgcApis.WinForms.FormSearch formSearch;

        public void ShowSearchBox()
        {
            if (formSearch != null)
            {
                formSearch.Activate();
                return;
            }

            formSearch = VgcApis.WinForms.FormSearch.CreateForm(editor);
            formSearch.FormClosed += (s, a) => formSearch = null;
        }
        #endregion


        #region Scintilla
        private int maxLineNumberCharLength;
        private ToolStripComboBox cboxNavigation;

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

        string GetCurrentLineText(int endPos)
        {
            int curPos = editor.CurrentPosition;
            int lineNumber = editor.LineFromPosition(curPos);
            int startPos = editor.Lines[lineNumber].Position;
            return editor.GetTextRange(startPos, (endPos - startPos)); //Text until the caret so that the whitespace is always equal in every line.
        }

        private void Scintilla_InsertCheck(object sender, InsertCheckEventArgs e)
        {
            if ((e.Text.EndsWith("\n") || e.Text.EndsWith("\r")))
            {
                //Text until the caret so that the whitespace is always equal in every line.
                string curLineText = GetCurrentLineText(e.Position);
                Match curIndentMatch = Regex.Match(curLineText, "^[ \\t]*");
                string curIndent = curIndentMatch.Value;

                e.Text += curIndent;

                if (
                    Regex.IsMatch(curLineText, @"\[\s*$")
                    || Regex.IsMatch(curLineText, @"{\s*$")
                    || curLineText.EndsWith("|")
                )
                {
                    e.Text += "  ";
                }

                UpdateLexer();
            }
        }

        private void Scintilla_CharAdded(object sender, CharAddedEventArgs e)
        {
            int curLine = editor.LineFromPosition(editor.CurrentPosition);
            if (curLine < 2)
            {
                return;
            }

            string ct = editor.Lines[curLine].Text.Trim();
            if (ct == "}" || ct == "]")
            { //Check whether the bracket is the only thing on the line.. For cases like "if() { }".
                SetIndent(editor, curLine, GetIndent(editor, curLine - 1) - 2);
            }
        }

        //Codes for the handling the Indention of the lines.
        //They are manually added here until they get officially added to the Scintilla control.

        const int SCI_SETLINEINDENTATION = 2126;
        const int SCI_GETLINEINDENTATION = 2127;

        private void SetIndent(Scintilla scin, int line, int indent)
        {
            scin.DirectMessage(SCI_SETLINEINDENTATION, new IntPtr(line), new IntPtr(indent));
        }

        private int GetIndent(Scintilla scin, int line)
        {
            return (
                scin.DirectMessage(SCI_GETLINEINDENTATION, new IntPtr(line), (IntPtr)null).ToInt32()
            );
        }

        void UpdateLexer()
        {
            var isJson = VgcApis.Misc.Utils.IsJson(content);
            var color = isJson ? Color.Silver : Color.Black;
            editor.Styles[Style.Json.Default].ForeColor = color;
            editor.Lexer = isJson ? Lexer.Json : Lexer.Null;
        }
        #endregion

        #region private methods
        Point editorPosition = new Point();

        void SaveCurrentPosition()
        {
            editorPosition = new Point(editor.CurrentPosition, editor.FirstVisibleLine);
        }

        void RestorePreviousPosition()
        {
            editor.GotoPosition(editorPosition.X);
            editor.FirstVisibleLine = editorPosition.Y;
        }

        void ReleaseEditorEvents()
        {
            editor.InsertCheck -= Scintilla_InsertCheck;
            editor.CharAdded -= Scintilla_CharAdded;
            editor.TextChanged -= Scintilla_TextChanged;
        }

        void AttachEditorEvents()
        {
            editor.InsertCheck += Scintilla_InsertCheck;
            editor.CharAdded += Scintilla_CharAdded;
            editor.TextChanged += Scintilla_TextChanged;

            cboxNavigation.DropDown += OnCboxNavigatorDropDownHandler;
            cboxNavigation.SelectedIndexChanged += OnCboxNavigatorIndexChangedHandler;
        }

        Dictionary<string, int> tags = new Dictionary<string, int>();

        void OnCboxNavigatorIndexChangedHandler(object sender, EventArgs args)
        {
            var key = cboxNavigation.Text;
            if (!string.IsNullOrEmpty(key) && tags.TryGetValue(key, out var num))
            {
                ScrollToLine(num);
                editor.Focus();
            }
        }

        void ScrollToLine(int num)
        {
            var count = editor.Lines.Count;
            if (count < 10 || num < 0)
            {
                return;
            }
            num = Math.Min(count - 1, num);
            var linesOnScreen = editor.LinesOnScreen - 2; // Fudge factor
            var top = num - (linesOnScreen / 2);
            var pos = editor.Lines[num].Position;
            editor.GotoPosition(pos);
            editor.FirstVisibleLine = Math.Max(0, top);
        }

        string lastDropDownContent = null;

        void OnCboxNavigatorDropDownHandler(object sender, EventArgs args)
        {
            if (_content == lastDropDownContent)
            {
                return;
            }

            lastDropDownContent = _content;

            var lines = editor.Lines.Select(line => line.Text).ToList();
            tags = VgcApis.Misc.Utils.GetConfigTags(lines);
            var keys = tags.Keys.ToList();
            keys.Sort();
            var items = cboxNavigation.Items;
            items.Clear();
            items.AddRange(keys.ToArray());
            VgcApis.Misc.UI.ResetComboBoxDropdownMenuWidth(cboxNavigation);
        }

        void CreateEditor(Panel container, bool isReadonly)
        {
            var editor = Misc.UI.CreateScintilla(container, isReadonly);
            VgcApis.Misc.Utils.BindEditorDragDropEvent(editor);

            this.editor = editor;

            // bind scintilla
            var bs = new BindingSource { DataSource = this };
            editor.DataBindings.Add(
                "Text",
                bs,
                nameof(this.content),
                true,
                DataSourceUpdateMode.OnPropertyChanged
            );
        }
        #endregion

        #region protected methods
        protected override void Cleanup()
        {
            VgcApis.Misc.UI.CloseFormIgnoreError(formSearch);
            ReleaseEditorEvents();
        }

        #endregion
    }
}
