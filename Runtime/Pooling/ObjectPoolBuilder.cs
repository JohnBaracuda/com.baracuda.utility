using System;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Pool;
using Object = UnityEngine.Object;

namespace Baracuda.Utility.Pooling
{
    /// <summary>
    ///     Builder for creating an instance of <see cref="ObjectPool{T}" />.
    /// </summary>
    public ref struct ObjectPoolBuilder<T> where T : Object
    {
        private readonly Func<T> _createFunc;
        private Action<T> _actionOnGet;
        private Action<T> _actionOnRelease;
        private Action<T> _actionOnDestroy;
        private bool _collectionCheck;
        private int _defaultCapacity;
        private int _maxSize;

        internal ObjectPoolBuilder(Func<T> createFunc)
        {
            _createFunc = createFunc ?? throw new ArgumentNullException(nameof(createFunc));
            _actionOnGet = null;
            _actionOnRelease = null;
            _actionOnDestroy = null;
            _collectionCheck = true;
            _defaultCapacity = 10;
            _maxSize = 10000;
        }

        [PublicAPI]
        [MustUseReturnValue]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ObjectPoolBuilder<T> WithActionOnGet(Action<T> actionOnGet)
        {
            _actionOnGet = actionOnGet;
            return this;
        }

        [PublicAPI]
        [MustUseReturnValue]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ObjectPoolBuilder<T> WithActionOnRelease(Action<T> actionOnRelease)
        {
            _actionOnRelease = actionOnRelease;
            return this;
        }

        [PublicAPI]
        [MustUseReturnValue]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ObjectPoolBuilder<T> WithActionOnDestroy(Action<T> actionOnDestroy)
        {
            _actionOnDestroy = actionOnDestroy;
            return this;
        }

        [PublicAPI]
        [MustUseReturnValue]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ObjectPoolBuilder<T> WithCollectionCheck(bool collectionCheck)
        {
            _collectionCheck = collectionCheck;
            return this;
        }

        [PublicAPI]
        [MustUseReturnValue]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ObjectPoolBuilder<T> WithDefaultCapacity(int defaultCapacity)
        {
            _defaultCapacity = defaultCapacity;
            return this;
        }

        [PublicAPI]
        [MustUseReturnValue]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ObjectPoolBuilder<T> WithMaxSize(int maxSize)
        {
            _maxSize = maxSize;
            return this;
        }

        [PublicAPI]
        [MustUseReturnValue]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ObjectPool<T> Build()
        {
            return new ObjectPool<T>(
                _createFunc,
                _actionOnGet,
                _actionOnRelease,
                _actionOnDestroy,
                _collectionCheck,
                _defaultCapacity,
                _maxSize);
        }

        [PublicAPI]
        [MustUseReturnValue]
        public static implicit operator ObjectPool<T>(ObjectPoolBuilder<T> builder)
        {
            return builder.Build();
        }

        [PublicAPI]
        [MustUseReturnValue]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ObjectPoolBuilder<T> WithCreateFunc(Func<T> createFunc)
        {
            return new ObjectPoolBuilder<T>(createFunc);
        }

        [PublicAPI]
        [MustUseReturnValue]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ObjectPoolBuilder<T> WithCreateFunc(T prefab, Transform parent = null)
        {
            return new ObjectPoolBuilder<T>(() => Object.Instantiate(prefab, parent));
        }
    }
}