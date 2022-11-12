using System;
using System.Runtime.CompilerServices;
using Unity.Collections.LowLevel.Unsafe;

namespace Baracuda.Utilities
{
    /// <summary>
    /// Class includes extensions and optimized functionality for enum operations.
    /// </summary>
    public static class EnumHelper
    {
        /*
         *  Unsafe
         */
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasFlagUnsafe<TEnum>(this TEnum lhs, TEnum rhs) where TEnum : unmanaged, Enum =>
            UnsafeUtility.SizeOf<TEnum>() switch
            {
                1 => (UnsafeUtility.As<TEnum, byte>(ref lhs) & UnsafeUtility.As<TEnum, byte>(ref rhs)) > 0,
                2 => (UnsafeUtility.As<TEnum, ushort>(ref lhs) & UnsafeUtility.As<TEnum, ushort>(ref rhs)) > 0,
                4 => (UnsafeUtility.As<TEnum, uint>(ref lhs) & UnsafeUtility.As<TEnum, uint>(ref rhs)) > 0,
                8 => (UnsafeUtility.As<TEnum, ulong>(ref lhs) & UnsafeUtility.As<TEnum, ulong>(ref rhs)) > 0,
                _ => throw new Exception($"Size of {typeof(TEnum).Name} does not match a known Enum backing type.")
            };

        public static bool HasFlagInt(this int lhs, int rhs)
        {
            return (lhs & rhs) > 0;
        }
                
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Equals<T>(ref T lhs, ref T rhs) where T : unmanaged, Enum
            => UnsafeUtility.EnumEquals(lhs, rhs);
        
        /*
         *  Equals Any   
         */
        
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
            => Equals(ref target, ref otherA) || Equals(ref target, ref otherB) || Equals(ref target, ref otherC);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool EqualsAny<T>(this T target, T otherA, T otherB) where T : unmanaged, Enum
            => Equals(ref target, ref otherA) || Equals(ref target, ref otherB);

        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool EnumEquals<T>(this T target, T other) where T : unmanaged, Enum
            => Equals(ref target, ref other);

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
            => Equals(ref target, ref otherA) && Equals(ref target, ref otherB) && Equals(ref target, ref otherC);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool EqualsAll<T>(this T target, T otherA, T otherB) where T : unmanaged, Enum
            => Equals(ref target, ref otherA) && Equals(ref target, ref otherB);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool EqualsAll<T>(this T target, T otherA) where T : unmanaged, Enum
            => Equals(ref target, ref otherA);
    }
}