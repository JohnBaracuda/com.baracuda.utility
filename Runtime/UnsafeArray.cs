using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace Baracuda.Utilities
{
    [BurstCompile]
    public struct UnsafeArray<T> : IDisposable where T : unmanaged
    {
        [NativeDisableUnsafePtrRestriction]
        private unsafe T* _buffer;
        private readonly Allocator _allocator;
        public int Length { get; private set; }
        public readonly bool IsCreated;

        public UnsafeArray(int capacity, Allocator allocator)
        {
            _allocator = allocator;
            Length = capacity;
            unsafe
            {
#if NATIVE_READONLY_ARRAY_LOGS
                Debug.Log("NativeReadonlyArray", "Malloc");
#endif
                _buffer = (T*) UnsafeUtility.Malloc(UnsafeUtility.SizeOf<T>() * capacity, UnsafeUtility.AlignOf<T>(),
                    allocator);
            }
            IsCreated = true;
        }

        public UnsafeArray(T[] array, Allocator allocator)
        {
            _allocator = allocator;
            Length = array.Length;
            unsafe
            {
#if NATIVE_READONLY_ARRAY_LOGS
                Debug.Log("NativeReadonlyArray", "Malloc");
#endif
                _buffer = (T*) UnsafeUtility.Malloc(UnsafeUtility.SizeOf<T>() * Length, UnsafeUtility.AlignOf<T>(),
                    allocator);
                for (var index = 0; index < Length; index++)
                {
                    UnsafeUtility.WriteArrayElement(_buffer, index, array[index]);
                }
            }
            IsCreated = true;
        }

        public UnsafeArray(UnsafeArray<T> array, Allocator allocator)
        {
            _allocator = allocator;
            Length = array.Length;
            unsafe
            {
#if NATIVE_READONLY_ARRAY_LOGS
                Debug.Log("NativeReadonlyArray", "Malloc");
#endif
                _buffer = (T*) UnsafeUtility.Malloc(UnsafeUtility.SizeOf<T>() * Length, UnsafeUtility.AlignOf<T>(),
                    allocator);
                for (var index = 0; index < Length; index++)
                {
                    UnsafeUtility.WriteArrayElement(_buffer, index, array[index]);
                }
            }
            IsCreated = true;
        }

        public UnsafeArray(NativeArray<T> array, Allocator allocator)
        {
            _allocator = allocator;
            Length = array.Length;
            unsafe
            {
#if NATIVE_READONLY_ARRAY_LOGS
                Debug.Log("NativeReadonlyArray", "Malloc");
#endif
                _buffer = (T*) UnsafeUtility.Malloc(UnsafeUtility.SizeOf<T>() * Length, UnsafeUtility.AlignOf<T>(),
                    allocator);
                for (var index = 0; index < Length; index++)
                {
                    UnsafeUtility.WriteArrayElement(_buffer, index, array[index]);
                }
            }
            IsCreated = true;
        }

        public UnsafeArray(UnsafeList<T> list, Allocator allocator)
        {
            _allocator = allocator;
            Length = list.Length;
            unsafe
            {
#if NATIVE_READONLY_ARRAY_LOGS
                Debug.Log("NativeReadonlyArray", "Malloc");
#endif
                _buffer = (T*) UnsafeUtility.Malloc(UnsafeUtility.SizeOf<T>() * Length, UnsafeUtility.AlignOf<T>(),
                    allocator);
                for (var index = 0; index < Length; index++)
                {
                    UnsafeUtility.WriteArrayElement(_buffer, index, list[index]);
                }
            }
            IsCreated = true;
        }

        public UnsafeArray(NativeList<T> list, Allocator allocator)
        {
            _allocator = allocator;
            Length = list.Length;
            unsafe
            {
#if NATIVE_READONLY_ARRAY_LOGS
                Debug.Log("NativeReadonlyArray", "Malloc");
#endif
                _buffer = (T*) UnsafeUtility.Malloc(UnsafeUtility.SizeOf<T>() * Length, UnsafeUtility.AlignOf<T>(),
                    allocator);
                for (var index = 0; index < Length; index++)
                {
                    UnsafeUtility.WriteArrayElement(_buffer, index, list[index]);
                }
            }
            IsCreated = true;
        }

        public UnsafeArray(IReadOnlyCollection<T> collection, Allocator allocator)
        {
            _allocator = allocator;
            Length = collection.Count;
            unsafe
            {
#if NATIVE_READONLY_ARRAY_LOGS
                Debug.Log("NativeReadonlyArray", "Malloc");
#endif
                _buffer = (T*) UnsafeUtility.Malloc(UnsafeUtility.SizeOf<T>() * Length, UnsafeUtility.AlignOf<T>(),
                    allocator);
                var index = 0;
                foreach (var item in collection)
                {
                    UnsafeUtility.WriteArrayElement(_buffer, index++, item);
                }
            }
            IsCreated = true;
        }

        public UnsafeArray(IEnumerable<T> collection, Allocator allocator)
        {
            _allocator = allocator;
            unsafe
            {
#if NATIVE_READONLY_ARRAY_LOGS
                Debug.Log("NativeReadonlyArray", "Malloc");
#endif
                // ReSharper disable once PossibleMultipleEnumeration
                Length = collection.Count();
                _buffer = (T*) UnsafeUtility.Malloc(UnsafeUtility.SizeOf<T>() * Length, UnsafeUtility.AlignOf<T>(),
                    allocator);
                var index = 0;
                // ReSharper disable once PossibleMultipleEnumeration
                foreach (var item in collection)
                {
                    UnsafeUtility.WriteArrayElement(_buffer, index++, item);
                }
            }
            IsCreated = true;
        }

        public T this[int index]
        {
            get
            {
                unsafe
                {
                    if (index < 0 || index >= Length)
                    {
                        throw new IndexOutOfRangeException($"Index {index} is out of range of '{Length}' Length.");
                    }
                    return UnsafeUtility.ReadArrayElement<T>(_buffer, index);
                }
            }
            set
            {
                unsafe
                {
                    if (index < 0 || index >= Length)
                    {
                        throw new IndexOutOfRangeException($"Index {index} is out of range of '{Length}' Length.");
                    }
                    UnsafeUtility.WriteArrayElement(_buffer, index, value);
                }
            }
        }

        public void Dispose()
        {
            unsafe
            {
                if (_buffer != null && Length > 0)
                {
#if NATIVE_READONLY_ARRAY_LOGS
                    Debug.Log("NativeReadonlyArray", "Free");
#endif
                    UnsafeUtility.Free(_buffer, _allocator);
                    _buffer = null;
                    Length = 0;
                }
            }
        }

        [Pure]
        public NativeArray<T> ToNativeArray(Allocator allocator)
        {
            var array = new NativeArray<T>(Length, allocator, NativeArrayOptions.UninitializedMemory);
            unsafe
            {
                UnsafeUtility.MemCpy(NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks(array), _buffer,
                    Length * UnsafeUtility.SizeOf<T>());
            }
            return array;
        }

        [Pure]
        public UnsafeList<T> ToUnsafeList(Allocator allocator)
        {
            var list = new UnsafeList<T>(Length, allocator);
            for (var index = 0; index < Length; index++)
            {
                list.Add(this[index]);
            }
            return list;
        }
    }
}