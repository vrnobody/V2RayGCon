using ProxySetter.Model.BaseClass;

namespace ProxySetter.Controllers.VGCPluginComponents
{
    abstract class ComponentCtrl : IFormComponentController
    {
        private FormComponentController auxComponentController
           = new FormComponentController();

        #region public method
        public void Bind(FormController container)
        {
            auxComponentController.Bind(container);
        }
        #endregion

        #region abstract method
        // Container closing.
        public abstract void Cleanup();

        public abstract bool IsOptionsChanged();

        // User click save-button of container.
        // true: save new options  false: options not changed
        public abstract bool SaveOptions();

        #endregion

        #region protected method
        protected FormVGCPluginCtrl GetContainer()
        {
            return auxComponentController
                .GetContainer<FormVGCPluginCtrl>();
        }
        #endregion

        #region private method

        #endregion
    }
}
