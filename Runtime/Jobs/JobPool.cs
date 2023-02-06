using UnityEngine.Pool;

namespace Baracuda.Gameloop.Jobs
{
    internal class JobPool<TJob> where TJob : JobBase, new()
    {
        private static readonly ObjectPool<TJob> pool =
            new ObjectPool<TJob>(() => new TJob(), job => job.IncrementId());

        public static TJob Get()
        {
            return pool.Get();
        }

        public static void Release(TJob toRelease)
        {
            pool.Release(toRelease);
        }
    }
}