using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Baracuda.Utilities.Types;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

namespace Baracuda.Utilities
{
    public static class RandomUtility
    {
        [MustUseReturnValue]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Bool()
        {
            return Random.value > .5f;
        }

        [MustUseReturnValue]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Bool(float percentage)
        {
            return Random.value < percentage * .01f;
        }

        [MustUseReturnValue]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Index<T>(T[] array)
        {
            if (array.Length <= 0)
            {
                return -1;
            }

            return Int(0, array.Length - 1);
        }

        [MustUseReturnValue]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Int(int min, int max)
        {
            return Random.Range(min, max);
        }

        [MustUseReturnValue]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Int(int max)
        {
            return Random.Range(0, max);
        }

        [MustUseReturnValue]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Int64()
        {
            return (long)Random.Range(int.MinValue, int.MaxValue) + Random.Range(int.MinValue, int.MaxValue);
        }

        [MustUseReturnValue]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Int(Range range)
        {
            return Random.Range(range.Start.Value, range.End.Value);
        }

        [MustUseReturnValue]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Float(float min = float.MinValue, float max = float.MaxValue)
        {
            return Random.Range(min, max);
        }

        [MustUseReturnValue]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 CreateRandomDeviationVector(Vector3 originalVector, float deviationAmount = .5f)
        {
            originalVector.Normalize();

            var deviationVector = new Vector3(Random.Range(-deviationAmount, deviationAmount),
                Random.Range(-deviationAmount, deviationAmount), Random.Range(-deviationAmount, deviationAmount)
            );

            var randomVector = originalVector + deviationVector;
            randomVector.Normalize();
            return randomVector;
        }

        [MustUseReturnValue]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 Vector()
        {
            return new Vector3(Float(-1, 1), Float(-1, 1), Float(-1, 1));
        }

        [MustUseReturnValue]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 Vector(float minX, float maxX, float minY, float maxY, float minZ, float maxZ)
        {
            return new Vector3(Float(minX, maxX), Float(minY, maxY), Float(minZ, maxZ));
        }

        [MustUseReturnValue]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 Vector(Vector3 min, Vector3 max)
        {
            return new Vector3(Float(min.x, max.x), Float(min.y, max.y), Float(min.z, max.z));
        }

        [MustUseReturnValue]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 Vector(float x, float y, float z)
        {
            return new Vector3(Float(-x, x), Float(-y, y), Float(-z, z));
        }

        [MustUseReturnValue]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 Vector(float value)
        {
            return new Vector3(Float(-value, value), Float(-value, value), Float(-value, value));
        }

        [MustUseReturnValue]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 VectorNormalized()
        {
            return new Vector3(Float(-1, 1), Float(-1, 1), Float(-1, 1)).normalized;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T FromSelection<T>([NotNull] T[] selection)
        {
            Assert.IsNotNull(selection);
            var index = Int(0, selection.Length);
            return selection[index];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T GetRandomItem<T>([NotNull] IReadOnlyList<T> source)
        {
            Assert.IsNotNull(source);

            if (source.Count == 0)
            {
                return default;
            }

            if (source.Count == 1)
            {
                return source[0];
            }

            var randomIndex = Random.Range(0, source.Count - 1);
            return source[randomIndex];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T GetRandomItemWithExceptionOf<T>([NotNull] IReadOnlyList<T> source, T itemToIgnore)
        {
            Assert.IsNotNull(source);

            if (source.Count == 1)
            {
                return source[0];
            }

            if (source.Count == 2)
            {
                if (source[0].Equals(itemToIgnore))
                {
                    return source[1];
                }

                if (source[1].Equals(itemToIgnore))
                {
                    return source[0];
                }

                return GetRandomItem(source);
            }

            var attemptLimit = 10;

            while (attemptLimit > 0)
            {
                var randomIndex = Random.Range(0, source.Count);
                var randomItem = source[randomIndex];

                if (randomItem.Equals(itemToIgnore))
                {
                    attemptLimit--;
                }
                else
                {
                    return randomItem;
                }
            }

            return GetRandomItem(source);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T GetAndRemoveRandomItem<T>([NotNull] IList<T> source)
        {
            Assert.IsNotNull(source);

            var randomIndex = Random.Range(0, source.Count);
            var item = source[randomIndex];
            source.RemoveAt(randomIndex);
            return item;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T[] GetRandomItems<T>([NotNull] IList<T> source, int count)
        {
            Assert.IsNotNull(source);
            Assert.IsTrue(source.Count >= count);

            var buffer = HashSetPool<T>.Get();

            while (buffer.Count < count)
            {
                buffer.Add(source[Random.Range(0, source.Count)]);
            }

            var selection = buffer.ToArray();
            HashSetPool<T>.Release(buffer);
            return selection;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static List<T> GetRandomItemList<T>([NotNull] IList<T> source, int count)
        {
            Assert.IsNotNull(source);
            Assert.IsTrue(source.Count >= count);

            var buffer = HashSetPool<T>.Get();

            while (buffer.Count < count)
            {
                buffer.Add(source[Random.Range(0, source.Count)]);
            }

            var selection = buffer.ToList();
            HashSetPool<T>.Release(buffer);
            return selection;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RandomItemsNoAlloc<T>([NotNull] IList<T> source, int count, [NotNull] ref List<T> results)
        {
            Assert.IsNotNull(source);
            Assert.IsNotNull(results);
            Assert.IsTrue(source.Count >= count);

            results.Clear();

            while (results.Count < count)
            {
                results.Add(source[Random.Range(0, source.Count)]);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RandomUniqueItemsNoAlloc<T>([NotNull] IList<T> source, [NotNull] ref List<T> results,
            int count)
        {
            Assert.IsNotNull(source);
            Assert.IsNotNull(results);
            Assert.IsTrue(source.Count >= count);

            results.Clear();
            var buffer = ListPool<T>.Get();
            buffer.AddRange(source);

            for (var index = buffer.Count - 1; index > 0; index--)
            {
                var randomIndex = Random.Range(0, index + 1);

                (buffer[index], buffer[randomIndex]) = (buffer[randomIndex], buffer[index]);
            }

            for (var i = 0; i < count; i++)
            {
                results.Add(buffer[i]);
            }

            ListPool<T>.Release(buffer);
        }

        private static readonly Color[] loggingColors =
        {
            new(0.4f, 0.7f, 0.7f), // Soft teal
            new(0.5f, 0.8f, 0.5f), // Soft green
            new(0.5f, 0.7f, 0.9f), // Soft blue
            new(0.9f, 0.5f, 0.6f), // Soft pink
            new(0.7f, 0.6f, 0.5f), // Warm beige
            new(0.7f, 0.7f, 0.7f), // Light grey
            new(0.6f, 0.4f, 0.7f), // Soft purple
            new(0.6f, 0.7f, 0.6f), // Grey
            new(0.9f, 0.7f, 0.5f), // Peach
            new(0.8f, 0.8f, 0.4f), // Soft yellow
            new(0.6f, 0.7f, 0.3f), // Olive green
            new(0.4f, 0.5f, 0.6f), // Slate
            new(0.6f, 0.5f, 0.4f), // Muted brown
            new(0.7f, 0.8f, 0.9f), // Light blue
            new(0.8f, 0.7f, 0.8f), // Lavender
            new(0.3f, 0.6f, 0.6f), // Darker teal
            new(0.5f, 0.5f, 0.7f), // Dusky blue
            new(0.4f, 0.4f, 0.5f), // Dark grey blue
            new(0.7f, 0.4f, 0.4f), // Soft red
            new(0.8f, 0.5f, 0.4f) // Soft orange
        };

        private static Loop? colorIndex;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color LoggingColor()
        {
            colorIndex ??= Loop.Create(loggingColors);
            var loop = colorIndex.Value;
            var color = loggingColors[loop];
            colorIndex = ++loop;
            return color;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void InitializeRandomUtility()
        {
            colorIndex = null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Name()
        {
            return GetRandomItem(randomNames);
        }

        private static readonly string[] randomNames =
        {
            "Tracer Melone",
            "Anna Bolik",
            "Smitty Werben",
            "Sugondese",
            "Joe Ligma",
            "Stun Amongus",
            "Al Beback",
            "Dan Druff",
            "Gene Poole",
            "I.P. Freely",
            "Justin Case",
            "Moe Lester",
            "Hugh Jass",
            "Ben Dover",
            "Anita Bath",
            "Gabe Itch",
            "Jim Nasium",
            "Willie Stroker",
            "Stu Pidman",
            "Al Coholic",
            "Sal Monella",
            "Farrah Moan",
            "Shanda Lear",
            "Faye King",
            "Ella Vator",
            "Robin Banks",
            "Bob Sled",
            "Warren Peace",
            "Doug Graves",
            "Ray Sin",
            "Mark Mywords"
        };
    }
}