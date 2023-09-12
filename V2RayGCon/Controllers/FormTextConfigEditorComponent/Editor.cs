using ScintillaNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

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
                SetField(ref _content, value);
                if (isReadonly)
                {
                    editor.ReadOnly = true;
                }
            }
        }
        #endregion

        #region public methods
        public void Init(Panel panel, bool isReadonly)
        {
            CreateEditor(panel, isReadonly);
            this.isReadonly = isReadonly;
            AttachEditorEvents();
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

                e.Text = (e.Text + curIndent);

                if (Regex.IsMatch(curLineText, @"\[\s*$") || Regex.IsMatch(curLineText, @"{\s*$"))
                {
                    e.Text = (e.Text + "  ");
                }
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
        #endregion



        #region private methods
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
        }

        void CreateEditor(Panel container, bool isReadonly)
        {
            var editor = Misc.UI.CreateScintilla(container, isReadonly);

            VgcApis.Misc.Utils.BindEditorDragDropEvent(editor);

            this.editor = editor;

            // bind scintilla
            var bs = new BindingSource();
            bs.DataSource = this;
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
            ReleaseEditorEvents();
        }

        #endregion
    }
}
