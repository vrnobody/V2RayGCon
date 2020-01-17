using AutocompleteMenuNS;

namespace V2RayGCon.Service.Caches
{
    internal sealed class JsonKeywordItems : AutocompleteItem
    {
        string lowerText;

        public JsonKeywordItems(string luaKeyword)
            : base(luaKeyword)
        {
            ToolTipTitle = luaKeyword
                ?? throw new System.ArgumentException(
                    @"keyword is null!");
            ToolTipText = @"";
            Text = luaKeyword;

            lowerText = Text.ToLower();
        }

        public override CompareResult Compare(string fragmentText)
        {
            if (fragmentText == Text)
                return CompareResult.VisibleAndSelected;
            if (VgcApis.Libs.Utils.PartialMatch(lowerText, fragmentText.ToLower()))
                return CompareResult.Visible;
            return CompareResult.Hidden;
        }
    }
}
