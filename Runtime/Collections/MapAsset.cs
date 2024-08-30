using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Baracuda.Bedrock.Collections
{
    /// <summary>
    ///     Collection Asset representing a serializable dictionary that cannot be modified during runtime.
    /// </summary>
    public abstract class MapAsset<TKey, TValue> : ScriptableObject, IReadOnlyDictionary<TKey, TValue>
    {
        [SerializeField] private Map<TKey, TValue> map;

        /// <summary>Returns an enumerator that iterates through the collection.</summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Dictionary<TKey, TValue>.Enumerator GetEnumerator()
        {
            return map.GetEnumerator();
        }

        /// <summary>Returns an enumerator that iterates through the collection.</summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
        {
            return map.GetEnumerator();
        }

        /// <summary>Returns an enumerator that iterates through a collection.</summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IEnumerator IEnumerable.GetEnumerator()
        {
            return map.GetEnumerator();
        }

        /// <summary>Gets the number of elements in the collection.</summary>
        /// <returns>The number of elements in the collection.</returns>
        public int Count
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => map.Count;
        }

        public bool IsReadOnly { get; } = false;

        /// <summary>Determines whether the read-only dictionary contains an element that has the specified key.</summary>
        /// <param name="key">The key to locate.</param>
        /// <returns>
        ///     <see langword="true" /> if the read-only dictionary contains an element that has the specified key; otherwise,
        ///     <see langword="false" />.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="key" /> is <see langword="null" />.
        /// </exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ContainsKey(TKey key)
        {
            return map.ContainsKey(key);
        }

        /// <summary>Gets the value that is associated with the specified key.</summary>
        /// <param name="key">The key to locate.</param>
        /// <param name="value">
        ///     When this method returns, the value associated with the specified key, if the key is found;
        ///     otherwise, the default value for the type of the <paramref name="value" /> parameter. This parameter is passed
        ///     uninitialized.
        /// </param>
        /// <returns>
        ///     <see langword="true" /> if the object that implements the
        ///     <see cref="T:System.Collections.Generic.IReadOnlyDictionary`2" /> interface contains an element that has the
        ///     specified key; otherwise, <see langword="false" />.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="key" /> is <see langword="null" />.
        /// </exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryGetValue(TKey key, out TValue value)
        {
            return map.TryGetValue(key, out value);
        }

        /// <summary>Gets the element that has the specified key in the read-only dictionary.</summary>
        /// <param name="key">The key to locate.</param>
        /// <returns>The element that has the specified key in the read-only dictionary.</returns>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="key" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="T:System.Collections.Generic.KeyNotFoundException">
        ///     The property is retrieved and
        ///     <paramref name="key" /> is not found.
        /// </exception>
        public TValue this[TKey key]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => map[key];
        }

        /// <summary>Gets an enumerable collection that contains the keys in the read-only dictionary.</summary>
        /// <returns>An enumerable collection that contains the keys in the read-only dictionary.</returns>
        public IEnumerable<TKey> Keys
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => map.Keys;
        }

        /// <summary>Gets an enumerable collection that contains the values in the read-only dictionary.</summary>
        /// <returns>An enumerable collection that contains the values in the read-only dictionary.</returns>
        public IEnumerable<TValue> Values
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => map.Values;
        }

        /// <summary>
        ///     AddSingleton an element to the map (Editor Only!)
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        [Conditional("UNITY_EDITOR")]
        protected void Add(TKey key, TValue value)
        {
            map.Add(key, value);
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            Debug.LogError("MapAsset", "AddSingleton(KeyValuePair<TKey, TValue> item) is not supported!");
        }
    }
}