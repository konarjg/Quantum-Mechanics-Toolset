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
    public class DiscreteFunctionComplex
    {
        private Func<double, Complex> Function;

        public DiscreteFunctionComplex(Func<double, Complex> function)
        {
            Function = function;
        }

        public Func<double, Complex> GetHandle()
        {
            return Function;
        }

        public DiscreteFunction GetMagnitudeSquared()
        {
            var f = new Func<double, double>(x => Evaluate(x).MagnitudeSquared());
            return new DiscreteFunction(f);
        }

        public DiscreteFunction GetRealPart()
        {
            var f = new Func<double, double>(x => Evaluate(x).Real);
            return new DiscreteFunction(f);
        }

        public DiscreteFunction GetImaginaryPart()
        {
            var f = new Func<double, double>(x => Evaluate(x).Imaginary);
            return new DiscreteFunction(f);
        }

        public Complex Evaluate(double x)
        {
            return MathUtils.Round(Function(x));
        }

        public Complex Integrate(double a, double b)
        {
            return MathUtils.Round(GaussLegendreRule.ContourIntegrate(Function, a, b, 10));
        }

        public DiscreteFunctionComplex FourierTransform(double[] domain)
        {
            var g = new Func<double, Complex>(k =>
            {
                var f = new Func<double, Complex>(x => 1 / Math.Sqrt(2 * Math.PI) * Evaluate(x) * Complex.Exp(-Complex.ImaginaryOne * k * x));
                return GaussLegendreRule.ContourIntegrate(f, domain[0], domain[1], 10);
            });

            return new DiscreteFunctionComplex(g);
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
                y[i] = Evaluate(x[i]).MagnitudeSquared();
            }

            plot.Plot.SetAxisLimits(domain[0], domain[1], y.Min(), y.Max());
            plot.Plot.AddSignalXY(x, y);
        }
    }
}