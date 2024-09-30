using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;

namespace Baracuda.Utility.Types
{
    public class Observable<TValue>
    {
        private TValue _value;
        private readonly Broadcast<TValue> _changed = new();

        public void AddObserver(Action<TValue> observer)
        {
            _changed.AddListener(observer);
            observer(_value);
        }

        public void RemoveObserver(Action<TValue> observer)
        {
            _changed.RemoveListener(observer);
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
            _value = value;
            _changed.Raise(value);
        }

        [PublicAPI]
        public bool TryGetValue(out TValue value)
        {
            if (_value is not null)
            {
                value = _value;
                return true;
            }

            value = default;
            return false;
        }

        [PublicAPI]
        public bool Is(TValue other)
        {
            return EqualityComparer<TValue>.Default.Equals(_value, other);
        }

        [PublicAPI]
        public bool HasValue => _value != null;

        public Observable()
        {
        }

        public Observable(TValue value)
        {
            _value = value;
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

        public override string ToString()
        {
            return _value != null ? _value.ToString() : "null";
        }
    }
}