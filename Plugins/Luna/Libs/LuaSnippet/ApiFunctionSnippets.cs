using AutocompleteMenuNS;

namespace Luna.Libs.LuaSnippet
{
    internal sealed class ApiFunctionSnippets : MatchItemBase
    {
        const string seperator = @":";

        string lowerText;

        public ApiFunctionSnippets(
            string returnType,
            string parent,
            string functionName,
            string defParam,
            string defParmaWithTypeInfo,
            string description)
            : base(GenApiFuncName(parent, functionName))
        {
            if (string.IsNullOrEmpty(parent) || string.IsNullOrEmpty(functionName))
            {
                throw new System.ArgumentNullException(
                    @"param must not null!");
            }

            ImageIndex = 2;
            ToolTipTitle = GenTitle(returnType, functionName, defParmaWithTypeInfo);
            ToolTipText = description ?? @"";
            Text = GenText(parent, functionName, defParam);

            lowerText = Text.ToLower();
        }

        static string GenApiFuncName(string parent, string fnName) =>
            $"{parent}{seperator}{fnName}";

        string GenTitle(string returnType, string fnName, string defParam) =>
            $"{returnType} {fnName}({defParam})";

        string GenText(string parent, string fnName, string defParam) =>
            $"{parent}{seperator}{fnName}({defParam})";

        public override CompareResult Compare(string fragmentText)
        {
            if (fragmentText == Text)
                return CompareResult.VisibleAndSelected;
            if (fragmentText.Contains(seperator)
                && VgcApis.Libs.Utils.PartialMatch(lowerText, fragmentText.ToLower()))
                return CompareResult.Visible;
            return CompareResult.Hidden;
        }
    }
}
