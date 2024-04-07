using System.Collections.Generic;
using UnityEngine.Pool;

namespace Baracuda.Utilities.Pools
{
    public class QueuePool<T>
    {
        private static readonly ObjectPool<Queue<T>> poolTBase
            = new(() => new Queue<T>(), actionOnRelease: l => l.Clear());

        public static Queue<T> Get()
        {
            return poolTBase.Get();
        }

        public static void Release(Queue<T> toRelease)
        {
            poolTBase.Release(toRelease);
        }
    }
}