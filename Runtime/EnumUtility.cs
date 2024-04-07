using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections.LowLevel.Unsafe;

namespace Baracuda.Utilities
{
    public static class EnumUtility
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasFlagFast<TEnum>(this TEnum lhs, TEnum rhs) where TEnum : unmanaged, Enum
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
        public static bool HasAnyFlagFast<TEnum>(this TEnum lhs, TEnum rhs) where TEnum : unmanaged, Enum
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

        public static bool HasFlagFast(this int lhs, int rhs)
        {
            return (lhs & rhs) > 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool Equals<T>(ref T lhs, ref T rhs) where T : unmanaged, Enum
        {
            return UnsafeUtility.EnumEquals(lhs, rhs);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool EqualsAny<T>(this T target, params T[] other) where T : unmanaged, Enum
        {
            for (var i = 0; i < other.Length; i++)
            {
                if (Equals(ref target, ref other[i]))
                {
                    return true;
                }
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool EqualsAny<T>(this T target, T otherA, T otherB, T otherC) where T : unmanaged, Enum
        {
            return Equals(ref target, ref otherA) || Equals(ref target, ref otherB) || Equals(ref target, ref otherC);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool EqualsAny<T>(this T target, T otherA, T otherB) where T : unmanaged, Enum
        {
            return Equals(ref target, ref otherA) || Equals(ref target, ref otherB);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool EnumEquals<T>(this T target, T other) where T : unmanaged, Enum
        {
            return Equals(ref target, ref other);
        }

        /*
         *  Equals All
         */

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool EqualsAll<T>(this T target, params T[] other) where T : unmanaged, Enum
        {
            for (var i = 0; i < other.Length; i++)
            {
                if (!Equals(ref target, ref other[i]))
                {
                    return false;
                }
            }

            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool EqualsAll<T>(this T target, T otherA, T otherB, T otherC) where T : unmanaged, Enum
        {
            return Equals(ref target, ref otherA) && Equals(ref target, ref otherB) && Equals(ref target, ref otherC);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool EqualsAll<T>(this T target, T otherA, T otherB) where T : unmanaged, Enum
        {
            return Equals(ref target, ref otherA) && Equals(ref target, ref otherB);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool EqualsAll<T>(this T target, T otherA) where T : unmanaged, Enum
        {
            return Equals(ref target, ref otherA);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<T> GetValues<T>() where T : unmanaged, Enum
        {
            return EnumUtility<T>.GetValueArray();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T[] GetValueArray<T>() where T : unmanaged, Enum
        {
            return EnumUtility<T>.GetValueArray();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T[] GetFlagsValueArray<T>(this T flagsEnum) where T : unmanaged, Enum
        {
            return EnumUtility<T>.GetFlagsValueArray(flagsEnum);
        }
    }
}