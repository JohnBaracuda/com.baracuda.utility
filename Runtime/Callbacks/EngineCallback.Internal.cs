using System;
using System.Collections.Generic;
using UnityEngine;

namespace Baracuda.Utilities.Callbacks
{
    public partial class EngineCallbacks
    {
        #region Update Callbacks

        private static readonly List<UpdateDelegate> updateDelegates = new(8);
        private static readonly List<LateUpdateDelegate> lateUpdateDelegates = new(8);
        private static readonly List<FixedUpdateDelegate> fixedUpdateDelegates = new(8);

        private static readonly List<IOnUpdate> updateCallbacks = new(8);
        private static readonly List<IOnLateUpdate> lateUpdateCallbacks = new(8);
        private static readonly List<IOnFixedUpdate> fixedUpdateCallbacks = new(8);

        private static void OnUpdate()
        {
#if DEBUG
            var deltaTime = Time.deltaTime;
            foreach (var listener in updateCallbacks)
            {
                try
                {
                    listener.OnUpdate(deltaTime);
                }
                catch (Exception exception)
                {
                    Debug.LogException(exception);
                }
            }
            foreach (var listener in updateDelegates)
            {
                try
                {
                    listener(deltaTime);
                }
                catch (Exception exception)
                {
                    Debug.LogException(exception);
                }
            }
#else
            var deltaTime = Time.deltaTime;
            foreach (var listener in updateCallbacks)
            {
                listener.OnUpdate(deltaTime);
            }
            foreach (var listener in updateDelegates)
            {
                listener(deltaTime);
            }
#endif
        }

        private static void OnLateUpdate()
        {
#if DEBUG
            var deltaTime = Time.deltaTime;
            foreach (var listener in lateUpdateCallbacks)
            {
                try
                {
                    listener.OnLateUpdate(deltaTime);
                }
                catch (Exception exception)
                {
                    Debug.LogException(exception);
                }
            }
            foreach (var listener in lateUpdateDelegates)
            {
                try
                {
                    listener(deltaTime);
                }
                catch (Exception exception)
                {
                    Debug.LogException(exception);
                }
            }
#else
            var deltaTime = Time.deltaTime;
            foreach (var listener in lateUpdateCallbacks)
            {
                listener.OnLateUpdate(deltaTime);
            }
            foreach (var listener in lateUpdateDelegates)
            {
                listener(deltaTime);
            }
#endif


        }

        private static void OnFixedUpdate()
        {
#if DEBUG
            var deltaTime = Time.fixedDeltaTime;
            foreach (var listener in fixedUpdateCallbacks)
            {
                try
                {
                    listener.OnFixedUpdate(deltaTime);
                }
                catch (Exception exception)
                {
                    Debug.LogException(exception);
                }
            }
            foreach (var listener in fixedUpdateDelegates)
            {
                try
                {
                    listener(deltaTime);
                }
                catch (Exception exception)
                {
                    Debug.LogException(exception);
                }
            }
#else
            var deltaTime = Time.fixedDeltaTime;
            foreach (var listener in fixedUpdateCallbacks)
            {
                listener.OnFixedUpdate(deltaTime);
            }
            foreach (var listener in fixedUpdateDelegates)
            {
                listener(deltaTime);
            }
#endif
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void SetupUpdateCallbacks()
        {
            RuntimeCallbacks.Create(OnUpdate, OnLateUpdate, OnFixedUpdate);
        }

        #endregion
    }
}