using MathNet.Numerics;
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

                result += y[i] * dx;
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

                result += y[i] * dx;
            }

            return result;
        }

        public static Vector<Complex32> Fourier(Vector<double> domain, Vector<Complex32> samples, int n)
        {
            var result = CreateVector.Sparse<Complex32>(n);
            var dx = (float)((domain[1] - domain[0]) / (n - 1));

            for (int i = 0; i < n; ++i)
            {
                var sample = Complex32.Zero;
                var k = (float)(domain[0] + i * dx);

                for (int j = 0; j < n; ++j)
                {
                    var x = (float)(domain[0] + j * dx);

                    sample += samples[j] * 1f / MathF.Sqrt(2 * MathF.PI) * Complex32.Exp(-Complex32.ImaginaryOne * k * x);
                }

                result[i] = sample;
            }

            return result;
        }

    }
}
