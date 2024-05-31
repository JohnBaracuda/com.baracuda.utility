using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace Baracuda.Utilities
{
    public static class NativeExtensions
    {
        #region Ref Index

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

        #endregion


        #region Native Array

        [BurstDiscard]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NativeArray<T1> ToNativeArray<T1, T2>(this IReadOnlyCollection<T2> collection,
            Func<T2, T1> converter,
            Allocator allocator) where T1 : struct
        {
            var nativeArray = new NativeArray<T1>(collection.Count, allocator);
            var index = 0;
            foreach (var element in collection)
            {
                nativeArray[index] = converter(element);
                index++;
            }

            return nativeArray;
        }

        [BurstDiscard]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NativeArray<T1> ToNativeArray<T1, T2>(this IReadOnlyCollection<T2> collection,
            Allocator allocator) where T1 : struct where T2 : INative<T1>
        {
            var nativeArray = new NativeArray<T1>(collection.Count, allocator);
            var index = 0;
            foreach (var element in collection)
            {
                nativeArray[index] = element.ToNative();
                index++;
            }

            return nativeArray;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NativeArray<T> ToNativeArray<T>(this UnsafeList<T> unsafeList,
            Allocator allocator) where T : unmanaged
        {
            var array = new NativeArray<T>(unsafeList.Length, allocator);
            var index = 0;
            foreach (var element in unsafeList)
            {
                array[index] = element;
                index++;
            }
            return array;
        }

        #endregion


        #region Unsafe List

        [BurstDiscard]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UnsafeList<T> ToUnsafeList<T>(this IReadOnlyCollection<T> collection,
            Allocator allocator) where T : unmanaged
        {
            var list = new UnsafeList<T>(collection.Count, allocator);
            foreach (var element in collection)
            {
                list.Add(element);
            }
            return list;
        }

        [BurstDiscard]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UnsafeList<T1> ToUnsafeList<T1, T2>(this IReadOnlyCollection<T2> collection,
            Allocator allocator) where T1 : unmanaged where T2 : INative<T1>
        {
            var list = new UnsafeList<T1>(collection.Count, allocator);
            foreach (var element in collection)
            {
                list.Add(element.ToNative());
            }
            return list;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UnsafeList<T> ToUnsafeList<T>(this NativeList<T> nativeList,
            Allocator allocator)
            where T : unmanaged
        {
            var list = new UnsafeList<T>(nativeList.Length, allocator);
            for (var index = 0; index < nativeList.Length; index++)
            {
                list.Add(nativeList[index]);
            }
            return list;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UnsafeList<T> ToUnsafeList<T>(this NativeArray<T> nativeArray,
            Allocator allocator)
            where T : unmanaged
        {
            var list = new UnsafeList<T>(nativeArray.Length, allocator);
            for (var index = 0; index < nativeArray.Length; index++)
            {
                list.Add(nativeArray[index]);
            }
            return list;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UnsafeList<T> Clone<T>(this UnsafeList<T> origin, Allocator allocator)
            where T : unmanaged
        {
            var list = new UnsafeList<T>(origin.Length, allocator);
            for (var index = 0; index < origin.Length; index++)
            {
                list.Add(origin[index]);
            }
            return list;
        }

        #endregion


        #region Unsafe Hash Set

        [BurstDiscard]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UnsafeHashSet<T> ToUnsafeHashSet<T>(this IReadOnlyCollection<T> collection,
            Allocator allocator) where T : unmanaged, IEquatable<T>
        {
            var set = new UnsafeHashSet<T>(collection.Count, allocator);
            foreach (var element in collection)
            {
                set.Add(element);
            }
            return set;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UnsafeHashSet<T> ToUnsafeHashSet<T>(this UnsafeList<T> origin, Allocator allocator)
            where T : unmanaged, IEquatable<T>
        {
            var set = new UnsafeHashSet<T>(origin.Length, allocator);
            foreach (var element in origin)
            {
                set.Add(element);
            }
            return set;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UnsafeHashSet<T> Clone<T>(this UnsafeHashSet<T> origin, Allocator allocator)
            where T : unmanaged, IEquatable<T>
        {
            var set = new UnsafeHashSet<T>(origin.Count, allocator);
            foreach (var element in origin)
            {
                set.Add(element);
            }
            return set;
        }

        #endregion


        #region Unsafe Array

        [BurstDiscard]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UnsafeArray<T> ToUnsafeArray<T>(this IReadOnlyCollection<T> collection,
            Allocator allocator) where T : unmanaged
        {
            return new UnsafeArray<T>(collection, allocator);
        }

        [BurstDiscard]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UnsafeArray<T1> ToUnsafeArray<T1, T2>(this IReadOnlyCollection<T2> collection,
            Allocator allocator) where T1 : unmanaged where T2 : INative<T1>
        {
            var buffer = collection.ToUnsafeList<T1, T2>(Allocator.Temp);
            var array = new UnsafeArray<T1>(buffer, allocator);
            buffer.Dispose();
            return array;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UnsafeArray<T> ToUnsafeArray<T>(this NativeList<T> nativeList,
            Allocator allocator)
            where T : unmanaged
        {
            return new UnsafeArray<T>(nativeList, allocator);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UnsafeArray<T> Clone<T>(this UnsafeArray<T> origin, Allocator allocator)
            where T : unmanaged
        {
            return new UnsafeArray<T>(origin, allocator);
        }

        #endregion


        #region Disposing

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DisposeElements<T>(this UnsafeList<T> collection) where T : unmanaged, IDisposable
        {
            foreach (var disposable in collection)
            {
                disposable.Dispose();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DisposeElements<T>(this NativeArray<T> collection) where T : unmanaged, IDisposable
        {
            foreach (var disposable in collection)
            {
                disposable.Dispose();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DisposeElements<T>(this NativeList<T> collection) where T : unmanaged, IDisposable
        {
            foreach (var disposable in collection)
            {
                disposable.Dispose();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DisposeElements<T>(this UnsafeArray<T> array) where T : unmanaged, IDisposable
        {
            for (var index = 0; index < array.Length; index++)
            {
                array[index].Dispose();
            }
        }

        #endregion
    }
}