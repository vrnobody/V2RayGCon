using Newtonsoft.Json.Linq;

namespace V2RayGCon.Controllers.FormJsonConfigEditorComponet
{
    abstract class ConfigerComponentController
        : BaseClasses.NotifyComponent,
            BaseClasses.IFormComponentController
    {
        protected Controllers.FormJsonConfigEditorCtrl container;

        // bind UI controls with component
        public void Bind(BaseClasses.FormController container)
        {
            this.container = container as Controllers.FormJsonConfigEditorCtrl;
        }

        // update component settings from config
        public abstract void Update(JObject config);
    }
}
