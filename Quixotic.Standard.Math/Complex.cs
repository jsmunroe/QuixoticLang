using Quixotic.Interop.Attributes;

namespace Quixotic.Standard.Math
{
    public class Complex(double real, double imaginary)
    {
        public double Real { get; } = real;

        public double Imaginary { get; } = imaginary;

        public double MagnitudeSqr => Real * Real + Imaginary * Imaginary;

        public double Magnitude => System.Math.Sqrt(MagnitudeSqr);

        public double Argument => System.Math.Atan2(Real, Imaginary);

        public Complex Conjugate => new Complex(Real, -Imaginary);

        public static Complex operator -(Complex x)
        {
            return new Complex(-x.Real, -x.Imaginary);
        }

        public static Complex operator +(Complex x, Complex y)
        {
            return new Complex(x.Real + y.Real, x.Imaginary + y.Imaginary);
        }

        public static Complex operator -(Complex x, Complex y)
        {
            return new Complex(x.Real - y.Real, x.Imaginary - y.Imaginary);
        }

        public static Complex operator *(Complex x, Complex y)
        {
            return new Complex(
                x.Real * y.Real - x.Imaginary * y.Imaginary,
                x.Real * y.Imaginary + x.Imaginary * y.Real
            );
        }

        public static Complex operator /(Complex x, Complex y)
        {
            var denominator = y.Real * y.Real + y.Imaginary * y.Imaginary;

            return new Complex(
                (x.Real * y.Real + x.Imaginary * y.Imaginary) / denominator,
                (x.Imaginary * y.Real - x.Real * y.Imaginary) / denominator
            );
        }

        public static bool operator ==(Complex x, Complex y)
        {
            return x.Equals(y);
        }

        public static bool operator !=(Complex x, Complex y)
        {
            return x.Equals(y);
        }

        [QuixoticIgnore]
        public bool Equals(Complex other)
        {
            return Real.Equals(other.Real) &&
                   Imaginary.Equals(other.Imaginary);
        }

        [QuixoticIgnore]
        public override bool Equals(object? obj)
        {
            return obj is Complex complex && Equals(complex);
        }

        [QuixoticIgnore]
        public override int GetHashCode()
        {
            return HashCode.Combine(Real, Imaginary).GetHashCode();
        }

    }
}
