using System;
using System.Runtime.CompilerServices;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using static Unity.Mathematics.math;

namespace Baracuda.Bedrock.Types
{
    public struct NativeCurve : IDisposable
    {
        public bool IsCreated => _values.IsCreated;

        private NativeArray<float> _values;
        private WrapMode _preWrapMode;
        private WrapMode _postWrapMode;
        private readonly Allocator _allocator;

        public NativeCurve(AnimationCurve curve, int resolution, Allocator allocator = Allocator.Persistent)
        {
            _preWrapMode = curve.preWrapMode;
            _postWrapMode = curve.postWrapMode;
            _allocator = allocator;

            _values = new NativeArray<float>(resolution, _allocator, NativeArrayOptions.UninitializedMemory);

            Update(curve, resolution);
        }

        private void InitializeValues(int count)
        {
            if (_values.IsCreated)
            {
                _values.Dispose();
            }

            _values = new NativeArray<float>(count, _allocator, NativeArrayOptions.UninitializedMemory);
        }

        public void Update(AnimationCurve curve, int resolution)
        {
            if (curve == null)
            {
                throw new NullReferenceException("Animation curve is null.");
            }

            _preWrapMode = curve.preWrapMode;
            _postWrapMode = curve.postWrapMode;

            if (!_values.IsCreated || _values.Length != resolution)
            {
                InitializeValues(resolution);
            }

            for (var i = 0; i < resolution; i++)
            {
                _values[i] = curve.Evaluate(i / (float)resolution);
            }
        }

        public float Evaluate(float t)
        {
            Assert.IsTrue(_values.IsCreated);

            var count = _values.Length;

            if (count == 1)
            {
                return _values[0];
            }

            // Handle wrap modes for t < 0 and t > 1
            t = HandleWrapModes(t);

            var it = t * (count - 1);
            var lower = (int)it;
            var upper = Mathf.Min(lower + 1, count - 1); // Ensure upper is within bounds

            return lerp(_values[lower], _values[upper], it - lower);
        }

        private float HandleWrapModes(float t)
        {
            if (t < 0f)
            {
                return HandlePreWrapMode(t);
            }
            if (t >= 1f) // Note the change to >= to handle t == 1
            {
                return HandlePostWrapMode(t);
            }
            return t;
        }

        private float HandlePreWrapMode(float t)
        {
            switch (_preWrapMode)
            {
                default:
                    return 0f;

                case WrapMode.Loop:
                    return 1f - abs(t) % 1f;

                case WrapMode.PingPong:
                    return Pingpong(t, 1f);
            }
        }

        private float HandlePostWrapMode(float t)
        {
            switch (_postWrapMode)
            {
                default:
                    return 1f;

                case WrapMode.Loop:
                    return t % 1f;

                case WrapMode.PingPong:
                    return Pingpong(t, 1f);
            }
        }

        public void Dispose()
        {
            if (_values.IsCreated)
            {
                _values.Dispose();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private float Repeat(float t, float length)
        {
            return clamp(t - floor(t / length) * length, 0, length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private float Pingpong(float t, float length)
        {
            t = Repeat(t, length * 2f);
            return length - abs(t - length);
        }
    }
}