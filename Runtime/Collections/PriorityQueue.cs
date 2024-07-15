using System;
using System.Collections;
using System.Collections.Generic;

namespace Baracuda.Utilities.Collections
{
    public sealed class PriorityQueue<T> : IEnumerable<T>, IReadOnlyCollection<T>
    {
        private readonly List<T> _elements;
        private readonly List<int> _priorities;

        public PriorityQueue(int capacity = 4)
        {
            _elements = new List<T>(capacity);
            _priorities = new List<int>(capacity);
        }

        public int Count => _elements.Count;

        public bool IsEmpty => Count <= 0;

        public void Enqueue(T item, int priority = 0)
        {
            _elements.Add(item);
            _priorities.Add(priority);

            var childIndex = _elements.Count - 1;

            // Move the new item up based on priority
            while (childIndex > 0)
            {
                var parentIndex = (childIndex - 1) / 2;

                if (_priorities[childIndex] <= _priorities[parentIndex])
                {
                    break;
                }

                Swap(childIndex, parentIndex);
                childIndex = parentIndex;
            }
        }

        public T Dequeue()
        {
            if (_elements.Count == 0)
            {
                throw new InvalidOperationException("The queue is empty.");
            }

            // Remove the element with the highest priority (top of the list)
            var result = _elements[0];
            _elements.RemoveAt(0);
            _priorities.RemoveAt(0);

            return result;
        }

        public T Peek()
        {
            if (_elements.Count == 0)
            {
                throw new InvalidOperationException("The queue is empty.");
            }

            return _elements[0];
        }

        public bool TryDequeue(out T element)
        {
            if (Count > 0)
            {
                element = Dequeue();
                return true;
            }

            element = default;
            return false;
        }

        private void Swap(int index1, int index2)
        {
            (_elements[index1], _elements[index2]) = (_elements[index2], _elements[index1]);
            (_priorities[index1], _priorities[index2]) = (_priorities[index2], _priorities[index1]);
        }

        public IEnumerator<T> GetEnumerator()
        {
            foreach (var element in _elements)
            {
                yield return element;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}