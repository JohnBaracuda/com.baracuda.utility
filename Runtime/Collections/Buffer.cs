using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Pool;

namespace Baracuda.Utilities.Collections
{
    public readonly struct Buffer<T> : IList<T>, IReadOnlyList<T>, IDisposable
    {
        private readonly List<T> _list;

        public Buffer(IEnumerable<T> collection)
        {
            _list = ListPool<T>.Get();
            _list.AddRange(collection);
        }

        public static Buffer<T> Create()
        {
            return new Buffer<T>(Enumerable.Empty<T>());
        }

        public T this[int index]
        {
            get => _list[index];
            set => _list[index] = value;
        }

        public bool IsEmpty => Count <= 0;
        public int Count => _list.Count;

        public bool IsReadOnly => ((IList<T>)_list).IsReadOnly;

        public void Add(T item)
        {
            _list.Add(item);
        }

        public void Clear()
        {
            _list.Clear();
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

        IEnumerator IEnumerable.GetEnumerator()
        {
            // ReSharper disable once NotDisposedResourceIsReturned
            return ((IEnumerable)_list).GetEnumerator();
        }

        public int IndexOf(T item)
        {
            return _list.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            _list.Insert(index, item);
        }

        public bool Remove(T item)
        {
            return _list.Remove(item);
        }

        public void RemoveAt(int index)
        {
            _list.RemoveAt(index);
        }

        public void Dispose()
        {
            ListPool<T>.Release(_list);
        }
    }
}