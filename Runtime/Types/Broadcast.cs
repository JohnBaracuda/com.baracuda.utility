using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using UnityEngine.Assertions;

namespace Baracuda.Bedrock.Types
{
    /// <summary>
    ///     A class that allows broadcasting events to multiple listeners.
    /// </summary>
    public class Broadcast
    {
        #region Member Variables

        /// <summary>
        ///     Gets the current number of listeners.
        /// </summary>
        public int Count { get; private set; }

        /// <summary>
        ///     Gets a read-only collection of the current listeners.
        /// </summary>
        public IReadOnlyCollection<Action> GetListenerCollection => _listener.Take(Count).ToList();

        /// <summary>
        ///     Gets the listener at the specified index.
        /// </summary>
        /// <param name="index">The index of the listener.</param>
        /// <returns>The listener at the specified index.</returns>
        public Action this[int index] => _listener[index];

        private Action[] _listener;

        #endregion


        #region Ctor

        /// <summary>
        ///     Initializes a new instance of the <see cref="Broadcast" /> class with a specified initial capacity.
        /// </summary>
        /// <param name="initialCapacity">The initial capacity of the listener array.</param>
        public Broadcast(int initialCapacity)
        {
            _listener = new Action[initialCapacity];
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Broadcast" /> class with a default initial capacity of 8.
        /// </summary>
        public Broadcast()
        {
            _listener = new Action[8];
        }

        #endregion


        #region Add Listener

        /// <summary>
        ///     Adds a listener if it is not already present in the list.
        /// </summary>
        /// <param name="listener">The listener to add.</param>
        /// <returns>True if the listener was added, false if it was already present.</returns>
        [PublicAPI]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool AddUnique(Action listener)
        {
            if (Contains(listener))
            {
                return false;
            }

            AddListener(listener);
            return true;
        }

        /// <summary>
        ///     Adds a listener to the list.
        /// </summary>
        /// <param name="listener">The listener to add.</param>
        [PublicAPI]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddListener(Action listener)
        {
            Assert.IsNotNull(listener);

            if (_listener.Length <= Count)
            {
                IncreaseCapacity();
            }

            _listener[Count] = listener;
            Count++;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void IncreaseCapacity()
        {
            var increasedArr = new Action[_listener.Length * 2];

            for (var i = 0; i < _listener.Length; i++)
            {
                increasedArr[i] = _listener[i];
            }

            _listener = increasedArr;
        }

        /// <summary>
        ///     Checks if the specified listener is in the list.
        /// </summary>
        /// <param name="listener">The listener to check for.</param>
        /// <returns>True if the listener is in the list, false otherwise.</returns>
        [PublicAPI]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(Action listener)
        {
            for (var i = 0; i < Count; i++)
            {
                if (_listener[i].Equals(listener))
                {
                    return true;
                }
            }

            return false;
        }

        #endregion


        #region Remove Listener

        /// <summary>
        ///     Removes the specified listener from the list.
        /// </summary>
        /// <param name="listener">The listener to remove.</param>
        /// <returns>True if the listener was removed, false if it was not found.</returns>
        [PublicAPI]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool RemoveListener(Action listener)
        {
            for (var i = 0; i < Count; i++)
            {
                if (_listener[i] == listener)
                {
                    RemoveAt(i);
                    return true;
                }
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void RemoveAt(int index)
        {
            --Count;

            for (var i = index; i < Count; ++i)
            {
                _listener[i] = _listener[i + 1];
            }

            _listener[Count] = null;
        }

        /// <summary>
        ///     Removes all listeners from the list.
        /// </summary>
        [PublicAPI]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear()
        {
            Count = 0;

            for (var i = _listener.Length - 1; i >= 0; i--)
            {
                _listener[i] = null;
            }
        }

        /// <summary>
        ///     Removes listeners that are no longer valid (null or with a null target).
        /// </summary>
        [PublicAPI]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ClearInvalid()
        {
            for (var i = Count - 1; i >= 0; i--)
            {
                if (_listener[i] is null || _listener[i].Target == null)
                {
                    RemoveAt(i);
                }
            }
        }

        #endregion


        #region Raise

        /// <summary>
        ///     Invokes all listeners in the list.
        /// </summary>
        [PublicAPI]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Raise()
        {
            for (var i = Count - 1; i >= 0; i--)
            {
                _listener[i]();
            }
        }

        /// <summary>
        ///     Invokes all listeners and logs any exceptions thrown by them.
        /// </summary>
        [PublicAPI]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RaiseCritical()
        {
            for (var i = Count - 1; i >= 0; i--)
            {
                try
                {
                    _listener[i]();
                }
                catch (Exception exception)
                {
                    Debug.LogException("Event", exception);
                }
            }
        }

        #endregion
    }

    /// <summary>
    ///     A class that allows broadcasting events with a single argument to multiple listeners.
    /// </summary>
    /// <typeparam name="T">The type of the argument passed to listeners.</typeparam>
    public class Broadcast<T>
    {
        #region Member Variables

        /// <summary>
        ///     Gets the listener at the specified index.
        /// </summary>
        /// <param name="index">The index of the listener.</param>
        /// <returns>The listener at the specified index.</returns>
        public Action<T> this[int index] => _listener[index];

        /// <summary>
        ///     Gets the current number of listeners.
        /// </summary>
        public int Count { get; private set; }

        /// <summary>
        ///     Gets a read-only collection of the current listeners.
        /// </summary>
        public IReadOnlyCollection<Action<T>> GetListenerCollection => _listener.Take(Count).ToList();

        private Action<T>[] _listener;

        #endregion


        #region Ctor

        /// <summary>
        ///     Initializes a new instance of the <see cref="Broadcast{T}" /> class with a specified initial capacity.
        /// </summary>
        /// <param name="initialCapacity">The initial capacity of the listener array.</param>
        public Broadcast(int initialCapacity)
        {
            _listener = new Action<T>[initialCapacity];
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Broadcast{T}" /> class with a default initial capacity of 8.
        /// </summary>
        public Broadcast()
        {
            _listener = new Action<T>[8];
        }

        #endregion


        #region Add Listener

        /// <summary>
        ///     Adds a listener if it is not already present in the list.
        /// </summary>
        /// <param name="listener">The listener to add.</param>
        /// <returns>True if the listener was added, false if it was already present.</returns>
        [PublicAPI]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool AddUnique(Action<T> listener)
        {
            if (Contains(listener))
            {
                return false;
            }

            AddListener(listener);
            return true;
        }

        /// <summary>
        ///     Adds a listener to the list.
        /// </summary>
        /// <param name="listener">The listener to add.</param>
        [PublicAPI]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddListener(Action<T> listener)
        {
            if (_listener.Length <= Count)
            {
                IncreaseCapacity();
            }

            _listener[Count] = listener;
            Count++;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void IncreaseCapacity()
        {
            var increasedArr = new Action<T>[_listener.Length * 2];

            for (var index = 0; index < _listener.Length; index++)
            {
                increasedArr[index] = _listener[index];
            }

            _listener = increasedArr;
        }

        /// <summary>
        ///     Checks if the specified listener is in the list.
        /// </summary>
        /// <param name="listener">The listener to check for.</param>
        /// <returns>True if the listener is in the list, false otherwise.</returns>
        [PublicAPI]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(Action<T> listener)
        {
            for (var i = 0; i < Count; i++)
            {
                if (_listener[i].Equals(listener))
                {
                    return true;
                }
            }

            return false;
        }

        #endregion


        #region Remove Listener

        /// <summary>
        ///     Removes the specified listener from the list.
        /// </summary>
        /// <param name="listener">The listener to remove.</param>
        /// <returns>True if the listener was removed, false if it was not found.</returns>
        [PublicAPI]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool RemoveListener(Action<T> listener)
        {
            for (var i = 0; i < Count; i++)
            {
                if (_listener[i].Equals(listener))
                {
                    RemoveAt(i);
                    return true;
                }
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void RemoveAt(int index)
        {
            --Count;

            for (var i = index; i < Count; ++i)
            {
                _listener[i] = _listener[i + 1];
            }

            _listener[Count] = null;
        }

        /// <summary>
        ///     Removes all listeners from the list.
        /// </summary>
        [PublicAPI]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear()
        {
            Count = 0;

            for (var i = _listener.Length - 1; i >= 0; i--)
            {
                _listener[i] = null;
            }
        }

        /// <summary>
        ///     Removes listeners that are no longer valid (null or with a null target).
        /// </summary>
        [PublicAPI]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ClearInvalid()
        {
            for (var i = Count - 1; i >= 0; i--)
            {
                if (_listener[i] is null || _listener[i].Target == null)
                {
                    RemoveAt(i);
                }
            }
        }

        #endregion


        #region Raise

        /// <summary>
        ///     Invokes all listeners with the specified argument.
        /// </summary>
        /// <param name="arg">The argument to pass to the listeners.</param>
        [PublicAPI]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Raise(T arg)
        {
            for (var i = Count - 1; i >= 0; i--)
            {
                _listener[i](arg);
            }
        }

        /// <summary>
        ///     Invokes all listeners with the specified argument and logs any exceptions thrown by them.
        /// </summary>
        /// <param name="value">The argument to pass to the listeners.</param>
        [PublicAPI]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RaiseCritical(T value)
        {
            for (var i = Count - 1; i >= 0; i--)
            {
                try
                {
                    _listener[i](value);
                }
                catch (Exception exception)
                {
                    Debug.LogException("Event", exception);
                }
            }
        }

        #endregion
    }

    /// <summary>
    ///     A class that allows broadcasting events with two arguments to multiple listeners.
    /// </summary>
    /// <typeparam name="T1">The type of the first argument passed to listeners.</typeparam>
    /// <typeparam name="T2">The type of the second argument passed to listeners.</typeparam>
    public class Broadcast<T1, T2>
    {
        #region Member Variables

        /// <summary>
        ///     Gets the listener at the specified index.
        /// </summary>
        /// <param name="index">The index of the listener.</param>
        /// <returns>The listener at the specified index.</returns>
        public Action<T1, T2> this[int index] => _listener[index];

        /// <summary>
        ///     Gets the current number of listeners.
        /// </summary>
        public int Count { get; private set; }

        /// <summary>
        ///     Gets a read-only collection of the current listeners.
        /// </summary>
        public IReadOnlyCollection<Action<T1, T2>> GetListenerCollection => _listener.Take(Count).ToList();

        private Action<T1, T2>[] _listener;

        #endregion


        #region Ctor

        /// <summary>
        ///     Initializes a new instance of the <see cref="Broadcast{T1, T2}" /> class with a specified initial capacity.
        /// </summary>
        /// <param name="initialCapacity">The initial capacity of the listener array.</param>
        public Broadcast(int initialCapacity)
        {
            _listener = new Action<T1, T2>[initialCapacity];
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Broadcast{T1, T2}" /> class with a default initial capacity of 8.
        /// </summary>
        public Broadcast()
        {
            _listener = new Action<T1, T2>[8];
        }

        #endregion


        #region Add Listener

        /// <summary>
        ///     Adds a listener if it is not already present in the list.
        /// </summary>
        /// <param name="listener">The listener to add.</param>
        /// <returns>True if the listener was added, false if it was already present.</returns>
        [PublicAPI]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool AddUnique(Action<T1, T2> listener)
        {
            if (Contains(listener))
            {
                return false;
            }

            Add(listener);
            return true;
        }

        /// <summary>
        ///     Adds a listener to the list.
        /// </summary>
        /// <param name="listener">The listener to add.</param>
        [PublicAPI]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(Action<T1, T2> listener)
        {
            Assert.IsNotNull(listener);

            if (_listener.Length <= Count)
            {
                IncreaseCapacity();
            }

            _listener[Count] = listener;
            Count++;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void IncreaseCapacity()
        {
            var increasedArr = new Action<T1, T2>[_listener.Length * 2];

            for (var i = 0; i < _listener.Length; i++)
            {
                increasedArr[i] = _listener[i];
            }

            _listener = increasedArr;
        }

        /// <summary>
        ///     Checks if the specified listener is in the list.
        /// </summary>
        /// <param name="listener">The listener to check for.</param>
        /// <returns>True if the listener is in the list, false otherwise.</returns>
        [PublicAPI]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(Action<T1, T2> listener)
        {
            for (var i = 0; i < Count; i++)
            {
                if (_listener[i].Equals(listener))
                {
                    return true;
                }
            }

            return false;
        }

        #endregion


        #region Remove Listener

        /// <summary>
        ///     Removes the specified listener from the list.
        /// </summary>
        /// <param name="listener">The listener to remove.</param>
        /// <returns>True if the listener was removed, false if it was not found.</returns>
        [PublicAPI]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Remove(Action<T1, T2> listener)
        {
            for (var i = 0; i < Count; i++)
            {
                if (_listener[i].Equals(listener))
                {
                    RemoveAt(i);
                    return true;
                }
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void RemoveAt(int index)
        {
            --Count;

            for (var i = index; i < Count; ++i)
            {
                _listener[i] = _listener[i + 1];
            }

            _listener[Count] = null;
        }

        /// <summary>
        ///     Removes all listeners from the list.
        /// </summary>
        [PublicAPI]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear()
        {
            Count = 0;

            for (var i = _listener.Length - 1; i >= 0; i--)
            {
                _listener[i] = null;
            }
        }

        /// <summary>
        ///     Removes listeners that are no longer valid (null or with a null target).
        /// </summary>
        [PublicAPI]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ClearInvalid()
        {
            for (var i = Count - 1; i >= 0; i--)
            {
                if (_listener[i] is null || _listener[i].Target == null)
                {
                    RemoveAt(i);
                }
            }
        }

        #endregion


        #region Raise

        /// <summary>
        ///     Invokes all listeners with the specified arguments.
        /// </summary>
        /// <param name="first">The first argument to pass to the listeners.</param>
        /// <param name="second">The second argument to pass to the listeners.</param>
        [PublicAPI]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Raise(T1 first, T2 second)
        {
            for (var i = Count - 1; i >= 0; i--)
            {
                _listener[i](first, second);
            }
        }

        /// <summary>
        ///     Invokes all listeners with the specified arguments and logs any exceptions thrown by them.
        /// </summary>
        /// <param name="arg1">The first argument to pass to the listeners.</param>
        /// <param name="arg2">The second argument to pass to the listeners.</param>
        [PublicAPI]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RaiseCritical(T1 arg1, T2 arg2)
        {
            for (var i = Count - 1; i >= 0; i--)
            {
                try
                {
                    _listener[i](arg1, arg2);
                }
                catch (Exception exception)
                {
                    Debug.LogException("Event", exception);
                }
            }
        }

        #endregion
    }

    /// <summary>
    ///     A class that allows broadcasting events with three arguments to multiple listeners.
    /// </summary>
    /// <typeparam name="T1">The type of the first argument passed to listeners.</typeparam>
    /// <typeparam name="T2">The type of the second argument passed to listeners.</typeparam>
    /// <typeparam name="T3">The type of the third argument passed to listeners.</typeparam>
    public class Broadcast<T1, T2, T3>
    {
        #region Member Variables

        /// <summary>
        ///     Gets the listener at the specified index.
        /// </summary>
        /// <param name="index">The index of the listener.</param>
        /// <returns>The listener at the specified index.</returns>
        public Action<T1, T2, T3> this[int index] => _listener[index];

        /// <summary>
        ///     Gets the current number of listeners.
        /// </summary>
        public int Count { get; private set; }

        /// <summary>
        ///     Gets a read-only collection of the current listeners.
        /// </summary>
        public IReadOnlyCollection<Action<T1, T2, T3>> GetListenerCollection => _listener.Take(Count).ToList();

        private Action<T1, T2, T3>[] _listener;

        #endregion


        #region Ctor

        /// <summary>
        ///     Initializes a new instance of the <see cref="Broadcast{T1, T2, T3}" /> class with a specified initial capacity.
        /// </summary>
        /// <param name="initialCapacity">The initial capacity of the listener array.</param>
        public Broadcast(int initialCapacity)
        {
            _listener = new Action<T1, T2, T3>[initialCapacity];
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Broadcast{T1, T2, T3}" /> class with a default initial capacity of 8.
        /// </summary>
        public Broadcast()
        {
            _listener = new Action<T1, T2, T3>[8];
        }

        #endregion


        #region Add Listener

        /// <summary>
        ///     Adds a listener if it is not already present in the list.
        /// </summary>
        /// <param name="listener">The listener to add.</param>
        /// <returns>True if the listener was added, false if it was already present.</returns>
        [PublicAPI]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool AddUnique(Action<T1, T2, T3> listener)
        {
            if (Contains(listener))
            {
                return false;
            }

            Add(listener);
            return true;
        }

        /// <summary>
        ///     Adds a listener to the list.
        /// </summary>
        /// <param name="listener">The listener to add.</param>
        [PublicAPI]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(Action<T1, T2, T3> listener)
        {
            Assert.IsNotNull(listener);

            if (_listener.Length <= Count)
            {
                IncreaseCapacity();
            }

            _listener[Count] = listener;
            Count++;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void IncreaseCapacity()
        {
            var increasedArr = new Action<T1, T2, T3>[_listener.Length * 2];

            for (var i = 0; i < _listener.Length; i++)
            {
                increasedArr[i] = _listener[i];
            }

            _listener = increasedArr;
        }

        /// <summary>
        ///     Checks if the specified listener is in the list.
        /// </summary>
        /// <param name="listener">The listener to check for.</param>
        /// <returns>True if the listener is in the list, false otherwise.</returns>
        [PublicAPI]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(Action<T1, T2, T3> listener)
        {
            for (var i = 0; i < Count; i++)
            {
                if (_listener[i].Equals(listener))
                {
                    return true;
                }
            }

            return false;
        }

        #endregion


        #region Remove Listener

        /// <summary>
        ///     Removes the specified listener from the list.
        /// </summary>
        /// <param name="listener">The listener to remove.</param>
        /// <returns>True if the listener was removed, false if it was not found.</returns>
        [PublicAPI]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Remove(Action<T1, T2, T3> listener)
        {
            for (var i = 0; i < Count; i++)
            {
                if (_listener[i].Equals(listener))
                {
                    RemoveAt(i);
                    return true;
                }
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void RemoveAt(int index)
        {
            --Count;

            for (var i = index; i < Count; ++i)
            {
                _listener[i] = _listener[i + 1];
            }

            _listener[Count] = null;
        }

        /// <summary>
        ///     Removes all listeners from the list.
        /// </summary>
        [PublicAPI]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear()
        {
            Count = 0;

            for (var i = _listener.Length - 1; i >= 0; i--)
            {
                _listener[i] = null;
            }
        }

        /// <summary>
        ///     Removes listeners that are no longer valid (null or with a null target).
        /// </summary>
        [PublicAPI]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ClearInvalid()
        {
            for (var i = Count - 1; i >= 0; i--)
            {
                if (_listener[i] is null || _listener[i].Target == null)
                {
                    RemoveAt(i);
                }
            }
        }

        #endregion


        #region Raise

        /// <summary>
        ///     Invokes all listeners with the specified arguments.
        /// </summary>
        /// <param name="first">The first argument to pass to the listeners.</param>
        /// <param name="second">The second argument to pass to the listeners.</param>
        /// <param name="third">The third argument to pass to the listeners.</param>
        [PublicAPI]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Raise(T1 first, T2 second, T3 third)
        {
            for (var i = Count - 1; i >= 0; i--)
            {
                _listener[i](first, second, third);
            }
        }

        /// <summary>
        ///     Invokes all listeners with the specified arguments and logs any exceptions thrown by them.
        /// </summary>
        /// <param name="arg1">The first argument to pass to the listeners.</param>
        /// <param name="arg2">The second argument to pass to the listeners.</param>
        /// <param name="arg3">The third argument to pass to the listeners.</param>
        [PublicAPI]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RaiseCritical(T1 arg1, T2 arg2, T3 arg3)
        {
            for (var i = Count - 1; i >= 0; i--)
            {
                try
                {
                    _listener[i](arg1, arg2, arg3);
                }
                catch (Exception exception)
                {
                    Debug.LogException("Event", exception);
                }
            }
        }

        #endregion
    }

    /// <summary>
    ///     A class that allows broadcasting events with four arguments to multiple listeners.
    /// </summary>
    /// <typeparam name="T1">The type of the first argument passed to listeners.</typeparam>
    /// <typeparam name="T2">The type of the second argument passed to listeners.</typeparam>
    /// <typeparam name="T3">The type of the third argument passed to listeners.</typeparam>
    /// <typeparam name="T4">The type of the fourth argument passed to listeners.</typeparam>
    public class Broadcast<T1, T2, T3, T4>
    {
        #region Member Variables

        /// <summary>
        ///     Gets the listener at the specified index.
        /// </summary>
        /// <param name="index">The index of the listener.</param>
        /// <returns>The listener at the specified index.</returns>
        public Action<T1, T2, T3, T4> this[int index] => _listener[index];

        /// <summary>
        ///     Gets the current number of listeners.
        /// </summary>
        public int Count { get; private set; }

        /// <summary>
        ///     Gets a read-only collection of the current listeners.
        /// </summary>
        public IReadOnlyCollection<Action<T1, T2, T3, T4>> GetListenerCollection => _listener.Take(Count).ToList();

        private Action<T1, T2, T3, T4>[] _listener;

        #endregion


        #region Ctor

        /// <summary>
        ///     Initializes a new instance of the <see cref="Broadcast{T1, T2, T3, T4}" /> class with a specified initial capacity.
        /// </summary>
        /// <param name="initialCapacity">The initial capacity of the listener array.</param>
        public Broadcast(int initialCapacity)
        {
            _listener = new Action<T1, T2, T3, T4>[initialCapacity];
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Broadcast{T1, T2, T3, T4}" /> class with a default initial capacity of
        ///     8.
        /// </summary>
        public Broadcast()
        {
            _listener = new Action<T1, T2, T3, T4>[8];
        }

        #endregion


        #region Add Listener

        /// <summary>
        ///     Adds a listener if it is not already present in the list.
        /// </summary>
        /// <param name="listener">The listener to add.</param>
        /// <returns>True if the listener was added, false if it was already present.</returns>
        [PublicAPI]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool AddUnique(Action<T1, T2, T3, T4> listener)
        {
            if (Contains(listener))
            {
                return false;
            }

            Add(listener);
            return true;
        }

        /// <summary>
        ///     Adds a listener to the list.
        /// </summary>
        /// <param name="listener">The listener to add.</param>
        [PublicAPI]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(Action<T1, T2, T3, T4> listener)
        {
            Assert.IsNotNull(listener);

            if (_listener.Length <= Count)
            {
                IncreaseCapacity();
            }

            _listener[Count] = listener;
            Count++;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void IncreaseCapacity()
        {
            var increasedArr = new Action<T1, T2, T3, T4>[_listener.Length * 2];

            for (var i = 0; i < _listener.Length; i++)
            {
                increasedArr[i] = _listener[i];
            }

            _listener = increasedArr;
        }

        /// <summary>
        ///     Checks if the specified listener is in the list.
        /// </summary>
        /// <param name="listener">The listener to check for.</param>
        /// <returns>True if the listener is in the list, false otherwise.</returns>
        [PublicAPI]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(Action<T1, T2, T3, T4> listener)
        {
            for (var i = 0; i < Count; i++)
            {
                if (_listener[i].Equals(listener))
                {
                    return true;
                }
            }

            return false;
        }

        #endregion


        #region Remove Listener

        /// <summary>
        ///     Removes the specified listener from the list.
        /// </summary>
        /// <param name="listener">The listener to remove.</param>
        /// <returns>True if the listener was removed, false if it was not found.</returns>
        [PublicAPI]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Remove(Action<T1, T2, T3, T4> listener)
        {
            for (var i = 0; i < Count; i++)
            {
                if (_listener[i].Equals(listener))
                {
                    RemoveAt(i);
                    return true;
                }
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void RemoveAt(int index)
        {
            --Count;

            for (var i = index; i < Count; ++i)
            {
                _listener[i] = _listener[i + 1];
            }

            _listener[Count] = null;
        }

        /// <summary>
        ///     Removes all listeners from the list.
        /// </summary>
        [PublicAPI]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear()
        {
            Count = 0;

            for (var i = _listener.Length - 1; i >= 0; i--)
            {
                _listener[i] = null;
            }
        }

        /// <summary>
        ///     Removes listeners that are no longer valid (null or with a null target).
        /// </summary>
        [PublicAPI]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ClearInvalid()
        {
            for (var i = Count - 1; i >= 0; i--)
            {
                if (_listener[i] is null || _listener[i].Target == null)
                {
                    RemoveAt(i);
                }
            }
        }

        #endregion


        #region Raise

        /// <summary>
        ///     Invokes all listeners with the specified arguments.
        /// </summary>
        /// <param name="first">The first argument to pass to the listeners.</param>
        /// <param name="second">The second argument to pass to the listeners.</param>
        /// <param name="third">The third argument to pass to the listeners.</param>
        /// <param name="fourth">The fourth argument to pass to the listeners.</param>
        [PublicAPI]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Raise(T1 first, T2 second, T3 third, T4 fourth)
        {
            for (var i = Count - 1; i >= 0; i--)
            {
                _listener[i](first, second, third, fourth);
            }
        }

        /// <summary>
        ///     Invokes all listeners with the specified arguments and logs any exceptions thrown by them.
        /// </summary>
        /// <param name="arg1">The first argument to pass to the listeners.</param>
        /// <param name="arg2">The second argument to pass to the listeners.</param>
        /// <param name="arg3">The third argument to pass to the listeners.</param>
        /// <param name="arg4">The fourth argument to pass to the listeners.</param>
        [PublicAPI]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RaiseCritical(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            for (var i = Count - 1; i >= 0; i--)
            {
                try
                {
                    _listener[i](arg1, arg2, arg3, arg4);
                }
                catch (Exception exception)
                {
                    Debug.LogException("Event", exception);
                }
            }
        }

        #endregion
    }
}