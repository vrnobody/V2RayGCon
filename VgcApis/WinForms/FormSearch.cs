using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ScintillaNET;
using VgcApis.Resources.Langs;

namespace VgcApis.WinForms
{
    public partial class FormSearch : Form
    {
        readonly Scintilla editor;
        readonly string title = "";
        SearchFlags searchFlag = SearchFlags.None;

        public static FormSearch CreateForm(Scintilla editor)
        {
            FormSearch r = null;
            Misc.UI.Invoke(() =>
            {
                r = new FormSearch(editor);
                r.Show();
                r.Activate();
                r.cboxSearchKeyword.Focus();
            });

            return r;
        }

        FormSearch(Scintilla editor)
        {
            InitializeComponent();
            Misc.UI.AutoSetFormIcon(this);
            title = this.Text;

            this.editor = editor;
            BindEvents();
        }

        #region public methods
        public void SearchPrevious()
        {
            var curPos = editor.CurrentPosition;
            var matches = SearchAllMatch(searchFlag);
            var count = matches.Count;

            for (int i = count - 1; i >= 0; i--)
            {
                var match = matches[i];
                if (curPos > match.X)
                {
                    ShowSearchResult(count, i + 1, match);
                    return;
                }
            }

            if (count > 0)
            {
                ShowSearchResult(count, count, matches.Last());
                return;
            }

            ShowSearchResult(-1, -1, new Point());
            WarnNoMatch();
        }

        public void SearchNext()
        {
            var curPos = editor.CurrentPosition;
            var matches = SearchAllMatch(searchFlag);
            var count = matches.Count;

            for (int i = 0; i < count; i++)
            {
                var match = matches[i];
                if (match.X > curPos)
                {
                    ShowSearchResult(count, i + 1, match);
                    return;
                }
            }

            if (count > 0)
            {
                ShowSearchResult(count, 1, matches.First());
                return;
            }

            ShowSearchResult(-1, -1, new Point());
            WarnNoMatch();
        }

        public void SearchFirst(bool quiet)
        {
            var matches = SearchAllMatch(searchFlag);
            var count = matches.Count;
            if (count > 0)
            {
                ShowSearchResult(count, 1, matches.First());
                return;
            }

            ShowSearchResult(-1, -1, new Point());
            if (!quiet)
            {
                WarnNoMatch();
            }
        }
        #endregion

        #region private methods
        void SetTitle(int total, int index)
        {
            var tail = "";
            if (total > 0)
            {
                tail = $" - {index}/{total}";
            }

            var t = $"{title}{tail}";
            if (this.Text != t)
            {
                this.Text = t;
            }
        }

        private void BindEvents()
        {
            this.FormClosed += (s, a) => ClearIndicator();
            this.KeyDown += (s, a) => Misc.UI.Invoke(() => KeyBoardShortcutHandler(a.KeyCode));
        }

        void KeyBoardShortcutHandler(Keys keyCode)
        {
            switch (keyCode)
            {
                case Keys.F2:
                    SearchPrevious();
                    break;
                case Keys.F3:
                    SearchNext();
                    break;
                case Keys.F4:
                    SearchFirst(false);
                    break;
                case Keys.Escape:
                    this.Close();
                    break;
            }
        }

        void ClearIndicator()
        {
            editor.IndicatorClearRange(0, editor.TextLength);
        }

        void WarnNoMatch()
        {
            Misc.UI.MsgBox(I18N.NoMatchResult);
        }

        void ShowSearchResult(int total, int index, Point p)
        {
            SetTitle(total, index);
            ClearIndicator();
            editor.TargetStart = p.X;
            editor.TargetEnd = p.Y;

            if (total < 0)
            {
                return;
            }

            ScrollTo(p.X);
            editor.IndicatorFillRange(p.X, p.Y - p.X);
        }

        List<Point> SearchAllMatch(SearchFlags flag)
        {
            List<Point> results = new List<Point>();
            var keyword = cboxSearchKeyword.Text;
            if (string.IsNullOrEmpty(keyword))
            {
                return results;
            }

            if (keyword.Length > 100)
            {
                Misc.UI.MsgBoxAsync(I18N.KeywordIsTooLong);
                cboxSearchKeyword.Text = keyword.Substring(0, 100);
                return results;
            }

            editor.SearchFlags = flag;
            editor.TargetStart = 0;
            editor.TargetEnd = editor.TextLength;

            while (editor.SearchInTarget(keyword) != -1)
            {
                var r = new Point(editor.TargetStart, editor.TargetEnd);
                results.Add(r);
                editor.TargetStart = editor.TargetEnd;
                editor.TargetEnd = editor.TextLength;
            }
            return results;
        }

        void ScrollTo(int pos)
        {
            var target = editor.LineFromPosition(pos);
            var linesOnScreen = editor.LinesOnScreen - 2; // Fudge factor
            var top = target - (linesOnScreen / 2);
            editor.GotoPosition(pos);
            editor.FirstVisibleLine = top;
        }

        private void PerformSearchIfKeywordIsLongerThanTwoCharacters()
        {
            var text = cboxSearchKeyword.Text;
            if (string.IsNullOrEmpty(text) || text.Length < 2)
            {
                return;
            }
            SearchFirst(true);
        }
        #endregion

        #region UI events

        private void btnNewSearch_Click(object sender, System.EventArgs e)
        {
            SearchFirst(false);
        }

        private void btnNext_Click(object sender, System.EventArgs e)
        {
            SearchNext();
        }

        private void btnPrevious_Click(object sender, System.EventArgs e)
        {
            SearchPrevious();
        }

        private void cboxSearchKeyword_TextChanged(object sender, System.EventArgs e)
        {
            PerformSearchIfKeywordIsLongerThanTwoCharacters();
        }

        private void btnReplaceOne_Click(object sender, System.EventArgs e)
        {
            var kw = cboxSearchKeyword.Text;
            if (string.IsNullOrEmpty(kw))
            {
                return;
            }

            var record = new Point(editor.TargetStart, editor.TargetEnd);
            if (editor.SearchInTarget(kw) == -1)
            {
                Misc.UI.MsgBoxAsync(I18N.PlsDoSearchFirst);
                editor.TargetStart = record.X;
                editor.TargetEnd = record.Y;
                return;
            }

            editor.ReplaceTarget(cboxReplaceKeyword.Text);
            SearchNext();
        }

        private void btnReplaceAll_Click(object sender, System.EventArgs e)
        {
            var kw = cboxSearchKeyword.Text;

            if (string.IsNullOrEmpty(kw))
            {
                return;
            }

            var rpl = cboxReplaceKeyword.Text;

            var count = 0;
            // https://github.com/jacobslusser/ScintillaNET/issues/352
            editor.TargetStart = 0;
            editor.TargetEnd = editor.TextLength;
            while (editor.SearchInTarget(kw) != -1)
            {
                count++;
                editor.ReplaceTarget(rpl);
                editor.TargetStart = editor.TargetEnd;
                editor.TargetEnd = editor.TextLength;
            }

            editor.TargetEnd = 0;
            editor.TargetStart = 0;
            Misc.UI.MsgBoxAsync(string.Format(I18N.ReplacedCount, count));
        }

        void UpdateOptions()
        {
            searchFlag =
                (chkOptionMatchCase.Checked ? SearchFlags.MatchCase : SearchFlags.None)
                | (chkOptionRegex.Checked ? SearchFlags.Regex : SearchFlags.None)
                | (chkOptionWholeWord.Checked ? SearchFlags.WholeWord : SearchFlags.None)
                | (chkOptionWordStart.Checked ? SearchFlags.WordStart : SearchFlags.None);

            PerformSearchIfKeywordIsLongerThanTwoCharacters();
        }

        private void chkOptionRegex_CheckedChanged(object sender, System.EventArgs e)
        {
            UpdateOptions();
        }

        private void chkOptionMatchCase_CheckedChanged(object sender, System.EventArgs e)
        {
            UpdateOptions();
        }

        private void chkOptionWholeWord_CheckedChanged(object sender, System.EventArgs e)
        {
            UpdateOptions();
        }

        private void chkOptionWordStart_CheckedChanged(object sender, System.EventArgs e)
        {
            UpdateOptions();
        }

        #endregion
    }
}
