using System;
using static UnityEngine.Random;

namespace Baracuda.Utilities.Random
{
    public static class RNG
    {
        public static bool Boolean()
        {
            return value > .5f;
        }

        public static bool Boolean(float percentage)
        {
            return value < percentage * .01f;
        }

        public static int Integer(int min = int.MinValue, int max = int.MaxValue)
        {
            return Range(min, max);
        }

        public static long Integer64()
        {
            return Range(int.MinValue, int.MaxValue) + (long)Range(int.MinValue, int.MaxValue);
        }

        public static int Integer(Range range)
        {
            return Range(range.Start.Value, range.End.Value);
        }


        public static float Single(float min = float.MinValue, float max = float.MaxValue)
        {
            return Range(min, max);
        }
    }
}
