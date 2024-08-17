using System.Text;
using UnityEngine.Pool;

namespace Baracuda.Bedrock.Pools
{
    public static class StringBuilderPool
    {
        private static readonly ObjectPool<StringBuilder> pool = new(() => new StringBuilder(100),
            actionOnRelease: builder => builder.Clear());

        public static StringBuilder Get()
        {
            return pool.Get();
        }

        public static void Release(StringBuilder toRelease)
        {
            pool.Release(toRelease);
        }

        public static string BuildAndRelease(StringBuilder toRelease)
        {
            var str = toRelease.ToString();
            pool.Release(toRelease);
            return str;
        }
    }
}