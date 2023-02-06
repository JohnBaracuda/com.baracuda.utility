// Copyright (c) 2022 Jonathan Lang

using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using UnityEngine.Pool;

namespace Baracuda.Utilities.Pooling.Source
{
    public class ConcurrentObjectPool<T>
    {
        public int CountAll { get; private protected set; }

        public int CountActive => CountAll - CountInactive;
        public int CountInactive
        {
            get
            {
                lock (_stack)
                {
                    return _stack.Count;
                }
            }
        }

        private readonly Stack<T> _stack;
        private readonly Func<T> _createFunc;
        private readonly Action<T> _actionOnGet;
        private readonly Action<T> _actionOnRelease;
        private readonly Action<T> _actionOnDestroy;
        private readonly int _maxSize;
        private readonly bool _collectionCheck;

        public ConcurrentObjectPool(
            [NotNull] Func<T> createFunc,
            Action<T> actionOnGet = null,
            Action<T> actionOnRelease = null,
            Action<T> actionOnDestroy = null,
            bool collectionCheck = true,
            int defaultCapacity = 1,
            int maxSize = 10000)
        {
            _stack = new Stack<T>(defaultCapacity);
            _createFunc = createFunc;
            _actionOnGet = actionOnGet;
            _actionOnRelease = actionOnRelease;
            _actionOnDestroy = actionOnDestroy;
            _maxSize = maxSize;
            _collectionCheck = collectionCheck;
        }

        public T Get()
        {
            T obj;
            lock (_stack)
            {
                if (_stack.Count == 0)
                {
                    obj = _createFunc();
                    ++CountAll;
                }
                else
                {
                    obj = _stack.Pop();
                }
            }
            _actionOnGet?.Invoke(obj);
            return obj;
        }

        public void Release(T element)
        {
            lock (_stack)
            {
                if (_collectionCheck && _stack.Count > 0 && _stack.Contains(element))
                {
                    throw new InvalidOperationException("Trying to release an object that has already been released to the pool.");
                }

                _actionOnRelease?.Invoke(element);
                if (CountInactive < _maxSize)
                {
                    _stack.Push(element);
                }
                else
                {
                    _actionOnDestroy?.Invoke(element);
                }
            }
        }

        public void Clear()
        {
            lock (_stack)
            {
                if (_actionOnDestroy != null)
                {
                    foreach (var obj in _stack)
                    {
                        _actionOnDestroy(obj);
                    }
                }

                _stack.Clear();
                CountAll = 0;
            }
        }
    }
}