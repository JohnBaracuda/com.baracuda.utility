using System;
using System.Collections;
using System.Collections.Generic;

namespace Baracuda.Utilities.Collections
{
    public sealed class OrderedList<T> : IEnumerable<T>
    {
        private readonly List<(T item, int order)> _items;
        private readonly Comparison<(T item, int order)> _comparison =
            (rhs, lhs) => rhs.order > lhs.order ? -1 : lhs.order > rhs.order ? 1 : 0;

        public OrderedList(int capacity = 32)
        {
            _items = new List<(T item, int order)>(capacity);
        }

        public void Add(T item, int order = 0)
        {
            _items.Add((item, order));
            _items.Sort(_comparison);
        }

        public void AddRange(IEnumerable<T> range, int order)
        {
            foreach (var element in range)
            {
                _items.Add((element, order));
            }
        }

        public void Remove(T item)
        {
            
        }
        
        public T this[int index] => _items[index].item;
        
        public IEnumerator<T> GetEnumerator()
        {
            foreach (var (item, order) in _items)
            {
                yield return item;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}