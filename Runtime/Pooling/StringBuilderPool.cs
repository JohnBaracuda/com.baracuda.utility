// Copyright (c) 2022 Jonathan Lang

using System.Text;
using UnityEngine.Pool;

namespace Baracuda.Utilities.Pooling
{
    public static class StringBuilderPool
    {
        private static readonly ObjectPool<StringBuilder> pool =
            new ObjectPool<StringBuilder>(() => new StringBuilder(100), actionOnRelease: builder => builder.Clear());

        public static StringBuilder Get()
        {
            return pool.Get();
        }

        public static void ReleaseStringBuilder(StringBuilder toRelease)
        {
            pool.Release(toRelease);
        }

        public static string Release(StringBuilder toRelease)
        {
            var str = toRelease.ToString();
            pool.Release(toRelease);
            return str;
        }
    }
}