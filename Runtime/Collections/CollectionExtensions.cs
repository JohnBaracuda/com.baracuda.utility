using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Baracuda.Utility.Pools;
using Baracuda.Utility.Utilities;
using JetBrains.Annotations;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

namespace Baracuda.Utility.Collections
{
    public static class CollectionExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddNullChecked<T>(this HashSet<T> hashSet, T value)
        {
            if (value != null)
            {
                hashSet.Add(value);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int InstanceCount<T>(this IEnumerable<T> enumerable, T element)
        {
            var count = 0;
            foreach (var entry in enumerable)
            {
                if (EqualityComparer<T>.Default.Equals(entry, element))
                {
                    count++;
                }
            }

            return count;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RemoveWhere<T>(this IList<T> source, Func<T, bool> predicate)
        {
            for (var index = source.Count - 1; index >= 0; index--)
            {
                if (predicate(source[index]))
                {
                    source.RemoveAt(index);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<T> TakeLast<T>(this IList<T> source, int amount)
        {
            return source.Skip(Math.Max(0, source.Count - amount));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            foreach (var item in enumerable)
            {
                action(item);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ForEach<T>(this T[] array, Action<T> action)
        {
            for (var i = 0; i < array.Length; i++)
            {
                action(array[i]);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNullOrEmpty<T>(this T[] array)
        {
            return array is not { Length: > 0 };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNotNullOrEmpty<T>(this T[] array)
        {
            return array is { Length: > 0 };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNullOrEmpty<T>(this IList<T> list)
        {
            return list is not { Count: > 0 };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNullOrHasNullElements<T>(this ICollection<T> list)
        {
            if (list == null)
            {
                return true;
            }
            foreach (var element in list)
            {
                if (element == null)
                {
                    return true;
                }
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNotNullOrEmpty<T>(this IReadOnlyList<T> list)
        {
            return list is { Count: > 0 };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetElementAt<T>(this T[] array, int index, out T element)
        {
            if (array.Length > index)
            {
                element = array[index];
                return true;
            }
            element = default;
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetElementAt<T>(this IList<T> list, int index, out T element)
        {
            if (list.Count > index)
            {
                element = list[index];
                return true;
            }
            element = default;
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool RemoveDuplicates<T>(this IList<T> list)
        {
            var hasDuplicates = false;
            var set = HashSetPool<T>.Get();

            for (var i = list.Count - 1; i >= 0; i--)
            {
                if (!set.Add(list[i]))
                {
                    hasDuplicates = true;
                    list.RemoveAt(i);
                }
            }

            HashSetPool<T>.Release(set);
            return hasDuplicates;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RemoveNull<T>(this IList<T> list)
        {
            for (var i = list.Count - 1; i >= 0; i--)
            {
                if (list[i] == null)
                {
                    list.RemoveAt(i);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static List<T> WithoutNullItems<T>(this IList<T> list)
        {
            var result = new List<T>();
            for (var i = list.Count - 1; i >= 0; i--)
            {
                if (list[i].IsNull())
                {
                    continue;
                }
                result.Add(list[i]);
            }
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AddNullChecked<T>(this IList<T> list, T value) where T : class
        {
            if (value == null)
            {
                return false;
            }

            list.Add(value);
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AddUnique<T>([NotNull] this IList<T> list, T value, bool nullCheck = false)
        {
            if (list == null)
            {
                throw new ArgumentNullException(nameof(list));
            }
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
        public static bool EnqueueUnique<T>([NotNull] this Queue<T> queue, T value)
        {
            if (queue == null)
            {
                throw new ArgumentNullException(nameof(queue));
            }

            if (queue.Contains(value))
            {
                return false;
            }

            queue.Enqueue(value);
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Remove<T>(this Queue<T> queue, T element)
        {
            if (queue == null)
            {
                throw new ArgumentNullException(nameof(queue));
            }

            var removed = false;
            var initialCount = queue.Count;
            var tempQueue = QueuePool<T>.Get();

            for (var i = 0; i < initialCount; i++)
            {
                var item = queue.Dequeue();
                if (!removed && EqualityComparer<T>.Default.Equals(item, element))
                {
                    removed = true;
                }
                else
                {
                    tempQueue.Enqueue(item);
                }
            }

            while (tempQueue.Count > 0)
            {
                queue.Enqueue(tempQueue.Dequeue());
            }

            QueuePool<T>.Release(tempQueue);
            return removed;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void EnqueueRange<T>(this Queue<T> queue, IEnumerable<T> range)
        {
            if (queue == null)
            {
                throw new ArgumentNullException(nameof(queue));
            }
            foreach (var item in range)
            {
                queue.Enqueue(item);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void UpdateEntry<T>([NotNull] this IList<T> list, T value, bool nullCheck = false)
        {
            if (list == null)
            {
                throw new ArgumentNullException(nameof(list));
            }
            if (nullCheck && value == null)
            {
                return;
            }

            var index = list.IndexOf(value);
            if (index > -1)
            {
                list.RemoveAt(index);
            }

            list.Add(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AddUnique<TKey, TValue>([NotNull] this IDictionary<TKey, TValue> target, TKey key,
            TValue value)
        {
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }
            if (!target.TryAdd(key, value))
            {
                return false;
            }

            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddOrUpdate<TKey, TValue>(this IDictionary<TKey, TValue> target, TKey key, TValue value)
        {
            if (target == null)
            {
                return;
            }

            if (target.ContainsKey(key))
            {
                target[key] = value;
                return;
            }

            target.Add(key, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddOrUpdate<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key,
            Func<TValue, TValue> updateFunc)
        {
            if (dictionary.ContainsKey(key))
            {
                dictionary[key] = updateFunc(dictionary[key]);
            }
            else
            {
                dictionary.Add(key, updateFunc(default));
            }
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

        /// <summary>
        ///     Remove all null objects from a dictionary.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RemoveNullItems<TKey, TValue>(this IDictionary<TKey, TValue> target) where TValue : class
        {
            var invalidKeys = ListPool<TKey>.Get();
            foreach (var (key, value) in target)
            {
                if (value.IsNull())
                {
                    invalidKeys.Add(key);
                }
            }

            foreach (var invalidKey in invalidKeys)
            {
                target.Remove(invalidKey);
            }

            ListPool<TKey>.Release(invalidKeys);
        }

        /// <summary>
        ///     Adds an object to a list and returns the same list. This allows method chaining.
        ///     <example>list.Append(item1).Append(item2);</example>
        /// </summary>
        public static List<T> Append<T>(this List<T> list, T value)
        {
            list.Add(value);
            return list;
        }

        /// <summary>
        ///     Returns true if the enumeration contains no elements.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsEmpty<T>(this IEnumerable<T> enumerable)
        {
            return !enumerable.Any();
        }

        /// <summary>
        ///     Returns a string, appending string representation of every element in the collection.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToCollectionString<T>(this IEnumerable<T> enumerable, string separator = "\n")
        {
            if (enumerable == null)
            {
                return "null";
            }
            var stringBuilder = StringBuilderPool.Get();
            foreach (var item in enumerable)
            {
                var text = item.ToString();
                stringBuilder.Append(text);
                stringBuilder.Append(separator);
            }

            return StringBuilderPool.BuildAndRelease(stringBuilder);
        }

        /// <summary>
        ///     Returns a string, appending string representation of every element in the collection.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToCollectionString<T>(this IEnumerable<T> enumerable, char separator)
        {
            if (enumerable == null)
            {
                return "null";
            }
            var stringBuilder = StringBuilderPool.Get();
            foreach (var item in enumerable)
            {
                var text = item.ToString();
                stringBuilder.Append(text);
                stringBuilder.Append(separator);
            }

            return StringBuilderPool.BuildAndRelease(stringBuilder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TValue ValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key,
            TValue defaultValue = default)
        {
            return dictionary.TryGetValue(key, out var value) ? value : defaultValue;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int IndexOf<T>(this T[] array, T element)
        {
            if (array == null)
            {
                return -1;
            }
            var comparer = EqualityComparer<T>.Default;
            for (var i = 0; i < array.Length; i++)
            {
                if (comparer.Equals(array[i], element))
                {
                    return i;
                }
            }
            return -1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryRemove<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key,
            out TValue value)
        {
            if (dictionary.Remove(key, out value))
            {
                return true;
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryRemove<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
        {
            if (dictionary.ContainsKey(key))
            {
                dictionary.Remove(key);
                return true;
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TValue Pop<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
        {
            var value = dictionary[key];
            dictionary.Remove(key);

            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TValue GetOrCreate<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
            where TValue : new()
        {
            if (!dictionary.TryGetValue(key, out var value))
            {
                value = new TValue();
                dictionary.Add(key, value);
            }
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddRange<T>(this HashSet<T> hashSet, IEnumerable<T> collection)
        {
            hashSet.UnionWith(collection);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ReleaseToPool<T>(this List<T> list)
        {
            ListPool<T>.Release(list);
        }

        public static bool TryGetValueAs<TKey, TValue, TResult>(this IDictionary<TKey, TValue> dictionary, TKey key,
            out TResult result)
        {
            if (dictionary.TryGetValue(key, out var value) && value is TResult casted)
            {
                result = casted;
                return true;
            }

            result = default;
            return false;
        }

        public static void PopulateBufferWithUniqueRandomIndices(this List<int> resultBuffer, int maxIndex,
            int indicesToSelect)
        {
            if (maxIndex <= 0 || resultBuffer == null || indicesToSelect > maxIndex)
            {
                throw new ArgumentException("Invalid arguments provided");
            }

            var allIndices = ListPool<int>.Get();
            for (var i = 0; i < maxIndex; ++i)
            {
                allIndices.Add(i);
            }

            for (var i = maxIndex - 1; i > 0; i--)
            {
                var j = Random.Range(0, i + 1);
                (allIndices[i], allIndices[j]) = (allIndices[j], allIndices[i]);
            }

            for (var i = 0; i < indicesToSelect; i++)
            {
                resultBuffer.Add(allIndices[i]);
            }

            ListPool<int>.Release(allIndices);
        }
    }
}