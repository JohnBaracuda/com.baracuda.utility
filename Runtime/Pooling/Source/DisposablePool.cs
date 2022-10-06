using System;

namespace Baracuda.Utilities.Pooling.Source
{
    public static class DisposablePool<T> where T : IDisposable, new()
    {
        public static int CountAll => pool.CountAll;

        private static readonly ObjectPoolT<T> pool = new(() => new T(), disposable => disposable.Dispose());

        public static T Get()
        {
            return pool.Get();
        }

        public static void Release(T toRelease)
        {
            pool.Release(toRelease);
        }
    }
}