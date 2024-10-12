using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using Baracuda.Utility.Collections;
using Baracuda.Utility.PlayerLoop;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Pool;
using Index = Baracuda.Utility.Types.Index;
using Object = UnityEngine.Object;

namespace Baracuda.Utility.Pooling
{
    public abstract class PoolAsset<T> : ScriptableObject, IObjectPool<T> where T : Object
    {
        #region Settings

        [Header("Assets")]
        [SerializeField] private T[] prefabs;

        [Header("Settings")]
        [SerializeField] private bool limitPoolSize;
        [ShowIf(nameof(limitPoolSize))]
        [SerializeField] private int poolLimit = 100;

        [SerializeField] private bool limitBorrowedElements;
        [ShowIf(nameof(limitBorrowedElements))]
        [SerializeField] private int borrowLimit = 100;
        [ShowIf(nameof(limitBorrowedElements))]
        [SerializeField] private bool recallBorrowedElements;

        [SerializeField] private bool initializeOnLoad = true;
        [SerializeField] private int initialElementsInPool = 10;

        #endregion


        #region Fields

        private readonly Queue<T> _pool = new();
        [ReadOnly]
        private readonly List<T> _borrowedElements = new();
        private Index _createIndex;
        [ReadOnly]
        private bool _isInitialized;
        [ReadOnly]
        private Transform _poolTransform;
        private CancellationTokenSource _cancellationTokenSource;

        #endregion


        #region Public API

        [PublicAPI]
        public int CountInactive => _pool.Count;

        [PublicAPI]
        public void Initialize()
        {
            InitializeInternal();
        }

        [PublicAPI]
        public T Get()
        {
            return GetInternal();
        }

        [PublicAPI]
        public PooledObject<T> Get(out T element)
        {
            throw new NotImplementedException();
        }

        [PublicAPI]
        public void Release(T element)
        {
            ReleaseInternal(element);
        }

        [PublicAPI]
        public void Clear()
        {
            ClearInternal();
        }

        [PublicAPI]
        public T Borrow(int milliseconds = 5000)
        {
            return BorrowInternal(TimeSpan.FromMilliseconds(milliseconds));
        }

        [PublicAPI]
        public T Borrow(TimeSpan timeSpan)
        {
            return BorrowInternal(timeSpan);
        }

        [PublicAPI]
        protected virtual void OnGetElementFromPool(T element)
        {
        }

        [PublicAPI]
        protected virtual void OnReleaseElementToPool(T element)
        {
        }

        #endregion


        #region Internal

        private void OnEnable()
        {
            Gameloop.InitializationCompleted += OnInitializationCompleted;
            Gameloop.ApplicationQuit += OnShutdown;
        }

        private void OnInitializationCompleted()
        {
            if (initializeOnLoad)
            {
                InitializeInternal();
            }
        }

        private void OnShutdown()
        {
            _isInitialized = false;
            _poolTransform = null;
            _pool.Clear();
            _borrowedElements.Clear();
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource = new CancellationTokenSource();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void InitializeInternal()
        {
            if (_isInitialized)
            {
                return;
            }
            _poolTransform = PoolTransform.Create(this);
            _createIndex = Index.Create(prefabs);
            _cancellationTokenSource = new CancellationTokenSource();
            _isInitialized = true;

            for (var i = 0; i < initialElementsInPool; i++)
            {
                var element = CreateElement();
                // ReSharper disable once SuspiciousTypeConversion.Global
                if (element is IPoolObject objectCallbacks)
                {
                    objectCallbacks.OnReleaseToPool();
                }
                OnReleaseElementToPool(element);

                _borrowedElements.Remove(element);
                _pool.Enqueue(element);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private T CreateElement()
        {
            Assert.IsTrue(_isInitialized, "Pool is not initialized!");

            var prefab = prefabs[_createIndex++];
            Assert.IsNotNull(prefab, "Prefab for object pool is null!");

            var element = Instantiate(prefab, _poolTransform);
            return element;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private T GetInternal()
        {
            Assert.IsTrue(_isInitialized, "Pool is not initialized!");

            if (_pool.TryDequeue(out var pooledElement))
            {
                Assert.IsNotNull(pooledElement, "Element from the pool is null!");
                if (pooledElement is IPoolObject poolCallbacks)
                {
                    poolCallbacks.OnGetFromPool();
                }
                OnGetElementFromPool(pooledElement);
                _borrowedElements.Add(pooledElement);
                return pooledElement;
            }

            if (limitBorrowedElements && _borrowedElements.Count >= borrowLimit)
            {
                var borrowedElement = _borrowedElements[0];
                _borrowedElements.RemoveAt(0);
                _borrowedElements.Add(borrowedElement);

                if (borrowedElement is IPoolObject objectCallbacks)
                {
                    objectCallbacks.OnReleaseToPool();
                    OnReleaseElementToPool(borrowedElement);
                    objectCallbacks.OnGetFromPool();
                    OnGetElementFromPool(borrowedElement);
                }

                return borrowedElement;
            }
            else
            {
                var element = CreateElement();
                if (element is IPoolObject objectCallbacks)
                {
                    objectCallbacks.OnReleaseToPool();
                    OnReleaseElementToPool(element);
                    objectCallbacks.OnGetFromPool();
                    OnGetElementFromPool(element);
                }
                _borrowedElements.Add(pooledElement);
                return element;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ReleaseInternal(T borrowedObject)
        {
            Assert.IsTrue(_isInitialized, "Pool is not initialized!");
            Assert.IsNotNull(borrowedObject, "Element returned to the pool is null!");

            if (limitPoolSize && _pool.Count >= poolLimit)
            {
                DestroyElement(borrowedObject);
                return;
            }

            if (borrowedObject is IPoolObject objectCallbacks)
            {
                objectCallbacks.OnReleaseToPool();
                OnReleaseElementToPool(borrowedObject);
            }

            _borrowedElements.Remove(borrowedObject);
            _pool.Enqueue(borrowedObject);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected virtual void DestroyElement(T element)
        {
            Assert.IsTrue(_isInitialized, "Pool is not initialized!");
            Assert.IsNotNull(element, "Element to destroy is null!");
            switch (element)
            {
                case GameObject:
                    Destroy(element);
                    break;

                case Component component:
                    Destroy(component.gameObject);
                    break;

                default:
                    Destroy(element);
                    break;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ClearInternal()
        {
            Assert.IsTrue(_isInitialized, "Pool is not initialized!");
            using var borrowed = new Buffer<T>(_borrowedElements);
            foreach (var borrowedObject in borrowed)
            {
                ReleaseInternal(borrowedObject);
            }

            using var pooled = new Buffer<T>(_pool);
            foreach (var pooledObject in pooled)
            {
                DestroyElement(pooledObject);
            }
            _pool.Clear();
            _borrowedElements.Clear();
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource = new CancellationTokenSource();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private T BorrowInternal(in TimeSpan timeSpan)
        {
            var borrowedElement = Get();
            ReleaseAfter(timeSpan, borrowedElement, _cancellationTokenSource.Token).Forget();
            return borrowedElement;
        }

        private async UniTaskVoid ReleaseAfter(TimeSpan timeSpan, T element, CancellationToken cancellationToken)
        {
            await UniTask.Delay(timeSpan, cancellationToken: cancellationToken);
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }
            Release(element);
        }

        #endregion
    }
}