using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Baracuda.Utilities.Collections.Extensions
{
    public static class Collection
    {
        #region Dictionary

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Dictionary<TKey, TValue> CreateEnumDictionary<TKey, TValue>(Func<TKey, TValue> valueFunc, bool ignoreSpecialNames = false) where TKey : Enum
        {
            var dictionary = new Dictionary<TKey, TValue>();

            foreach (TKey key in Enum.GetValues(typeof(TKey)))
            {
                if (ignoreSpecialNames && IsEnumSpecialName(key))
                {
                    continue;
                }

                dictionary.Add(key, valueFunc(key));
            }

            return dictionary;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Dictionary<TKey, TValue> CreateEnumDictionary<TKey, TValue>(Func<TValue> valueFunc, bool ignoreSpecialNames = false) where TKey : Enum
        {
            var dictionary = new Dictionary<TKey, TValue>();

            foreach (TKey key in Enum.GetValues(typeof(TKey)))
            {
                if (ignoreSpecialNames && IsEnumSpecialName(key))
                {
                    continue;
                }

                dictionary.Add(key, valueFunc());
            }

            return dictionary;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Dictionary<TKey, TValue> CreateEnumDictionary<TKey, TValue>(bool ignoreSpecialNames = false) where TKey : Enum
        {
            var dictionary = new Dictionary<TKey, TValue>();

            foreach (TKey key in Enum.GetValues(typeof(TKey)))
            {
                if (ignoreSpecialNames && IsEnumSpecialName(key))
                {
                    continue;
                }

                dictionary.Add(key, default);
            }

            return dictionary;
        }


        /// <summary>
        /// Returns true if the name of the enum is either "None" or "Everything" for flags enums
        /// </summary>
        public static bool IsEnumSpecialName<TKey>(this TKey key)
        {
            var name = key.ToString();
            return name.Equals("None") || name.Equals("Everything");
        }

        #endregion
    }
}