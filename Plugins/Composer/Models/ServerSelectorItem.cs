using System;
using System.Collections.Generic;

namespace Composer.Models
{
    public class ServerSelectorItem : VgcApis.Interfaces.IHasIndex
    {
        #region properties
        public double index = 1; // serializer require this field to be public

        public string id = Guid.NewGuid().ToString();
        public string tag = "";
        public string filter = "";
        public List<ServerInfoItem> servInfos = new List<ServerInfoItem>();
        #endregion

        public ServerSelectorItem() { }

        public void Copy(ServerSelectorItem item)
        {
            this.tag = item.tag;
            this.filter = item.filter;
            this.servInfos = VgcApis.Misc.Utils.Clone(item.servInfos);
        }

        #region IHasIndex
        public void SetIndex(double index)
        {
            this.index = index;
        }

        public double GetIndex() => this.index;
        #endregion
    }
}
