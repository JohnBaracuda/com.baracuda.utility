using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Baracuda.Utilities.Collections.Extensions
{
    public static class CollectionExtensions
    {
        /*
         *  Foreach loop
         */

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> handling)
        {
            foreach (var item in enumerable)
            {
                handling(item);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ForEach<T>(this T[] array, Action<T> handling)
        {
            for (var i = 0; i < array.Length; i++)
            {
                var item = array[i];
                handling(item);
            }
        }

        /*
         *  Null checks
         */

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNullOrEmpty<T>(this T[] array)
        {
            return array is not {Length: > 0};
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNotNullOrEmpty<T>(this T[] array)
        {
            return array is {Length: > 0};
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNullOrEmpty<T>(this IList<T> list)
        {
            return list is not {Count: > 0};
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNotNullOrEmpty<T>(this IList<T> list)
        {
            return list is {Count: > 0};
        }

        /*
         *  Try get
         */

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetElementAt<T>(this T[] array, int index, out T element)
        {
            if (array.Length > index)
            {
                element = array[index];
                return true;
            }
            else
            {
                element = default;
                return false;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetElementAt<T>(this IList<T> list, int index, out T element)
        {
            if (list.Count > index)
            {
                element = list[index];
                return true;
            }
            else
            {
                element = default;
                return false;
            }
        }

        /*
         *  Add Unique
         */

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RemoveDuplicates<T>(this IList<T> list)
        {
            var set = new HashSet<T>();

            for (var i = list.Count - 1; i >= 0; i--)
            {
                if (!set.Add(list[i]))
                {
                    list.RemoveAt(i);
                }
            }
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AddUnique<T>(this IList<T> list, T value, bool nullCheck = false)
        {
            if (nullCheck && value == null)
            {
                return false;
            }

            if (list.Contains(value))
            {
                return false;
            }

            list.Add(value);
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddUnique<TKey, TValue>(this IDictionary<TKey, TValue> target, TKey key, TValue value)
        {
            if (target.ContainsKey(key))
            {
                return;
            }

            target.Add(key, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddTyped<TValue>(this IDictionary<Type, TValue> target, TValue value)
        {
            var key = value.GetType();
            if (target.ContainsKey(key))
            {
                target[key] = value;
            }

            target.Add(key, value);
        }

        /*
         * Add Appending
         */

        public static List<T> Adhere<T>(this List<T> list, T value)
        {
            list.Add(value);
            return list;
        }

        /*
         * IEnumerable
         */

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool None<T>(this IEnumerable<T> enumerable)
        {
            return !enumerable.Any();
        }
    }
}
