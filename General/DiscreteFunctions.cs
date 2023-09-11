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

                result += y[i];
            }

            return result;
        }

        public static double DoubleIntegrate(Matrix<double> r, Matrix<double> u, Matrix<double> bounds)
        {
            var n = u.ColumnCount;

            var x = r.Row(0);
            var y = r.Row(1);

            var boundsX = bounds.Row(0);
            var boundsY = bounds.Row(1);

            var dx = (x[1] - x[0]) / (n - 1);
            var dy = (y[1] - y[0]) / (n - 1);

            var a = (boundsX[0] - x[0]) / dx;
            var b = (boundsX[1] - x[0]) / dx;
            var c = (boundsY[0] - y[0]) / dy;
            var d = (boundsY[1] - y[0]) / dy;

            var result = 0d;

            for (int i = (int)a; i <= (int)b; ++i)
            {
                if (i < 0 || i >= n)
                    continue;

                for (int j = (int)c; j <= (int)d; ++j)
                {
                    if (j < 0 || j >= n)
                        continue;

                    result += u[i, j] * dx * dy;
                }
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
            var dx = (x[n - 1] - x[0]) / (n - 1);
            return 1/MathF.Sqrt(2 * MathF.PI) * k.OuterProduct(x).Multiply(-Complex32.ImaginaryOne).PointwiseExp() * y * dx;
        }

        public static Matrix<Complex32> Fourier2D(Vector<Complex32> x, Vector<Complex32> y, Vector<Complex32> kx, Vector<Complex32> ky, Matrix<Complex32> u, int n)
        {
            var dx = (x[n - 1] - x[0]) / (n - 1);
            var dy = (y[n - 1] - y[0]) / (n - 1);

            var rx = x.Multiply(-Complex32.ImaginaryOne).OuterProduct(kx).PointwiseExp();
            var ry = y.Multiply(-Complex32.ImaginaryOne).OuterProduct(ky).PointwiseExp();

            return 1 / MathF.Sqrt(2 * MathF.PI) * rx * ry * u * dx * dy;
        }
    }
}
