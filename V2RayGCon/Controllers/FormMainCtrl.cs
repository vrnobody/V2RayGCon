namespace V2RayGCon.Controllers
{
    class FormMainCtrl : BaseClasses.FormController
    {
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
