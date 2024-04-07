using JetBrains.Annotations;
using System;
using System.Collections.Generic;

namespace Baracuda.Utilities
{
    public class ArrayUtility
    {
        public static void Add<T>([NotNull] ref T[] array, params T[] other)
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
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
                throw new ArgumentNullException(nameof(array));
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