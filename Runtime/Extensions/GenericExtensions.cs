using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Baracuda.Utilities
{
    public static class ObjectExtensions
    {
        #region Equals Any

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


        #region Equals None

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


        #region Equals All

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


        #region Convert

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


        #region Conditions

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Not(this bool value)
        {
            return !value;
        }

        #endregion


        #region Null Checks

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
    }
}
