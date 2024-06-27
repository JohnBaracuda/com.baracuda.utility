using System;
using System.Collections.Generic;

namespace Baracuda.Utilities.Types
{
    public struct DynamicLoop
    {
        #region Properties

        public int Iterations { get; private set; }

        #endregion


        #region Fields

        private readonly int _startValue;
        private readonly Func<int> _min;
        private readonly Func<int> _max;

        private static readonly Func<int> zeroFunc = () => 0;

        public int Value { get; set; }

        #endregion


        #region Factory

        /// <summary>
        ///     Create a new <see cref="DynamicLoop" /> that will dynamically adjust its range based on the min and max provider
        ///     methods.
        /// </summary>
        public static DynamicLoop Create(int startValue, Func<int> minFunc, Func<int> maxFunc)
        {
            return new DynamicLoop(startValue, minFunc, maxFunc);
        }

        /// <summary>
        ///     Create a new <see cref="DynamicLoop" /> that will dynamically adjust its range based on the passed and max provider
        ///     method.
        ///     Min value for the loop is zero.
        /// </summary>
        public static DynamicLoop Create(int startValue, Func<int> maxFunc)
        {
            return new DynamicLoop(startValue, zeroFunc, maxFunc);
        }

        /// <summary>
        ///     Create a new <see cref="DynamicLoop" /> that will dynamically adjust its range for the passed collection.
        /// </summary>
        public static DynamicLoop Create<T>(int startValue, IList<T> list)
        {
            return new DynamicLoop(startValue, zeroFunc, () => list.Count - 1);
        }

        /// <summary>
        ///     Create a new <see cref="DynamicLoop" /> that will dynamically adjust its range based on the min and max provider
        ///     methods.
        /// </summary>
        public static DynamicLoop Create(Func<int> minFunc, Func<int> maxFunc)
        {
            return new DynamicLoop(0, minFunc, maxFunc);
        }

        /// <summary>
        ///     Create a new <see cref="DynamicLoop" /> that will dynamically adjust its range based on the passed and max provider
        ///     method.
        ///     Min value for the loop is zero.
        /// </summary>
        public static DynamicLoop Create(Func<int> maxFunc)
        {
            return new DynamicLoop(0, zeroFunc, maxFunc);
        }

        /// <summary>
        ///     Create a new <see cref="DynamicLoop" /> that will dynamically adjust its range for the passed collection.
        /// </summary>
        public static DynamicLoop Create<T>(IList<T> list)
        {
            return new DynamicLoop(0, zeroFunc, () => list.Count - 1);
        }

        private DynamicLoop(int value, Func<int> min, Func<int> max)
        {
            Value = value;
            _min = min;
            _max = max;
            _startValue = value.ClampPure(min(), max());
            Iterations = 0;
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        #endregion


        #region Operator

        public static DynamicLoop operator ++(DynamicLoop looping)
        {
            looping.Value++;
            looping.ValidateIndex();

            if (looping.Value == looping._startValue)
            {
                looping.Iterations++;
            }

            return looping;
        }

        public static DynamicLoop operator --(DynamicLoop looping)
        {
            looping.Value--;
            looping.ValidateIndex();

            if (looping.Value == looping._startValue)
            {
                looping.Iterations--;
            }

            return looping;
        }

        public static implicit operator int(DynamicLoop loop)
        {
            loop.ValidateIndex();
            return loop.Value;
        }

        private void ValidateIndex()
        {
            if (Value < _min())
            {
                Value = _max();
            }

            if (Value > _max())
            {
                Value = _min();
            }
        }

        #endregion
    }
}