using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Baracuda.Utilities.Collections
{
    public sealed class StackList<T> : IEnumerable<T>
    {
        /// <summary>Returns an enumerator that iterates through the collection.</summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public List<T>.Enumerator GetEnumerator()
        {
            return List.GetEnumerator();
        }

        /// <summary>Returns an enumerator that iterates through the collection.</summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return List.GetEnumerator();
        }

        /// <summary>Returns an enumerator that iterates through a collection.</summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IEnumerator IEnumerable.GetEnumerator()
        {
            return List.GetEnumerator();
        }

        /// <summary>Inserts an object at the top of the <see cref="T:System.Collections.Generic.Stack`1" />.</summary>
        /// <param name="item">
        ///     The object to push onto the <see cref="T:System.Collections.Generic.Stack`1" />. The value can be
        ///     <see langword="null" /> for reference types.
        /// </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Push(T item)
        {
            List.Add(item);
        }

        /// <summary>Inserts an object at the top of the stack and ensure that it is only contained once in the stack</summary>
        /// <param name="item">
        ///     The object to push onto the <see cref="T:System.Collections.Generic.Stack`1" />. The value can be
        ///     <see langword="null" /> for reference types.
        /// </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void PushUnique(T item)
        {
            Remove(item);
            List.Add(item);
        }

        /// <summary>Removes and returns the object at the top of the <see cref="T:System.Collections.Generic.Stack`1" />.</summary>
        /// <returns>The object removed from the top of the <see cref="T:System.Collections.Generic.Stack`1" />.</returns>
        /// <exception cref="T:System.InvalidOperationException">The <see cref="T:System.Collections.Generic.Stack`1" /> is empty.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Pop()
        {
            if (List.Count <= 0)
            {
                return default(T);
            }

            var index = List.Count - 1;
            var result = List[index];
            List.RemoveAt(index);
            return result;
        }

        /// <summary>Returns the object at the top of the <see cref="T:System.Collections.Generic.Stack`1" /> without removing it.</summary>
        /// <returns>The object at the top of the <see cref="T:System.Collections.Generic.Stack`1" />.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Peek()
        {
            if (List.Count <= 0)
            {
                return default(T);
            }

            var index = List.Count - 1;
            var result = List[index];
            return result;
        }

        /// <summary>Returns the object at the root of the <see cref="T:System.Collections.Generic.Stack`1" /> without removing it.</summary>
        /// <returns>The object at the root of the <see cref="T:System.Collections.Generic.Stack`1" />.</returns>
        public T Root()
        {
            if (List.Count <= 0)
            {
                return default(T);
            }

            return List[0];
        }

        /// <summary>
        ///     Removes the item from the stack if it is contained. The item does not need to be at the top of the stack.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Remove(T item)
        {
            var result = List.Remove(item);
            return result;
        }

        /// <summary>Adds the elements of the specified collection to the stack />.</summary>
        /// <param name="collection">
        ///     The collection whose elements should be added to the stack. The collection itself cannot be
        ///     <see langword="null" />, but it can contain elements that are <see langword="null" />, if type T is a reference
        ///     type.
        /// </param>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="collection" /> is <see langword="null" />.
        /// </exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void PushRange([NotNull] IEnumerable<T> collection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }

            foreach (var element in collection)
            {
                List.Add(element);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryPeek(out T item)
        {
            if (List.Count > 0)
            {
                item = Peek();
                return true;
            }
            item = default(T);
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryPop(out T item)
        {
            if (List.Count > 0)
            {
                item = Pop();
                return true;
            }
            item = default(T);
            return false;
        }

        /// <summary>Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1" />.</summary>
        /// <exception cref="T:System.NotSupportedException">
        ///     The <see cref="T:System.Collections.Generic.ICollection`1" /> is
        ///     read-only.
        /// </exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear()
        {
            List.Clear();
        }

        /// <summary>Determines whether the <see cref="T:System.Collections.Generic.ICollection`1" /> contains a specific value.</summary>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
        /// <returns>
        ///     <see langword="true" /> if <paramref name="item" /> is found in the
        ///     <see cref="T:System.Collections.Generic.ICollection`1" />; otherwise, <see langword="false" />.
        /// </returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(T item)
        {
            return List.Contains(item);
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
            List.CopyTo(array, arrayIndex);
        }

        /// <summary>Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1" />.</summary>
        /// <returns>The number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1" />.</returns>
        public int Count
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => List.Count;
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

        /// <summary>Gets or sets the element at the specified index.</summary>
        /// <param name="index">The zero-based index of the element to get or set.</param>
        /// <returns>The element at the specified index.</returns>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        ///     <paramref name="index" /> is not a valid index in the <see cref="T:System.Collections.Generic.IList`1" />.
        /// </exception>
        /// <exception cref="T:System.NotSupportedException">
        ///     The property is set and the
        ///     <see cref="T:System.Collections.Generic.IList`1" /> is read-only.
        /// </exception>
        public T this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => List[index];

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => List[index] = value;
        }

        /// <summary>
        ///     The wrapped list object.
        /// </summary>
        public List<T> List { get; } = new(8);
    }
}