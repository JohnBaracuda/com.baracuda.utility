using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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
        public static int Int(int min = int.MinValue, int max = int.MaxValue)
        {
            return Random.Range(min, max);
        }

        [MustUseReturnValue]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Int64()
        {
            return (long) Random.Range(int.MinValue, int.MaxValue) + Random.Range(int.MinValue, int.MaxValue);
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
            if (source.Count == 1)
            {
                return source[0];
            }

            var randomIndex = Random.Range(0, source.Count);
            return source[randomIndex];
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
    }
}