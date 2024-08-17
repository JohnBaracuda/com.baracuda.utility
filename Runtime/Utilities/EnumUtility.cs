using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Pool;

namespace Baracuda.Bedrock.Utilities
{
    public static class EnumUtility
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasFlagFast(int lhs, int rhs)
        {
            return (lhs & rhs) > 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasFlagFast<TEnum>(TEnum lhs, TEnum rhs) where TEnum : unmanaged, Enum
        {
            return UnsafeUtility.SizeOf<TEnum>() switch
            {
                1 => (UnsafeUtility.As<TEnum, byte>(ref lhs) & UnsafeUtility.As<TEnum, byte>(ref rhs)) > 0,
                2 => (UnsafeUtility.As<TEnum, ushort>(ref lhs) & UnsafeUtility.As<TEnum, ushort>(ref rhs)) > 0,
                4 => (UnsafeUtility.As<TEnum, uint>(ref lhs) & UnsafeUtility.As<TEnum, uint>(ref rhs)) > 0,
                8 => (UnsafeUtility.As<TEnum, ulong>(ref lhs) & UnsafeUtility.As<TEnum, ulong>(ref rhs)) > 0,
                _ => throw new Exception($"Size of {typeof(TEnum).Name} does not match a known Enum backing type.")
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasAnyFlagFast<TEnum>(TEnum lhs, TEnum rhs) where TEnum : unmanaged, Enum
        {
            return UnsafeUtility.SizeOf<TEnum>() switch
            {
                1 => (UnsafeUtility.As<TEnum, byte>(ref lhs) & UnsafeUtility.As<TEnum, byte>(ref rhs)) != 0,
                2 => (UnsafeUtility.As<TEnum, ushort>(ref lhs) & UnsafeUtility.As<TEnum, ushort>(ref rhs)) != 0,
                4 => (UnsafeUtility.As<TEnum, uint>(ref lhs) & UnsafeUtility.As<TEnum, uint>(ref rhs)) != 0,
                8 => (UnsafeUtility.As<TEnum, ulong>(ref lhs) & UnsafeUtility.As<TEnum, ulong>(ref rhs)) != 0,
                _ => throw new Exception($"Size of {typeof(TEnum).Name} does not match a known Enum backing type.")
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Equals<T>(ref T lhs, ref T rhs) where T : unmanaged, Enum
        {
            return UnsafeUtility.EnumEquals(lhs, rhs);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T[] GetValueArray<T>() where T : unmanaged, Enum
        {
            return Enum.GetValues(typeof(T)).Cast<T>().ToArray();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T[] GetFlagsValueArray<T>(T flagsEnum) where T : unmanaged, Enum
        {
            var buffer = ListPool<T>.Get();

            foreach (var flag in GetValueArray<T>())
            {
                if (HasFlagFast(flagsEnum, flag))
                {
                    buffer.Add(flag);
                }
            }

            var result = buffer.ToArray();
            ListPool<T>.Release(buffer);
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsDefined<T>(T enumValue) where T : unmanaged, Enum
        {
            return Enum.IsDefined(typeof(T), enumValue);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ToInt<T>(T value) where T : unmanaged, Enum
        {
            return UnsafeUtility.As<T, int>(ref value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T FromInt<T>(int value) where T : unmanaged, Enum
        {
            return UnsafeUtility.As<int, T>(ref value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T FromString<T>(string value) where T : unmanaged, Enum
        {
            return Enum.TryParse(typeof(T), value, out var result) ? (T)result : default;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryParse<T>(string text, out T result) where T : unmanaged, Enum
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                result = default;
                return false;
            }
            if (Enum.TryParse(typeof(T), text, out var enumObject))
            {
                result = (T)enumObject;
                return true;
            }

            result = default;
            return false;
        }
    }
}