namespace V2RayGCon.Controllers
{
    public class FormOptionCtrl : BaseClasses.FormController
    {
        public FormOptionCtrl() { }

        public void Cleanup()
        {
            foreach (var kv in GetAllComponents())
            {
                var ctrl = kv.Value as OptionComponent.OptionComponentController;
                ctrl.Cleanup();
            }
        }

        public bool IsOptionsSaved()
        {
            foreach (var component in GetAllComponents())
            {
                var optCtrl = component.Value as OptionComponent.OptionComponentController;
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
                var component = kvPair.Value as OptionComponent.OptionComponentController;
                if (component.SaveOptions())
                {
                    changed = true;
                }
            }
            return changed;
        }
    }
}
