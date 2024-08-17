using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Baracuda.Bedrock.Collections;
using Baracuda.Bedrock.Odin;
using Baracuda.Bedrock.PlayerLoop;
using Baracuda.Bedrock.Timing;
using Baracuda.Bedrock.Utilities;
using JetBrains.Annotations;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Assertions;
using Action = System.Action;

namespace Baracuda.Bedrock.Types
{
    public class StateMachine<TState> : MonoBehaviour where TState : unmanaged, Enum
    {
        #region Fields

        [Debug]
        private TState _state;
        private TState _previousState;
        private readonly LimitedQueue<TState> _stateHistory = new(16);
        private readonly Func<bool> _trueFunc = () => true;
        private readonly EqualityComparer<TState> _comparer = EqualityComparer<TState>.Default;

        // Callbacks
        private readonly Dictionary<TState, List<Action>> _stateEnterCallbacks = new();
        private readonly List<Action<TState>> _parameterizedStateEnterCallbacks = new();
        private readonly Dictionary<ulong, List<Action>> _stateEnterFromToCallbacks = new();
        private readonly Dictionary<TState, List<Action>> _stateExitCallbacks = new();
        private readonly List<Action<TState>> _parameterizedStateExitCallbacks = new();
        private readonly Dictionary<TState, List<Action<float>>> _stateUpdateCallbacksWithDeltaTime = new();
        private readonly Dictionary<TState, List<Action>> _stateUpdateCallbacks = new();
        private readonly Dictionary<TState, List<Action<float>>> _stateTransitionUpdateCallbacks = new();
        private readonly List<Action<TState, TState>> _globalStateTransitionCallbacks = new();

        // Transitions
        private readonly Dictionary<TState, List<ConditionalStateTransition>> _conditionalStateTransitions = new();
        private readonly Dictionary<TState, TimedStateTransition> _timedStateTransitions = new();
        [Debug] private bool _isTimerTransitionActive;
        [Debug] private bool _isTransitionActive;
        [Debug] private Transition _transition;

        // Blocking
        [Debug] private readonly Dictionary<TState, HashSet<object>> _stateBlocker = new();
        [Debug] private readonly HashSet<object> _stateMachineBlocker = new();

        // Alias
        private readonly Dictionary<string, TState> _stateAlias = new(StringComparer.CurrentCultureIgnoreCase);

        // Logging
        private readonly LogCategory _log;
        private readonly Color _color = RandomUtility.LoggingColor();

        // Overrides
        private TState _lastNonOverrideState;
        private readonly HashSet<object> _overrideObjects = new();

        protected StateMachine()
        {
            _log = GetType().Name;
        }

        #endregion


        #region Public API:

        /// <summary>
        ///     Returns the current active state of the state machine.
        /// </summary>
        [PublicAPI]
        public TState GetCurrentState()
        {
            return _state;
        }

        /// <summary>
        ///     Returns the state that was active before the current state.
        /// </summary>
        [PublicAPI]
        public TState GetPreviousState()
        {
            return _previousState;
        }

        /// <summary>
        ///     Returns the last state before the specified state, ignoring the specified state.
        /// </summary>
        [PublicAPI]
        public TState GetPreviousStateThatWasNot(TState previousStateToIgnore)
        {
            return GetPreviousStateThatWasNotInternal(previousStateToIgnore);
        }

        /// <summary>
        ///     Returns the last state before the current state, ignoring the specified states.
        /// </summary>
        [PublicAPI]
        public TState GetPreviousStateThatWasNot(params TState[] previousStatesToIgnore)
        {
            return GetPreviousStateThatWasNotInternal(previousStatesToIgnore);
        }

        /// <summary>
        ///     Returns a read-only collection of the state history.
        /// </summary>
        [PublicAPI]
        public IReadOnlyCollection<TState> GetStateHistory()
        {
            return _stateHistory;
        }

        /// <summary>
        ///     Sets the state without invoking enter or exit callbacks.
        /// </summary>
        [PublicAPI]
        public void SetStateWithoutCallbacks(TState state)
        {
            _stateHistory.Enqueue(_state);
            _previousState = _state;
            _state = state;
        }

        /// <summary>
        ///     Checks if the specified state is currently active.
        /// </summary>
        [PublicAPI]
        public bool IsStateActive(TState state)
        {
            return _comparer.Equals(_state, state);
        }

        /// <summary>
        ///     Checks if any of the specified states are currently active.
        /// </summary>
        [PublicAPI]
        public bool IsStateActive(params TState[] states)
        {
            return _state.EqualsAny(states);
        }

        /// <summary>
        ///     Checks if the specified state was the state before the current state.
        /// </summary>
        [PublicAPI]
        public bool WasPreviousState(TState state)
        {
            return _comparer.Equals(state, GetPreviousState());
        }

        /// <summary>
        ///     Checks if the specified state is not currently active.
        /// </summary>
        [PublicAPI]
        public bool IsStateNotActive(TState state)
        {
            return !_comparer.Equals(_state, state);
        }

        /// <summary>
        ///     Converts a string alias to the corresponding state if possible.
        /// </summary>
        [PublicAPI]
        public TState? FromAlias(string alias)
        {
            return _stateAlias.TryGetValue(alias, out var state) ? state : null;
        }

        #endregion


        #region Public API: State Transition

        /// <summary>
        ///     Sets the state with the normal transition process, invoking relevant callbacks.
        /// </summary>
        [PublicAPI]
        public void SetState(TState state)
        {
            BeginImmediateTransitionToState(state, false, false);
        }

        /// <summary>
        ///     Transitions to a state but will remember the last state before the first override.
        ///     Overrides can and should be removed when the state is or should be exited.
        ///     When the last override was removed, the last state before the first override is transitioned to again.
        /// </summary>
        [PublicAPI]
        public void SetStateOverride<T>(TState state, T target) where T : class
        {
            SetStateOverrideInternal(state, target);
        }

        /// <summary>
        ///     Removes an override state, potentially reverting to the last non-override state.
        /// </summary>
        [PublicAPI]
        public void RemoveStateOverride<T>(T target) where T : class
        {
            RemoveStateOverrideInternal(target);
        }

        /// <summary>
        ///     Sets the state after a specified delay in seconds.
        /// </summary>
        [PublicAPI]
        public void SetStateAfter(TState state, float durationInSeconds)
        {
            BeginTimedTransitionToState(state, durationInSeconds);
        }

        /// <summary>
        ///     Sets the state and re-enters it, allowing re invocation of callbacks even if it is the current state.
        /// </summary>
        [PublicAPI]
        public void SetStateAndReinvoke(TState state)
        {
            BeginImmediateTransitionToState(state, true, false);
        }

        /// <summary>
        ///     Cancels any currently active state transition.
        /// </summary>
        [PublicAPI]
        public void CancelActiveTransition()
        {
            CancelActiveTransitionInternal();
        }

        #endregion


        #region Public API: Blocking

        /// <summary>
        ///     Blocks a state from being entered, with a specific blocker.
        /// </summary>
        [PublicAPI]
        public void BlockState(TState state, object blocker)
        {
            BlockStateInternal(state, blocker);
        }

        /// <summary>
        ///     Unblocks a previously blocked state, allowing it to be entered again.
        /// </summary>
        [PublicAPI]
        public void UnblockState(TState state, object blocker)
        {
            UnblockStateInternal(state, blocker);
        }

        /// <summary>
        ///     Checks if a state is currently blocked.
        /// </summary>
        [PublicAPI]
        public void IsStateBlocked(TState state)
        {
            IsStateBlockedInternal(state);
        }

        /// <summary>
        ///     Blocks the entire state machine from transitioning states.
        /// </summary>
        [PublicAPI]
        public bool BlockStateMachine(object blocker)
        {
            return _stateMachineBlocker.Add(blocker);
        }

        /// <summary>
        ///     Unblocks the state machine, allowing state transitions again.
        /// </summary>
        [PublicAPI]
        public bool UnblockStateMachine(object blocker)
        {
            return _stateMachineBlocker.Remove(blocker);
        }

        #endregion


        #region Pubic API: Adding State Objects

        /// <summary>
        ///     Associates an alias with a state for easier reference.
        /// </summary>
        [PublicAPI]
        public void AddStateAlias(TState state, string alias)
        {
            AddStateAliasInternal(state, alias);
        }

        #endregion


        #region Public API: Adding Callbacks

        /// <summary>
        ///     Adds a callback to be called when entering a specific state.
        /// </summary>
        [PublicAPI]
        public void AddStateEnterCallback(TState state, Action callback)
        {
            AddStateEnterCallbackInternal(state, callback);
        }

        /// <summary>
        ///     Adds a callback to be called when entering any state, providing the state as a parameter.
        /// </summary>
        [PublicAPI]
        public void AddStateEnterCallback(Action<TState> callback)
        {
            AddStateEnterCallbackInternal(callback);
        }

        /// <summary>
        ///     Adds a callback to be called during a transition between specified states.
        /// </summary>
        [PublicAPI]
        public void AddStateTransitionCallback(TState from, TState to, Action callback)
        {
            AddStateTransitionCallbackInternal(from, to, callback);
        }

        /// <summary>
        ///     Adds a callback to be called when exiting a specific state.
        /// </summary>
        [PublicAPI]
        public void AddStateExitCallback(TState state, Action callback)
        {
            AddStateExitCallbackInternal(state, callback);
        }

        /// <summary>
        ///     Adds a callback to be called when exiting any state, providing the state as a parameter.
        /// </summary>
        [PublicAPI]
        public void AddStateExitCallback(Action<TState> callback)
        {
            AddStateExitCallbackInternal(callback);
        }

        /// <summary>
        ///     Adds a callback to be invoked on state update, providing delta time as a parameter.
        /// </summary>
        [PublicAPI]
        public void AddStateUpdateCallback(TState state, Action<float> callback)
        {
            AddStateUpdateCallbackInternal(state, callback);
        }

        /// <summary>
        ///     Adds a callback to be invoked on state update.
        /// </summary>
        [PublicAPI]
        public void AddStateUpdateCallback(TState state, Action callback)
        {
            AddStateUpdateCallbackInternal(state, callback);
        }

        /// <summary>
        ///     Adds a callback during a timed transition into a specific state, providing transition delta.
        /// </summary>
        [PublicAPI]
        public void AddStateTransitionUpdateCallback(TState to, Action<float> callback)
        {
            AddStateTransitionUpdateCallbackInternal(to, callback);
        }

        /// <summary>
        ///     Adds a callback to be called during any state transition, providing the from and to states as parameters.
        /// </summary>
        [PublicAPI]
        public void AddStateTransitionCallback(Action<TState, TState> callback)
        {
            AddStateTransitionCallbackInternal(callback);
        }

        #endregion


        #region Public API: Adding Transitions

        /// <summary>
        ///     Adds a conditional state transition that will occur if the condition is met.
        /// </summary>
        [PublicAPI]
        public void AddStateTransition(TState from, TState to, Func<bool> condition = null)
        {
            AddStateTransitionInternal(from, to, condition);
        }

        /// <summary>
        ///     Adds a state transition that will occur after a specified delay when a given state is entered.
        /// </summary>
        [PublicAPI]
        public void AddStateTransition(TState from, TState to, float delayInSeconds)
        {
            AddStateTransitionInternal(from, to, delayInSeconds);
        }

        #endregion


        #region Callback Adding

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void AddStateEnterCallbackInternal(TState state, Action callback)
        {
            if (!_stateEnterCallbacks.TryGetValue(state, out var list))
            {
                list = new List<Action>();
                _stateEnterCallbacks.Add(state, list);
            }

            list.Add(callback);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void AddStateEnterCallbackInternal(Action<TState> callback)
        {
            _parameterizedStateEnterCallbacks.Add(callback);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void AddStateTransitionCallbackInternal(TState from, TState to, Action callback)
        {
            var transitionKey = Combine(from, to);

            if (!_stateEnterFromToCallbacks.TryGetValue(transitionKey, out var list))
            {
                list = new List<Action>();
                _stateEnterFromToCallbacks.Add(transitionKey, list);
            }

            list.Add(callback);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void AddStateExitCallbackInternal(TState state, Action callback)
        {
            if (!_stateExitCallbacks.TryGetValue(state, out var list))
            {
                list = new List<Action>();
                _stateExitCallbacks.Add(state, list);
            }

            list.Add(callback);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void AddStateExitCallbackInternal(Action<TState> callback)
        {
            _parameterizedStateExitCallbacks.Add(callback);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void AddStateUpdateCallbackInternal(TState state, Action<float> callback)
        {
            if (!_stateUpdateCallbacksWithDeltaTime.TryGetValue(state, out var list))
            {
                list = new List<Action<float>>();
                _stateUpdateCallbacksWithDeltaTime.Add(state, list);
            }

            list.Add(callback);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void AddStateUpdateCallbackInternal(TState state, Action callback)
        {
            if (!_stateUpdateCallbacks.TryGetValue(state, out var list))
            {
                list = new List<Action>();
                _stateUpdateCallbacks.Add(state, list);
            }

            list.Add(callback);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void AddStateTransitionUpdateCallbackInternal(TState to, Action<float> callback)
        {
            if (!_stateTransitionUpdateCallbacks.TryGetValue(to, out var list))
            {
                list = new List<Action<float>>();
                _stateTransitionUpdateCallbacks.Add(to, list);
            }

            list.Add(callback);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void AddStateTransitionCallbackInternal(Action<TState, TState> callback)
        {
            _globalStateTransitionCallbacks.Add(callback);
        }

        #endregion


        #region Transition Adding

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void AddStateTransitionInternal(TState from, TState to, Func<bool> condition = null)
        {
            if (!_conditionalStateTransitions.TryGetValue(from, out var list))
            {
                list = new List<ConditionalStateTransition>(4);
                _conditionalStateTransitions.Add(from, list);
            }

            var transition = new ConditionalStateTransition(from, to, condition ?? _trueFunc);
            list.Add(transition);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void AddStateTransitionInternal(TState from, TState to, float delayInSeconds)
        {
            var wasAdded = _timedStateTransitions.TryAdd(from, new TimedStateTransition(from, to, delayInSeconds));
            Assert.IsTrue(wasAdded);
        }

        #endregion


        #region State Transition

        private struct Transition
        {
            public TState ToState;
            public ScaledTimer ScaledTimer;
        }

        private void BeginImmediateTransitionToState(TState toState, bool allowStateReentering, bool isStateOverride)
        {
            if (_stateMachineBlocker.Count > 0)
            {
                Debug.Log(_log, "StateMachine is blocked!", _color);
                return;
            }

            if (IsStateBlockedInternal(toState))
            {
                Debug.Log(_log, $"State: [{Colorized(toState)}] is blocked!", _color);
                return;
            }

            if (allowStateReentering is false && IsStateActive(toState))
            {
                Debug.Log(_log, $"State: [{Colorized(toState)}] is already active!", _color);
                return;
            }

            _transition = new Transition
            {
                ToState = toState,
                ScaledTimer = ScaledTimer.None
            };

            CompleteTransitionToStateInternal(ref _transition, allowStateReentering, isStateOverride);
        }

        private void BeginTimedTransitionToState(TState toState, float durationInSeconds)
        {
            if (_stateMachineBlocker.Count > 0)
            {
                Debug.Log(_log, "StateMachine is blocked!", _color);
                return;
            }

            if (IsStateBlockedInternal(toState))
            {
                Debug.Log(_log, $"State: [{Colorized(toState)}] is blocked!", _color);
                return;
            }

            if (IsStateActive(toState))
            {
                Debug.Log(_log, $"State: [{Colorized(toState)}] is already active!", _color);
                return;
            }

            _transition = new Transition
            {
                ToState = toState,
                ScaledTimer = ScaledTimer.FromSeconds(durationInSeconds)
            };

            _isTimerTransitionActive = true;
        }

        /// <summary>
        ///     This is the method where the actual enter and exit callbacks happen
        /// </summary>
        private void CompleteTransitionToStateInternal(ref Transition transition, bool allowStateReentering,
            bool isStateOverride)
        {
            if (IsStateBlockedInternal(transition.ToState))
            {
                Debug.Log(_log, $"State: [{Colorized(transition.ToState)}] is is blocked active!", _color);
                return;
            }

            if (_isTransitionActive)
            {
                Debug.Log(_log, "Transition is already active! Do not transition in state enter/exit callbacks!", Verbosity.Error);
                return;
            }

            _isTransitionActive = true;

            if (allowStateReentering is false)
            {
                Assert.AreNotEqual(transition.ToState, GetCurrentState());
            }

            var fromState = GetCurrentState();
            var toState = transition.ToState;
            var transitionKey = Combine(fromState, toState);

            if (isStateOverride is false)
            {
                _lastNonOverrideState = toState;
            }

            if (!_comparer.Equals(fromState, toState))
            {
                _previousState = _state;
                _stateHistory.Enqueue(_state);
            }

            _state = toState;

            // Exit Callbacks
            if (_stateExitCallbacks.TryGetValue(fromState, out var exitCallbacks))
            {
                foreach (var callback in exitCallbacks)
                {
                    callback();
                }
            }

            // Exit Callbacks Parameterized
            foreach (var callback in _parameterizedStateExitCallbacks)
            {
                callback(fromState);
            }

            // General State Transition Callbacks
            if (_globalStateTransitionCallbacks.Any())
            {
                foreach (var globalStateTransitionCallback in _globalStateTransitionCallbacks)
                {
                    globalStateTransitionCallback(fromState, toState);
                }
            }

            // Enter Callbacks
            if (_stateEnterCallbacks.TryGetValue(toState, out var enterCallbacks))
            {
                foreach (var callback in enterCallbacks)
                {
                    callback();
                }
            }

            // Specific State Transition Callbacks
            if (_stateEnterFromToCallbacks.TryGetValue(transitionKey, out var transitionCallbacks))
            {
                foreach (var callback in transitionCallbacks)
                {
                    callback();
                }
            }

            // Parameterized Enter
            foreach (var callback in _parameterizedStateEnterCallbacks)
            {
                callback(toState);
            }

            Debug.Log(_log, $"Transitioned from state [{Colorized(fromState)}] to [{Colorized(toState)}]", _color);

            _isTimerTransitionActive = false;
            _isTransitionActive = false;

            // Automatic Timed Transitions
            if (_timedStateTransitions.TryGetValue(_state, out var timedTransition))
            {
                Assert.AreEqual(GetCurrentState(), timedTransition.FromState);

                BeginTimedTransitionToState(timedTransition.ToState,
                    timedTransition.Duration);
            }
        }

        private void CancelActiveTransitionInternal()
        {
            _isTimerTransitionActive = false;
            _isTransitionActive = false;
            _transition = default;
        }

        #endregion


        #region State Machine Update

        private void LateUpdate()
        {
#if UNITY_EDITOR
            if (Gameloop.IsQuitting)
            {
                return;
            }
#endif
            if (_isTimerTransitionActive)
            {
                ref var timer = ref _transition.ScaledTimer;
                if (timer.IsRunning)
                {
                    if (_stateTransitionUpdateCallbacks.TryGetValue(_transition.ToState, out var callbacks))
                    {
                        foreach (var callback in callbacks)
                        {
                            callback(timer.Delta());
                        }
                    }
                }
                else if (timer.Expired)
                {
                    CompleteTransitionToStateInternal(ref _transition, false, false);
                }
            }

            if (_stateUpdateCallbacks.TryGetValue(GetCurrentState(), out var deltaTimeUpdates))
            {
                foreach (var updateCallback in deltaTimeUpdates)
                {
                    updateCallback();
                }
            }

            if (_stateUpdateCallbacksWithDeltaTime.TryGetValue(GetCurrentState(), out var updates))
            {
                var deltaTime = Time.deltaTime;
                foreach (var updateCallback in updates)
                {
                    updateCallback(deltaTime);
                }
            }

            if (_conditionalStateTransitions.TryGetValue(GetCurrentState(), out var conditionalStateTransitions))
            {
                foreach (var transition in conditionalStateTransitions)
                {
                    if (transition.Condition())
                    {
                        BeginImmediateTransitionToState(transition.ToState, false, false);
                        break;
                    }
                }
            }
        }

        #endregion


        #region State Blocking

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void BlockStateInternal(TState state, object blocker)
        {
            if (!_stateBlocker.TryGetValue(state, out var blockerSet))
            {
                blockerSet = new HashSet<object>();
                _stateBlocker.Add(state, blockerSet);
            }

            blockerSet.Add(blocker);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void UnblockStateInternal(TState state, object blocker)
        {
            if (!_stateBlocker.TryGetValue(state, out var blockerSet))
            {
                blockerSet = new HashSet<object>();
                _stateBlocker.Add(state, blockerSet);
            }

            blockerSet.Remove(blocker);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool IsStateBlockedInternal(TState state)
        {
            return _stateBlocker.TryGetValue(state, out var blocker) && blocker.Any();
        }

        #endregion


        #region State Objects

        private void AddStateAliasInternal(TState state, string alias)
        {
            _stateAlias.TryAdd(alias, state);
        }

        #endregion


        #region Misc

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SetStateOverrideInternal(TState state, object target)
        {
            _overrideObjects.Add(target);
            BeginImmediateTransitionToState(state, false, true);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void RemoveStateOverrideInternal(object target)
        {
            if (_overrideObjects.Remove(target) && _overrideObjects.IsEmpty())
            {
                BeginImmediateTransitionToState(_lastNonOverrideState, false, false);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private TState GetPreviousStateThatWasNotInternal(TState previousStateToIgnore)
        {
            foreach (var state in _stateHistory.Reverse())
            {
                if (_comparer.Equals(previousStateToIgnore, state))
                {
                    continue;
                }

                return state;
            }

            return default;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private TState GetPreviousStateThatWasNotInternal(params TState[] previousStatesToIgnore)
        {
            foreach (var state in _stateHistory)
            {
                var isStateValid = true;
                foreach (var previous in previousStatesToIgnore)
                {
                    if (_comparer.Equals(previous, state))
                    {
                        isStateValid = false;
                        break;
                    }
                }

                if (isStateValid)
                {
                    return state;
                }
            }

            return default;
        }

        #endregion


        #region Helper

        private static ulong Combine(TState fromState, TState toState)
        {
            var aInt = UnsafeUtility.As<TState, int>(ref fromState);
            var bInt = UnsafeUtility.As<TState, int>(ref toState);
            return Combine(aInt, bInt);
        }

        private static ulong Combine(int a, int b)
        {
            var ua = (uint)a;
            ulong ub = (uint)b;
            return (ub << 32) | ua;
        }

        private static void Separate(ulong c, out int a, out int b)
        {
            a = (int)(c & 0xFFFFFFFFUL);
            b = (int)(c >> 32);
        }

        private string Colorized(TState state)
        {
            return state.ToString().Colorize(_color);
        }

        private class ConditionalStateTransition
        {
            public readonly TState FromState;
            public readonly TState ToState;
            public readonly Func<bool> Condition;

            public ConditionalStateTransition(TState fromState, TState toState, Func<bool> condition)
            {
                FromState = fromState;
                ToState = toState;
                Condition = condition;
            }
        }

        private class TimedStateTransition
        {
            public readonly TState FromState;
            public readonly TState ToState;
            public readonly float Duration;

            public TimedStateTransition(TState fromState, TState toState, float duration)
            {
                FromState = fromState;
                ToState = toState;
                Duration = duration;
            }
        }

        #endregion
    }
}