namespace Baracuda.Bedrock.Pooling
{
    public interface IPoolObject
    {
        public void OnGetFromPool();

        public void OnReleaseToPool();
    }
}