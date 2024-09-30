using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using Baracuda.Utility.Collections;
using JetBrains.Annotations;
using UnityEngine;

namespace Baracuda.Utility.PlayerLoop
{
    /// <summary>
    ///     Class handles custom gameloop specific callbacks as well as other gameloop relevant data.
    ///     This includes time scale, frame count, editor state, initialization callbacks and more.
    /// </summary>
    public partial class Gameloop
    {
        #region Callbacks

        /// <summary>
        ///     Inject the initialization completed callback.
        /// </summary>
        public static void RaiseInitializationCompleted()
        {
            Debug.Log("Gameloop", "Raising initialization completed!");
            RaiseInitializationCompletedInternal();
        }

        /// <summary>
        ///     Begin an asynchronous shutdown process.
        /// </summary>
        public static void Shutdown()
        {
            ShutdownInternal();
        }

        #endregion


        #region State

        /// <summary>
        ///     Returns true if the application is not quitting.
        /// </summary>
        public static bool IsRunning => Application.isPlaying && !IsQuitting;

        /// <summary>
        ///     Returns true if the application is not running.
        /// </summary>
        public static bool IsNotRunning => !IsRunning;

        /// <summary>
        ///     Returns true if the application quitting process has started.
        /// </summary>
        public static bool IsQuitting { get; private set; }

        /// <summary>
        ///     The editor play mode state. Will only return a valid value in editor.
        /// </summary>
        public static int EditorState { get; private set; }

        /// <summary>
        ///     Returns true if <see cref="RaiseInitializationCompleted" /> was raised.
        /// </summary>
        public static bool InitializationCompletedState { get; private set; }

        /// <summary>
        ///     Returns true if the before scene load callback was raised.
        /// </summary>
        public static bool BeforeSceneLoadCompleted { get; private set; }

        /// <summary>
        ///     Returns true if the after scene load callback was raised.
        /// </summary>
        public static bool AfterSceneLoadCompleted { get; private set; }

        /// <summary>
        ///     Get the current gameloop segment. (Update, LateUpdate etc.)
        /// </summary>
        public static Segment Segment { get; private set; } = Segment.None;

        /// <summary>
        ///     Get the current physics update count.
        /// </summary>
        public static int FixedUpdateCount { get; private set; }

        /// <summary>
        ///     Return a <see cref="CancellationToken" /> that is valid for the duration of the applications runtime.
        ///     This means until OnApplicationQuit is called in a build
        ///     or until the play state is changed in the editor.
        /// </summary>
        public static CancellationToken RuntimeToken => cancellationTokenSource.Token;

        #endregion


        #region Events

        /// <summary>
        ///     Called before the first scene is loaded.
        ///     This event is called retroactively.
        /// </summary>
        [PublicAPI]
        public static event Action BeforeFirstSceneLoad
        {
            add => beforeFirstSceneLoadCallbacks.AddListener(value);
            remove => beforeFirstSceneLoadCallbacks.RemoveListener(value);
        }

        /// <summary>
        ///     Called after the first scene is loaded.
        ///     This event is called retroactively.
        /// </summary>
        [PublicAPI]
        public static event Action AfterFirstSceneLoad
        {
            add => afterFirstSceneLoadCallbacks.AddListener(value);
            remove => afterFirstSceneLoadCallbacks.RemoveListener(value);
        }

        /// <summary>
        ///     Called during shutdown.
        /// </summary>
        [PublicAPI]
        public static event Action ApplicationQuit
        {
            add => applicationQuitCallbacks.AddListener(value);
            remove => applicationQuitCallbacks.RemoveListener(value);
        }

        /// <summary>
        ///     Called when the focus of the application was changed.
        /// </summary>
        [PublicAPI]
        public static event Action<bool> ApplicationFocusChanged
        {
            add => applicationFocusCallbacks.AddListener(value);
            remove => applicationFocusCallbacks.RemoveListener(value);
        }

        /// <summary>
        ///     Called when pause state of the application was changed.
        /// </summary>
        [PublicAPI]
        public static event Action<bool> ApplicationPauseChanged
        {
            add => applicationPauseCallbacks.AddListener(value);
            remove => applicationPauseCallbacks.RemoveListener(value);
        }

        /// <summary>
        ///     Called every frame.
        /// </summary>
        [PublicAPI]
        public static event Action Update
        {
            add => updateCallbacks.AddListener(value);
            remove => updateCallbacks.RemoveListener(value);
        }

        /// <summary>
        ///     Called every frame during late update.
        /// </summary>
        [PublicAPI]
        public static event Action LateUpdate
        {
            add => lateUpdateCallbacks.AddListener(value);
            remove => lateUpdateCallbacks.RemoveListener(value);
        }

        /// <summary>
        ///     Called every physics update.
        /// </summary>
        [PublicAPI]
        public static event Action FixedUpdate
        {
            add => fixedUpdateCallbacks.AddListener(value);
            remove => fixedUpdateCallbacks.RemoveListener(value);
        }

        /// <summary>
        ///     Called once every second.
        /// </summary>
        [PublicAPI]
        public static event Action SlowTick
        {
            add => slowTickUpdateCallbacks.AddListener(value);
            remove => slowTickUpdateCallbacks.RemoveListener(value);
        }

        /// <summary>
        ///     Called 10 times per second.
        /// </summary>
        [PublicAPI]
        public static event Action Tick
        {
            add => tickUpdateCallbacks.AddListener(value);
            remove => tickUpdateCallbacks.RemoveListener(value);
        }

        /// <summary>
        ///     Called when asynchronous initialization has completed.
        ///     Requires <see cref="RaiseInitializationCompleted" /> to be called.
        ///     This event is called retroactively.
        /// </summary>
        [PublicAPI]
        public static event Action InitializationCompleted
        {
            add => initializationCompletedCallbacks.AddListener(value);
            remove => initializationCompletedCallbacks.RemoveListener(value);
        }

        #endregion


        #region Time Scale

        /// <summary>
        ///     When enabled, TimeScaleDelegates can be added to control the time scale of the game.
        /// </summary>
        [PublicAPI]
        public static bool ControlTimeScale { get; set; } = true;

        /// <summary>
        ///     AddSingleton a custom timescale modification source.
        /// </summary>
        [PublicAPI]
        public static void AddTimeScaleModifier(TimeScaleDelegate modifier)
        {
            timeScaleModifier.AddUnique(modifier);
        }

        /// <summary>
        ///     Remove a custom timescale modification source.
        /// </summary>
        [PublicAPI]
        public static void RemoveTimeScaleModifier(TimeScaleDelegate modifier)
        {
            timeScaleModifier.Remove(modifier);
        }

        /// <summary>
        ///     Removes all active time scale modification sources.
        /// </summary>
        [PublicAPI]
        public static void ClearTimeScaleModifier()
        {
            timeScaleModifier.Clear();
        }

        #endregion


        #region Coroutines

        [PublicAPI]
        public static Coroutine StartCoroutine(IEnumerator enumerator)
        {
            if (monoBehaviour == null)
            {
                return null;
            }

            return monoBehaviour.StartCoroutine(enumerator);
        }

        [PublicAPI]
        public static void StopCoroutine(Coroutine coroutine)
        {
            if (monoBehaviour == null)
            {
                return;
            }

            monoBehaviour.StopCoroutine(coroutine);
        }

        #endregion


        #region Validations

        [PublicAPI]
        public static bool IsDelegateSubscribedToUpdate(Action update)
        {
            return updateCallbacks.Contains(update);
        }

        [PublicAPI]
        public static bool IsDelegateSubscribedToLateUpdate(Action update)
        {
            return lateUpdateCallbacks.Contains(update);
        }

        [PublicAPI]
        public static bool IsDelegateSubscribedToFixedUpdate(Action update)
        {
            return fixedUpdateCallbacks.Contains(update);
        }

        #endregion


        #region Editor

        [PublicAPI]
        public static float TickDelta => TickScaledTimer.CalculateDelta();

        [PublicAPI]
        public static async Task DelayedCallAsync()
        {
#if UNITY_EDITOR
            var completionSource = new TaskCompletionSource<object>();
            UnityEditor.EditorApplication.delayCall += () => { completionSource.SetResult(null); };
            await completionSource.Task;
#else
            await Task.CompletedTask;
#endif
        }

#if UNITY_EDITOR

        [PublicAPI]
        public static event Action EditorUpdate
        {
            add => editorUpdateCallbacks.AddListener(value);
            remove => editorUpdateCallbacks.RemoveListener(value);
        }

        [PublicAPI]
        public static event Action ExitPlayMode
        {
            add => exitPlayModeDelegate.AddListener(value);
            remove => exitPlayModeDelegate.RemoveListener(value);
        }

        [PublicAPI]
        public static event Action EnterPlayMode
        {
            add => enterPlayModeDelegate.AddListener(value);
            remove => enterPlayModeDelegate.RemoveListener(value);
        }

        [PublicAPI]
        public static event Action ExitEditMode
        {
            add => exitEditModeDelegate.AddListener(value);
            remove => exitEditModeDelegate.RemoveListener(value);
        }

        [PublicAPI]
        public static event Action EnterEditMode
        {
            add => enterEditModeDelegate.AddListener(value);
            remove => enterEditModeDelegate.RemoveListener(value);
        }

        [PublicAPI]
        public static event WillDeleteAssetCallback BeforeDeleteAsset;
#endif

        #endregion
    }
}