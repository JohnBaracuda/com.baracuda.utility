using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Pool;

namespace Baracuda.Utilities
{
#if UNITY_EDITOR
    [UnityEditor.InitializeOnLoadAttribute]
#endif
    public static class EnumUtility<T> where T : unmanaged, Enum
    {
        private static readonly T[] values;
        public static readonly EqualityComparer<T> Comparer = EqualityComparer<T>.Default;

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Equals(T value1, T value2)
        {
            return Comparer.Equals(value1, value2);
        }

        static EnumUtility()
        {
            values = Enum.GetValues(typeof(T)).Cast<T>().ToArray();
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ToInt(T value)
        {
            return UnsafeUtility.As<T, int>(ref value);
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T FromInt(int value)
        {
            return UnsafeUtility.As<int, T>(ref value);
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T FromString(string value)
        {
            return Enum.TryParse(typeof(T), value, out var result) ? (T)result : default;
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T[] GetValueArray()
        {
            return values;
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T[] GetFlagsValueArray(T flagsEnum)
        {
            var buffer = ListPool<T>.Get();

            foreach (var flag in GetValueArray())
            {
                if (flagsEnum.HasFlagFast(flag))
                {
                    buffer.Add(flag);
                }
            }

            var result = buffer.ToArray();
            ListPool<T>.Release(buffer);
            return result;
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsDefined(T enumValue)
        {
            return Enum.IsDefined(typeof(T), enumValue);
        }
    }
}