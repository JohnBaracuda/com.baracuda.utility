using System;
using System.Collections;
using System.Collections.Generic;
using Baracuda.Utilities.Events;

namespace Baracuda.Utilities.Collections
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
                _changed.Add(value);
                value();
            }
            remove => _changed.Remove(value);
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

        private readonly HashSet<T> _set = new();
        private readonly Broadcast _changed = new();
        private readonly Broadcast<T> _added = new();
        private readonly Broadcast<T> _removed = new();

        public bool IsNotEmpty => _set.Count > 0;
        public bool IsEmpty => _set.Count <= 0;

        public HashSet<T> GetInternalSet()
        {
            return _set;
        }

        public int Count => _set.Count;

        public bool IsReadOnly => false;

        public bool Add(T item)
        {
            var added = _set.Add(item);
            if (added)
            {
                _added.Raise(item);
                _changed.Raise();
            }
            return added;
        }

        public void AddRange(IEnumerable<T> range)
        {
            foreach (var item in range)
            {
                Add(item);
            }
        }

        public void RemoveRange(IEnumerable<T> range)
        {
            foreach (var item in range)
            {
                Remove(item);
            }
        }

        public void Clear()
        {
            _set.Clear();
            _changed.Raise();
        }

        public bool Contains(T item)
        {
            return _set.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _set.CopyTo(array, arrayIndex);
        }

        public void ExceptWith(IEnumerable<T> other)
        {
            foreach (var item in other)
            {
                if (_set.Remove(item))
                {
                    _removed.Raise(item);
                }
            }
            _changed.Raise();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _set.GetEnumerator();
        }

        public void IntersectWith(IEnumerable<T> other)
        {
            _set.IntersectWith(other);
            _changed.Raise();
        }

        public bool IsProperSubsetOf(IEnumerable<T> other)
        {
            return _set.IsProperSubsetOf(other);
        }

        public bool IsProperSupersetOf(IEnumerable<T> other)
        {
            return _set.IsProperSupersetOf(other);
        }

        public bool IsSubsetOf(IEnumerable<T> other)
        {
            return _set.IsSubsetOf(other);
        }

        public bool IsSupersetOf(IEnumerable<T> other)
        {
            return _set.IsSupersetOf(other);
        }

        public bool Overlaps(IEnumerable<T> other)
        {
            return _set.Overlaps(other);
        }

        public bool SetEquals(IEnumerable<T> other)
        {
            return _set.SetEquals(other);
        }

        public void SymmetricExceptWith(IEnumerable<T> other)
        {
            _set.SymmetricExceptWith(other);
            _changed.Raise();
        }

        public void UnionWith(IEnumerable<T> other)
        {
            foreach (var item in other)
            {
                if (_set.Add(item))
                {
                    _added.Raise(item);
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
            var removed = _set.Remove(item);
            if (removed)
            {
                _removed.Raise(item);
                _changed.Raise();
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
            _set.Clear();
        }
    }
}