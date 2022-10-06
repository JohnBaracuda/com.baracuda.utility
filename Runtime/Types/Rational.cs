using JetBrains.Annotations;
using System;
using System.Runtime.InteropServices;

namespace Baracuda.Utilities.Types
{
    [StructLayout(LayoutKind.Explicit, Size = 8)]
    public readonly struct Rational
    {
        [FieldOffset(0)]
        public readonly int n;
        [FieldOffset(4)]
        public readonly int d;

        public Rational(int num, int den)
        {
            n = num;
            d = den;
        }

        public static Rational operator +(Rational a, Rational b) => checked(new(a.n * b.d + a.d * b.n, a.d * b.d));
        public static Rational operator +(Rational a, int b) => checked(new(a.n + a.d * b, a.d));
        public static Rational operator +(int a, Rational b) => checked(new(a * b.d + b.n, b.d));

        #region --- String Representation ---

        [Pure]
        public Rational Simplify()
        {
            var sign = MathF.Sign(d);
            var num = sign * n;
            var den = sign * d;
            var gcd = Gcd(n, d);
            num /= gcd;
            den /= gcd;
            return new Rational(num, den);
        }

        [Pure]
        public (int num, int den) SimplifyDeconstruct()
        {
            var sign = MathF.Sign(d);
            var num = sign * n;
            var den = sign * d;
            var gcd = Gcd(n, d);
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
    }
}