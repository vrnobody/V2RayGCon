namespace Composer.Models
{
    public class ServerInfoItem : VgcApis.Interfaces.IHasIndex
    {
        #region properties
        public double index = 1; // serializer require this field to be public

        public string uid = "";
        public string title = "";
        #endregion

        public ServerInfoItem() { }

        public ServerInfoItem(VgcApis.Interfaces.ICoreServCtrl ctrl)
        {
            var cs = ctrl.GetCoreStates();
            this.uid = cs.GetUid();
            this.title = cs.GetTitle();
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
