using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Baracuda.Bedrock.Types
{
    [StructLayout(LayoutKind.Auto)]
    public struct Index
    {
        #region Properties

        public int Iterations { get; private set; }
        public int Value { get; set; }
        public int Max { get; }
        public int Min { get; }
        public readonly bool IsMin => Value == Min;
        public readonly bool IsMax => Value == Max;

        #endregion


        #region Fields

        private readonly int startValue;

        #endregion


        #region Factory

        public static Index Create(int min, int max)
        {
            return new Index(0, min, max);
        }

        public static Index Create(int max)
        {
            return new Index(0, 0, max);
        }

        public static Index Create<T>(IList<T> list)
        {
            return new Index(0, 0, list.Count - 1);
        }

        public static Index Create(int startIndex, int min, int max)
        {
            return new Index(startIndex, min, max);
        }

        public static Index Create<T>(int startIndex, IList<T> list)
        {
            return new Index(startIndex, 0, list.Count - 1);
        }

        public Index(int value, int min, int max)
        {
            Value = value;
            Min = min;
            Max = max;
            startValue = value;
            Iterations = 0;
        }

        public readonly override string ToString()
        {
            return Value.ToString();
        }

        #endregion


        #region Operator

        public static Index operator ++(Index looping)
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

        public static Index operator --(Index looping)
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

        public static implicit operator int(Index index)
        {
            return index.Value;
        }

        #endregion
    }
}