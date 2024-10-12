using System;
using System.Collections;
using System.Collections.Generic;

namespace Baracuda.Utility.Collections
{
    public class ListQueue<T> : ICollection<T>
    {
        private readonly List<T> _list = new();

        // Enqueue: Add an element to the end of the queue
        public void Enqueue(T item)
        {
            _list.Add(item);
        }

        public T DequeueOrDefault(T defaultValue = default)
        {
            return TryDequeue(out var result) ? result : defaultValue;
        }

        // Dequeue: Remove and return the element at the front of the queue
        public T Dequeue()
        {
            if (_list.Count == 0)
            {
                throw new InvalidOperationException("Queue is empty.");
            }

            var value = _list[0];
            _list.RemoveAt(0);
            return value;
        }

        // Peek: Return the element at the front of the queue without removing it
        public T Peek()
        {
            if (_list.Count == 0)
            {
                throw new InvalidOperationException("Queue is empty.");
            }

            return _list[0];
        }

        // PeekOrDefault: Return the element at the front of the queue or default value if empty
        public T PeekOrDefault()
        {
            return _list.Count > 0 ? _list[0] : default;
        }

        // TryPeek: Try to get the element at the front of the queue without removing it
        public bool TryPeek(out T result)
        {
            if (_list.Count > 0)
            {
                result = _list[0];
                return true;
            }

            result = default;
            return false;
        }

        // TryDequeue: Try to remove and return the element at the front of the queue
        public bool TryDequeue(out T result)
        {
            if (_list.Count > 0)
            {
                result = _list[0];
                _list.RemoveAt(0);
                return true;
            }

            result = default;
            return false;
        }

        // Remove: Remove a specific element from the queue
        public bool Remove(T item)
        {
            return _list.Remove(item);
        }

        // ICollection<T> implementation
        public int Count => _list.Count;

        public bool IsReadOnly => false;

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
            return GetEnumerator();
        }

        public void Add(T item)
        {
            Enqueue(item);
        }
    }
}