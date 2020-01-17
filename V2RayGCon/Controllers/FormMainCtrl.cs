namespace V2RayGCon.Controllers
{
    class FormMainCtrl : BaseClasses.FormController
    {
        public bool RefreshUI()
        {
            var result = false;
            foreach (var component in GetAllComponents())
            {
                var c = component.Value as Controllers.FormMainComponent.FormMainComponentController;
                if (c.RefreshUI())
                {
                    result = true;
                }
            }

            return result;
        }

        public void Cleanup()
        {
            foreach (var component in GetAllComponents())
            {
                var c = component.Value as Controllers.FormMainComponent.FormMainComponentController;
                c.Cleanup();
            }
        }
    }
}
