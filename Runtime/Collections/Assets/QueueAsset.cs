using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using NaughtyAttributes;

namespace Baracuda.Bedrock.Collections.Assets
{
    /// <summary>
    ///     Collection Asset representing a queue that can be modified during runtime.
    /// </summary>
    public abstract class QueueAsset<T> : RuntimeCollectionAsset<T>, IEnumerable<T>
    {
        [ReadOnly]
        [ShowNonSerializedField]
        [Foldout("Elements")]
        private readonly Queue<T> _queue = new(8);

        /// <summary>Returns an enumerator that iterates through the collection.</summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Queue<T>.Enumerator GetEnumerator()
        {
            return _queue.GetEnumerator();
        }

        /// <summary>Returns an enumerator that iterates through the collection.</summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return _queue.GetEnumerator();
        }

        /// <summary>Returns an enumerator that iterates through a collection.</summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _queue.GetEnumerator();
        }

        /// <summary>Adds an object to the end of the <see cref="T:System.Collections.Generic.Queue`1" />.</summary>
        /// <param name="item">
        ///     The object to add to the <see cref="T:System.Collections.Generic.Queue`1" />. The value can be
        ///     <see langword="null" /> for reference types.
        /// </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Enqueue(T item)
        {
            _queue.Enqueue(item);
        }

        /// <summary>Removes and returns the object at the beginning of the <see cref="T:System.Collections.Generic.Queue`1" />.</summary>
        /// <returns>The object that is removed from the beginning of the <see cref="T:System.Collections.Generic.Queue`1" />.</returns>
        /// <exception cref="T:System.InvalidOperationException">The <see cref="T:System.Collections.Generic.Queue`1" /> is empty.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Dequeue()
        {
            var element = _queue.Dequeue();
            return element;
        }

        /// <summary>
        ///     Returns the object at the beginning of the <see cref="T:System.Collections.Generic.Queue`1" /> without
        ///     removing it.
        /// </summary>
        /// <returns>The object at the beginning of the <see cref="T:System.Collections.Generic.Queue`1" />.</returns>
        /// <exception cref="T:System.InvalidOperationException">The <see cref="T:System.Collections.Generic.Queue`1" /> is empty.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Peek()
        {
            return _queue.Peek();
        }

        /// <summary>Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1" />.</summary>
        /// <exception cref="T:System.NotSupportedException">
        ///     The <see cref="T:System.Collections.Generic.ICollection`1" /> is
        ///     read-only.
        /// </exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear()
        {
            _queue.Clear();
        }

        /// <summary>Determines whether the <see cref="T:System.Collections.Generic.ICollection`1" /> contains a specific value.</summary>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
        /// <returns>
        ///     <see langword="true" /> if <paramref name="item" /> is found in the
        ///     <see cref="T:System.Collections.Generic.ICollection`1" />; otherwise, <see langword="false" />.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(T item)
        {
            return _queue.Contains(item);
        }

        /// <summary>Adds the elements of the specified collection to the queue />.</summary>
        /// <param name="collection">
        ///     The collection whose elements should be added to the queue. The collection itself cannot be
        ///     <see langword="null" />, but it can contain elements that are <see langword="null" />, if type T is a reference
        ///     type.
        /// </param>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="collection" /> is <see langword="null" />.
        /// </exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void EnqueueRange([NotNull] IEnumerable<T> collection)
        {
            foreach (var element in collection)
            {
                _queue.Enqueue(element);
            }
        }

        /// <summary>
        ///     Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1" /> to an
        ///     <see cref="T:System.Array" />, starting at a particular <see cref="T:System.Array" /> index.
        /// </summary>
        /// <param name="array">
        ///     The one-dimensional <see cref="T:System.Array" /> that is the destination of the elements copied
        ///     from <see cref="T:System.Collections.Generic.ICollection`1" />. The <see cref="T:System.Array" /> must have
        ///     zero-based indexing.
        /// </param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array" /> at which copying begins.</param>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="array" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        ///     <paramref name="arrayIndex" /> is less than 0.
        /// </exception>
        /// <exception cref="T:System.ArgumentException">
        ///     The number of elements in the source
        ///     <see cref="T:System.Collections.Generic.ICollection`1" /> is greater than the available space from
        ///     <paramref name="arrayIndex" /> to the end of the destination <paramref name="array" />.
        /// </exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CopyTo(T[] array, int arrayIndex)
        {
            _queue.CopyTo(array, arrayIndex);
        }

        /// <summary>Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1" />.</summary>
        /// <returns>The number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1" />.</returns>
        public int Count
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _queue.Count;
        }

        /// <summary>Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only.</summary>
        /// <returns>
        ///     <see langword="true" /> if the <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only; otherwise,
        ///     <see langword="false" />.
        /// </returns>
        public bool IsReadOnly
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => false;
        }

        /// <summary>
        ///     Internal call to get the underlying collection.
        /// </summary>
        private protected sealed override IEnumerable<T> CollectionInternal
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _queue;
        }

        /// <summary>
        ///     Internal call to clear the underlying collection.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private protected sealed override void ClearInternal()
        {
            Clear();
        }

        /// <summary>
        ///     Internal call to get the count of the underlying collection.
        /// </summary>
        private protected sealed override int CountInternal
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Count;
        }
    }
}