using System;
using System.Runtime.CompilerServices;

namespace Baracuda.Bedrock.Utilities
{
    public static class MathUtility
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void NormalizeAngle(ref float angle)
        {
            while (angle > 180f)
            {
                angle -= 360f;
            }
            while (angle < -180f)
            {
                angle += 360f;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Round(ref float value, float stepSize = 1)
        {
            value = (float)Math.Round(value / stepSize) * stepSize;
        }
    }
}