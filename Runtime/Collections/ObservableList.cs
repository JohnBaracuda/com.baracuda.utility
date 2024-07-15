using System;
using System.Collections;
using System.Collections.Generic;

namespace Baracuda.Utilities.Collections
{
    /// <summary>
    ///     Wrapper for a generic List that exposes events when an element is added, removed or when the list has changed.
    /// </summary>
    public class ObservableList<T> : IList<T>, IReadOnlyList<T>, IObservableCollection<T>
    {
        public event Action Changed
        {
            add
            {
                _changed += value;
                value();
            }
            remove => _changed -= value;
        }
        public event Action<T> Added
        {
            add => _added += value;
            remove => _added -= value;
        }
        public event Action<T> Removed
        {
            add => _removed += value;
            remove => _removed -= value;
        }

        private readonly List<T> _list = new();
        private Action _changed;
        private Action<T> _added;
        private Action<T> _removed;

        public List<T> GetInternalList()
        {
            return _list;
        }

        public T this[int index]
        {
            get => _list[index];
            set
            {
                _list[index] = value;
                _changed?.Invoke();
            }
        }

        public ObservableList(int capacity)
        {
            _list = new List<T>(capacity);
        }

        public ObservableList()
        {
        }

        public int Count => _list.Count;

        public bool IsReadOnly => false;

        public void Add(T item)
        {
            _list.Add(item);
            _added?.Invoke(item);
            _changed?.Invoke();
        }

        public void Clear()
        {
            _list.Clear();
            _changed?.Invoke();
        }

        public bool Contains(T item)
        {
            return _list.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        public int IndexOf(T item)
        {
            return _list.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            _list.Insert(index, item);
            _added?.Invoke(item);
            _changed?.Invoke();
        }

        public bool Remove(T item)
        {
            var removed = _list.Remove(item);
            if (removed)
            {
                _removed?.Invoke(item);
                _changed?.Invoke();
            }
            return removed;
        }

        public void RemoveAt(int index)
        {
            var item = _list[index];
            _list.RemoveAt(index);
            _removed?.Invoke(item);
            _changed?.Invoke();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}