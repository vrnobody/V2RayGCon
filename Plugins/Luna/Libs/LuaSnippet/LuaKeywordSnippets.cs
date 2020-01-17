using AutocompleteMenuNS;

namespace Luna.Libs.LuaSnippet
{
    internal sealed class LuaKeywordSnippets : MatchItemBase
    {
        string lowerText;

        public LuaKeywordSnippets(string luaKeyword)
            : base(luaKeyword)
        {
            ImageIndex = 0;
            ToolTipTitle = luaKeyword
                ?? throw new System.ArgumentException(@"luaKeyword is null!");
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
