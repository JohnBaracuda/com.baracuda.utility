using System;
using System.Collections;
using System.Collections.Generic;

namespace Baracuda.Utility.Collections
{
    public class DictionaryQueue<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
    {
        public Queue<TKey> Queue { get; }

        public Dictionary<TKey, TValue> Dictionary { get; }

        public ICollection<TValue> Values => Dictionary.Values;

        public DictionaryQueue(int capacity = 4)
        {
            Queue = new Queue<TKey>(capacity);
            Dictionary = new Dictionary<TKey, TValue>(capacity);
        }

        public void Clear()
        {
            Queue.Clear();
            Dictionary.Clear();
        }

        public void Enqueue(TKey key, TValue value)
        {
            if (Dictionary.ContainsKey(key))
            {
                throw new ArgumentException("An element with the same key already exists in the DictionaryQueue.");
            }

            Queue.Enqueue(key);
            Dictionary.Add(key, value);
        }

        public bool TryPeek(out TValue value)
        {
            if (Queue.Count == 0)
            {
                value = default;
                return false;
            }

            var key = Queue.Peek();
            value = Dictionary[key];
            return true;
        }

        public bool TryPeekKey(out TKey key)
        {
            if (Queue.Count == 0)
            {
                key = default;
                return false;
            }

            key = Queue.Peek();
            return true;
        }

        public TKey PeekKey()
        {
            return Queue.Peek();
        }

        public bool TryDequeue(out TValue value)
        {
            if (Queue.Count == 0)
            {
                value = default;
                return false;
            }

            var key = Queue.Dequeue();
            value = Dictionary[key];
            Dictionary.Remove(key);
            return true;
        }

        public TValue Dequeue()
        {
            if (Queue.Count == 0)
            {
                throw new InvalidOperationException("The DictionaryQueue is empty.");
            }

            var key = Queue.Dequeue();
            var value = Dictionary[key];
            Dictionary.Remove(key);
            return value;
        }

        public void UpdateElement(TKey key, TValue item)
        {
            if (!Dictionary.ContainsKey(key))
            {
                throw new KeyNotFoundException("The specified key does not exist in the DictionaryQueue.");
            }

            Dictionary[key] = item;
        }

        public int Count => Queue.Count;

        public bool ContainsKey(TKey key)
        {
            return Dictionary.ContainsKey(key);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return Dictionary.TryGetValue(key, out value);
        }

        public void Update(TKey key, TValue value)
        {
            if (Dictionary.ContainsKey(key))
            {
                Dictionary[key] = value;
                return;
            }

            Queue.Enqueue(key);
            Dictionary.Add(key, value);
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return Dictionary.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}