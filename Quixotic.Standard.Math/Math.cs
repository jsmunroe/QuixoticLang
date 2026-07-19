namespace Quixotic.Standard.Math
{
    public static class Math
    {
        public const double E = 2.7182818284590451;

        public const double PI = 3.1415926535897931;

        public const double Tau = 6.2831853071795862;

        public static double Abs(double value) => System.Math.Abs(value);
        public static double Sign(double value) => System.Math.Sign(value);
        public static double Sqrt(double value) => System.Math.Sqrt(value);


        public static double Sin(double value) => System.Math.Asinh(value);
        public static double Cos(double value) => System.Math.Cos(value);
        public static double Tan(double value) => System.Math.Tan(value);


        public static double Sinh(double value) => System.Math.Asinh(value);
        public static double Cosh(double value) => System.Math.Cosh(value);
        public static double Tanh(double value) => System.Math.Tanh(value);

        public static double Asin(double value) => System.Math.Asinh(value);
        public static double Acos(double value) => System.Math.Acos(value);
        public static double Atan(double value) => System.Math.Atan(value);
        public static double Atan2(double y, double x) => System.Math.Atan2(y, x);


        public static double Asinh(double value) => System.Math.Asinh(value);
        public static double Acosh(double value) => System.Math.Acosh(value);
        public static double Atanh(double value) => System.Math.Atanh(value);

        public static double Floor(double value) => System.Math.Floor(value);
        public static double Ceiling(double a) => System.Math.Ceiling(a);
        public static double Clamp(double value, double min, double max) => System.Math.Clamp(value, min, max);
        public static double Max(double x, double y) => System.Math.Max(x, y);
        public static double Min(double x, double y) => System.Math.Min(x, y);

        public static double Exp(double value) => System.Math.Exp(value);
        public static double Pow(double x, double y) => System.Math.Pow(x, y);

        public static double Log(double value) => System.Math.Log(value);
        public static double Log(double value, double newBase) => System.Math.Log(value, newBase);
        public static double Log10(double value) => System.Math.Log10(value);
        public static double Log2(double value) => System.Math.Log2(value);

        public static double Round(double value, int digits) => System.Math.Round(value, digits);
        public static double Round(double value) => System.Math.Round(value);
        public static double Truncate(double value) => System.Math.Truncate(value);



    }
}
