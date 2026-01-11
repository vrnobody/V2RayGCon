using System.Collections.Generic;

namespace Composer.Models
{
    public class PackageItem : VgcApis.Interfaces.IHasIndex
    {
        #region properties
        public double index = 1; // serializer require this field to be public

        public string name = "";
        public string skelecton = "";
        public string uid = "";
        public bool isAppend = false;
        public List<ServerSelectorItem> selectors = new List<ServerSelectorItem>();
        #endregion

        public PackageItem() { }

        #region public methods
        public void Copy(PackageItem item)
        {
            this.name = item.name;
            this.uid = item.uid;
            this.skelecton = item.skelecton;
            this.isAppend = item.isAppend;
            this.selectors = VgcApis.Misc.Utils.Clone(item.selectors);
        }
        #endregion

        #region IHasIndex
        public void SetIndex(double index)
        {
            this.index = index;
        }

        public double GetIndex() => this.index;
        #endregion
    }
}
