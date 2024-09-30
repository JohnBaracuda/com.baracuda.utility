namespace Baracuda.Utility.Pooling
{
    public interface IPoolObject
    {
        public void OnGetFromPool();

        public void OnReleaseToPool();
    }
}