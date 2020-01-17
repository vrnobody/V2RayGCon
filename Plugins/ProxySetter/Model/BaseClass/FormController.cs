using System;
using System.Collections.Generic;

namespace ProxySetter.Model.BaseClass
{
    public class FormController
    {
        Dictionary<Type, IFormComponentController> components;

        public FormController()
        {
            components = new Dictionary<Type, IFormComponentController>();
        }

        #region public method
        public Dictionary<Type, IFormComponentController> GetAllComponents()
        {
            return this.components;
        }

        public T GetComponent<T>() where T : class, IFormComponentController
        {
            var type = typeof(T);

            if (!components.ContainsKey(type))
            {
                throw new KeyNotFoundException();
            }

            return components[type] as T;
        }

        public FormController Plug(IFormComponentController component)
        {
            var type = component.GetType();
            if (components.ContainsKey(type))
            {
                throw new ArgumentException("Key already existed!");
            }
            component.Bind(this);
            components[type] = component;
            return this;
        }

        public void Plug(List<IFormComponentController> components)
        {
            foreach (var component in components)
            {
                Plug(component);
            }
        }
        #endregion

        #region private method
        #endregion

    }
}
