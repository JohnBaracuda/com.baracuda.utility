using System;
using JetBrains.Annotations;
using UnityEngine.Pool;

namespace Baracuda.Utility.Pooling
{
    public ref struct PoolBuilder<T> where T : class
    {
        private Func<T> _createFunc;
        private Action<T> _actionOnGet;
        private Action<T> _actionOnRelease;
        private Action<T> _actionOnDestroy;
        private bool _collectionCheck;
        private int _defaultCapacity;
        private int _maxSize;

        [MustUseReturnValue]
        public static PoolBuilder<T> Create()
        {
            return new PoolBuilder<T>(10000);
        }

        public PoolBuilder(int maxSize)
        {
            _createFunc = null;
            _actionOnGet = null;
            _actionOnRelease = null;
            _actionOnDestroy = null;
            _collectionCheck = true;
            _defaultCapacity = 10;
            _maxSize = maxSize;
        }

        [MustUseReturnValue]
        public PoolBuilder<T> WithCreateFunc(Func<T> createFunc)
        {
            _createFunc = createFunc;
            return this;
        }

        [MustUseReturnValue]
        public PoolBuilder<T> WithActionOnGet(Action<T> actionOnGet)
        {
            _actionOnGet = actionOnGet;
            return this;
        }

        [MustUseReturnValue]
        public PoolBuilder<T> WithActionOnRelease(Action<T> actionOnRelease)
        {
            _actionOnRelease = actionOnRelease;
            return this;
        }

        [MustUseReturnValue]
        public PoolBuilder<T> WithActionOnDestroy(Action<T> actionOnDestroy)
        {
            _actionOnDestroy = actionOnDestroy;
            return this;
        }

        [MustUseReturnValue]
        public PoolBuilder<T> WithCollectionCheck(bool collectionCheck)
        {
            _collectionCheck = collectionCheck;
            return this;
        }

        [MustUseReturnValue]
        public PoolBuilder<T> WithDefaultCapacity(int defaultCapacity)
        {
            _defaultCapacity = defaultCapacity;
            return this;
        }

        [MustUseReturnValue]
        public PoolBuilder<T> WithMaxSize(int maxSize)
        {
            _maxSize = maxSize;
            return this;
        }

        [MustUseReturnValue]
        public readonly ObjectPool<T> Build()
        {
            if (_createFunc == null)
            {
                throw new InvalidOperationException("Create function must be provided.");
            }

            return new ObjectPool<T>(
                _createFunc,
                _actionOnGet,
                _actionOnRelease,
                _actionOnDestroy,
                _collectionCheck,
                _defaultCapacity,
                _maxSize
            );
        }
    }
}