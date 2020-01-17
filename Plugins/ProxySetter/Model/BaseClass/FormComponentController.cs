namespace ProxySetter.Model.BaseClass
{
    public class FormComponentController : IFormComponentController
    {
        protected FormController container = null;

        // bind UI controls with component
        public void Bind(FormController container)
        {
            this.container = container;
        }

        public T GetContainer<T>() where T : FormController
        {
            if (this.container == null)
            {
                return null;
            }
            return this.container as T;
        }
    }
}
