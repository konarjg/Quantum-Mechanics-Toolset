using MathNet.Numerics;
using MathNet.Numerics.Differentiation;
using MathNet.Numerics.Integration;
using MathNet.Numerics.LinearAlgebra;
using Quantum_Mechanics.DE_Solver;
using ScottPlot;
using ScottPlot.Drawing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net.Http.Json;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Quantum_Mechanics.DE_Solver
{
    internal class MathUtils
    {
        public static double Round(double x)
        {
            var y = Math.Round(x, 6);

            if (y.ToString() == "-0")
                return 0;

            return y;
        }

        public static Complex Round(Complex x)
        {
            var y_real = Math.Round(x.Real, 6);
            var y_imaginary = Math.Round(x.Imaginary, 6);

            if (y_real.ToString() == "-0")
                y_real = 0d;

            if (y_imaginary.ToString() == "-0")
                y_imaginary = 0d;

            return new Complex(y_real, y_imaginary);
        }
    }

    public class DiscreteFunction
    {
        private Func<double, double> Function;

        public DiscreteFunction(Func<double, double> function)
        {
            Function = function;
        }

        public Func<double, double> GetHandle()
        {
            return Function;
        }

        public double Evaluate(double x)
        {
            return MathUtils.Round(Function(x));
        }

        public double Integrate(double a, double b)
        {
            return MathUtils.Round(GaussLegendreRule.Integrate(Function, a, b, 10));
        }

        public DiscreteFunction Inverse(double[] domain, int precision)
        {
            var n = precision;
            var dx = (domain[1] - domain[0]) / (n - 1);

            var x = CreateVector.Sparse<double>(n);
            var y = CreateVector.Sparse<double>(n);

            for (int i = 0; i < n; ++i)
            {
                x[i] = domain[0] + i * dx;
                y[i] = Evaluate(x[i]);
            }

            return Interpolator.Cubic(y, x);
        }

        public void Plot(FormsPlot plot, double[] domain, int points)
        {
            var n = points;
            var x = new double[n];
            var y = new double[n];
            var dx = (domain[1] - domain[0]) / (n - 1);

            for (int i = 0; i < n; ++i)
            {
                x[i] = domain[0] + i * dx;
                y[i] = Evaluate(x[i]);
            }

            plot.Plot.SetAxisLimits(domain[0], domain[1], y.Min(), y.Max());
            plot.Plot.AddSignalXY(x, y);
        }
    }
}