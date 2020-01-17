using System.Collections.Generic;

namespace VgcApis.Interfaces.ComponentOf
{
    // has container
    internal interface IComponentOf<TParent> :
        IChildComponent,
        IParentComponent
        where TParent : class, IParentComponent
    {
        TParent GetParent(); // 要使用parent的public方法

        IReadOnlyCollection<object> GetSiblings();

        TSibling GetSibling<TSibling>() where TSibling : class;
    }
}
