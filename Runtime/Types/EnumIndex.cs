using System;
using Baracuda.Utility.Collections;
using Baracuda.Utility.Utilities;

namespace Baracuda.Utility.Types
{
    public struct EnumIndex<TEnum> where TEnum : unmanaged, Enum
    {
        #region Properties

        private Index _index;

        #endregion


        #region Fields

        public readonly TEnum Value => _values[_index];

        private readonly TEnum[] _values;

        #endregion


        #region Factory

        public EnumIndex(TEnum startValue, params TEnum[] without)
        {
            _values = EnumUtility.GetValueArray<TEnum>();
            ArrayUtility.Remove(ref _values, without);
            var startIndex = _values.IndexOf(startValue);
            _index = Index.Create(startIndex, _values);
        }

        public readonly override string ToString()
        {
            return Value.ToString();
        }

        public void Set(TEnum value)
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

        public static EnumIndex<TEnum> operator ++(EnumIndex<TEnum> enumIndex)
        {
            ref var index = ref enumIndex._index;
            index++;
            return enumIndex;
        }

        public static EnumIndex<TEnum> operator --(EnumIndex<TEnum> enumIndex)
        {
            ref var index = ref enumIndex._index;
            index--;
            return enumIndex;
        }

        public static implicit operator TEnum(EnumIndex<TEnum> index)
        {
            return index.Value;
        }

        #endregion
    }
}