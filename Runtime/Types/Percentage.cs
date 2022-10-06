
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Baracuda.Utilities.Types
{
    [StructLayout(LayoutKind.Auto, Size = 8)]
    public struct Percentage : IComparable<Percentage>, IComparer<Percentage>, IEquatable<Percentage>, IEqualityComparer<Percentage>
    {
        #region --- Statics ---

        public static readonly Percentage HundredPercent = new Percentage(1);
        public static readonly Percentage FiftyPercent = new Percentage(.5f);
        public static readonly Percentage TwentyPercent = new Percentage(.2f);
        public static readonly Percentage TenPercent = new Percentage(.1f);
        public static readonly Percentage Zero = new Percentage(0);

        #endregion


        #region --- Value ---

        private double value;

        #endregion


        #region --- Operator ---

        public static Percentage operator ++(Percentage percentage)
        {
            percentage.value++;
            return percentage;
        }

        public static Percentage operator --(Percentage percentage)
        {
            percentage.value--;
            return percentage;
        }

        public static implicit operator float(Percentage percentage)
        {
            return percentage.ToDecimal();
        }

        public static implicit operator Percentage(float decimalValue)
        {
            return FromDecimal(decimalValue);
        }

        public static implicit operator double(Percentage percentage)
        {
            return percentage.ToDecimal64();
        }

        public static implicit operator Percentage(double decimalValue)
        {
            return FromDecimal(decimalValue);
        }

        public static implicit operator int(Percentage percentage)
        {
            return percentage.ToInt();
        }

        public static implicit operator Percentage(int decimalValue)
        {
            return FromDecimal(decimalValue);
        }

        public static implicit operator long(Percentage percentage)
        {
            return percentage.ToInt64();
        }

        public static implicit operator Percentage(long decimalValue)
        {
            return FromDecimal(decimalValue);
        }

        public static bool operator ==(Percentage lhs, Percentage rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(Percentage lhs, Percentage rhs)
        {
            return !lhs.Equals(rhs);
        }

        public static bool operator >(Percentage lhs, Percentage rhs)
        {
            return lhs.value > rhs.value;
        }

        public static bool operator <(Percentage lhs, Percentage rhs)
        {
            return lhs.value < rhs.value;
        }

        public static bool operator >=(Percentage lhs, Percentage rhs)
        {
            return lhs.value >= rhs.value;
        }

        public static bool operator <=(Percentage lhs, Percentage rhs)
        {
            return lhs.value <= rhs.value;
        }

        #endregion


        #region --- Ctor ---

        private Percentage(double value)
        {
            this.value = value;
        }

        #endregion


        #region --- From Methods ---

        public static Percentage FromInteger(int integer)
        {
            return new Percentage(integer);
        }

        public static Percentage FromInteger(long integer)
        {
            return new Percentage(integer);
        }

        public static Percentage FromDecimal(float value)
        {
            return new Percentage(value * 100);
        }

        public static Percentage FromDecimal(double value)
        {
            return new Percentage(value * 100);
        }

        #endregion


        #region --- To Methods ---


        public float ToDecimal()
        {
            return (float) value * .01f;
        }

        public double ToDecimal64()
        {
            return value * .01f;
        }

        public int ToInt()
        {
            return (int) value;
        }

        public long ToInt64()
        {
            return (long) value;
        }

        public override string ToString()
        {
            return $"{value.ToString("000.00")}%";
        }

        #endregion


        #region --- Comparission ---

        public int CompareTo(Percentage other)
        {
            return value.CompareTo(other.value);
        }

        public int Compare(Percentage x, Percentage y)
        {
            return x.value.CompareTo(y.value);
        }

        public bool Equals(Percentage other)
        {
            return value.Equals(other.value);
        }

        public override bool Equals(object obj)
        {
            return obj is Percentage other && Equals(other);
        }

        public override int GetHashCode()
        {
            return value.GetHashCode();
        }

        public bool Equals(Percentage x, Percentage y)
        {
            return x.value.Equals(y.value);
        }

        public int GetHashCode(Percentage obj)
        {
            return obj.value.GetHashCode();
        }

        #endregion

    }
}
