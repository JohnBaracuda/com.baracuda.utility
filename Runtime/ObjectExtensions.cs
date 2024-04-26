using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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
            return !target.Equals(otherA);
        }

        #endregion


        #region Equals All

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool EqualsAll<T>(this T target, params T[] other)
        {
            var comparer = EqualityComparer<T>.Default;
            foreach (var obj in other)
            {
                if (!comparer.Equals(target, obj))
                {
                    return false;
                }
            }

            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool EqualsAll<T>(this T target, T otherA, T otherB, T otherC)
        {
            var comparer = EqualityComparer<T>.Default;
            return comparer.Equals(target, otherA) && comparer.Equals(target, otherB) &&
                   comparer.Equals(target, otherC);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool EqualsAll<T>(this T target, T otherA, T otherB)
        {
            var comparer = EqualityComparer<T>.Default;
            return comparer.Equals(target, otherA) && comparer.Equals(target, otherB);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool EqualsAll<T>(this T target, T otherA)
        {
            var comparer = EqualityComparer<T>.Default;
            return comparer.Equals(target, otherA);
        }

        #endregion


        #region Convert

        /// <summary>
        ///     Converts the target <see cref="object" /> to be of the specified <see cref="Type" />
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T ConvertTo<T>(this object value)
        {
            if (value is not null)
            {
                return (T) Convert.ChangeType(value, typeof(T));
            }

            return default(T);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T CastExplicit<T>(this object value)
        {
            return (T) value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T TryCast<T>(this object value) where T : new()
        {
            try
            {
                return (T) value;
            }
            catch (Exception)
            {
                return new T();
            }
        }

        /// <summary>
        ///     Converts the target <see cref="object" /> to be of the specified <see cref="Type" />
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


        #region Null Checks

        /// <summary>
        ///     Unity object sensitive null check.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNull<T>(this T target)
        {
            return target is Object obj ? obj == null : target == null;
        }

        /// <summary>
        ///     Unity object sensitive null check.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T OrNull<T>(this T target) where T : class
        {
            if (target == null)
            {
                return null;
            }

            return target;
        }

        /// <summary>
        ///     Unity object sensitive null check.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNotNull<T>(this T target) where T : class
        {
            return target is Object obj ? obj != null : target != null;
        }

        /// <summary>
        ///     Unity object sensitive null check.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNotNull<T>(this T target, out T result) where T : class
        {
            result = target;
            return target is Object obj ? obj != null : target != null;
        }

        /// <summary>
        ///     Unity object sensitive null check.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsObjectNotNull<T>(this T target) where T : Object
        {
            return target is Object obj ? obj != null : target != null;
        }

        /// <summary>
        ///     Unity object sensitive null check.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNull(this Object target)
        {
            return target == null;
        }

        /// <summary>
        ///     Unity object sensitive null check.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNotNull(this Object target)
        {
            return target != null;
        }

        /// <summary>
        ///     Returns the targets string representation or a string indicating that the target is null.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToNullString<T>(this T target) where T : class
        {
            return target == null ? "null" : target.ToString();
        }

        #endregion
    }
}