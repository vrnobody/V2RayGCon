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
                if (string.IsNullOrEmpty(text))
                    return "";
                if (Start >= text.Length)
                    return "";
                if (End > text.Length)
                    return "";

                return TargetWrapper.Text.Substring(Start, End - Start);
            }
            set { TargetWrapper.Text = value; }
        }
    }
}
