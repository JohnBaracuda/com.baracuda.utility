using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Baracuda.Utility.Types;

namespace Baracuda.Utility.Collections
{
    /// <summary>
    ///     Wrapper for a generic Dictionary that exposes events when an element is added, removed or when the dictionary has
    ///     changed.
    /// </summary>
    public class ObservableDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IReadOnlyDictionary<TKey, TValue>,
        IObservableCollection<KeyValuePair<TKey, TValue>>
    {
        public Broadcast Changed { get; } = new();
        public Broadcast<KeyValuePair<TKey, TValue>> Added { get; } = new();
        public Broadcast<KeyValuePair<TKey, TValue>> Removed { get; } = new();
        public Broadcast<KeyValuePair<TKey, TValue>> Replaced { get; } = new();
        public Broadcast FirstAdded { get; } = new();
        public Broadcast LastRemoved { get; } = new();

        private readonly Dictionary<TKey, TValue> _dictionary = new();

        public bool IsNotEmpty => _dictionary.Count > 0;
        public bool IsEmpty => _dictionary.Count == 0;

        public int Count => _dictionary.Count;
        public bool IsReadOnly => false;

        public ICollection<TKey> Keys => _dictionary.Keys;
        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => _dictionary.Keys;
        public ICollection<TValue> Values => _dictionary.Values;
        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => _dictionary.Values;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(TKey key, TValue value)
        {
            var wasEmpty = Count == 0;
            _dictionary.Add(key, value);
            Added.Raise(new KeyValuePair<TKey, TValue>(key, value));
            if (wasEmpty)
            {
                FirstAdded.Raise();
            }
            Changed.Raise();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Remove(TKey key)
        {
            if (_dictionary.Remove(key, out var value))
            {
                Removed.Raise(new KeyValuePair<TKey, TValue>(key, value));
                Changed.Raise();
                if (Count == 0)
                {
                    LastRemoved.Raise();
                }
                return true;
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ContainsKey(TKey key)
        {
            return _dictionary.ContainsKey(key);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryGetValue(TKey key, out TValue value)
        {
            return _dictionary.TryGetValue(key, out value);
        }

        public TValue this[TKey key]
        {
            get => _dictionary[key];
            set
            {
                var wasEmpty = Count == 0;
                if (!_dictionary.TryAdd(key, value))
                {
                    _dictionary[key] = value;
                    Replaced.Raise(new KeyValuePair<TKey, TValue>(key, value));
                    Changed.Raise();
                }
                else
                {
                    Added.Raise(new KeyValuePair<TKey, TValue>(key, value));
                    if (wasEmpty)
                    {
                        FirstAdded.Raise();
                    }
                    Changed.Raise();
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear()
        {
            var wasNotEmpty = Count > 0;
            _dictionary.Clear();
            Changed.Raise();
            if (wasNotEmpty)
            {
                LastRemoved.Raise();
            }
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
        {
            return _dictionary.TryGetValue(item.Key, out var value) && EqualityComparer<TValue>.Default.Equals(value, item.Value);
        }

        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            ((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).CopyTo(array, arrayIndex);
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        {
            return Remove(item.Key);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddRange(IEnumerable<KeyValuePair<TKey, TValue>> items)
        {
            var wasEmpty = Count == 0;
            foreach (var item in items)
            {
                _dictionary.Add(item.Key, item.Value);
                Added.Raise(item);
            }
            if (wasEmpty && Count > 0)
            {
                FirstAdded.Raise();
            }
            Changed.Raise();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RemoveRange(IEnumerable<TKey> keys)
        {
            foreach (var key in keys)
            {
                if (_dictionary.Remove(key, out var value))
                {
                    Removed.Raise(new KeyValuePair<TKey, TValue>(key, value));
                }
            }
            if (Count == 0)
            {
                LastRemoved.Raise();
            }
            Changed.Raise();
        }

        public void Reset()
        {
            Changed.Clear();
            Added.Clear();
            Removed.Clear();
            Replaced.Clear();
            var wasNotEmpty = Count > 0;
            _dictionary.Clear();
            if (wasNotEmpty)
            {
                LastRemoved.Raise();
            }
        }
    }
}