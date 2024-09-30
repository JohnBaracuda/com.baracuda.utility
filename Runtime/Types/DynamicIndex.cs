using System;
using System.Collections.Generic;
using Baracuda.Bedrock.Utilities;

namespace Baracuda.Bedrock.Types
{
    public struct DynamicIndex
    {
        #region Properties

        public int Iterations { get; private set; }

        public readonly bool CanIncrement()
        {
            return _enableWrapping || Value + 1 <= _max();
        }

        public readonly bool CanDecrement()
        {
            return _enableWrapping || Value - 1 >= _min();
        }

        #endregion


        #region Fields

        private readonly int _startValue;
        private readonly Func<int> _min;
        private readonly Func<int> _max;
        private readonly bool _enableWrapping;

        private static readonly Func<int> zeroFunc = () => 0;

        public int Value { get; set; }

        #endregion


        #region Factory

        /// <summary>
        ///     Create a new <see cref="DynamicIndex" /> that will dynamically adjust its range based on the min and max provider
        ///     methods.
        /// </summary>
        public static DynamicIndex Create(int startValue, Func<int> minFunc, Func<int> maxFunc, bool enableWrapping = true)
        {
            return new DynamicIndex(startValue, minFunc, maxFunc, enableWrapping);
        }

        /// <summary>
        ///     Create a new <see cref="DynamicIndex" /> that will dynamically adjust its range based on the passed and max
        ///     provider
        ///     method.
        ///     Min value for the loop is zero.
        /// </summary>
        public static DynamicIndex Create(int startValue, Func<int> maxFunc, bool enableWrapping = true)
        {
            return new DynamicIndex(startValue, zeroFunc, maxFunc, enableWrapping);
        }

        /// <summary>
        ///     Create a new <see cref="DynamicIndex" /> that will dynamically adjust its range for the passed collection.
        /// </summary>
        public static DynamicIndex Create<T>(int startValue, IList<T> list, bool enableWrapping = true)
        {
            return new DynamicIndex(startValue, zeroFunc, () => list.Count - 1, enableWrapping);
        }

        /// <summary>
        ///     Create a new <see cref="DynamicIndex" /> that will dynamically adjust its range based on the min and max provider
        ///     methods.
        /// </summary>
        public static DynamicIndex Create(Func<int> minFunc, Func<int> maxFunc, bool enableWrapping = true)
        {
            return new DynamicIndex(0, minFunc, maxFunc, enableWrapping);
        }

        /// <summary>
        ///     Create a new <see cref="DynamicIndex" /> that will dynamically adjust its range based on the passed and max
        ///     provider
        ///     method.
        ///     Min value for the loop is zero.
        /// </summary>
        public static DynamicIndex Create(Func<int> maxFunc, bool enableWrapping = true)
        {
            return new DynamicIndex(0, zeroFunc, maxFunc, enableWrapping);
        }

        /// <summary>
        ///     Create a new <see cref="DynamicIndex" /> that will dynamically adjust its range for the passed collection.
        /// </summary>
        public static DynamicIndex Create<T>(IList<T> list, bool enableWrapping = true)
        {
            return new DynamicIndex(0, zeroFunc, () => list.Count - 1, enableWrapping);
        }

        private DynamicIndex(int value, Func<int> min, Func<int> max, bool enableWrapping = true)
        {
            Value = value;
            _min = min;
            _max = max;
            _startValue = value.ClampPure(min(), max());
            _enableWrapping = enableWrapping;
            Iterations = 0;
        }

        public readonly override string ToString()
        {
            return Value.ToString();
        }

        #endregion


        #region Operator

        public static DynamicIndex operator ++(DynamicIndex looping)
        {
            if (looping._enableWrapping)
            {
                looping.Value++;
                looping.ValidateIndex();
            }
            else
            {
                if (looping.Value + 1 > looping._max())
                {
                    return looping;
                }
                looping.Value++;
                looping.ValidateIndex();
            }

            if (looping.Value == looping._startValue)
            {
                looping.Iterations++;
            }

            return looping;
        }

        public static DynamicIndex operator --(DynamicIndex looping)
        {
            if (looping._enableWrapping)
            {
                looping.Value--;
                looping.ValidateIndex();
            }
            else
            {
                if (looping.Value - 1 < looping._min())
                {
                    return looping;
                }
                looping.Value--;
                looping.ValidateIndex();
            }

            if (looping.Value == looping._startValue)
            {
                looping.Iterations--;
            }

            return looping;
        }

        public static implicit operator int(DynamicIndex index)
        {
            index.ValidateIndex();
            return index.Value;
        }

        public void ValidateIndex()
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