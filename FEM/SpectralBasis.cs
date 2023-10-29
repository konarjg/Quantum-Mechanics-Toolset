using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Complex = System.Numerics.Complex;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Factorization;

namespace FEM
{
    public static class SpectralBasis
    {
        //Chebyshev polynomials of the first kind
        public static double T(int n, double x)
        {
            if (n < 0)
                throw new ArgumentOutOfRangeException();

            if (n == 0)
                return 1;
            else if (n == 1)
                return x;

            var T2 = 1d; //Tn-2
            var T1 = x; //Tn-1

            for (int i = 2; i <= n; ++i)
            {
                var t = 2 * x * T1 - T2;
                T2 = T1;
                T1 = t;
            }

            return T1;
        }

        //Chebyshev polynomials of the second kind
        public static double U(int n, double x)
        {
            if (n < 0)
                throw new ArgumentOutOfRangeException();

            if (n == 0)
                return 1;
            else if (n == 1)
                return 2 * x;

            var U2 = 1d; //Un-2
            var U1 = 2 * x; //Un-1

            for (int i = 2; i <= n; ++i)
            {
                var t = 2 * x * U1 - U2;
                U2 = U1;
                U1 = t;
            }

            return U1;
        }

        //First derivative of Chebyshev polynomials
        public static double TPrime(int n, double x)
        {
            if (n < 0)
                throw new ArgumentOutOfRangeException();

            if (n == 0)
                return 0;

            return n * U(n - 1, x);
        }

        //Second derivative of Chebyshev polynomials
        public static double TBis(int n, double x)
        {
            if (n < 0)
                throw new ArgumentOutOfRangeException();

            if (n == 0 || n == 1)
                return 0;

            if (x == 1)
                return (Math.Pow(n, 4) - n * n) / 3d;
            else if (x == -1)
                return Math.Pow(-1, n) * (Math.Pow(n, 4) - n * n) / 3d;

            return n * ((n + 1) * T(n, x) - U(n, x)) / (x * x - 1);
        }
    }
}
