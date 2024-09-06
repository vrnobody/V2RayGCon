using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using VgcApis.UserControls;
using VgcApis.UserControls.AcmComboBoxComps;
using static System.Net.Mime.MediaTypeNames;

namespace V2RayGCon.Controllers.FormMainComponent
{
    internal sealed class Snippet : AutocompleteItem
    {
        public Snippet(string text, int iconIdx)
            : base(text, iconIdx) { }

        public override CompareResult Compare(string _)
        {
            var ctext = this.Parent.TargetControlWrapper.Text;
            if (string.IsNullOrEmpty(ctext))
            {
                return CompareResult.Hidden;
            }

            try
            {
                if (Regex.IsMatch(ctext, @"^#\d+"))
                {
                    return CompareResult.Hidden;
                }
            }
            catch
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

        public override string GetTextForReplace()
        {
            return Text;
        }
    }
}
