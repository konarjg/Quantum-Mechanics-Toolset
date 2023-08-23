using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quantum_Mechanics.General
{
    public struct Complex
    {
        public double Real { get; set; }
        public double Imaginary { get; set; }

        public double Norm
        {
            get
            {
                return Math.Sqrt(Real * Real + Imaginary * Imaginary);
            }
        }

        public double Argument
        {
            get
            {
                return Math.Atan2(Imaginary, Real);
            }
        }

        public Complex(double real, double imaginary)
        {
            Real = real;
            Imaginary = imaginary;
        }

        public static Complex operator +(Complex a, Complex b)
        {
            return new Complex(a.Real + b.Real, a.Imaginary + b.Imaginary);
        }

        public static Complex operator -(Complex a)
        {
            return new Complex(-a.Real, -a.Imaginary);
        }

        public static Complex operator -(Complex a, Complex b)
        {
            return a + -b;
        }

        public static Complex operator *(Complex a, Complex b)
        {
            return new Complex(a.Real * b.Real - a.Imaginary * b.Imaginary, a.Imaginary * b.Real + a.Real * b.Imaginary);
        }

        public static Complex operator /(Complex a, Complex b)
        {
            var inverse = new Complex(b.Real / (b.Norm * b.Norm), -b.Imaginary / (b.Norm * b.Norm));

            return a * inverse;
        }

        public static Complex operator !(Complex a)
        {
            return new Complex(a.Real, -a.Imaginary);
        }

        public static bool operator ==(Complex a, Complex b)
        {
            return a.Real == b.Real && a.Imaginary == b.Imaginary;
        }

        public static bool operator !=(Complex a, Complex b)
        {
            return !(a == b);
        }
    }
}
