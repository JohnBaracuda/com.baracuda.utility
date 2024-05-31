using System;
using Unity.Burst;
using Unity.Collections.LowLevel.Unsafe;

namespace Baracuda.Utilities
{
    [BurstCompile]
    public static class NativeLinq
    {
        public static void ExceptWith<T>(ref UnsafeHashSet<T> hashSet, UnsafeList<T> array)
            where T : unmanaged, IEquatable<T>
        {
            for (var index = 0; index < array.Length; index++)
            {
                var element = array[index];
                hashSet.Remove(element);
            }
        }

        public static void UnionWith<T>(ref UnsafeHashSet<T> hashSet, UnsafeList<T> array)
            where T : unmanaged, IEquatable<T>
        {
            for (var index = 0; index < array.Length; index++)
            {
                var arrayElement = array[index];
                if (hashSet.Contains(arrayElement))
                {
                    continue;
                }
                hashSet.Add(arrayElement);
            }
        }

        public static void ExceptWith<T>(ref UnsafeList<T> list, UnsafeList<T> array)
            where T : unmanaged, IEquatable<T>
        {
            for (var indexList = 0; indexList < list.Length; indexList++)
            {
                for (var indexArray = 0; indexArray < array.Length; indexArray++)
                {
                    if (list[indexList].Equals(array[indexArray]))
                    {
                        list.RemoveAtSwapBack(indexList);
                        indexList--;
                        break;
                    }
                }
            }
        }

        public static void UnionWith<T>(ref UnsafeList<T> list, UnsafeList<T> array)
            where T : unmanaged, IEquatable<T>
        {
            for (var index = 0; index < array.Length; index++)
            {
                var arrayElement = array[index];
                if (list.Contains(arrayElement))
                {
                    continue;
                }
                list.Add(arrayElement);
            }
        }
    }
}