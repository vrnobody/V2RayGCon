using AutocompleteMenuNS;

namespace Luna.Libs.LuaSnippet
{
    internal sealed class LuaImportClrSnippets : MatchItemBase
    {
        const string seperator = @"(";
        readonly string lowerText;

        public LuaImportClrSnippets(string luaImportClrStr)
            : base(luaImportClrStr)
        {
            if (!luaImportClrStr.Contains(seperator))
            {
                throw new System.ArgumentException($"luaSubFuncStr must contains {seperator}");
            }

            var pairs = luaImportClrStr.Split(seperator[0]);

            if (pairs == null || pairs.Length != 2)
            {
                throw new System.ArgumentException(@"luaSubFuncStr split fail!");
            }

            // import('System')
            string strImport = pairs[0]; // import
            string clrName = pairs[1]; // 'System')

            if (string.IsNullOrEmpty(strImport) || string.IsNullOrEmpty(clrName))
            {
                throw new System.ArgumentNullException(@"param must not null!");
            }

            ImageIndex = 2;
            Text = GenText(strImport, clrName);
            ToolTipTitle = GetToolTip(clrName);
            ToolTipText = @"";

            lowerText = Text.ToLower();
        }

        string GetToolTip(string clrName)
        {
            // 'System') => System

            if (clrName.Length < 4)
            {
                return string.Empty;
            }
            return clrName.Substring(1, clrName.Length - 3);
        }

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
