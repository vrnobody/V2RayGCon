using AutocompleteMenuNS;
using System.Collections.Generic;

namespace Luna.Libs.LuaSnippet
{
    internal sealed class LuaKeywordSnippets : MatchItemBase
    {
        static readonly List<string> hiddenList = new List<string>() { "end", };
        readonly string lowerText;

        public LuaKeywordSnippets(string luaKeyword)
            : base(luaKeyword)
        {
            ImageIndex = 0;
            ToolTipTitle = luaKeyword ?? throw new System.ArgumentException(@"luaKeyword is null!");
            ToolTipText = @"";
            Text = luaKeyword;

            lowerText = Text.ToLower();
        }

        public override CompareResult Compare(string fragmentText)
        {
            if (hiddenList.Contains(fragmentText))
            {
                return CompareResult.Hidden;
            }

            if (fragmentText == Text)
            {
                return CompareResult.VisibleAndSelected;
            }

            if (VgcApis.Misc.Utils.PartialMatch(lowerText, fragmentText.ToLower()))
            {
                return CompareResult.Visible;
            }
            return CompareResult.Hidden;
        }
    }
}
