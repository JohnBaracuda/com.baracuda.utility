using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;

namespace Baracuda.Utilities
{
    public static class MathExtensions
    {
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsPositive(this int value)
        {
            return value > 0;
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNegative(this int value)
        {
            return value < 0;
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsPositive(this float value)
        {
            return value > 0;
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNegative(this float value)
        {
            return value < 0;
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsZero(this int value)
        {
            return value == 0;
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsZero(this float value)
        {
            return value == 0;
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNotZero(this float value)
        {
            return value != 0;
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNotZero(this int value)
        {
            return value != 0;
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int RoundToInt(this float origin)
        {
            return Mathf.RoundToInt(origin);
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Clamp(this int origin, int min, int max)
        {
            return Mathf.Clamp(origin, min, max);
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Min(this int origin, int min)
        {
            return origin < min ? min : origin;
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Max(this int origin, int max)
        {
            return origin > max ? max : origin;
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsEven(this int n)
        {
            return (n ^ 1) == n + 1;
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsOdd(this int n)
        {
            return (n ^ 1) != n + 1;
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsBinarySequenceExcludeZero(this int n)
        {
            return n > 0 && (n & (n - 1)) == 0;
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsBinarySequence(this int n)
        {
            return (n & (n - 1)) == 0;
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float CheckNan(this float value, float fallback)
        {
            return float.IsNaN(value) ? fallback : value;
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Clamp(this float value, float min, float max)
        {
            return Mathf.Clamp(value, min, max);
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int WithMaxLimit(this int value, int limit)
        {
            return Mathf.Min(value, limit);
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float WithMaxLimit(this float value, float limit)
        {
            return Mathf.Min(value, limit);
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int WithMinLimit(this int value, int min)
        {
            return Mathf.Max(value, min);
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float WithMinLimit(this float value, float min)
        {
            return Mathf.Max(value, min);
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CeilToInt(this float value)
        {
            return Mathf.CeilToInt(value);
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int FloorToInt(this float value)
        {
            return Mathf.FloorToInt(value);
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsInfinity(this float value)
        {
            return float.IsInfinity(value);
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsInRange(this float value, float min, float max)
        {
            return value >= min && value <= max;
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ApproximatelyEquals(this double rhs, double lhs, double acceptableDifference)
        {
            return math.abs(rhs - lhs) <= acceptableDifference;
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ApproximatelyEquals(this float rhs, float lhs, float acceptableDifference)
        {
            return math.abs(rhs - lhs) <= acceptableDifference;
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ApproximatelyEquals(this int rhs, int lhs, int acceptableDifference)
        {
            return math.abs(rhs - lhs) <= acceptableDifference;
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 Clamp(this Vector2 target, Vector2 min, Vector2 max)
        {
            return new Vector2(
                Mathf.Clamp(target.x, min.x, max.x),
                Mathf.Clamp(target.y, min.y, max.y));
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 Min(this Vector2 target, Vector2 other)
        {
            return Vector2.Min(target, other);
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 Max(this Vector2 target, Vector2 other)
        {
            return Vector2.Max(target, other);
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasNan(this Vector3 vector3)
        {
            return float.IsNaN(vector3.x) || float.IsNaN(vector3.y) || float.IsNaN(vector3.z);
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 ToVector4(this Quaternion quaternion)
        {
            return new Vector4(quaternion.x, quaternion.y, quaternion.z, quaternion.w);
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float MillisecondsToSeconds(this int milliseconds)
        {
            return milliseconds * 0.001f;
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SecondsToMilliseconds(this float seconds)
        {
            return (int) (seconds * 1000);
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SecondsToMilliseconds(this int seconds)
        {
            return seconds * 1000;
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 ToFloat3(this Vector3 vector3)
        {
            return new float3(vector3.x, vector3.y, vector3.z);
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float GetPercentage(this float value, float percentage)
        {
            return value * (percentage * .01f);
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float GetPercentage(this byte value, float percentage)
        {
            return value * (percentage * .01f);
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float GetPercentage(this int value, float percentage)
        {
            return value * (percentage * .01f);
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 With(this Vector3 value, float? x = null, float? y = null, float? z = null)
        {
            value.x = x ?? value.x;
            value.y = y ?? value.y;
            value.z = z ?? value.z;
            return value;
        }
    }
}