namespace ProxySetter.Controllers
{
    class FormVGCPluginCtrl : Model.BaseClass.FormController
    {
        public FormVGCPluginCtrl() { }

        public bool IsOptionsSaved()
        {
            foreach (var component in GetAllComponents())
            {
                var optCtrl = component.Value as VGCPluginComponents.ComponentCtrl;
                if (optCtrl.IsOptionsChanged())
                {
                    return false;
                }
            }
            return true;
        }

        public bool SaveAllOptions()
        {
            var changed = false;
            foreach (var kvPair in GetAllComponents())
            {
                var component = kvPair.Value as VGCPluginComponents.ComponentCtrl;
                if (component.SaveOptions())
                {
                    changed = true;
                }
            }

            return changed;
        }

        public void Cleanup()
        {
            foreach (var kvPair in GetAllComponents())
            {
                var component = kvPair.Value as VGCPluginComponents.ComponentCtrl;
                component.Cleanup();
            }
        }

    }
}
