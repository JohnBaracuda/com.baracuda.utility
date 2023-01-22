using Baracuda.Utilities.Pooling;
using Baracuda.Utilities.Pooling.Source;

namespace Baracuda.Gameloop.Jobs
{
    internal class JobPool<TJob> where TJob : JobBase, new()
    {
        private static readonly ObjectPoolT<TJob> pool =
            new ObjectPoolT<TJob>(() => new TJob(), job => job.IncrementId());

        public static TJob Get()
        {
            return pool.Get();
        }

        public static void Release(TJob toRelease)
        {
            pool.Release(toRelease);
        }

        public static PooledObject<TJob> GetDisposable()
        {
            return pool.GetDisposable();
        }
    }
}