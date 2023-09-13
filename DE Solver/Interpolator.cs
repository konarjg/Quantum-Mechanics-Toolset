using MathNet.Numerics;
using MathNet.Numerics.Interpolation;
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
        public static DiscreteFunction Cubic(Vector<double> x, Vector<double> y)
        {
            var u = new Func<double, double>(t =>
            {
                var interpolated = Interpolate.CubicSpline(x, y);

                return Math.Round(interpolated.Interpolate(t), 5);
            });

            return new DiscreteFunction(u);
        }

        public static DiscreteFunction2D Bicubic(Vector<double> x, Vector<double> y, Vector<double> u)
        {
            var f = new Func<double, double, double>((x0, y0) =>
            {
                var x1 = new double[u.Count];

                for (int i = 0; i < u.Count; ++i)
                    x1[i] = Interpolate.CubicSpline(x, u).Interpolate(x0);

                return Interpolate.CubicSpline(y, x1).Interpolate(y0);
            });

            return new DiscreteFunction2D(f);
        }
    }
}
