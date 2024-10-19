using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Baracuda.Utility.Collections
{
    /// <summary>
    ///     A queue-like data structure that uses a <see cref="List{T}" /> as its internal storage.
    ///     Provides enqueue, dequeue, and peek functionality.
    /// </summary>
    /// <typeparam name="T">The type of elements stored in the queue.</typeparam>
    [PublicAPI]
    public class ListQueue<T> : ICollection<T>
    {
        private readonly List<T> _list = new();

        /// <summary>
        ///     Enqueues an element to the end of the queue.
        /// </summary>
        /// <param name="item">The element to add.</param>
        [PublicAPI]
        public void Enqueue(T item)
        {
            _list.Add(item);
        }

        /// <summary>
        ///     Attempts to dequeue the first element of the queue. If the queue is empty, returns the provided default value.
        /// </summary>
        /// <param name="defaultValue">The default value to return if the queue is empty.</param>
        /// <returns>The first element in the queue or the default value.</returns>
        [PublicAPI]
        public T DequeueOrDefault(T defaultValue = default)
        {
            return TryDequeue(out var result) ? result : defaultValue;
        }

        /// <summary>
        ///     Dequeues the first element in the queue, removing it from the queue.
        ///     Throws an exception if the queue is empty.
        /// </summary>
        /// <returns>The first element in the queue.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the queue is empty.</exception>
        [PublicAPI]
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

        /// <summary>
        ///     Returns the first element in the queue without removing it.
        ///     Throws an exception if the queue is empty.
        /// </summary>
        /// <returns>The first element in the queue.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the queue is empty.</exception>
        [PublicAPI]
        public T Peek()
        {
            if (_list.Count == 0)
            {
                throw new InvalidOperationException("Queue is empty.");
            }

            return _list[0];
        }

        /// <summary>
        ///     Returns the last element in the queue without removing it.
        ///     Throws an exception if the queue is empty.
        /// </summary>
        /// <returns>The last element in the queue.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the queue is empty.</exception>
        [PublicAPI]
        public T PeekLast()
        {
            if (_list.Count == 0)
            {
                throw new InvalidOperationException("Queue is empty.");
            }

            return _list[^1];
        }

        /// <summary>
        ///     Returns the last element in the queue or a default value if the queue is empty.
        /// </summary>
        /// <param name="defaultValue">The default value to return if the queue is empty.</param>
        /// <returns>The last element in the queue or the default value.</returns>
        [PublicAPI]
        public T PeekLastOrDefault(T defaultValue = default)
        {
            return _list.Count > 0 ? _list[^1] : defaultValue;
        }

        /// <summary>
        ///     Returns the first element in the queue or a default value if the queue is empty.
        /// </summary>
        /// <param name="defaultValue">The default value to return if the queue is empty.</param>
        /// <returns>The first element in the queue or the default value.</returns>
        [PublicAPI]
        public T PeekOrDefault(T defaultValue = default)
        {
            return _list.Count > 0 ? _list[0] : defaultValue;
        }

        /// <summary>
        ///     Attempts to return the first element in the queue without removing it.
        /// </summary>
        /// <param name="result">The first element in the queue, if any.</param>
        /// <returns><c>true</c> if the queue contains elements; otherwise, <c>false</c>.</returns>
        [PublicAPI]
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

        /// <summary>
        ///     Attempts to dequeue the first element of the queue.
        /// </summary>
        /// <param name="result">The first element in the queue, if any.</param>
        /// <returns><c>true</c> if the queue contains elements; otherwise, <c>false</c>.</returns>
        [PublicAPI]
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

        /// <summary>
        ///     Removes a specific element from the queue.
        /// </summary>
        /// <param name="item">The element to remove.</param>
        /// <returns><c>true</c> if the element was removed; otherwise, <c>false</c>.</returns>
        [PublicAPI]
        public bool Remove(T item)
        {
            return _list.Remove(item);
        }

        /// <summary>
        ///     Gets the number of elements in the queue.
        /// </summary>
        [PublicAPI]
        public int Count => _list.Count;

        /// <summary>
        ///     Gets a value indicating whether the queue is read-only.
        /// </summary>
        [PublicAPI]
        public bool IsReadOnly => false;

        /// <summary>
        ///     Clears all elements from the queue.
        /// </summary>
        [PublicAPI]
        public void Clear()
        {
            _list.Clear();
        }

        /// <summary>
        ///     Determines whether the queue contains a specific element.
        /// </summary>
        /// <param name="item">The element to locate.</param>
        /// <returns><c>true</c> if the queue contains the element; otherwise, <c>false</c>.</returns>
        [PublicAPI]
        public bool Contains(T item)
        {
            return _list.Contains(item);
        }

        /// <summary>
        ///     Copies the elements of the queue to an array, starting at the specified array index.
        /// </summary>
        /// <param name="array">The array to copy the elements to.</param>
        /// <param name="arrayIndex">The zero-based index in the array at which to begin copying.</param>
        [PublicAPI]
        public void CopyTo(T[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }

        /// <summary>
        ///     Returns an enumerator that iterates through the queue.
        /// </summary>
        /// <returns>An enumerator for the queue.</returns>
        [PublicAPI]
        public IEnumerator<T> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        ///     Adds an element to the end of the queue.
        ///     Equivalent to <see cref="Enqueue" />.
        /// </summary>
        /// <param name="item">The element to add.</param>
        [PublicAPI]
        public void Add(T item)
        {
            Enqueue(item);
        }
    }
}