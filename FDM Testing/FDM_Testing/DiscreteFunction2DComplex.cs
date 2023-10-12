using MathNet.Numerics;
using MathNet.Numerics.Differentiation;
using MathNet.Numerics.Integration;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.RootFinding;
using ScottPlot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Quantum_Mechanics.DE_Solver
{
    public class DiscreteFunction2DComplex
    {
        private Func<double, double, Complex> Function;

        public DiscreteFunction2DComplex(Func<double, double, Complex> function)
        {
            Function = function;
        }

        public Func<double, double, Complex> GetHandle()
        {
            return Function;
        }

        public DiscreteFunction2D GetMagnitudeSquared()
        {
            var f = new Func<double, double, double>((x, y) => Evaluate(x, y).MagnitudeSquared());
            return new DiscreteFunction2D(f);
        }

        public DiscreteFunction2D GetRealPart()
        {
            var f = new Func<double, double, double>((x, y) => Evaluate(x, y).Real);
            return new DiscreteFunction2D(f);
        }

        public DiscreteFunction2D GetImaginaryPart()
        {
            var f = new Func<double, double, double>((x, y) => Evaluate(x, y).Imaginary);
            return new DiscreteFunction2D(f);
        }

        public DiscreteFunction2DComplex ToPolarCoordinates(DiscreteFunction2DComplex f, bool includeJacobian = false)
        {
            var g = new Func<double, double, Complex>((r, fi) =>
            {
                var value = f.Evaluate(r * Math.Cos(fi), r * Math.Sin(fi));

                if (includeJacobian)
                    return r * value;

                return value;
            });

            return new DiscreteFunction2DComplex(g);
        }

        public DiscreteFunction2DComplex ToCartesianCoordinates(DiscreteFunction2DComplex f)
        {
            var g = new Func<double, double, Complex>((x, y) =>
            {
                var r = Math.Sqrt(x * x + y * y);
                var fi = Math.Atan2(y, x);

                var value = f.Evaluate(r, fi);

                return value;
            });

            return new DiscreteFunction2DComplex(g);
        }

        public Complex Evaluate(double x, double y)
        {
            return MathUtils.Round(Function.Invoke(x, y));
        }

        public Complex IntegratePolar(double a, double b, double c, double d)
        {
            return ToPolarCoordinates(this, true).Integrate(a, b, c, d);
        }

        public Complex Integrate(double a, double b, double c, double d, bool jacobian = false)
        {
            if (jacobian)
            {
                var f = new DiscreteFunction2DComplex(new Func<double, double, Complex>((x, y) => x * Function(x, y)));
                var re = MathUtils.Round(GaussLegendreRule.Integrate(f.GetRealPart().GetHandle(), a, b, c, d, 10));
                var im = MathUtils.Round(GaussLegendreRule.Integrate(f.GetImaginaryPart().GetHandle(), a, b, c, d, 10));

                return MathUtils.Round(new Complex(re, im));
            }

            var real = GaussLegendreRule.Integrate(GetRealPart().GetHandle(), a, b, c, d, 10);
            var imaginary = GaussLegendreRule.Integrate(GetImaginaryPart().GetHandle(), a, b, c, d, 10);

            return MathUtils.Round(new Complex(real, imaginary));
        }

        public DiscreteFunction2DComplex FourierTransform(double[,] domain)
        {
            var g = new Func<double, double, Complex>((kx, ky) =>
            {
                var f = new Func<double, double, Complex>((x, y) => 1 / Math.Sqrt(2 * Math.PI) * Evaluate(x, y) * Complex.Exp(-Complex.ImaginaryOne * (kx * x + ky * y)));
                var f_real = new Func<double, double, double>((x, y) => f(x, y).Real);
                var f_imaginary = new Func<double, double, double>((x, y) => f(x, y).Imaginary);

                var g_real = GaussLegendreRule.Integrate(f_real, domain[0, 0], domain[0, 1], domain[1, 0], domain[1, 1], 10);
                var g_imaginary = GaussLegendreRule.Integrate(f_imaginary, domain[0, 0], domain[0, 1], domain[1, 0], domain[1, 1], 10);

                return new Complex(g_real, g_imaginary);
            });

            return new DiscreteFunction2DComplex(g);
        }
    }

}
