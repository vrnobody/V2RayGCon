using System;
using System.Collections.Generic;

namespace VgcApis.Interfaces.ComponentOf
{
    // has container
    public interface IParentComponent : IDisposable
    {
        void Prepare();

        void AddChild<TChild>(TChild child)
            where TChild : class, IDisposable;

        TChild GetChild<TChild>()
            where TChild : class;

        IReadOnlyCollection<object> GetChildren();
    }
}
