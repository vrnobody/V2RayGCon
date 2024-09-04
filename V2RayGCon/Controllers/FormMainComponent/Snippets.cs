using System.Collections;
using System.Collections.Generic;
using VgcApis.UserControls;
using VgcApis.UserControls.AcmComboBoxComps;

namespace V2RayGCon.Controllers.FormMainComponent
{
    internal sealed class Snippets : IEnumerable<AutocompleteItem>
    {
        readonly AcmComboBox cbox;
        readonly IEnumerable<AutocompleteItem> tips;

        public Snippets(AcmComboBox cbox, IEnumerable<AutocompleteItem> tips)
        {
            this.cbox = cbox;
            this.tips = tips;
        }

        #region public methods

        #endregion

        #region private methods
        private IEnumerable<AutocompleteItem> BuildList()
        {
            var space = @" ";
            var text = this.cbox.Text.ToLower();
            var requireSpace = text.Contains(space);

            var snps = new List<AutocompleteItem>();
            foreach (var tip in this.tips)
            {
                var s = tip.Text;
                var hasSpace = s.Contains(space);
                if (hasSpace == requireSpace)
                {
                    var match = VgcApis.Misc.Utils.PartialMatchCi(s, text);
                    if (match)
                    {
                        snps.Add(tip);
                    }
                }
            }

            //return autocomplete items
            foreach (var item in snps)
                yield return item;
        }

        #endregion

        #region IEnumerable thinggy
        public IEnumerator<AutocompleteItem> GetEnumerator() => BuildList().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        #endregion
    }
}
