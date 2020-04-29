using V2RayGCon.BaseClasses;

namespace V2RayGCon.Controllers.OptionComponent
{
    public abstract class OptionComponentController :
        IFormComponentController
    {
        readonly private FormComponentController auxComponentController
            = new FormComponentController();

        #region public method
        public void Bind(BaseClasses.FormController container)
        {
            auxComponentController.Bind(container);
        }

        public virtual void Cleanup()
        {

        }
        #endregion

        #region abstract method
        // Container closing.
        public abstract bool IsOptionsChanged();

        // User click save-button of container.
        // true: save new options  false: options not changed
        public abstract bool SaveOptions();

        #endregion

        #region protected method


        protected FormOptionCtrl GetContainer()
        {
            return auxComponentController.GetContainer<FormOptionCtrl>();
        }
        #endregion

        #region private method

        #endregion
    }
}
