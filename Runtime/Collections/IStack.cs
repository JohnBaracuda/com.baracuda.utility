using System.Collections.Generic;

namespace Baracuda.Utilities.Collections
{
    public interface IStack<T> : IEnumerable<T>
    {
        void Push(T item);
        T Pop();
        T Peek();
        bool TryPeek(out T item);
        bool TryPop(out T item);
    }
}