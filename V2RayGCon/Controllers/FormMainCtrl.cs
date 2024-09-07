using V2RayGCon.Controllers.FormMainComponent;

namespace V2RayGCon.Controllers
{
    class FormMainCtrl : BaseClasses.FormController
    {
        public void ResetServUiBorders()
        {
            this.GetComponent<FlyServer>()?.ResetServUiBorders();
        }

        public void Cleanup()
        {
            foreach (var component in GetAllComponents())
            {
                var c = component.Value as FormMainComponent.FormMainComponentController;
                c.Cleanup();
            }
        }
    }
}
