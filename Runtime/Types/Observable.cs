using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;

namespace Baracuda.Bedrock.Types
{
    public class Observable<TValue>
    {
        private TValue _value;
        private readonly Broadcast<TValue> _changed = new();

        public event Action<TValue> Changed
        {
            add
            {
                _changed.AddListener(value);
                value(_value);
            }
            remove => _changed.RemoveListener(value);
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
            if (_value != null)
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
    }
}