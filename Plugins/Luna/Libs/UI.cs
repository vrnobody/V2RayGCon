using ScintillaNET;
using System.Drawing;
using System.Windows.Forms;

namespace Luna.Libs
{
    public static class UI
    {
        public static Scintilla CreateLuaEditor(Panel container)
        {
            var scintilla = new Scintilla();
            container.Controls.Add(scintilla);

            // dock inside container
            scintilla.Dock = DockStyle.Fill;
            scintilla.WrapMode = WrapMode.None;
            scintilla.IndentationGuides = IndentView.LookBoth;

            // https://gist.github.com/jacobslusser/91a3a00ec8eb52ea238ee7c18c8cdf99

            // Extracted from the Lua Scintilla lexer and SciTE .properties file

            var alphaChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var numericChars = "0123456789";
            var accentedChars = "ŠšŒœŸÿÀàÁáÂâÃãÄäÅåÆæÇçÈèÉéÊêËëÌìÍíÎîÏïÐðÑñÒòÓóÔôÕõÖØøÙùÚúÛûÜüÝýÞþßö";

            // Configuring the default style with properties
            // we have common to every lexer style saves time.
            scintilla.StyleResetDefault();
            scintilla.Styles[Style.Default].Font = "Consolas";
            scintilla.Styles[Style.Default].Size = 11;
            scintilla.StyleClearAll();

            // Configure the Lua lexer styles
            scintilla.Styles[Style.Lua.Default].ForeColor = Color.Silver;
            scintilla.Styles[Style.Lua.Comment].ForeColor = Color.Green;
            scintilla.Styles[Style.Lua.CommentLine].ForeColor = Color.Green;
            scintilla.Styles[Style.Lua.Number].ForeColor = Color.Olive;
            scintilla.Styles[Style.Lua.Word].ForeColor = Color.Blue;
            scintilla.Styles[Style.Lua.Word2].ForeColor = Color.BlueViolet;
            scintilla.Styles[Style.Lua.Word3].ForeColor = Color.DarkSlateBlue;
            scintilla.Styles[Style.Lua.Word4].ForeColor = Color.Violet;
            scintilla.Styles[Style.Lua.String].ForeColor = Color.Red;
            scintilla.Styles[Style.Lua.Character].ForeColor = Color.Red;
            scintilla.Styles[Style.Lua.LiteralString].ForeColor = Color.Red;
            scintilla.Styles[Style.Lua.StringEol].BackColor = Color.Pink;
            scintilla.Styles[Style.Lua.Operator].ForeColor = Color.Purple;
            scintilla.Styles[Style.Lua.Preprocessor].ForeColor = Color.Maroon;
            scintilla.Lexer = Lexer.Lua;
            scintilla.WordChars = alphaChars + numericChars + accentedChars;

            // Console.WriteLine(scintilla.DescribeKeywordSets());

            // Keywords
            scintilla.SetKeywords(0, VgcApis.Models.Consts.Lua.LuaKeywords);

            // Basic Functions

            scintilla.SetKeywords(1,
                VgcApis.Models.Consts.Lua.ApiFuncNames +
                " " +
                VgcApis.Models.Consts.Lua.LuaFunctions);

            // String Manipulation & Mathematical
            // Input and Output Facilities and System Facilities
            scintilla.SetKeywords(2, VgcApis.Models.Consts.Lua.LuaSubFunctions);

            scintilla.SetKeywords(3, VgcApis.Models.Consts.Lua.LuaModules);

            // Instruct the lexer to calculate folding
            scintilla.SetProperty("fold", "1");
            scintilla.SetProperty("fold.compact", "1");

            // Configure a margin to display folding symbols
            scintilla.Margins[2].Type = MarginType.Symbol;
            scintilla.Margins[2].Mask = Marker.MaskFolders;
            scintilla.Margins[2].Sensitive = true;
            scintilla.Margins[2].Width = 20;

            // Set colors for all folding markers
            for (int i = 25; i <= 31; i++)
            {
                scintilla.Markers[i].SetForeColor(SystemColors.ControlLightLight);
                scintilla.Markers[i].SetBackColor(SystemColors.ControlDark);
            }

            // Configure folding markers with respective symbols
            scintilla.Markers[Marker.Folder].Symbol = MarkerSymbol.BoxPlus;
            scintilla.Markers[Marker.FolderOpen].Symbol = MarkerSymbol.BoxMinus;
            scintilla.Markers[Marker.FolderEnd].Symbol = MarkerSymbol.BoxPlusConnected;
            scintilla.Markers[Marker.FolderMidTail].Symbol = MarkerSymbol.TCorner;
            scintilla.Markers[Marker.FolderOpenMid].Symbol = MarkerSymbol.BoxMinusConnected;
            scintilla.Markers[Marker.FolderSub].Symbol = MarkerSymbol.VLine;
            scintilla.Markers[Marker.FolderTail].Symbol = MarkerSymbol.LCorner;

            // Enable automatic folding
            scintilla.AutomaticFold = (AutomaticFold.Show | AutomaticFold.Click | AutomaticFold.Change);

            // Disable cmd
            scintilla.ClearCmdKey(Keys.Control | Keys.F);
            scintilla.ClearCmdKey(Keys.Control | Keys.S);
            scintilla.ClearCmdKey(Keys.Control | Keys.N);

            // Configure a margin to display line number
            scintilla.Margins[0].Type = MarginType.Number;
            scintilla.Margins[0].Width = 16;
            scintilla.Styles[Style.LineNumber].ForeColor = Color.DarkGray;

            return scintilla;
        }

    }
}
