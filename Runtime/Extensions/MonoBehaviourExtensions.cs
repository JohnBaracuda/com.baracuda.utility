
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Baracuda.Utilities.Extensions
{
    public static class MonoBehaviourExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetActive<TComponent>(this TComponent component, bool active) where TComponent : Component
        {
            component.gameObject.SetActive(active);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetParent<TComponent>(this TComponent component, Transform parent) where TComponent : Component
        {
            component.transform.SetParent(parent);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DontDestroyOnLoad<TComponent>(this TComponent component, bool setParent = true) where TComponent : Component
        {
            if (setParent)
            {
                component.SetParent(null);
            }
            Object.DontDestroyOnLoad(component);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DontDestroyOnLoad(this GameObject gameObject, bool setParent = true)
        {
            if (setParent)
            {
                gameObject.transform.SetParent(null);
            }
            Object.DontDestroyOnLoad(gameObject);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetGameObjectName<TComponent>(this TComponent component, string name) where TComponent : Component
        {
            component.gameObject.name = name;
        }

        public static bool IsPrefab(this GameObject gameObject)
        {
            return gameObject.scene.rootCount == 0;
        }
    }
}
