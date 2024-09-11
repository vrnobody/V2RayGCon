namespace VgcApis.UserControls.AcmComboBoxComps
{
    public class Range
    {
        public ITextBoxWrapper TargetWrapper { get; private set; }
        public int Start { get; set; }
        public int End { get; set; }

        public Range(ITextBoxWrapper targetWrapper)
        {
            this.TargetWrapper = targetWrapper;
        }

        public string Text
        {
            get
            {
                var text = TargetWrapper.Text;
                if (
                    string.IsNullOrEmpty(text)
                    || Start < 0
                    || Start >= text.Length
                    || Start >= End
                    || End > text.Length
                )
                {
                    return "";
                }
                return text.Substring(Start, End - Start);
            }
            set
            {
                if (Start >= End)
                {
                    return;
                }

                var text = TargetWrapper.Text;
                if (Start >= text.Length)
                {
                    return;
                }

                var tail = "";
                if (End < text.Length)
                {
                    tail = text.Substring(End);
                }

                var front = text.Substring(0, Start) + value;
                text = front + tail;
                TargetWrapper.Text = text;
                TargetWrapper.Select(front.Length);
            }
        }
    }
}
