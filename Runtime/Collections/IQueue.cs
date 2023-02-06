using System.Collections.Generic;

namespace Baracuda.Utilities.Collections
{
    public interface IQueue<T> : IEnumerable<T>, IReadOnlyCollection<T>
    {
        /// <summary>Adds an object to the end of the <see cref="T:System.Collections.Generic.Queue`1" />.</summary>
        /// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.Queue`1" />. The value can be <see langword="null" /> for reference types.</param>
        void Enqueue(T item);

        /// <summary>Removes and returns the object at the beginning of the <see cref="T:System.Collections.Generic.Queue`1" />.</summary>
        /// <returns>The object that is removed from the beginning of the <see cref="T:System.Collections.Generic.Queue`1" />.</returns>
        /// <exception cref="T:System.InvalidOperationException">The <see cref="T:System.Collections.Generic.Queue`1" /> is empty.</exception>
        T Dequeue();

        /// <summary>Returns the object at the beginning of the <see cref="T:System.Collections.Generic.Queue`1" /> without removing it.</summary>
        /// <returns>The object at the beginning of the <see cref="T:System.Collections.Generic.Queue`1" />.</returns>
        /// <exception cref="T:System.InvalidOperationException">The <see cref="T:System.Collections.Generic.Queue`1" /> is empty.</exception>
        T Peek();
    }
}