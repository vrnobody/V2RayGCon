using V2RayGCon.BaseClasses;

namespace V2RayGCon.Controllers.FormMainComponent
{
    abstract class FormMainComponentController : IFormComponentController
    {
        private FormComponentController auxComponentController
            = new FormComponentController();

        #region public method
        public void Bind(BaseClasses.FormController container)
        {
            auxComponentController.Bind(container);
        }
        #endregion

        #region abstract method
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
