using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Baracuda.Bedrock.Types
{
    [StructLayout(LayoutKind.Auto)]
    public struct Loop
    {
        #region Properties

        public int Iterations { get; private set; }
        public int Value { get; set; }
        public int Max { get; }
        public int Min { get; }
        public bool IsMin => Value == Min;
        public bool IsMax => Value == Max;

        #endregion


        #region Fields

        private readonly int startValue;

        #endregion


        #region Factory

        public static Loop Create(int min, int max)
        {
            return new Loop(0, min, max);
        }

        public static Loop Create(int max)
        {
            return new Loop(0, 0, max);
        }

        public static Loop Create<T>(IList<T> list)
        {
            return new Loop(0, 0, list.Count - 1);
        }

        public static Loop Create(int startIndex, int min, int max)
        {
            return new Loop(startIndex, min, max);
        }

        public static Loop Create<T>(int startIndex, IList<T> list)
        {
            return new Loop(startIndex, 0, list.Count - 1);
        }

        public Loop(int value, int min, int max)
        {
            Value = value;
            Min = min;
            Max = max;
            startValue = value;
            Iterations = 0;
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        #endregion


        #region Operator

        public static Loop operator ++(Loop looping)
        {
            if (++looping.Value > looping.Max)
            {
                looping.Value = looping.Min;
            }

            if (looping.Value == looping.startValue)
            {
                looping.Iterations++;
            }

            return looping;
        }

        public static Loop operator --(Loop looping)
        {
            if (--looping.Value < looping.Min)
            {
                looping.Value = looping.Max;
            }

            if (looping.Value == looping.startValue)
            {
                looping.Iterations--;
            }

            return looping;
        }

        public static implicit operator int(Loop loop)
        {
            return loop.Value;
        }

        #endregion
    }
}