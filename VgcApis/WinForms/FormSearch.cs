using ScintillaNET;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace VgcApis.WinForms
{
    public partial class FormSearch : Form
    {
        Scintilla scintilla;
        List<int> result;
        int curResult, keywordLength;

        public static FormSearch CreateForm(Scintilla editor)
        {
            FormSearch r = null;
            VgcApis.Misc.UI.Invoke(() =>
            {
                r = new FormSearch(editor);
                r.Show();
            });

            return r;
        }


        FormSearch(Scintilla editor)
        {
            InitializeComponent();
            scintilla = editor;
            result = new List<int>();
            curResult = 0;
            InitIndicator();

            this.FormClosed += (s, a) => ClearIndicator();

            VgcApis.Misc.UI.AutoSetFormIcon(this);

            this.KeyDown += KeyBoardShortcutHandler;
        }

        void KeyBoardShortcutHandler(object sender, KeyEventArgs e)
        {
            var keyCode = e.KeyCode;

            switch (keyCode)
            {
                case Keys.F2:
                    ShowResult(false);
                    break;
                case Keys.F3:
                    ShowResult(true);
                    break;
                case Keys.F4:
                    SearchAll();
                    break;
                case Keys.Escape:
                    this.Close();
                    break;
            }
        }

        string GetKeyword()
        {
            return tboxKeyword.Text;
        }

        void InitIndicator()
        {
            // Indicators 0-7 could be in use by a lexer
            // so we'll use indicator 8 to highlight words.
            int NUM = 8;

            // Remove all uses of our indicator
            scintilla.IndicatorCurrent = NUM;
            // scintilla.IndicatorClearRange(0, scintilla.TextLength);

            // Update indicator appearance
            scintilla.Indicators[NUM].Style = IndicatorStyle.StraightBox;
            scintilla.Indicators[NUM].Under = true;
            scintilla.Indicators[NUM].ForeColor = Color.Yellow;
            scintilla.Indicators[NUM].OutlineAlpha = 220;
            scintilla.Indicators[NUM].Alpha = 180;
        }

        void ClearIndicator()
        {
            scintilla.IndicatorClearRange(0, scintilla.TextLength);
        }


        private void SearchAll()
        {
            var key = GetKeyword();
            if (string.IsNullOrEmpty(key) || key.Length < 2)
            {
                return;
            }

            keywordLength = key.Length;

            result.Clear();
            ClearIndicator();
            scintilla.TargetStart = 0;
            scintilla.TargetEnd = scintilla.TextLength;
            while (scintilla.SearchInTarget(key) != -1)
            {
                result.Add(scintilla.TargetStart);
                scintilla.TargetStart = scintilla.TargetEnd;
                scintilla.TargetEnd = scintilla.TextLength;
            }
            curResult = -1;
            ShowResult(true);
        }

        void UpdateLbResults(int curIdx, int total)
        {
            Misc.UI.Invoke(() =>
            {
                if (total < 1)
                {
                    lbResults.Text = "0";
                    return;
                }

                var text = $"{curIdx}/{total}";
                lbResults.Text = text;
            });
        }

        void ShowResult(bool forward)
        {
            var count = result.Count;

            if (count < 1)
            {
                UpdateLbResults(-1, count);
                return;
            }

            int delta = 1;
            if (!forward)
            {
                delta = -1;
            }
            curResult = (curResult + delta + count) % count;
            UpdateLbResults(curResult + 1, count);

            var kwPos = result[curResult];
            var kwLine = scintilla.LineFromPosition(kwPos);
            var linesOnScreen = scintilla.LinesOnScreen - 2; // Fudge factor            
            var topLine = kwLine - (linesOnScreen / 2);

            scintilla.GotoPosition(kwPos);
            scintilla.FirstVisibleLine = topLine;

            ClearIndicator();
            scintilla.IndicatorFillRange(kwPos, keywordLength);
        }

        private void btnNext_Click(object sender, System.EventArgs e)
        {
            ShowResult(true);
        }

        private void tboxKeyword_TextChanged(object sender, System.EventArgs e)
        {
            SearchAll();
        }

        private void btnPrevious_Click(object sender, System.EventArgs e)
        {
            ShowResult(false);
        }
    }
}
