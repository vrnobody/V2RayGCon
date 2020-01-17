using System;
using System.Collections.Generic;

namespace VgcApis.Models.Interfaces
{
    // has container
    public interface IComponentOf<TContainer> :
        IDisposable
        where TContainer : class
    {

        TContainer GetContainer();

        void BindTo(TContainer container);

        IReadOnlyCollection<object> GetAllComponents();

        void Prepare();

        TComponent GetComponent<TComponent>()
            where TComponent : class;


        void Plug<TSelf, TComponent>(TSelf container, TComponent component)
            where TSelf : class, IComponentOf<TContainer>
            where TComponent : class, IComponentOf<TSelf>;

    }


}
