using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Baracuda.Utilities.Collections
{
    public abstract class RuntimeStack<T> : ScriptableObject, IStack<T>
    {
        [NonSerialized] private readonly Stack<T> _stack = new();

        public void Push(T item)
        {
            _stack.Push(item);
        }

        public T Pop()
        {
            return _stack.Pop();
        }

        public T Peek()
        {
            return _stack.Peek();
        }

        public bool TryPeek(out T item)
        {
            return _stack.TryPeek(out item);
        }
        
        public bool TryPop(out T item)
        {
            return _stack.TryPop(out item);
        }


        public IEnumerator<T> GetEnumerator()
        {
            return _stack.GetEnumerator();
        }

        public Stack<T>.Enumerator Enumerator()
        {
            return _stack.GetEnumerator();
        }
        
        IEnumerator IEnumerable.GetEnumerator()
        {
            return Enumerator();
        }
    }
}