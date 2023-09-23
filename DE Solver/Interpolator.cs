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

        public static DiscreteFunction2D Bicubic(Vector<double> x, Vector<double> y, Matrix<double> u)
        {
            var f = new Func<double, double, double>((x0, y0) =>
            {
                var fx = new double[u.RowCount];

                for (int i = 0; i < u.RowCount; ++i)
                    fx[i] = Interpolate.CubicSpline(x, u.Row(i)).Interpolate(y0);

                return Interpolate.CubicSpline(y, fx).Interpolate(x0);
            });

            return new DiscreteFunction2D(f);
        }
    }
}
