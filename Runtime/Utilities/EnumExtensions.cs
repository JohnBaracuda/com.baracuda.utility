using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Baracuda.Bedrock.Utilities
{
    public static class EnumExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasFlagFast<TEnum>(this TEnum lhs, TEnum rhs) where TEnum : unmanaged, Enum
        {
            return EnumUtility.HasFlagFast(lhs, rhs);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasAnyFlagFast<TEnum>(this TEnum lhs, TEnum rhs) where TEnum : unmanaged, Enum
        {
            return EnumUtility.HasAnyFlagFast(lhs, rhs);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool Equals<T>(this T target, T other) where T : unmanaged, Enum
        {
            return EnumUtility.Equals(ref target, ref other);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool EqualsAny<T>(this T target, params T[] other) where T : unmanaged, Enum
        {
            foreach (var value in other)
            {
                if (target.Equals(value))
                {
                    return true;
                }
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool EqualsAny<T>(this T target, T otherA, T otherB, T otherC) where T : unmanaged, Enum
        {
            return target.Equals(otherA) || target.Equals(otherB) || target.Equals(otherC);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool EqualsAny<T>(this T target, T otherA, T otherB) where T : unmanaged, Enum
        {
            return target.Equals(otherA) || target.Equals(otherB);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool EqualsAll<T>(this T target, params T[] other) where T : unmanaged, Enum
        {
            for (var i = 0; i < other.Length; i++)
            {
                if (!target.Equals(other[i]))
                {
                    return false;
                }
            }

            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool EqualsAll<T>(this T target, T otherA, T otherB, T otherC) where T : unmanaged, Enum
        {
            return target.Equals(otherA) && target.Equals(otherB) && target.Equals(otherC);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool EqualsAll<T>(this T target, T otherA, T otherB) where T : unmanaged, Enum
        {
            return target.Equals(otherA) && target.Equals(otherB);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool EqualsAll<T>(this T target, T otherA) where T : unmanaged, Enum
        {
            return target.Equals(otherA);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<T> GetValues<T>() where T : unmanaged, Enum
        {
            return EnumUtility.GetValueArray<T>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T[] GetFlagsValueArray<T>(this T flagsEnum) where T : unmanaged, Enum
        {
            return EnumUtility.GetFlagsValueArray(flagsEnum);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsDefined<T>(this T enumValue) where T : unmanaged, Enum
        {
            return EnumUtility.IsDefined(enumValue);
        }
    }
}