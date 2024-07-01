using MathNet.Numerics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Quantum_Mechanics.Differential_Equations_Solver
{
    public static class MathUtils
    {
        public static Func<double, Complex> Fourier(Func<double, double> f, double[] domain)
        {
            var F = new Func<double, Complex>(k =>
            {
                var g = new Func<double, Complex>(x => f(x) * Complex.Exp(Complex.ImaginaryOne * 2 * Math.PI * k * x));
                return ContourIntegrate.GaussLegendre(g, domain[0], domain[1], 5);
            });

            return F;
        }

        public static Func<double, double> Normalize(Func<double, double> f, double[] domain)
        {
            var F = new Func<double, double>(x => f(x) * f(x));
            var g = new Func<double, double>(x => 1d / Math.Sqrt(Integrate.GaussLegendre(F, domain[0], domain[1], 5)) * f(x));
            return g;
        }

        public static Func<double, Complex> NormalizeComplex(Func<double, Complex> f, double[] domain)
        {
            var F = new Func<double, double>(x => f(x).MagnitudeSquared());
            var g = new Func<double, Complex>(x => 1d / Math.Sqrt(Integrate.GaussLegendre(F, domain[0], domain[1], 5)) * f(x));
            return g;
        }
    }
}
