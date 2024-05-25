﻿using AutocompleteMenuNS;

namespace Luna.Libs.LuaSnippet
{
    internal class LuaSubFuncSnippets : MatchItemBase
    {
        protected string seperator = @".";
        readonly string lowerText;

        public LuaSubFuncSnippets(string luaSubFuncStr, string seperator)
            : base(luaSubFuncStr)
        {
            if (
                string.IsNullOrEmpty(seperator)
                || seperator.Length != 1
                || !":.".Contains(seperator)
                || !luaSubFuncStr.Contains(seperator)
            )
            {
                throw new System.ArgumentException($"luaSubFuncStr must contains {seperator}");
            }

            this.seperator = seperator;
            var pairs = luaSubFuncStr.Split(seperator[0]);

            if (pairs == null || pairs.Length != 2)
            {
                throw new System.ArgumentException(@"luaSubFuncStr split fail!");
            }

            string parent = pairs[0];
            string functionName = pairs[1];

            if (string.IsNullOrEmpty(parent) || string.IsNullOrEmpty(functionName))
            {
                throw new System.ArgumentNullException(@"param must not null!");
            }

            ImageIndex = 2;
            ToolTipTitle = GenTitle(functionName);
            ToolTipText = @"";
            Text = GenText(parent, functionName);

            lowerText = Text.ToLower();
        }

        string GenTitle(string fnName) => $"{fnName}";

        string GenText(string parent, string fnName) => $"{parent}{seperator}{fnName}";

        public override CompareResult Compare(string fragmentText)
        {
            if (fragmentText == Text)
                return CompareResult.VisibleAndSelected;
            if (
                fragmentText.Contains(seperator)
                && VgcApis.Misc.Utils.PartialMatch(lowerText, fragmentText.ToLower())
            )
                // && lowerText.Contains(fragmentText.ToLower()))
                return CompareResult.Visible;
            return CompareResult.Hidden;
        }
    }
}
