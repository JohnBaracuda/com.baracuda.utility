using System.Collections.Generic;
using UnityEngine.Pool;

namespace Baracuda.Utility.Pools
{
    public class SortedListPool<T1, T2> : CollectionPool<SortedList<T1, T2>, KeyValuePair<T1, T2>>
    {
    }
}