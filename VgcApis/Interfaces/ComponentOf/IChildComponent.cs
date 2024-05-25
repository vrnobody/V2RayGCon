using System;

namespace VgcApis.Interfaces.ComponentOf
{
    // has container
    internal interface IChildComponent : IDisposable
    {
        void SetParent(object parent);
    }
}
