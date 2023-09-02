using Newtonsoft.Json.Linq;

namespace V2RayGCon.Controllers.ConfigerComponet
{
    abstract class ConfigerComponentController
        : BaseClasses.NotifyComponent,
            BaseClasses.IFormComponentController
    {
        protected Controllers.FormConfigerCtrl container;

        // bind UI controls with component
        public void Bind(BaseClasses.FormController container)
        {
            this.container = container as Controllers.FormConfigerCtrl;
        }

        // update component settings from config
        public abstract void Update(JObject config);
    }
}
