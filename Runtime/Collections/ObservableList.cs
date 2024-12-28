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
        private readonly List<T> _list = new();

        public Broadcast Changed { get; } = new();
        public Broadcast<T> Added { get; } = new();
        public Broadcast<T> Removed { get; } = new();

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
                Changed.Raise();
            }
        }

        public ObservableList(int capacity)
        {
            _list = new List<T>(capacity);
        }

        public ObservableList(IEnumerable<T> enumerable)
        {
            _list = new List<T>(enumerable);
        }

        public ObservableList()
        {
        }

        public int Count => _list.Count;

        public bool IsReadOnly => false;

        public void Add(T item)
        {
            _list.Add(item);
            Added.RaiseCritical(item);
            Changed.Raise();
        }

        public void AddRange(IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                _list.Add(item);
                Added.RaiseCritical(item);
            }
            Changed.Raise();
        }

        public void Clear()
        {
            _list.Clear();
            Changed.Raise();
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
            Added.RaiseCritical(item);
            Changed.Raise();
        }

        public bool Remove(T item)
        {
            var removed = _list.Remove(item);
            if (removed)
            {
                Removed.RaiseCritical(item);
                Changed.Raise();
            }
            return removed;
        }

        public void RemoveAt(int index)
        {
            var item = _list[index];
            _list.RemoveAt(index);
            Removed.RaiseCritical(item);
            Changed.Raise();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Reset()
        {
            Changed.Clear();
            Added.Clear();
            Removed.Clear();
            _list.Clear();
        }
    }
}