namespace V2RayGCon.Controller
{
    class FormMainCtrl : Model.BaseClass.FormController
    {
        public bool RefreshUI()
        {
            var result = false;
            foreach (var component in GetAllComponents())
            {
                var c = component.Value as Controller.FormMainComponent.FormMainComponentController;
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
                var c = component.Value as Controller.FormMainComponent.FormMainComponentController;
                c.Cleanup();
            }
        }
    }
}
