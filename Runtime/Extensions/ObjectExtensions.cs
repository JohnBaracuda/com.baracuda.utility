using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Baracuda.Utilities.Collections.Extensions;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Baracuda.Utilities.Extensions
{
    public static class ObjectExtensions
    {
        #region --- Equals Any ---

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool EqualsAny<T>(this T target, params T[] other) where T : class
        {
            foreach (var obj in other)
            {
                if (target.Equals(obj))
                {
                    return true;
                }
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool EqualsAny<T>(this T target, T otherA, T otherB, T otherC) where T : class
        {
            return target.Equals(otherA) || target.Equals(otherB) || target.Equals(otherC);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool EqualsAny<T>(this T target, T otherA, T otherB) where T : class
        {
            return target.Equals(otherA) || target.Equals(otherB);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool EqualsAny<T>(this T target, T otherA) where T : class
        {
            return target.Equals(otherA);
        }

        #endregion

        #region --- Equals None ---

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool EqualsNone<T>(this T target, params T[] other) where T : class
        {
            foreach (var obj in other)
            {
                if (target.Equals(obj))
                {
                    return false;
                }
            }

            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool EqualsNone<T>(this T target, T otherA, T otherB, T otherC) where T : class
        {
            return !(target.Equals(otherA) || target.Equals(otherB) || target.Equals(otherC));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool EqualsNone<T>(this T target, T otherA, T otherB) where T : class
        {
            return !(target.Equals(otherA) || target.Equals(otherB));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool EqualsNone<T>(this T target, T otherA) where T : class
        {
            return !(target.Equals(otherA));
        }

        #endregion

        #region --- Equals All ---

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool EqualsAll<T>(this T target, params T[] other) where T : class
        {
            foreach (var obj in other)
            {
                if (!target.Equals(obj))
                {
                    return false;
                }
            }
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool EqualsAll<T>(this T target, T otherA, T otherB, T otherC) where T : class
        {
            return target.Equals(otherA) && target.Equals(otherB) && target.Equals(otherC);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool EqualsAll<T>(this T target, T otherA, T otherB) where T : class
        {
            return target.Equals(otherA) && target.Equals(otherB);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool EqualsAll<T>(this T target, T otherA) where T : class
        {
            return target.Equals(otherA);
        }

        #endregion

        #region --- Convert ---

        /// <summary>
        /// Converts the target <see cref="object"/> to be of the specified <see cref="Type"/>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T ConvertTo<T>(this object value)
        {
            if (value is not null)
            {
                return (T) Convert.ChangeType(value, typeof(T));
            }

            return default;
        }

        public static T Cast<T>(this object target)
        {
            return (T) target;
        }

        /// <summary>
        ///
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T As<T>(this object value)
        {
            return (T) value;
        }

        /// <summary>
        /// Converts the target <see cref="object"/> to be of the specified <see cref="Type"/>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T[] ConvertTo<T>(this Object[] value) where T : Object
        {
            if (value != null)
            {
                try
                {
                    return Array.ConvertAll(value, input => input as T);
                }
                catch (Exception exception)
                {
                    Debug.LogException(exception);
                }
            }

            return null;
        }

        #endregion

        #region --- Conditions ---

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Not(this bool value)
        {
            return !value;
        }

        #endregion

        #region --- Null Checks ---

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNull<T>(this T target) where T : class
        {
            return target == null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNotNull<T>(this T target) where T : class
        {
            return target != null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNotNull(this Object target)
        {
            return target != null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNotNull(this GameObject target)
        {
            return target != null;
        }

        #endregion

        #region --- Editor ---

        [Conditional("UNITY_EDITOR")]
        public static void SetObjectDirty(this Object target)
        {
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(target);
#endif
        }

        #endregion

        #region --- Components & GameObjects ---

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetDontDestroyOnLoad(this Object target)
        {
            Object.DontDestroyOnLoad(target);
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

        #region --- Filtering ---

        public static bool IsValidForFilterString(this Object obj, string filter)
        {
            if (obj.name.Contains(filter, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            if (obj.GetType().Name.Contains(filter, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            if (obj.GetType().BaseType?.Name.Contains(filter, StringComparison.OrdinalIgnoreCase) ?? false)
            {
                return true;
            }

            return false;
        }

        #endregion
    }
}
