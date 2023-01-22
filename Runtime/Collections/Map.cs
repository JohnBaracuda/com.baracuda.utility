using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using System;

namespace Baracuda.Utilities.Collections
{
    /// <summary>
    /// Serializable Dictionary
    /// </summary>
    [Serializable]
    public class Map<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        [SerializeField] private List<TKey> keys = new(16);
        [SerializeField] private List<TValue> values = new(16);


        public bool TryGetElementAtIndex(int index, out KeyValuePair<TKey, TValue> result)
        {
            if (keys.Count < index && values.Count < index)
            {
                result = new KeyValuePair<TKey, TValue>(keys[index], values[index]);
                return true;
            }

            result = default;
            return false;
        }

        public bool RemoveElementAtIndex(int index)
        {
            if (keys.Count < index && values.Count < index)
            {
                keys.RemoveAt(index);
                values.RemoveAt(index);
                OnAfterDeserialize();
                return true;
            }

            return false;
        }

        public void OnBeforeSerialize()
        {
            keys.Clear();
            values.Clear();
            foreach (var (key, value) in this)
            {
                keys.Add(key);
                values.Add(value);
            }
        }

        public void OnAfterDeserialize()
        {
            Clear();
            Assert.IsTrue(keys.Count == values.Count, $"there are {keys.Count.ToString()} keys and {values.Count.ToString()} values after deserialization. Make sure that both key and value types are serializable.");

            for (var i = 0; i < keys.Count; i++)
            {
                Add(keys[i], values[i]);
            }
        }
    }
}