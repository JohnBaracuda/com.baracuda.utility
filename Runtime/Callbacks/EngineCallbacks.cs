using System.Diagnostics;

namespace Baracuda.Utilities.Callbacks
{
    public partial class EngineCallbacks
    {
        #region Callbacks

        public static void AddCallbacks<T>(T listener) where T : class
        {
            if (listener is IOnUpdate onUpdate)
            {
                AddUpdateListener(onUpdate);
            }
            if (listener is IOnLateUpdate onLateUpdate)
            {
                AddLateUpdateListener(onLateUpdate);
            }
            if (listener is IOnFixedUpdate onFixedUpdate)
            {
                AddFixedUpdateListener(onFixedUpdate);
            }
#if UNITY_EDITOR
            AddOnExitPlayInternal(listener as IOnExitPlay);
            AddOnEnterPlayInternal(listener as IOnEnterPlay);
            AddOnExitEditInternal(listener as IOnExitEdit);
            AddOnEnterEditInternal(listener as IOnEnterEdit);
#endif
        }

        public static void RemoveCallbacks<T>(T listener) where T : class
        {
            if (listener is IOnUpdate onUpdate)
            {
                RemoveUpdateListener(onUpdate);
            }
            if (listener is IOnLateUpdate onLateUpdate)
            {
                RemoveLateUpdateListener(onLateUpdate);
            }
            if (listener is IOnFixedUpdate onFixedUpdate)
            {
                RemoveFixedUpdateListener(onFixedUpdate);
            }
#if UNITY_EDITOR
            RemoveOnExitPlayInternal(listener as IOnExitPlay);
            RemoveOnEnterPlayInternal(listener as IOnEnterPlay);
            RemoveOnExitEditInternal(listener as IOnExitEdit);
            RemoveOnEnterEditInternal(listener as IOnEnterEdit);
#endif
        }

        #endregion


        #region Add Update Callbacks

        public static void AddUpdateListener<T>(T listener) where T : class, IOnUpdate
        {
            updateCallbacks.AddNullChecked(listener);
        }

        public static void AddLateUpdateListener<T>(T listener) where T : class, IOnLateUpdate
        {
            lateUpdateCallbacks.AddNullChecked(listener);
        }

        public static void AddFixedUpdateListener<T>(T listener) where T : class, IOnFixedUpdate
        {
            fixedUpdateCallbacks.AddNullChecked(listener);
        }

        #endregion


        #region Remove Update Callbacks

        public static void RemoveUpdateListener<T>(T listener) where T : class, IOnUpdate
        {
            updateCallbacks.Remove(listener);
        }

        public static void RemoveLateUpdateListener<T>(T listener) where T : class, IOnLateUpdate
        {
            lateUpdateCallbacks.Remove(listener);
        }

        public static void RemoveFixedUpdateListener<T>(T listener) where T : class, IOnFixedUpdate
        {
            fixedUpdateCallbacks.Remove(listener);
        }

        #endregion


        #region Add State Callbacks

        [Conditional("UNITY_EDITOR")]
        public static void AddExitPlayModeListener<T>(T listener) where T : class, IOnExitPlay
        {
#if UNITY_EDITOR
            AddOnExitPlayInternal(listener);
#endif
        }

        [Conditional("UNITY_EDITOR")]
        public static void AddEnterPlayModeListener<T>(T listener) where T : class, IOnEnterPlay
        {
#if UNITY_EDITOR
            AddOnEnterPlayInternal(listener);
#endif
        }

        [Conditional("UNITY_EDITOR")]
        public static void AddExitEditModeListener<T>(T listener) where T : class, IOnExitEdit
        {
#if UNITY_EDITOR
            AddOnExitEditInternal(listener);
#endif
        }

        [Conditional("UNITY_EDITOR")]
        public static void AddEnterEditModeListener<T>(T listener) where T : class, IOnEnterEdit
        {
#if UNITY_EDITOR
            AddOnEnterEditInternal(listener);
#endif
        }

        #endregion


        #region Remove State Callbacks

        [Conditional("UNITY_EDITOR")]
        public static void RemoveExitPlaymodeListener<T>(T listener) where T : class, IOnExitPlay
        {
#if UNITY_EDITOR
            RemoveOnExitPlayInternal(listener);
#endif
        }

        [Conditional("UNITY_EDITOR")]
        public static void RemoveEnterPlaymodeListener<T>(T listener) where T : class, IOnEnterPlay
        {
#if UNITY_EDITOR
            RemoveOnEnterPlayInternal(listener);
#endif
        }

        [Conditional("UNITY_EDITOR")]
        public static void RemoveExitEditModeListener<T>(T listener) where T : class, IOnExitEdit
        {
#if UNITY_EDITOR
            RemoveOnExitEditInternal(listener);
#endif
        }

        [Conditional("UNITY_EDITOR")]
        public static void RemoveEnterEditModeListener<T>(T listener) where T : class, IOnEnterEdit
        {
#if UNITY_EDITOR
            RemoveOnEnterEditInternal(listener);
#endif
        }

        #endregion
    }
}