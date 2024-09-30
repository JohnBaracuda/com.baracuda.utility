using System.Collections.Generic;
using UnityEngine.Pool;

namespace Baracuda.Utility.Pools
{
    public class StackPool<T>
    {
        private static readonly ObjectPool<Stack<T>> pool = new(() => new Stack<T>(), actionOnRelease: l => l.Clear());

        public static Stack<T> Get()
        {
            return pool.Get();
        }

        public static void Release(Stack<T> toRelease)
        {
            pool.Release(toRelease);
        }
    }
}