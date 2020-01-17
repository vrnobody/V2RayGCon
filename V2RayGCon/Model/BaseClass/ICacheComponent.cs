using System.Collections.Generic;

namespace V2RayGCon.Model.BaseClass
{
    interface ICacheComponent<TKey, TValue>
    {
        TValue this[TKey url] { get; }

        int Count { get; }

        TKey[] Keys { get; }

        void Remove(List<TKey> urls);

        void Clear();
    }
}
