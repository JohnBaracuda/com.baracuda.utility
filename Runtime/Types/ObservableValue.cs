using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;

namespace Baracuda.Utility.Types
{
    public class ObservableValue<TValue> : IObservableValue<TValue>
    {
        private TValue _value;
        private readonly Broadcast<TValue> _changed = new();
        private readonly Broadcast<TValue, TValue> _changedFrom = new();
        private readonly IEqualityComparer<TValue> _comparer = EqualityComparer<TValue>.Default;

        [PublicAPI]
        public void AddObserver(Action<TValue> observer)
        {
            _changed.AddListener(observer);
            observer(_value);
        }

        [PublicAPI]
        public void AddObserver(Action<TValue, TValue> observer)
        {
            _changedFrom.AddListener(observer);
            observer(default, _value);
        }

        [PublicAPI]
        public void RemoveObserver(Action<TValue> observer)
        {
            _changed.RemoveListener(observer);
        }

        [PublicAPI]
        public void RemoveObserver(Action<TValue, TValue> observer)
        {
            _changedFrom.RemoveListener(observer);
        }

        [PublicAPI]
        public void AddListener(Action<TValue> observer)
        {
            _changed.AddListener(observer);
        }

        [PublicAPI]
        public void AddListener(Action<TValue, TValue> observer)
        {
            _changedFrom.AddListener(observer);
        }

        [PublicAPI]
        public void RemoveListener(Action<TValue> observer)
        {
            _changed.RemoveListener(observer);
        }

        [PublicAPI]
        public void RemoveListener(Action<TValue, TValue> observer)
        {
            _changedFrom.RemoveListener(observer);
        }

        [PublicAPI]
        public TValue Value
        {
            get => GetValue();
            set => SetValue(value);
        }

        [PublicAPI]
        public TValue GetValue()
        {
            return _value;
        }

        [PublicAPI]
        public void SetValue(TValue value)
        {
            if (Is(value))
            {
                return;
            }
            var lastValue = _value;
            _value = value;
            _changed.Raise(value);
            _changedFrom.Raise(lastValue, value);
        }

        [PublicAPI]
        public void SetValue<T>(T value) where T : MonoBehaviour, TValue
        {
            if (Is(value))
            {
                return;
            }
            var lastValue = _value;
            _value = value;
            _changed.Raise(value);
            _changedFrom.Raise(lastValue, value);
        }

        /// <summary>
        ///     Set the value to its default if the passed value equals the current value.
        /// </summary>
        [PublicAPI]
        public void UnsetValue(TValue value)
        {
            if (Is(value))
            {
                SetValue(default);
            }
        }

        [PublicAPI]
        public bool TryGetValue(out TValue value)
        {
            if (_value != null)
            {
                value = _value;
                return true;
            }

            value = default;
            return false;
        }

        [PublicAPI]
        public bool TryGetValue<T>(out T value) where T : MonoBehaviour, TValue
        {
            if (_value != null)
            {
                value = (T)_value;
                return true;
            }

            value = default;
            return false;
        }

        [PublicAPI]
        public bool Is(TValue other)
        {
            return _comparer.Equals(_value, other);
        }

        [PublicAPI]
        public bool Is<T>(T other) where T : MonoBehaviour, TValue
        {
            return (T)_value == other;
        }

        [PublicAPI]
        public bool HasValue => _value != null;

        public ObservableValue()
        {
        }

        public ObservableValue(TValue value)
        {
            _value = value;
        }

        public ObservableValue(IEqualityComparer<TValue> comparer)
        {
            _comparer = comparer;
        }

        public ObservableValue(TValue value, IEqualityComparer<TValue> comparer)
        {
            _value = value;
            _comparer = comparer;
        }

        [PublicAPI]
        public void Clear()
        {
            _value = default;
            _changed.Clear();
        }

        [PublicAPI]
        public async UniTask<TValue> GetAsync()
        {
            while (_value is null)
            {
                await UniTask.Yield();
            }
            return _value;
        }

        [PublicAPI]
        public override string ToString()
        {
            return _value != null ? _value.ToString() : "null";
        }
    }
}