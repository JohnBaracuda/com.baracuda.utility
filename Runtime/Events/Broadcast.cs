using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine.Assertions;

namespace Baracuda.Utilities.Events
{
    public class Broadcast
    {
        #region Member Variables

        public int Count { get; private set; }

        public IReadOnlyCollection<Action> GetListenerCollection => _listener.Take(Count).ToList();

        public Action this[int index] => _listener[index];

        private Action[] _listener;

        #endregion


        #region Ctor

        public Broadcast(int initialCapacity)
        {
            _listener = new Action[initialCapacity];
        }

        public Broadcast()
        {
            _listener = new Action[8];
        }

        #endregion


        #region AddSingleton Listener

        /// <inheritdoc />
        public bool AddUnique(Action listener)
        {
            if (Contains(listener))
            {
                return false;
            }

            Add(listener);
            return true;
        }

        /// <inheritdoc />
        public void Add(Action listener)
        {
            Assert.IsNotNull(listener);

            if (_listener.Length <= Count)
            {
                IncreaseCapacity();
            }

            _listener[Count] = listener;

            Count++;
        }

        private void IncreaseCapacity()
        {
            var increasedArr = new Action[_listener.Length * 2];

            for (var i = 0; i < _listener.Length; i++)
            {
                increasedArr[i] = _listener[i];
            }

            _listener = increasedArr;
        }

        /// <inheritdoc />
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

        /// <inheritdoc />
        public bool Remove(Action listener)
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

        private void RemoveAt(int index)
        {
            --Count;

            for (var i = index; i < Count; ++i)
            {
                _listener[i] = _listener[i + 1];
            }

            _listener[Count] = null;
        }

        /// <inheritdoc />
        public void Clear()
        {
            Count = 0;

            for (var i = _listener.Length - 1; i >= 0; i--)
            {
                _listener[i] = null;
            }
        }

        /// <inheritdoc />
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Raise()
        {
            for (var i = Count - 1; i >= 0; i--)
            {
                _listener[i]();
            }
        }

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

    public class Broadcast<T>
    {
        #region Member Variables

        public Action<T> this[int index] => _listener[index];

        public int Count { get; private set; }

        public IReadOnlyCollection<Action<T>> GetListenerCollection => _listener.Take(Count).ToList();

        private Action<T>[] _listener;

        #endregion


        #region Ctor

        public Broadcast(int initialCapacity)
        {
            _listener = new Action<T>[initialCapacity];
        }

        public Broadcast()
        {
            _listener = new Action<T>[8];
        }

        #endregion


        #region AddSingleton Listener

        /// <inheritdoc />
        public bool AddUnique(Action<T> listener)
        {
            if (Contains(listener))
            {
                return false;
            }

            AddListener(listener);
            return true;
        }

        /// <inheritdoc />
        public void AddListener(Action<T> listener)
        {
            if (_listener.Length <= Count)
            {
                IncreaseCapacity();
            }

            _listener[Count] = listener;

            Count++;
        }

        private void IncreaseCapacity()
        {
            var increasedArr = new Action<T>[_listener.Length * 2];

            for (var i = 0; i < _listener.Length; i++)
            {
                increasedArr[i] = _listener[i];
            }

            _listener = increasedArr;
        }

        /// <inheritdoc />
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

        /// <inheritdoc />
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

        private void RemoveAt(int index)
        {
            --Count;

            for (var i = index; i < Count; ++i)
            {
                _listener[i] = _listener[i + 1];
            }

            _listener[Count] = null;
        }

        /// <inheritdoc />
        public void Clear()
        {
            Count = 0;

            for (var i = _listener.Length - 1; i >= 0; i--)
            {
                _listener[i] = null;
            }
        }

        /// <inheritdoc />
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Raise(T arg)
        {
            for (var i = Count - 1; i >= 0; i--)
            {
                _listener[i](arg);
            }
        }

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

    public class Broadcast<T1, T2>
    {
        #region Member Variables

        public Action<T1, T2> this[int index] => _listener[index];

        public int Count { get; private set; }
        public IReadOnlyCollection<Action<T1, T2>> GetListenerCollection => _listener.Take(Count).ToList();
        private Action<T1, T2>[] _listener;

        #endregion


        #region Ctor

        public Broadcast(int initialCapacity)
        {
            _listener = new Action<T1, T2>[initialCapacity];
        }

        public Broadcast()
        {
            _listener = new Action<T1, T2>[8];
        }

        #endregion


        #region AddSingleton Listener

        /// <inheritdoc />
        public bool AddUnique(Action<T1, T2> listener)
        {
            if (Contains(listener))
            {
                return false;
            }

            Add(listener);
            return true;
        }

        /// <inheritdoc />
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

        private void IncreaseCapacity()
        {
            var increasedArr = new Action<T1, T2>[_listener.Length * 2];

            for (var i = 0; i < _listener.Length; i++)
            {
                increasedArr[i] = _listener[i];
            }

            _listener = increasedArr;
        }

        /// <inheritdoc />
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

        /// <inheritdoc />
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

        private void RemoveAt(int index)
        {
            --Count;

            for (var i = index; i < Count; ++i)
            {
                _listener[i] = _listener[i + 1];
            }

            _listener[Count] = null;
        }

        /// <inheritdoc />
        public void Clear()
        {
            Count = 0;

            for (var i = _listener.Length - 1; i >= 0; i--)
            {
                _listener[i] = null;
            }
        }

        /// <inheritdoc />
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Raise(T1 first, T2 second)
        {
            for (var i = Count - 1; i >= 0; i--)
            {
                _listener[i](first, second);
            }
        }

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

    public class Broadcast<T1, T2, T3>
    {
        #region Member Variables

        public Action<T1, T2, T3> this[int index] => _listener[index];

        public int Count { get; private set; }
        public IReadOnlyCollection<Action<T1, T2, T3>> GetListenerCollection => _listener.Take(Count).ToList();
        private Action<T1, T2, T3>[] _listener;

        #endregion


        #region Ctor

        public Broadcast(int initialCapacity)
        {
            _listener = new Action<T1, T2, T3>[initialCapacity];
        }

        public Broadcast()
        {
            _listener = new Action<T1, T2, T3>[8];
        }

        #endregion


        #region AddSingleton Listener

        /// <inheritdoc />
        public bool AddUnique(Action<T1, T2, T3> listener)
        {
            if (Contains(listener))
            {
                return false;
            }

            Add(listener);
            return true;
        }

        /// <inheritdoc />
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

        private void IncreaseCapacity()
        {
            var increasedArr = new Action<T1, T2, T3>[_listener.Length * 2];

            for (var i = 0; i < _listener.Length; i++)
            {
                increasedArr[i] = _listener[i];
            }

            _listener = increasedArr;
        }

        /// <inheritdoc />
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

        /// <inheritdoc />
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

        private void RemoveAt(int index)
        {
            --Count;

            for (var i = index; i < Count; ++i)
            {
                _listener[i] = _listener[i + 1];
            }

            _listener[Count] = null;
        }

        /// <inheritdoc />
        public void Clear()
        {
            Count = 0;

            for (var i = _listener.Length - 1; i >= 0; i--)
            {
                _listener[i] = null;
            }
        }

        /// <inheritdoc />
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Raise(T1 first, T2 second, T3 third)
        {
            for (var i = Count - 1; i >= 0; i--)
            {
                _listener[i](first, second, third);
            }
        }

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

    public class Broadcast<T1, T2, T3, T4>
    {
        #region Member Variables

        public Action<T1, T2, T3, T4> this[int index] => _listener[index];

        public int Count { get; private set; }
        public IReadOnlyCollection<Action<T1, T2, T3, T4>> GetListenerCollection => _listener.Take(Count).ToList();
        private Action<T1, T2, T3, T4>[] _listener;

        #endregion


        #region Ctor

        public Broadcast(int initialCapacity)
        {
            _listener = new Action<T1, T2, T3, T4>[initialCapacity];
        }

        public Broadcast()
        {
            _listener = new Action<T1, T2, T3, T4>[8];
        }

        #endregion


        #region AddSingleton Listener

        /// <inheritdoc />
        public bool AddUnique(Action<T1, T2, T3, T4> listener)
        {
            if (Contains(listener))
            {
                return false;
            }

            Add(listener);
            return true;
        }

        /// <inheritdoc />
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

        private void IncreaseCapacity()
        {
            var increasedArr = new Action<T1, T2, T3, T4>[_listener.Length * 2];

            for (var i = 0; i < _listener.Length; i++)
            {
                increasedArr[i] = _listener[i];
            }

            _listener = increasedArr;
        }

        /// <inheritdoc />
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

        /// <inheritdoc />
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

        private void RemoveAt(int index)
        {
            --Count;

            for (var i = index; i < Count; ++i)
            {
                _listener[i] = _listener[i + 1];
            }

            _listener[Count] = null;
        }

        /// <inheritdoc />
        public void Clear()
        {
            Count = 0;

            for (var i = _listener.Length - 1; i >= 0; i--)
            {
                _listener[i] = null;
            }
        }

        /// <inheritdoc />
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Raise(T1 first, T2 second, T3 third, T4 fourth)
        {
            for (var i = Count - 1; i >= 0; i--)
            {
                _listener[i](first, second, third, fourth);
            }
        }

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