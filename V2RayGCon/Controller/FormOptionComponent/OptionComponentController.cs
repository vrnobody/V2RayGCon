using V2RayGCon.Model.BaseClass;

namespace V2RayGCon.Controller.OptionComponent
{
    public abstract class OptionComponentController :
        IFormComponentController
    {
        readonly private FormComponentController auxComponentController
            = new FormComponentController();

        #region public method
        public void Bind(Model.BaseClass.FormController container)
        {
            auxComponentController.Bind(container);
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
