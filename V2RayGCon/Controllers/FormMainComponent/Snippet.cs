using System.Runtime.InteropServices.ComTypes;
using System.Text.RegularExpressions;
using VgcApis.UserControls.AcmComboBoxComps;

namespace V2RayGCon.Controllers.FormMainComponent
{
    internal sealed class Snippet : AutocompleteItem
    {
        public Snippet(string text, int iconIdx)
            : base(text, iconIdx) { }

        public override CompareResult Compare(string _)
        {
            var ctext = this.Parent.Fragment.Text;
            if (string.IsNullOrEmpty(ctext) || !ctext.StartsWith("#"))
            {
                return CompareResult.Hidden;
            }

            if (int.TryParse(ctext.Substring(1), out var _))
            {
                return CompareResult.Hidden;
            }

            var space = @" ";
            var requireSpace = ctext.Contains(space);
            if (
                Text.Contains(space) == requireSpace
                && VgcApis.Misc.Utils.PartialMatchCi(Text, ctext)
            )
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
