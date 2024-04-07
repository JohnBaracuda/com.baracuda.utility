using System;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace Baracuda.Utilities
{
    public static class UnsafeExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe ref T GetRef<T>(this NativeArray<T> array, int index)
            where T : struct
        {
            if (index < 0 || index >= array.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }
            return ref UnsafeUtility.ArrayElementAsRef<T>(array.GetUnsafePtr(), index);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe ref T GetRef<T>(this NativeList<T> list, int index)
            where T : unmanaged
        {
            if (index < 0 || index >= list.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }
            return ref UnsafeUtility.ArrayElementAsRef<T>(list.GetUnsafePtr(), index);
        }
    }
}