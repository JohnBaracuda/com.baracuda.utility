using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Baracuda.Utility.Types;

namespace Baracuda.Utility.Collections
{
    /// <summary>
    ///     Wrapper for a generic HashSet that exposes events when an element is added, removed or when the set has changed.
    /// </summary>
    public class ObservableHashSet<T> : ISet<T>, IReadOnlyCollection<T>, IObservableCollection<T>
    {
        public event Action Changed
        {
            add
            {
                _changed.AddListener(value);
                value();
            }
            remove => _changed.RemoveListener(value);
        }

        public event Action<T> Added
        {
            add => _added.AddListener(value);
            remove => _added.RemoveListener(value);
        }

        public event Action<T> Removed
        {
            add => _removed.AddListener(value);
            remove => _removed.RemoveListener(value);
        }

        public event Action FirstAdded
        {
            add => _firstAdded.AddListener(value);
            remove => _firstAdded.RemoveListener(value);
        }

        public event Action LastRemoved
        {
            add => _lastRemoved.AddListener(value);
            remove => _lastRemoved.RemoveListener(value);
        }

        private readonly HashSet<T> _hashSet = new();

        private readonly Broadcast _changed = new();
        private readonly Broadcast<T> _added = new();
        private readonly Broadcast<T> _removed = new();
        private readonly Broadcast _firstAdded = new();
        private readonly Broadcast _lastRemoved = new();

        public bool IsNotEmpty => _hashSet.Count > 0;
        public bool IsEmpty => _hashSet.Count <= 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public HashSet<T> GetInternalSet()
        {
            return _hashSet;
        }

        public int Count => _hashSet.Count;

        public bool IsReadOnly => false;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Add(T item)
        {
            var added = _hashSet.Add(item);
            if (added)
            {
                _added.Raise(item);
                if (Count == 1)
                {
                    _firstAdded.Raise();
                }
                _changed.Raise();
            }
            return added;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddOrRemove(T value, bool add)
        {
            if (add)
            {
                Add(value);
            }
            else
            {
                Remove(value);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddRange(IEnumerable<T> range)
        {
            foreach (var item in range)
            {
                if (_hashSet.Add(item))
                {
                    _added.Raise(item);
                    if (Count == 1)
                    {
                        _firstAdded.Raise();
                    }
                }
            }

            _changed.Raise();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RemoveRange(IEnumerable<T> range)
        {
            foreach (var item in range)
            {
                if (_hashSet.Remove(item))
                {
                    _removed.Raise(item);
                    if (Count == 0)
                    {
                        _lastRemoved.Raise();
                    }
                }
            }

            _changed.Raise();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear()
        {
            var wasNotEmpty = IsNotEmpty;
            _hashSet.Clear();
            _changed.Raise();
            if (wasNotEmpty)
            {
                _lastRemoved.Raise();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(T item)
        {
            return _hashSet.Contains(item);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CopyTo(T[] array, int arrayIndex)
        {
            _hashSet.CopyTo(array, arrayIndex);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ExceptWith(IEnumerable<T> other)
        {
            foreach (var item in other)
            {
                if (_hashSet.Remove(item))
                {
                    _removed.Raise(item);
                    if (Count == 0)
                    {
                        _lastRemoved.Raise();
                    }
                }
            }
            _changed.Raise();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _hashSet.GetEnumerator();
        }

        public void IntersectWith(IEnumerable<T> other)
        {
            _hashSet.IntersectWith(other);
            _changed.Raise();
        }

        public bool IsProperSubsetOf(IEnumerable<T> other)
        {
            return _hashSet.IsProperSubsetOf(other);
        }

        public bool IsProperSupersetOf(IEnumerable<T> other)
        {
            return _hashSet.IsProperSupersetOf(other);
        }

        public bool IsSubsetOf(IEnumerable<T> other)
        {
            return _hashSet.IsSubsetOf(other);
        }

        public bool IsSupersetOf(IEnumerable<T> other)
        {
            return _hashSet.IsSupersetOf(other);
        }

        public bool Overlaps(IEnumerable<T> other)
        {
            return _hashSet.Overlaps(other);
        }

        public bool SetEquals(IEnumerable<T> other)
        {
            return _hashSet.SetEquals(other);
        }

        public void SymmetricExceptWith(IEnumerable<T> other)
        {
            _hashSet.SymmetricExceptWith(other);
            _changed.Raise();
        }

        public void UnionWith(IEnumerable<T> other)
        {
            foreach (var item in other)
            {
                if (_hashSet.Add(item))
                {
                    _added.Raise(item);
                    if (Count == 1)
                    {
                        _firstAdded.Raise();
                    }
                }
            }
            _changed.Raise();
        }

        void ICollection<T>.Add(T item)
        {
            Add(item);
        }

        public bool Remove(T item)
        {
            var removed = _hashSet.Remove(item);
            if (removed)
            {
                _removed.Raise(item);
                _changed.Raise();
                if (Count == 0)
                {
                    _lastRemoved.Raise();
                }
            }
            return removed;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Reset()
        {
            _changed.Clear();
            _added.Clear();
            _removed.Clear();
            var isNotEmpty = IsNotEmpty;
            _hashSet.Clear();
            if (isNotEmpty)
            {
                _lastRemoved.Raise();
            }
        }
    }
}