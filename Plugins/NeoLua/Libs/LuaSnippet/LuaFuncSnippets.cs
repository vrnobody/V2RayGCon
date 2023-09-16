using AutocompleteMenuNS;

namespace NeoLuna.Libs.LuaSnippet
{
    internal sealed class LuaFuncSnippets : MatchItemBase
    {
        readonly string lowerText;

        public LuaFuncSnippets(string luaFuncStr)
            : base(luaFuncStr)
        {
            if (luaFuncStr == null)
            {
                throw new System.ArgumentException(@"luaFuncStr is null!");
            }

            ImageIndex = 1;
            ToolTipTitle = luaFuncStr;
            ToolTipText = @"";
            Text = luaFuncStr;

            lowerText = Text.ToLower();
        }

        public override CompareResult Compare(string fragmentText)
        {
            if (fragmentText == Text)
                return CompareResult.VisibleAndSelected;
            // if (lowerText.StartsWith(fragmentText.ToLower()))
            if (VgcApis.Misc.Utils.PartialMatch(lowerText, fragmentText.ToLower()))
                return CompareResult.Visible;
            return CompareResult.Hidden;
        }
    }
}
