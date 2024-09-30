using System;
using System.Collections;
using System.Collections.Generic;
using Baracuda.Utility.Types;

namespace Baracuda.Utility.Collections
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

        private readonly List<T> _list = new();
        private readonly Broadcast _changed = new();
        private readonly Broadcast<T> _added = new();
        private readonly Broadcast<T> _removed = new();

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
                _changed.Raise();
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
            _added.RaiseCritical(item);
            _changed.Raise();
        }

        public void Clear()
        {
            _list.Clear();
            _changed.Raise();
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
            _added.RaiseCritical(item);
            _changed.Raise();
        }

        public bool Remove(T item)
        {
            var removed = _list.Remove(item);
            if (removed)
            {
                _removed.RaiseCritical(item);
                _changed.Raise();
            }
            return removed;
        }

        public void RemoveAt(int index)
        {
            var item = _list[index];
            _list.RemoveAt(index);
            _removed.RaiseCritical(item);
            _changed.Raise();
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
            _list.Clear();
        }
    }
}