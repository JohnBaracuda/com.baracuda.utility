using System;

namespace Baracuda.Utilities.Types
{
    public struct EnumLoop<TEnum> where TEnum : unmanaged, Enum
    {
        #region Properties

        private Loop _index;

        #endregion


        #region Fields

        public TEnum Value => _values[_index];

        private readonly TEnum[] _values;

        #endregion


        #region Factory

        public EnumLoop(TEnum startValue, params TEnum[] without)
        {
            _values = EnumUtility<TEnum>.GetValueArray();
            ArrayUtility.Remove(ref _values, without);
            var startIndex = _values.IndexOf(startValue);
            _index = Loop.Create(startIndex, _values);
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        public void Update(TEnum value)
        {
            var index = _values.IndexOf(value);
            if (index == -1)
            {
                return;
            }
            _index.Value = index;
        }

        #endregion


        #region Operator

        public static EnumLoop<TEnum> operator ++(EnumLoop<TEnum> enumLoop)
        {
            ref var index = ref enumLoop._index;
            index++;
            return enumLoop;
        }

        public static EnumLoop<TEnum> operator --(EnumLoop<TEnum> enumLoop)
        {
            ref var index = ref enumLoop._index;
            index--;
            return enumLoop;
        }

        public static implicit operator TEnum(EnumLoop<TEnum> loop)
        {
            return loop.Value;
        }

        #endregion
    }
}