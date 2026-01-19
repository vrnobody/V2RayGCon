using System.Linq;
using VgcApis.UserControls.AcmComboBoxComps;

namespace VgcApis.Controllers
{
    internal sealed class KeywordFilterSnippet : AutocompleteItem
    {
        public KeywordFilterSnippet(string text, int iconIdx)
            : base(text, iconIdx) { }

        public override CompareResult Compare(string _)
        {
            var fragment = this.Parent.Fragment.Text;
            if (string.IsNullOrEmpty(fragment) || !fragment.StartsWith("#"))
            {
                return CompareResult.Hidden;
            }

            var first = fragment.Substring(1).FirstOrDefault();
            if (first != default(char) && !char.IsLetter(first))
            {
                return CompareResult.Hidden;
            }

            var nFrag = $"{fragment}dirtyhack"
                .Split(new char[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries)
                .Length;
            var nText = Text.Split(
                new char[] { ' ' },
                System.StringSplitOptions.RemoveEmptyEntries
            ).Length;
            if (nFrag != nText)
            {
                return CompareResult.Hidden;
            }
            if (VgcApis.Misc.Utils.PartialMatchCi(Text, fragment))
            {
                return CompareResult.Visible;
            }
            return CompareResult.Hidden;
        }

        #region public methods

        public override string GetTextForReplace()
        {
            return Text;
        }
        #endregion

        #region private methods

        #endregion
    }
}
