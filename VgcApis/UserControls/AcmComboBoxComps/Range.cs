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
                var text = TargetWrapper.Text;
                if (Start >= text.Length)
                {
                    return;
                }

                if (Start < 1)
                {
                    TargetWrapper.Text = value;
                    return;
                }

                text = text.Substring(0, Start) + value;
                TargetWrapper.Text = text;
            }
        }
    }
}
