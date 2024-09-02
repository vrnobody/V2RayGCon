using System;
using System.Drawing;
using System.Windows.Forms;

namespace VgcApis.Libs.Infr
{
    public class Highlighter
    {
        readonly int index = 0;
        readonly string keyword = "";
        readonly Action<RichTextBox, double> worker;

        public Highlighter()
            : this("") { }

        public Highlighter(string keyword)
        {
            this.keyword = keyword;
            worker = HighLightTitle;
        }

        public Highlighter(int index)
        {
            this.index = index;
            worker = HighlightIndex;
        }

        #region public methods
        public void Highlight(RichTextBox box, double coreIndex)
        {
            if (string.IsNullOrEmpty(box.Text))
            {
                return;
            }
            Clear(box);
            this.worker.Invoke(box, coreIndex);
            Deselect(box);
        }
        #endregion

        #region private methods
        void Clear(RichTextBox box)
        {
            var title = box.Text;
            if (string.IsNullOrEmpty(title))
            {
                return;
            }
            box.SelectionStart = 0;
            box.SelectionLength = title.Length;
            box.SelectionBackColor = box.BackColor;
        }

        void Deselect(RichTextBox box)
        {
            box.SelectionStart = 0;
            box.SelectionLength = 0;
            box.DeselectAll();
        }

        void HighlightIndex(RichTextBox box, double coreIndex)
        {
            if ((int)coreIndex != this.index)
            {
                return;
            }

            var title = box.Text ?? "";
            box.SelectionStart = 0;
            box.SelectionLength = Math.Min(title.Length, $"{index}".Length);
            box.SelectionBackColor = Color.Yellow;
        }

        void HighLightTitle(RichTextBox box, double _)
        {
            var title = box.Text?.ToLower() ?? "";
            if (string.IsNullOrEmpty(this.keyword) || !Misc.Utils.PartialMatch(title, this.keyword))
            {
                return;
            }

            int idxTitle = 0;
            int idxKeyword = 0;
            while (idxTitle < title.Length && idxKeyword < keyword.Length)
            {
                if (title[idxTitle].CompareTo(keyword[idxKeyword]) == 0)
                {
                    box.SelectionStart = idxTitle;
                    box.SelectionLength = 1;
                    box.SelectionBackColor = Color.Yellow;
                    idxKeyword++;
                }
                idxTitle++;
            }
        }

        #endregion
    }
}
