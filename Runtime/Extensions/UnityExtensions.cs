using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Baracuda.Utilities
{
    public static class UnityExtensions
    {
        #region Component

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetActive<TComponent>(this TComponent component, bool active) where TComponent : Component
        {
            component.gameObject.SetActive(active);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetParent<TComponent>(this TComponent component, Transform parent)
            where TComponent : Component
        {
            component.transform.SetParent(parent);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DontDestroyOnLoad<TComponent>(this TComponent component)
            where TComponent : Component
        {
            component.SetParent(null);
            Object.DontDestroyOnLoad(component);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsActive<TComponent>(this TComponent component) where TComponent : Component
        {
            return component.gameObject.activeSelf;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsActiveInHierarchy<TComponent>(this TComponent component) where TComponent : Component
        {
            return component.gameObject.activeInHierarchy;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNotActive<TComponent>(this TComponent component) where TComponent : Component
        {
            return !component.gameObject.activeSelf;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNotActiveInHierarchy<TComponent>(this TComponent component) where TComponent : Component
        {
            return !component.gameObject.activeInHierarchy;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DestroyGameObject<TComponent>(this TComponent component) where TComponent : Component
        {
            Object.Destroy(component.gameObject);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DestroyGameObject<TComponent>(this TComponent component, float secondsDelay) where TComponent : Component
        {
            Object.Destroy(component.gameObject, secondsDelay);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetComponents<TComponent>(this GameObject gameObject, out TComponent[] components) where TComponent : Component
        {
            components = gameObject.GetComponents<TComponent>();
            return components.IsNotNullOrEmpty();
        }

        #endregion


        #region GameObject

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
        public static bool IsPrefab(this GameObject gameObject)
        {
            return gameObject.scene.rootCount == 0;
        }

        #endregion


        #region Object

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsSelected(this Object obj)
        {
#if UNITY_EDITOR
            return UnityEditor.Selection.activeObject == obj;
#else
            return false;
#endif
        }

        [Conditional("UNITY_EDITOR")]
        public static void SetObjectDirty(this Object target)
        {
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(target);
#endif
        }

        #endregion
    }
}
