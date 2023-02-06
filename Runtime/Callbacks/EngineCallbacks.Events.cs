using System;

namespace Baracuda.Utilities.Callbacks
{
    public partial class EngineCallbacks
    {
        #region Update Events

        public delegate void UpdateDelegate(float deltaTime);

        public delegate void LateUpdateDelegate(float deltaTime);

        public delegate void FixedUpdateDelegate(float fixedDeltaTime);

        public static event UpdateDelegate Update
        {
            add => updateDelegates.AddNullChecked(value);
            remove => updateDelegates.Remove(value);
        }

        public static event LateUpdateDelegate LateUpdate
        {
            add => lateUpdateDelegates.AddNullChecked(value);
            remove => lateUpdateDelegates.Remove(value);
        }

        public static event FixedUpdateDelegate FixedUpdate
        {
            add => fixedUpdateDelegates.AddNullChecked(value);
            remove => fixedUpdateDelegates.Remove(value);
        }

        #endregion


        #region Play State Events

        public static event Action EnteredEditMode
        {
            add
            {
#if UNITY_EDITOR
                enterEditModeDelegate.AddNullChecked(value);
#endif
            }
            remove
            {
#if UNITY_EDITOR
                enterEditModeDelegate.Remove(value);
#endif
            }
        }

        public static event Action ExitedEditMode
        {
            add
            {
#if UNITY_EDITOR
                exitEditModeDelegate.AddNullChecked(value);
#endif
            }
            remove
            {
#if UNITY_EDITOR
                exitEditModeDelegate.Remove(value);
#endif
            }
        }

        public static event Action EnteredPlayMode
        {
            add
            {
#if UNITY_EDITOR
                enterPlayModeDelegate.AddNullChecked(value);
#endif
            }
            remove
            {
#if UNITY_EDITOR
                enterPlayModeDelegate.Remove(value);
#endif
            }
        }

        public static event Action ExitedPlayMode
        {
            add
            {
#if UNITY_EDITOR
                exitPlayModeDelegate.AddNullChecked(value);
#endif
            }
            remove
            {
#if UNITY_EDITOR
                exitPlayModeDelegate.Remove(value);
#endif
            }
        }

        #endregion
    }
}