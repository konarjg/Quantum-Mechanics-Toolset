using MathNet.Numerics;
using MathNet.Numerics.Differentiation;
using MathNet.Numerics.Integration;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.RootFinding;
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

        public Complex Integrate(double a, double b, bool polar = false)
        {
            if (polar)
            {
                var f = new Func<double, Complex>(x => x * Function(x));
                return MathUtils.Round(GaussLegendreRule.ContourIntegrate(f, a, b, 10));
            }

            return MathUtils.Round(GaussLegendreRule.ContourIntegrate(Function, a, b, 10));
        }

        public DiscreteFunctionComplex FourierTransform(double[] domain, bool polar = false)
        {
            var g = new Func<double, Complex>(k =>
            {
                var f = new Func<double, Complex>(x => 1 / Math.Sqrt(2 * Math.PI) * x * Evaluate(x) * Complex.Exp(-Complex.ImaginaryOne * k * x));

                if (polar)
                {
                    var h = new Func<double, Complex>(x => x * f(x));
                    return GaussLegendreRule.ContourIntegrate(h, domain[0], domain[1], 10);
                }


                return GaussLegendreRule.ContourIntegrate(f, domain[0], domain[1], 10);
            });

            return new DiscreteFunctionComplex(g);
        }

        public void Plot(int resolution, double[] domain, DiscreteFunctionComplex against = null)
        {
            var n = resolution;
            var dx = (domain[1] - domain[0]) / (n - 1);

            var x = new double[n];
            var u = new double[n];
            var f = new double[n];

            for (int i = 0; i < n; ++i)
            {
                x[i] = domain[0] + i * dx;
                u[i] = Evaluate(x[i]).Real;
            }

            var plot = new Plot();
            plot.SetAxisLimits(domain[0], domain[1], u.Min(), u.Max() + 1e-9);
            plot.AddSignalXY(x, u);

            if (against != null)
            {
                for (int i = 0; i < n; ++i)
                    f[i] = against.Evaluate(x[i]).MagnitudeSquared();

                plot.AddSignalXY(x, f);
            }

            plot.SaveFig("plot.png");

            Process.Start("explorer.exe", "plot.png");
        }
    }
}