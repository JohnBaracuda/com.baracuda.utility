using System.Collections.Generic;

namespace Baracuda.Utilities.Types
{
    public class LoopObject
    {
        #region Properties

        public int Iterations { get; private set; }
        public int Value { get; set; }

        #endregion


        #region Fields

        private readonly int _startValue;
        private readonly int _max;
        private readonly int _min;

        #endregion


        #region Factory

        public static LoopObject Create(int min, int max)
        {
            return new LoopObject(0, min, max);
        }

        public static LoopObject Create(int max)
        {
            return new LoopObject(0, 0, max);
        }

        public static LoopObject Create<T>(IList<T> list)
        {
            return new LoopObject(0, 0, list.Count - 1);
        }

        public static LoopObject Create(int startIndex, int min, int max)
        {
            return new LoopObject(startIndex, min, max);
        }

        public static LoopObject Create<T>(int startIndex, IList<T> list)
        {
            return new LoopObject(startIndex, 0, list.Count - 1);
        }

        public LoopObject(int value, int min, int max)
        {
            Value = value;
            _min = min;
            _max = max;
            _startValue = value;
            Iterations = 0;
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        #endregion


        #region Operator

        public static LoopObject operator ++(LoopObject looping)
        {
            if (++looping.Value >= looping._max)
            {
                looping.Value = looping._min;
            }

            if (looping.Value == looping._startValue)
            {
                looping.Iterations++;
            }

            return looping;
        }

        public static LoopObject operator --(LoopObject looping)
        {
            if (--looping.Value <= looping._min)
            {
                looping.Value = looping._max;
            }

            if (looping.Value == looping._startValue)
            {
                looping.Iterations--;
            }

            return looping;
        }

        public static implicit operator int(LoopObject loop)
        {
            return loop.Value;
        }

        #endregion
    }
}