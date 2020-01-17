using Newtonsoft.Json.Linq;

namespace V2RayGCon.Controller.ConfigerComponet
{
    abstract class ConfigerComponentController :
        Model.BaseClass.NotifyComponent,
        Model.BaseClass.IFormComponentController
    {
        protected Controller.FormConfigerCtrl container;

        // bind UI controls with component
        public void Bind(Model.BaseClass.FormController container)
        {
            this.container = container as Controller.FormConfigerCtrl;
        }

        // update component settings from config
        public abstract void Update(JObject config);
    }
}
