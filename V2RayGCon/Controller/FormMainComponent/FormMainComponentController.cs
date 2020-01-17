using V2RayGCon.Model.BaseClass;

namespace V2RayGCon.Controller.FormMainComponent
{
    abstract class FormMainComponentController : IFormComponentController
    {
        private FormComponentController auxComponentController
            = new FormComponentController();

        #region public method
        public void Bind(Model.BaseClass.FormController container)
        {
            auxComponentController.Bind(container);
        }
        #endregion

        #region abstract method
        public abstract bool RefreshUI();
        public abstract void Cleanup();

        #endregion

        #region protected method
        protected FormMainCtrl GetContainer()
        {
            return auxComponentController.GetContainer<FormMainCtrl>();
        }
        #endregion

        #region private method
        #endregion
    }
}
