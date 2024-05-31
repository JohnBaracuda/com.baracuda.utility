using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Baracuda.Utilities.Collections
{
    /// <summary>
    ///     Class acts as a limited and fast generic first in last out collection.
    /// </summary>
    public sealed class LimitedQueue<T> : IEnumerable<T>, IReadOnlyCollection<T>
    {
        private readonly Queue<T> _queue;

        public int Capacity { get; }
        public int Count => _queue.Count;

        public LimitedQueue(int max)
        {
            Capacity = Mathf.Max(1, max);
            _queue = new Queue<T>(Capacity);
        }

        public T Dequeue()
        {
            return _queue.Dequeue();
        }

        public void Enqueue(T item)
        {
            _queue.Enqueue(item);
            if (_queue.Count > Capacity)
            {
                _queue.Dequeue();
            }
        }

        public T Peek()
        {
            return _queue.Peek();
        }

        public void Clear()
        {
            _queue.Clear();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _queue.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}