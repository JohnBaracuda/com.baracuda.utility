using System;
using UnityEngine;

namespace Baracuda.Utilities.Callbacks
{
    [DisallowMultipleComponent]
    internal sealed class RuntimeCallbacks : MonoBehaviour
    {
        private Action _onUpdate;
        private Action _onLateUpdate;
        private Action _onFixedUpdate;

        internal static RuntimeCallbacks Create(Action onUpdate, Action onLateUpdate, Action onFixedUpdate)
        {
            var gameObject = new GameObject(nameof(RuntimeCallbacks));
            var instance = gameObject.AddComponent<RuntimeCallbacks>();
            gameObject.DontDestroyOnLoad();
            gameObject.hideFlags |= HideFlags.HideInHierarchy;

            instance._onUpdate = onUpdate;
            instance._onLateUpdate = onLateUpdate;
            instance._onFixedUpdate = onFixedUpdate;
            return instance;
        }

        private void Update()
        {
            _onUpdate();
        }

        private void LateUpdate()
        {
            _onLateUpdate();
        }

        private void FixedUpdate()
        {
            _onFixedUpdate();
        }
    }
}