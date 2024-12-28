using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace Baracuda.Utility.Utilities
{
    public class ArrayUtility
    {
        public static void Remove<T>(ref T[] array, params T[] remove)
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }
            if (remove == null || remove.Length == 0)
            {
                return;
            }

            var hashSet = new HashSet<T>(remove);
            var list = new List<T>();

            foreach (var item in array)
            {
                if (!hashSet.Contains(item))
                {
                    list.Add(item);
                }
            }

            array = list.ToArray();
        }

        public static void Add<T>([NotNull] ref T[] array, params T[] other)
        {
            if (array == null)
            {
                array = other;
                return;
            }
            if (other == null)
            {
                return;
            }

            var originalToLength = array.Length;
            Array.Resize(ref array, originalToLength + other.Length);
            Array.Copy(other, 0, array, originalToLength, other.Length);
        }

        public static void Add<T>([NotNull] ref T[] array, ICollection<T> collection)
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }
            if (collection == null)
            {
                return;
            }

            var originalToLength = array.Length;
            Array.Resize(ref array, originalToLength + collection.Count);
            collection.CopyTo(array, originalToLength);
        }

        public static void Add<T>([NotNull] ref T[] array, T item)
        {
            if (array == null)
            {
                array = new[] { item };
                return;
            }
            var originalToLength = array.Length;
            Array.Resize(ref array, originalToLength + 1);
            array[originalToLength] = item;
        }

        public static void AddUnique<T>([NotNull] ref T[] array, T item)
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }
            if (array.Contains(item))
            {
                return;
            }
            var originalToLength = array.Length;
            Array.Resize(ref array, originalToLength + 1);
            array[originalToLength] = item;
        }

        public static TTo[] Cast<TFrom, TTo>(TFrom[] array)
        {
            if (array == null)
            {
                return Array.Empty<TTo>();
            }

            var result = new TTo[array.Length];
            for (var i = 0; i < array.Length; i++)
            {
                result[i] = array[i].CastExplicit<TTo>();
            }

            return result;
        }
    }
}