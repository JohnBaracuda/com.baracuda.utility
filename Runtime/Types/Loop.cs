using System.Collections.Generic;
using Unity.Plastic.Antlr3.Runtime.Misc;

namespace Baracuda.Utilities.Types
{
    public struct Loop
    {
        private int value;
        private readonly int max;
        private readonly int min;

        public static Loop operator ++(Loop looping)
        {
            if (++looping.value > looping.max)
            {
                looping.value = looping.min;
            }

            return looping;
        }

        public static Loop operator --(Loop looping)
        {
            if (--looping.value < looping.min)
            {
                looping.value = looping.max;
            }

            return looping;
        }

        public static implicit operator int(Loop loop)
        {
            return loop.value;
        }

        public static Loop FromRange(int min, int max)
        {
            return new Loop(0, min, max);
        }

        public static Loop WithMax(int max)
        {
            return new Loop(0, 0, max);
        }

        public static Loop For<T>(IList<T> list)
        {
            return new Loop(0, 0, list.Count - 1);
        }

        public Loop(int value, int min, int max)
        {
            this.value = value;
            this.min = min;
            this.max = max;
        }

        public override string ToString()
        {
            return value.ToString();
        }
    }

    public struct DynamicLoop
    {
        private int value;
        private readonly Func<int> min;
        private readonly Func<int> max;

        private static readonly Func<int> zeroFunc = () => 0;

        public static DynamicLoop operator ++(DynamicLoop looping)
        {
            if (++looping.value > looping.max())
            {
                looping.value = looping.min();
            }

            return looping;
        }

        public static DynamicLoop operator --(DynamicLoop looping)
        {
            if (--looping.value < looping.min())
            {
                looping.value = looping.max();
            }

            return looping;
        }

        public static implicit operator int(DynamicLoop loop)
        {
            return loop.value;
        }

        public static DynamicLoop FromRange(Func<int> minFunc, Func<int> maxFunc)
        {
            return new DynamicLoop(0, minFunc, maxFunc);
        }

        public static DynamicLoop WithMax(Func<int> maxFunc)
        {
            return new DynamicLoop(0, zeroFunc, maxFunc);
        }

        public static DynamicLoop For<T>(IList<T> list)
        {
            return new DynamicLoop(0, zeroFunc, () => list.Count - 1);
        }

        private DynamicLoop(int value, Func<int> min, Func<int> max)
        {
            this.value = value;
            this.min = min;
            this.max = max;
        }

        public override string ToString()
        {
            return value.ToString();
        }
    }
}