using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Baracuda.Utilities.Collections
{
    /// <summary>
    /// Class acts as a limited and fast generic first in last out collection.
    /// </summary>
    public sealed class LimitedQueue<T> : IEnumerable<T>
    {
        /*
         *  Data
         */
        
        private readonly List<T> _data;
        private int _index;
        private readonly int _maxCapacity;

        public int Capacity => _maxCapacity;
        public int Count => _data.Count - _index;
        public int Range { get; private set; }

        /*
         *  Ctor   
         */
        
        public LimitedQueue(int max)
        {
            _maxCapacity = Mathf.Max(1, max);
            _data = new List<T>(_maxCapacity);
        }

        //--------------------------------------------------------------------------------------------------------------
        
        public T this[int i]
        {
            get
            {
                var index = _index + i;
                if (index >= _data.Count)
                {
                    throw new ArgumentException($"queue index {index} > last index {_data.Count - 1}");
                }

                if (index < 0)
                {
                    throw new ArgumentException($"queue index {index} < 0");
                }

                if (index > Range)
                {
                    Range = index;
                }

                return _data[index];
            }
        }

        public T Dequeue()
        {
            if (Count == 0)
            {
                throw new InvalidOperationException();
            }

            var obj = this[0];
            ++_index;
            if (_index != _data.Count)
            {
                return obj;
            }

            Clear();
            return obj;
        }

        public void Enqueue(T item)
        {
            _data.Add(item);
            if (_data.Count > _maxCapacity)
            {
                Dequeue();
            }
        }

        public T Peek()
        {
            return this[0];
        }

        public void Clear()
        {
            _index = 0;
            _data.Clear();
        }
        
        public override string ToString()
        {
            var stringBuilder = new StringBuilder();
            var count = Count;
            for (var i = 0; i < count; ++i)
            {
                stringBuilder.Append(this[i]);
                if (i + 1 < count)
                {
                    stringBuilder.Append(" ");
                }
            }

            return stringBuilder.ToString();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _data.GetEnumerator();
        }
        
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}