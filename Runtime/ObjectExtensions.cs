using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
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

        /// <summary>
        ///     Converts the target <see cref="object" /> to be of the specified <see cref="Type" />
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object ConvertTo(this Object[] value, Type type)
        {
            if (value != null)
            {
                try
                {
                    return Convert.ChangeType(value, type);
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNot<T>(this T target, params T[] other) where T : class
        {
            foreach (var element in other)
            {
                if (target == element)
                {
                    return false;
                }
            }
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Is<T>(this T target, T other) where T : class
        {
            return target == other;
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


        #region GUID

        public static Guid CombineWith(this Guid guid, long objectIdentifier)
        {
            var guidBytes = guid.ToByteArray();
            var longBytes = BitConverter.GetBytes(objectIdentifier);

            // Ensure both byte arrays are of the expected lengths
            if (guidBytes.Length != 16 || longBytes.Length != 8)
            {
                throw new ArgumentException("Unexpected byte array Length.");
            }

            // Combine both arrays into a 16-byte array
            var combinedBytes = new byte[16];

            // Copy the first 8 bytes from the long
            Array.Copy(longBytes, 0, combinedBytes, 0, 8);

            // Copy the first 8 bytes from the guid
            Array.Copy(guidBytes, 0, combinedBytes, 8, 8);

            // Create a new GUID from the combined bytes
            return new Guid(combinedBytes);
        }

        public static Guid CreateGuidFromText(this string text)
        {
            // Compute the hash of the input text
            using var sha256 = SHA256.Create();
            var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(text));

            // Ensure the hash is at least 16 bytes long
            if (hashBytes.Length < 16)
            {
                throw new ArgumentException("Hash is too short to create a GUID.");
            }

            // Use the first 16 bytes of the hash to create a GUID
            var guidBytes = new byte[16];
            Array.Copy(hashBytes, guidBytes, 16);

            return new Guid(guidBytes);
        }

        public static string CreateGuidStringFromArguments(params object[] arguments)
        {
            var text = arguments.Aggregate("", (current, argument) => current + argument);
            return CreateGuidFromText(text).ToString();
        }

        #endregion


        #region Delegate

        public static void InvokeCritical(this Action action)
        {
            foreach (var @delegate in action.GetInvocationList())
            {
                try
                {
                    @delegate.DynamicInvoke();
                }
                catch (Exception exception)
                {
                    Debug.Log(exception);
                }
            }
        }

        #endregion
    }
}