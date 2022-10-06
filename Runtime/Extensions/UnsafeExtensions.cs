using System.Runtime.CompilerServices;
using Unity.Collections.LowLevel.Unsafe;

namespace Baracuda.Utilities.Extensions
{
    public static class UnsafeExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T2 ConvertUnsafe<T1, T2>(this T1 value)
        {
            return UnsafeUtility.As<T1, T2>(ref value);
        }
    }
}