using JetBrains.Annotations;
using System;
using System.Collections.Generic;

namespace Baracuda.Utilities
{
    public readonly struct Rational : IEquatable<Rational>, IComparable<Rational>, IEqualityComparer<Rational>
    {
        #region Fields

        public readonly int numerator;

        public readonly int denominator;

        public Rational(int num, int den)
        {
            numerator = num;
            denominator = den;
        }

        #endregion


        #region Operator (Convertion)

        public static implicit operator float(Rational rational)
        {
            return (float) rational.numerator / rational.denominator;
        }

        public static implicit operator double(Rational rational)
        {
            return (double) rational.numerator / rational.denominator;
        }

        #endregion


        #region Operator (Calculation)

        // Additions
        public static Rational operator +(Rational a, Rational b) => checked(new(a.numerator * b.denominator + a.denominator * b.numerator, a.denominator * b.denominator));
        public static Rational operator +(Rational a, int b) => checked(new(a.numerator + a.denominator * b, a.denominator));
        public static Rational operator +(int a, Rational b) => checked(new(a * b.denominator + b.numerator, b.denominator));

        // Subtraction
        public static Rational operator -(Rational a, Rational b) => checked(new(a.numerator * b.denominator - a.denominator * b.numerator, a.denominator * b.denominator));
        public static Rational operator -(Rational a, int b) => checked(new(a.numerator - a.denominator * b, a.denominator));
        public static Rational operator -(int a, Rational b) => checked(new(a * b.denominator - b.numerator, b.denominator));

        // Multiplication
        public static Rational operator *(Rational a, Rational b) => checked(new(a.numerator * b.numerator, a.denominator * b.denominator));
        public static Rational operator *(Rational a, int b) => checked(new(a.numerator * b, a.denominator));
        public static Rational operator *(int a, Rational b) => checked(new(b.numerator * a, b.denominator));

        // Division
        public static Rational operator /(Rational a, Rational b) => checked(new(a.numerator * b.denominator, a.denominator * b.numerator));
        public static Rational operator /(Rational a, int b) => checked(new(a.numerator, a.denominator * b));
        public static Rational operator /(int a, Rational b) => checked(new(a * b.denominator, b.numerator));

        #endregion


        #region String Representation

        [Pure]
        public Rational Simplify()
        {
            var sign = MathF.Sign(denominator);
            var num = sign * numerator;
            var den = sign * denominator;
            var gcd = Gcd(numerator, denominator);
            num /= gcd;
            den /= gcd;
            return new Rational(num, den);
        }

        [Pure]
        public (int num, int den) SimplifyDeconstruct()
        {
            var sign = MathF.Sign(denominator);
            var num = sign * numerator;
            var den = sign * denominator;
            var gcd = Gcd(numerator, denominator);
            num /= gcd;
            den /= gcd;
            return (num, den);
        }

        private static int Gcd(int a, int b)
        {
            while (b != 0)
            {
                var t = b;
                b = a % b;
                a = t;
            }
            return a;
        }

        public override string ToString()
        {
            var (num, den) = SimplifyDeconstruct();
            return $"{num.ToString()} / {den.ToString()}";
        }

        #endregion


        #region IEquatable IEqualityComparer

        public bool Equals(Rational other)
        {
            return (numerator, denominator) == (other.numerator, other.denominator);
        }

        public override bool Equals(object obj)
        {
            return obj is Rational other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(numerator, denominator);
        }

        public bool Equals(Rational x, Rational y)
        {
            return x.numerator == y.numerator && x.denominator == y.denominator;
        }

        public int GetHashCode(Rational rational)
        {
            return rational.GetHashCode();
        }

        #endregion


        #region IComparable

        public int CompareTo(Rational other)
        {
            var numeratorComparison = numerator.CompareTo(other.numerator);
            if (numeratorComparison != 0)
            {
                return numeratorComparison;
            }

            return denominator.CompareTo(other.denominator);
        }

        #endregion
    }
}