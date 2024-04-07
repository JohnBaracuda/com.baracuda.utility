using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine.Pool;

namespace Baracuda.Utilities
{
#if UNITY_EDITOR
    [UnityEditor.InitializeOnLoadAttribute]
#endif
    public static class EnumUtility<T> where T : unmanaged, Enum
    {
        private static readonly T[] values;

        static EnumUtility()
        {
            values = Enum.GetValues(typeof(T)).Cast<T>().ToArray();
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
    }
}