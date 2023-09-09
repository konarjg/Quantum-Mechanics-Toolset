﻿using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quantum_Mechanics.General
{
    public static class DiscreteFunctions
    {
        public static double Integrate(Vector<double> x, Vector<double> y, double[] bounds)
        {
            var result = 0d;

            var n = y.Count;
            var dx = (x[1] - x[0]) / (n - 1);
            var a = (int)((bounds[0] - x[0]) / dx);
            var b = (int)((bounds[1] - x[0]) / dx);

            for (int i = a; i <= b; ++i)
            {
                if (i < 0 || i >= n)
                    continue;

                result += y[i];
            }

            return result;
        }

        public static Complex32 IntegrateComplex(Vector<double> x, Vector<Complex32> y, double[] bounds)
        {
            var result = Complex32.Zero;

            var n = y.Count;
            var dx = (float)(x[1] - x[0]) / (n - 1);
            var a = (int)((bounds[0] - x[0]) / dx);
            var b = (int)((bounds[1] - x[0]) / dx);

            for (int i = a; i <= b; ++i)
            {
                if (i < 0 || i >= n)
                    continue;

                result += y[i];
            }

            return result;
        }

        public static Vector<Complex32> Fourier(Vector<Complex32> x, Vector<Complex32> k, Vector<Complex32> y, int n)
        {
            var result = CreateVector.Sparse<Complex32>(n);

            for (int i = 0; i < n; ++i)
                result[i] = y.Multiply(1 / MathF.Sqrt(2 * MathF.PI)).DotProduct(x.Multiply(-Complex32.ImaginaryOne * k[i]).PointwiseExp());

            return result;
        }

    }
}
