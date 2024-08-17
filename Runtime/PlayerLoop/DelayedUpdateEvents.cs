using System;
using Baracuda.Bedrock.Utilities;
using JetBrains.Annotations;
using UnityEngine;

namespace Baracuda.Bedrock.PlayerLoop
{
    [ExecutionOrder(10000)]
    internal sealed class DelayedUpdateEvents : MonoBehaviour
    {
        private Action _onUpdate;
        private Action _onLateUpdate;

        internal static DelayedUpdateEvents Create([NotNull] Action onUpdate, [NotNull] Action onLateUpdate)
        {
            if (onUpdate == null)
            {
                throw new ArgumentNullException(nameof(onUpdate));
            }
            if (onLateUpdate == null)
            {
                throw new ArgumentNullException(nameof(onLateUpdate));
            }
            var gameObject = new GameObject(nameof(DelayedUpdateEvents));
            var instance = gameObject.AddComponent<DelayedUpdateEvents>();
            gameObject.DontDestroyOnLoad();
            gameObject.hideFlags |= HideFlags.HideInHierarchy;

            instance._onUpdate = onUpdate;
            instance._onLateUpdate = onLateUpdate;
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
    }
}