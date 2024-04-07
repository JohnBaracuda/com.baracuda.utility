using System.Runtime.CompilerServices;

namespace Baracuda.Utilities
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
    }
}