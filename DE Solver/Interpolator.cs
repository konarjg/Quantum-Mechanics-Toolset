using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quantum_Mechanics.DE_Solver
{
    public static class Interpolator
    {
        public static Func<double, double> Interpolate(Vector<double> domain, Vector<double> values)
        {
            return x =>
            {
                if (domain.Contains(x))
                    return values[domain.Find(t => t == x).Item1];

                throw new ArgumentException();
            };
        }

        public static Func<double, Complex32> InterpolateComplex(Vector<double> domain, Vector<Complex32> values)
        {
            return x =>
            {
                if (domain.Contains(x))
                    return values[domain.Find(t => t == x, Zeros.Include).Item1];

                throw new ArgumentException();
            };
        }

        public static Func<double, double> Interpolate(double[] domain, Vector<double> values, int n)
        {
            return x =>
            {
                if (x >= domain[0] && x <= domain[1])
                {
                    var dx = (domain[1] - domain[0]) / (n - 1);
                    var i = (int)((x - domain[0]) / dx);

                    return values[i];
                }

                throw new ArgumentException();
            };
        }

        public static Func<double, Complex32> InterpolateComplex(double[] domain, Vector<Complex32> values, int n)
        {
            return x =>
            {
                if (x >= domain[0] && x <= domain[1])
                {
                    var dx = (domain[1] - domain[0]) / (n - 1);
                    var i = (int)((x - domain[0]) / dx);

                    return values[i];
                }

                throw new ArgumentException();
            };
        }

        public static Func<double, double, double> Interpolate(double[,] domain, Matrix<double> values, int n)
        {
            return (x, y) =>
            {
                if (x >= domain[0, 0] && x <= domain[0, 1] && y >= domain[1, 0] && y <= domain[1, 1])
                {
                    var dx = (domain[0, 1] - domain[0, 0]) / (n - 1);
                    var dy = (domain[1, 1] - domain[1, 0]) / (n - 1);

                    var i = (int)((x - domain[0, 0]) / dx);
                    var j = (int)((y - domain[1, 0]) / dy);

                    return values[i, j];
                }

                throw new ArgumentException();
            };
        }

        public static Func<double, double, double> Interpolate(double[,] domain, Vector<double> values, int n)
        {
            return (x, y) =>
            {
                if (x >= domain[0, 0] && x <= domain[0, 1] && y >= domain[1, 0] && y <= domain[1, 1])
                {
                    var dx = (domain[0, 1] - domain[0, 0]) / (n - 1);
                    var dy = (domain[1, 1] - domain[1, 0]) / (n - 1);

                    var i = (int)((x - domain[0, 0]) / dx);
                    var j = (int)((y - domain[1, 0]) / dy);

                    return values[n * i + j];
                }

                throw new ArgumentException();
            };
        }

        public static Func<double, double, Complex32> InterpolateComplex(double[,] domain, Matrix<Complex32> values, int n)
        {
            return (x, y) =>
            {
                if (x >= domain[0, 0] && x <= domain[0, 1] && y >= domain[1, 0] && y <= domain[1, 1])
                {
                    var dx = (domain[0, 1] - domain[0, 0]) / (n - 1);
                    var dy = (domain[1, 1] - domain[1, 0]) / (n - 1);

                    var i = (int)((x - domain[0, 0]) / dx);
                    var j = (int)((y - domain[1, 0]) / dy);

                    return values[i, j];
                }

                throw new ArgumentException();
            };
        }

        public static Func<double, double, Complex32> InterpolateComplex(double[,] domain, Vector<Complex32> values, int n)
        {
            return (x, y) =>
            {
                if (x >= domain[0, 0] && x <= domain[0, 1] && y >= domain[1, 0] && y <= domain[1, 1])
                {
                    var dx = (domain[0, 1] - domain[0, 0]) / (n - 1);
                    var dy = (domain[1, 1] - domain[1, 0]) / (n - 1);

                    var i = (int)((x - domain[0, 0]) / dx);
                    var j = (int)((y - domain[1, 0]) / dy);

                    return values[n * i + j];
                }

                throw new ArgumentException();
            };
        }
    }
}
