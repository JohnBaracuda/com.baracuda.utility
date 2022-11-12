using System;
using System.Collections.Generic;

namespace Baracuda.Utilities
{
    public struct DynamicLoop
    {
        #region Fields

        private int value;
        private readonly Func<int> min;
        private readonly Func<int> max;

        private static readonly Func<int> zeroFunc = () => 0;

        #endregion

        #region Factory

        /// <summary>
        /// Create a new <see cref="DynamicLoop"/> that will dynamically adjust its range based on the min and max provider methods.
        /// </summary>
        public static DynamicLoop Create(int startValue, Func<int> minFunc, Func<int> maxFunc)
        {
            return new DynamicLoop(startValue, minFunc, maxFunc);
        }

        /// <summary>
        /// Create a new <see cref="DynamicLoop"/> that will dynamically adjust its range based on the passed and max provider method.
        /// Min value for the loop is zero.
        /// </summary>
        public static DynamicLoop Create(int startValue, Func<int> maxFunc)
        {
            return new DynamicLoop(startValue, zeroFunc, maxFunc);
        }

        /// <summary>
        /// Create a new <see cref="DynamicLoop"/> that will dynamically adjust its range for the passed collection.
        /// </summary>
        public static DynamicLoop Create<T>(int startValue, IList<T> list)
        {
            return new DynamicLoop(startValue, zeroFunc, () => list.Count - 1);
        }

        /// <summary>
        /// Create a new <see cref="DynamicLoop"/> that will dynamically adjust its range based on the min and max provider methods.
        /// </summary>
        public static DynamicLoop Create(Func<int> minFunc, Func<int> maxFunc)
        {
            return new DynamicLoop(0, minFunc, maxFunc);
        }

        /// <summary>
        /// Create a new <see cref="DynamicLoop"/> that will dynamically adjust its range based on the passed and max provider method.
        /// Min value for the loop is zero.
        /// </summary>
        public static DynamicLoop Create(Func<int> maxFunc)
        {
            return new DynamicLoop(0, zeroFunc, maxFunc);
        }

        /// <summary>
        /// Create a new <see cref="DynamicLoop"/> that will dynamically adjust its range for the passed collection.
        /// </summary>
        public static DynamicLoop Create<T>(IList<T> list)
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

        #endregion

        #region Operator

        public static DynamicLoop operator ++(DynamicLoop looping)
        {
            looping.value++;
            looping.ValidateIndex();
            return looping;
        }

        public static DynamicLoop operator --(DynamicLoop looping)
        {
            looping.value--;
            looping.ValidateIndex();
            return looping;
        }

        public static implicit operator int(DynamicLoop loop)
        {
            loop.ValidateIndex();
            return loop.value;
        }

        private void ValidateIndex()
        {
            if (value < min())
            {
                value = max();
            }
            if (value > max())
            {
                value = min();
            }
        }

        #endregion
    }
}