using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Baracuda.Bedrock.Reflection;
using Baracuda.Bedrock.Timing;
using UnityEngine;
using UnityEngine.Profiling;
using Object = UnityEngine.Object;

namespace Baracuda.Bedrock.PlayerLoop
{
    public partial class Gameloop
    {
        #region Fields

        private const int Capacity4 = 4;
        private const int Capacity8 = 8;
        private const int Capacity16 = 16;
        private const int Capacity32 = 32;
        private const int Capacity64 = 64;
        private const int Capacity128 = 128;
        private const int Capacity256 = 256;
        private static readonly LogCategory logCategory = nameof(Gameloop);

        private static readonly HashSet<Object> registeredObjects = new(Capacity256);
        private static readonly Dictionary<Type, CallbackMethodInfo> callbackMethodInfoCache = new(Capacity128);

        private static readonly List<Action> initializationCompletedCallbacks = new(Capacity64);
        private static readonly List<Action> beforeFirstSceneLoadCallbacks = new(Capacity16);
        private static readonly List<Action> afterFirstSceneLoadCallbacks = new(Capacity16);
        private static readonly List<Action> preUpdateCallbacks = new(Capacity32);
        private static readonly List<Action> postUpdateCallbacks = new(Capacity32);
        private static readonly List<Action> preLateUpdateCallbacks = new(Capacity32);
        private static readonly List<Action> postLateUpdateCallbacks = new(Capacity32);
        private static readonly List<Action> updateCallbacks = new(Capacity64);
        private static readonly List<Action> lateUpdateCallbacks = new(Capacity32);
        private static readonly List<Action> fixedUpdateCallbacks = new(Capacity32);
        private static readonly List<Action> slowTickUpdateCallbacks = new(Capacity32);
        private static readonly List<Action> tickUpdateCallbacks = new(Capacity32);
        private static readonly List<Action> applicationQuitCallbacks = new(Capacity32);
        private static readonly List<Action> firstUpdateCallbacks = new(Capacity8);
        private static readonly List<Action<bool>> applicationFocusCallbacks = new(Capacity16);
        private static readonly List<Action<bool>> applicationPauseCallbacks = new(Capacity16);
        private static readonly Dictionary<string, List<Action>> customCallbacks = new();
        private static MonoBehaviour monoBehaviour;

        private static CancellationTokenSource cancellationTokenSource = new();

        private static ScaledTimer OneSecondScaledTimer { get; set; } = ScaledTimer.None;
        private static ScaledTimer TickScaledTimer { get; set; }

        #endregion


        #region Timeloop

        public delegate void TimeScaleDelegate(ref float timeScale);

        private static readonly List<TimeScaleDelegate> timeScaleModifier = new();

        private static void UpdateTimeScale()
        {
            if (ControlTimeScale is false)
            {
                return;
            }

            var timeScale = 1f;
            foreach (var timeScaleDelegate in timeScaleModifier)
            {
                timeScaleDelegate(ref timeScale);
            }

            Time.timeScale = timeScale;
        }

        #endregion


        #region Callbacks

        private static void UnregisterInternal(Object target)
        {
            Profiler.BeginSample("Gameloop.UnregisterInternal");

            var wasRemoved = registeredObjects.Remove(target);
            if (wasRemoved is false)
            {
                Profiler.EndSample();
                return;
            }

            RemoveCallbacksFromList(updateCallbacks, target);
            RemoveCallbacksFromList(preUpdateCallbacks, target);
            RemoveCallbacksFromList(postUpdateCallbacks, target);
            RemoveCallbacksFromList(preLateUpdateCallbacks, target);
            RemoveCallbacksFromList(postLateUpdateCallbacks, target);
            RemoveCallbacksFromList(lateUpdateCallbacks, target);
            RemoveCallbacksFromList(fixedUpdateCallbacks, target);
            RemoveCallbacksFromList(applicationQuitCallbacks, target);
            RemoveCallbacksFromList(applicationFocusCallbacks, target);
            RemoveCallbacksFromList(applicationPauseCallbacks, target);
            RemoveCallbacksFromList(afterFirstSceneLoadCallbacks, target);
            RemoveCallbacksFromList(beforeFirstSceneLoadCallbacks, target);
            RemoveCallbacksFromList(initializationCompletedCallbacks, target);

            foreach (var list in customCallbacks.Values)
            {
                RemoveCallbacksFromList(list, target);
            }

#if UNITY_EDITOR
            RemoveCallbacksFromList(enterEditModeDelegate, target);
            RemoveCallbacksFromList(exitEditModeDelegate, target);
            RemoveCallbacksFromList(enterPlayModeDelegate, target);
            RemoveCallbacksFromList(exitPlayModeDelegate, target);
            RemoveCallbacksFromList(buildPreprocessorCallbacks, target);
#endif

            Profiler.EndSample();
        }

        private static void RegisterInternal(Object target)
        {
            Profiler.BeginSample("Gameloop.RegisterInternal");

            var wasAdded = registeredObjects.Add(target);
            if (wasAdded is false)
            {
                Profiler.EndSample();
                return;
            }

            var type = target.GetType();
            var callbackMethodInfo = GenerateCallbackMethodInfo(type);

            if (callbackMethodInfo.HasMethods is false)
            {
                Profiler.EndSample();
                return;
            }

            foreach (var (segment, methodInfo) in callbackMethodInfo.SegmentMethods)
            {
                try
                {
                    CreateDelegateCallbacks(target, segment, methodInfo);
                }
                catch (Exception exception)
                {
                    Debug.LogException(exception);
                }
            }

            foreach (var (segment, methodInfo) in callbackMethodInfo.CustomMethods)
            {
                try
                {
                    CreatDelegateCustomCallbacks(target, segment, methodInfo);
                }
                catch (Exception exception)
                {
                    Debug.LogException(exception);
                }
            }

            Profiler.EndSample();
        }

        private static void CreatDelegateCustomCallbacks(Object target, string callbackName, MethodInfo methodInfo)
        {
            if (!customCallbacks.TryGetValue(callbackName, out var list))
            {
                list = new List<Action>(Capacity16);
                customCallbacks.Add(callbackName, list);
            }

            var callback = (Action)methodInfo.CreateDelegate(typeof(Action), target);
            list.Add(callback);
        }

        private static void CreateDelegateCallbacks(Object target, Segment segment, MethodInfo methodInfo)
        {
            switch (segment)
            {
                case Segment.None:
                case Segment.Custom:
                    break;

                case Segment.Update:
                    var updateCallback = (Action)methodInfo.CreateDelegate(typeof(Action), target);
                    updateCallbacks.Add(updateCallback);
                    break;

                case Segment.LateUpdate:
                    var lateUpdateCallback = (Action)methodInfo.CreateDelegate(typeof(Action), target);
                    lateUpdateCallbacks.Add(lateUpdateCallback);
                    break;

                case Segment.FixedUpdate:
                    var fixedUpdateCallback = (Action)methodInfo.CreateDelegate(typeof(Action), target);
                    fixedUpdateCallbacks.Add(fixedUpdateCallback);
                    break;

                case Segment.ApplicationQuit:
                    var applicationQuitCallback = (Action)methodInfo.CreateDelegate(typeof(Action), target);
                    applicationQuitCallbacks.Add(applicationQuitCallback);
                    break;

                case Segment.ApplicationFocus:
                    var applicationFocusCallback =
                        (Action<bool>)methodInfo.CreateDelegate(typeof(Action<bool>), target);

                    applicationFocusCallbacks.Add(applicationFocusCallback);
                    break;

                case Segment.ApplicationPause:
                    var applicationPauseCallback =
                        (Action<bool>)methodInfo.CreateDelegate(typeof(Action<bool>), target);

                    applicationPauseCallbacks.Add(applicationPauseCallback);
                    break;

                case Segment.FirstUpdate:
                    var firstUpdateCallback = (Action)methodInfo.CreateDelegate(typeof(Action), target);
                    firstUpdateCallbacks.Add(firstUpdateCallback);
                    break;

                case Segment.InitializationCompleted:
                    var initializationCompletedCallback = (Action)methodInfo.CreateDelegate(typeof(Action), target);
                    initializationCompletedCallbacks.Add(initializationCompletedCallback);
                    if (InitializationCompletedState)
                    {
                        initializationCompletedCallback();
                    }

                    break;

                case Segment.BeforeFirstSceneLoad:
                    var beforeFirstSceneLoadCallback = (Action)methodInfo.CreateDelegate(typeof(Action), target);
                    beforeFirstSceneLoadCallbacks.Add(beforeFirstSceneLoadCallback);
                    if (BeforeSceneLoadCompleted)
                    {
                        beforeFirstSceneLoadCallback();
                    }

                    break;

                case Segment.AfterFirstSceneLoad:
                    var afterFirstSceneLoadCallback = (Action)methodInfo.CreateDelegate(typeof(Action), target);
                    afterFirstSceneLoadCallbacks.Add(afterFirstSceneLoadCallback);
                    if (AfterSceneLoadCompleted)
                    {
                        afterFirstSceneLoadCallback();
                    }

                    break;

                case Segment.PreUpdate:
                    var preUpdateCallback = (Action)methodInfo.CreateDelegate(typeof(Action), target);
                    preUpdateCallbacks.Add(preUpdateCallback);
                    break;

                case Segment.PostUpdate:
                    var postUpdateCallback = (Action)methodInfo.CreateDelegate(typeof(Action), target);
                    postUpdateCallbacks.Add(postUpdateCallback);
                    break;

                case Segment.PreLateUpdate:
                    var preLateUpdateCallback = (Action)methodInfo.CreateDelegate(typeof(Action), target);
                    preLateUpdateCallbacks.Add(preLateUpdateCallback);
                    break;

                case Segment.PostLateUpdate:
                    var postLateUpdateCallback = (Action)methodInfo.CreateDelegate(typeof(Action), target);
                    postLateUpdateCallbacks.Add(postLateUpdateCallback);
                    break;

#if UNITY_EDITOR
                case Segment.EditorUpdate:
                    var editorUpdateCallback = (Action)methodInfo.CreateDelegate(typeof(Action), target);
                    editorUpdateCallbacks.Add(editorUpdateCallback);
                    break;

                case Segment.EnteredEditMode:
                    var enteredEditModeCallback = (Action)methodInfo.CreateDelegate(typeof(Action), target);
                    enterEditModeDelegate.Add(enteredEditModeCallback);
                    break;

                case Segment.ExitingEditMode:
                    var exitingEditModeCallback = (Action)methodInfo.CreateDelegate(typeof(Action), target);
                    exitEditModeDelegate.Add(exitingEditModeCallback);
                    break;

                case Segment.EnteredPlayMode:
                    var enteredPlayModeCallback = (Action)methodInfo.CreateDelegate(typeof(Action), target);
                    enterPlayModeDelegate.Add(enteredPlayModeCallback);
                    break;

                case Segment.ExitingPlayMode:
                    var exitingPlayModeCallback = (Action)methodInfo.CreateDelegate(typeof(Action), target);
                    exitPlayModeDelegate.Add(exitingPlayModeCallback);
                    break;

                case Segment.BuildPreprocessor:
                    var buildPreprocessor =
                        (Action<BuildReportData>)methodInfo.CreateDelegate(typeof(Action<BuildReportData>), target);

                    buildPreprocessorCallbacks.Add(buildPreprocessor);
                    break;

                case Segment.BuildPostprocessor:
                    var buildPostprocessor =
                        (Action<BuildReportData>)methodInfo.CreateDelegate(typeof(Action<BuildReportData>), target);

                    buildPostprocessorCallbacks.Add(buildPostprocessor);
                    break;
#endif
            }
        }

        private static void RemoveCallbacksFromList<T>(IList<T> list, Object target) where T : Delegate
        {
            for (var index = list.Count - 1; index >= 0; index--)
            {
                if (ReferenceEquals(list[index].Target, target))
                {
                    list.RemoveAt(index);
                }
            }
        }

        private static CallbackMethodInfo GenerateCallbackMethodInfo(Type type)
        {
            if (!callbackMethodInfoCache.TryGetValue(type, out var callbackMethodInfo))
            {
                callbackMethodInfo = CallbackMethodInfo.Create(type);
                callbackMethodInfoCache.Add(type, callbackMethodInfo);
            }

            return callbackMethodInfo;
        }

        private struct CallbackMethodInfo
        {
            public bool HasMethods { get; set; }

            public List<(Segment segment, MethodInfo methodInfo)> SegmentMethods;
            public List<(string callback, MethodInfo methodInfo)> CustomMethods;

            public static CallbackMethodInfo Create(Type type)
            {
                var methods = type.GetMethodsIncludeBaseTypes(
                    BindingFlags.Instance |
                    BindingFlags.Public |
                    BindingFlags.NonPublic |
                    BindingFlags.DeclaredOnly |
                    BindingFlags.FlattenHierarchy);

                var data = new CallbackMethodInfo
                {
                    SegmentMethods = new List<(Segment segment, MethodInfo methodInfo)>(),
                    CustomMethods = new List<(string callback, MethodInfo methodInfo)>()
                };

                foreach (var methodInfo in methods)
                {
                    var attributes = methodInfo.GetCustomAttributes<CallbackMethodAttribute>(true);
                    foreach (var attribute in attributes)
                    {
                        if (attribute.Segment == Segment.Custom)
                        {
                            data.HasMethods = true;
                            data.CustomMethods.Add((attribute.Custom, methodInfo));
                        }
                        else
                        {
                            data.HasMethods = true;
                            data.SegmentMethods.Add((attribute.Segment, methodInfo));
                        }
                    }
                }

                return data;
            }
        }

        #endregion


        #region Shutdown

        private static void ShutdownInternal()
        {
            Debug.Log("Gameloop", "Quitting Application");
            Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.ExitPlaymode();
#endif
        }

        #endregion


        static Gameloop()
        {
            Application.quitting += () =>
            {
                IsQuitting = true;
                OnQuit();
            };
#if UNITY_EDITOR
            UnityEditor.EditorApplication.playModeStateChanged -= EditorApplicationOnplayModeStateChanged;
            UnityEditor.EditorApplication.playModeStateChanged += EditorApplicationOnplayModeStateChanged;
            UnityEditor.EditorApplication.update += OnEditorUpdate;
            UnityEditor.EditorApplication.projectWindowItemInstanceOnGUI += (_, _) => Segment = Segment.OnGUI;
#endif
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetCancellationTokenSource()
        {
            cancellationTokenSource?.Dispose();
            cancellationTokenSource = new CancellationTokenSource();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void OnBeforeFirstSceneLoaded()
        {
            Segment = Segment.BeforeFirstSceneLoad;
            IsQuitting = false;
#if !DISABLE_GAMELOOP_CALLBACKS
            monoBehaviour = RuntimeMonoBehaviourEvents.Create(
                OnStart,
                OnUpdate,
                OnLateUpdate,
                OnFixedUpdate,
                OnApplicationFocus,
                OnApplicationPause);

            EarlyUpdateEvents.Create(OnPreUpdate, OnPreLateUpdate);
            DelayedUpdateEvents.Create(OnPostUpdate, OnPostLateUpdate);
#endif
            for (var index = beforeFirstSceneLoadCallbacks.Count - 1; index >= 0; index--)
            {
                beforeFirstSceneLoadCallbacks[index]();
            }

            BeforeSceneLoadCompleted = true;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void OnAfterFirstSceneLoaded()
        {
            Segment = Segment.AfterFirstSceneLoad;
            for (var index = afterFirstSceneLoadCallbacks.Count - 1; index >= 0; index--)
            {
                afterFirstSceneLoadCallbacks[index]();
            }

            AfterSceneLoadCompleted = true;
        }

        private static void OnStart()
        {
#if DEBUG
            for (var index = firstUpdateCallbacks.Count - 1; index >= 0; index--)
            {
                try
                {
                    firstUpdateCallbacks[index]();
                }
                catch (Exception exception)
                {
                    Debug.LogException(logCategory, exception);
                }
            }
#else
            for (var index = firstUpdateCallbacks.Count - 1; index >= 0; index--)
            {
                firstUpdateCallbacks[index]();
            }
#endif
        }

        private static void OnPreUpdate()
        {
            Segment = Segment.PreUpdate;
#if DEBUG
            for (var index = preUpdateCallbacks.Count - 1; index >= 0; index--)
            {
                try
                {
                    preUpdateCallbacks[index]();
                }
                catch (Exception exception)
                {
                    Debug.LogException(logCategory, exception);
                }
            }
#else
            for (var index = preUpdateCallbacks.Count - 1; index >= 0; index--)
            {
                preUpdateCallbacks[index]();
            }
#endif
        }

        private static void OnPostUpdate()
        {
            Segment = Segment.PostUpdate;
#if DEBUG
            for (var index = postUpdateCallbacks.Count - 1; index >= 0; index--)
            {
                try
                {
                    postUpdateCallbacks[index]();
                }
                catch (Exception exception)
                {
                    Debug.LogException(logCategory, exception);
                }
            }
#else
            for (var index = postUpdateCallbacks.Count - 1; index >= 0; index--)
            {
                postUpdateCallbacks[index]();
            }
#endif
        }

        private static void OnPreLateUpdate()
        {
            Segment = Segment.PreLateUpdate;
#if DEBUG
            for (var index = preLateUpdateCallbacks.Count - 1; index >= 0; index--)
            {
                try
                {
                    preLateUpdateCallbacks[index]();
                }
                catch (Exception exception)
                {
                    Debug.LogException(logCategory, exception);
                }
            }
#else
            for (var index = preLateUpdateCallbacks.Count - 1; index >= 0; index--)
            {
                preLateUpdateCallbacks[index]();
            }
#endif
        }

        private static void OnPostLateUpdate()
        {
            Segment = Segment.PostLateUpdate;
#if DEBUG
            for (var index = postLateUpdateCallbacks.Count - 1; index >= 0; index--)
            {
                try
                {
                    postLateUpdateCallbacks[index]();
                }
                catch (Exception exception)
                {
                    Debug.LogException(logCategory, exception);
                }
            }
#else
            for (var index = postLateUpdateCallbacks.Count - 1; index >= 0; index--)
            {
                postLateUpdateCallbacks[index]();
            }
#endif
        }

        private static void OnUpdate()
        {
            Segment = Segment.Update;
#if DEBUG
            for (var index = updateCallbacks.Count - 1; index >= 0; index--)
            {
                try
                {
                    updateCallbacks[index]();
                }
                catch (Exception exception)
                {
                    Debug.LogException(logCategory, exception);
                }
            }
#else
            for (var index = updateCallbacks.Count - 1; index >= 0; index--)
            {
                updateCallbacks[index]();
            }
#endif
            OnSlowTickUpdate();
            OnTickUpdate();
        }

        private static void OnSlowTickUpdate()
        {
            if (OneSecondScaledTimer.IsRunning)
            {
                return;
            }

            OneSecondScaledTimer = ScaledTimer.FromSeconds(1);
#if DEBUG
            for (var index = slowTickUpdateCallbacks.Count - 1; index >= 0; index--)
            {
                try
                {
                    slowTickUpdateCallbacks[index]();
                }
                catch (Exception exception)
                {
                    Debug.LogException(logCategory, exception);
                }
            }
#else
            for (var index = slowTickUpdateCallbacks.Count - 1; index >= 0; index--)
            {
                slowTickUpdateCallbacks[index]();
            }
#endif
        }

        private static void OnTickUpdate()
        {
            if (TickScaledTimer.IsRunning)
            {
                return;
            }

            TickScaledTimer = ScaledTimer.FromSeconds(.1f);
#if DEBUG
            for (var index = tickUpdateCallbacks.Count - 1; index >= 0; index--)
            {
                try
                {
                    tickUpdateCallbacks[index]();
                }
                catch (Exception exception)
                {
                    Debug.LogException(logCategory, exception);
                }
            }
#else
            for (var index = tickUpdateCallbacks.Count - 1; index >= 0; index--)
            {
                tickUpdateCallbacks[index]();
            }
#endif
        }

        private static void OnLateUpdate()
        {
            Segment = Segment.LateUpdate;
#if DEBUG
            for (var index = lateUpdateCallbacks.Count - 1; index >= 0; index--)
            {
                try
                {
                    lateUpdateCallbacks[index]();
                }
                catch (Exception exception)
                {
                    Debug.LogException(logCategory, exception);
                }
            }
#else
            for (var index = lateUpdateCallbacks.Count - 1; index >= 0; index--)
            {
                lateUpdateCallbacks[index]();
            }
#endif
            try
            {
                UpdateTimeScale();
            }
            catch (Exception exception)
            {
                Debug.LogException(logCategory, exception);
            }
        }

        private static void OnFixedUpdate()
        {
            Segment = Segment.FixedUpdate;
#if DEBUG
            for (var index = fixedUpdateCallbacks.Count - 1; index >= 0; index--)
            {
                try
                {
                    fixedUpdateCallbacks[index]();
                }
                catch (Exception exception)
                {
                    Debug.LogException(logCategory, exception);
                }
            }
#else
            for (var index = fixedUpdateCallbacks.Count - 1; index >= 0; index--)
            {
                fixedUpdateCallbacks[index]();
            }
#endif
            FixedUpdateCount++;
        }

        private static void OnQuit()
        {
            Segment = Segment.ApplicationQuit;
            IsQuitting = true;
            for (var index = applicationQuitCallbacks.Count - 1; index >= 0; index--)
            {
                try
                {
                    applicationQuitCallbacks[index]();
                }
                catch (Exception exception)
                {
                    Debug.LogException(logCategory, exception);
                }
            }

            cancellationTokenSource.Cancel();
        }

        private static void OnApplicationFocus(bool hasFocus)
        {
            Segment = Segment.ApplicationFocus;
#if DEBUG
            for (var index = applicationFocusCallbacks.Count - 1; index >= 0; index--)
            {
                try
                {
                    applicationFocusCallbacks[index](hasFocus);
                }
                catch (Exception exception)
                {
                    Debug.LogException(logCategory, exception);
                }
            }
#else
            for (var index = applicationFocusCallbacks.Count - 1; index >= 0; index--)
            {
                applicationFocusCallbacks[index](hasFocus);
            }
#endif
        }

        private static void OnApplicationPause(bool pauseState)
        {
            Segment = Segment.ApplicationPause;
#if DEBUG
            for (var index = applicationPauseCallbacks.Count - 1; index >= 0; index--)
            {
                try
                {
                    applicationPauseCallbacks[index](pauseState);
                }
                catch (Exception exception)
                {
                    Debug.LogException(logCategory, exception);
                }
            }
#else
            for (var index = applicationPauseCallbacks.Count - 1; index >= 0; index--)
            {
                applicationPauseCallbacks[index](pauseState);
            }
#endif
        }

        private static void RaiseInitializationCompletedInternal()
        {
            Segment = Segment.InitializationCompleted;
            if (InitializationCompletedState)
            {
                return;
            }

            InitializationCompletedState = true;

#if DEBUG
            for (var index = initializationCompletedCallbacks.Count - 1; index >= 0; index--)
            {
                try
                {
                    initializationCompletedCallbacks[index]();
                }
                catch (Exception exception)
                {
                    Debug.LogException(logCategory, exception);
                }
            }
#else
            for (var index = initializationCompletedCallbacks.Count - 1; index >= 0; index--)
            {
                initializationCompletedCallbacks[index]();
            }
#endif
        }

        private static void RaiseCallbackInternal(string callbackName)
        {
            if (!customCallbacks.TryGetValue(callbackName, out var callbacks))
            {
                return;
            }
#if DEBUG
            for (var index = callbacks.Count - 1; index >= 0; index--)
            {
                try
                {
                    callbacks[index]();
                }
                catch (Exception exception)
                {
                    Debug.LogException(logCategory, exception);
                }
            }
#else
            for (var index = callbacks.Count - 1; index >= 0; index--)
            {
                callbacks[index]();
            }
#endif
        }

#if UNITY_EDITOR

        private static void OnEditorUpdate()
        {
            if (Application.isPlaying is false)
            {
                Segment = Segment.EditorUpdate;
            }
#if DEBUG
            for (var index = editorUpdateCallbacks.Count - 1; index >= 0; index--)
            {
                try
                {
                    editorUpdateCallbacks[index]();
                }
                catch (Exception exception)
                {
                    Debug.LogException(logCategory, exception);
                }
            }
#else
            for (var index = editorUpdateCallbacks.Count - 1; index >= 0; index--)
            {
                editorUpdateCallbacks[index]();
            }
#endif
        }

        private static void EditorApplicationOnplayModeStateChanged(UnityEditor.PlayModeStateChange state)
        {
            EditorState = (int)state;
            switch (state)
            {
                case UnityEditor.PlayModeStateChange.EnteredEditMode:
                    OnEnterEditMode();
                    break;

                case UnityEditor.PlayModeStateChange.ExitingEditMode:
                    OnExitEditMode();
                    break;

                case UnityEditor.PlayModeStateChange.EnteredPlayMode:
                    OnEnterPlayMode();
                    break;

                case UnityEditor.PlayModeStateChange.ExitingPlayMode:
                    OnExitPlayMode();
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }

        private static void OnExitPlayMode()
        {
            IsQuitting = true;
            for (var i = 0; i < exitPlayModeDelegate.Count; i++)
            {
                if (exitPlayModeDelegate[i] == null)
                {
                    continue;
                }

                try
                {
                    exitPlayModeDelegate[i]();
                }
                catch (Exception exception)
                {
                    Debug.LogException(exception);
                }
            }
        }

        private static void OnEnterPlayMode()
        {
            for (var i = 0; i < enterPlayModeDelegate.Count; i++)
            {
                if (enterPlayModeDelegate[i] == null)
                {
                    continue;
                }

                try
                {
                    enterPlayModeDelegate[i]();
                }
                catch (Exception exception)
                {
                    Debug.LogException(exception);
                }
            }
        }

        private static void OnExitEditMode()
        {
            for (var i = 0; i < exitEditModeDelegate.Count; i++)
            {
                if (exitEditModeDelegate[i] == null)
                {
                    continue;
                }

                try
                {
                    exitEditModeDelegate[i]();
                }
                catch (Exception exception)
                {
                    Debug.LogException(exception);
                }
            }

            FixedUpdateCount = 0;
        }

        private static void OnEnterEditMode()
        {
            IsQuitting = false;
            BeforeSceneLoadCompleted = false;
            AfterSceneLoadCompleted = false;
            InitializationCompletedState = false;

            for (var i = 0; i < enterEditModeDelegate.Count; i++)
            {
                if (enterEditModeDelegate[i] == null)
                {
                    continue;
                }

                try
                {
                    enterEditModeDelegate[i]();
                }
                catch (Exception exception)
                {
                    Debug.LogException(exception);
                }
            }

            FixedUpdateCount = 0;
        }

        internal static void RaiseBuildReportPreprocessor(BuildReportData reportData)
        {
            for (var index = buildPreprocessorCallbacks.Count - 1; index >= 0; index--)
            {
                buildPreprocessorCallbacks[index](reportData);
            }
        }

        internal static void RaiseBuildReportPostprocessor(BuildReportData reportData)
        {
            for (var index = buildPostprocessorCallbacks.Count - 1; index >= 0; index--)
            {
                buildPostprocessorCallbacks[index](reportData);
            }
        }
#endif
    }
}