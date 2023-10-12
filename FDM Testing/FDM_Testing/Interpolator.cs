using MathNet.Numerics;
using MathNet.Numerics.Interpolation;
using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Quantum_Mechanics.DE_Solver
{
    public static class Interpolator
    {
        public static DiscreteFunction Cubic(MathNet.Numerics.LinearAlgebra.Vector<double> x, MathNet.Numerics.LinearAlgebra.Vector<double> y)
        {
            var u = new Func<double, double>(t =>
            {
                var interpolated = Interpolate.CubicSpline(x, y);

                return interpolated.Interpolate(t);
            });

            return new DiscreteFunction(u);
        }

        public static DiscreteFunctionComplex Cubic(MathNet.Numerics.LinearAlgebra.Vector<double> x, MathNet.Numerics.LinearAlgebra.Vector<Complex> y)
        {
            var u = new Func<double, Complex>(t =>
            {
                var interpolated_real = Interpolate.CubicSpline(x, y.Real());
                var interpolated_imaginary = Interpolate.CubicSpline(x, y.Imaginary());

                return new Complex(interpolated_real.Interpolate(t), interpolated_imaginary.Interpolate(t));
            });

            return new DiscreteFunctionComplex(u);
        }

        public static DiscreteFunction2D Bicubic(MathNet.Numerics.LinearAlgebra.Vector<double> x, MathNet.Numerics.LinearAlgebra.Vector<double> y, MathNet.Numerics.LinearAlgebra.Matrix<double> u)
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

        public static DiscreteFunction2DComplex Bicubic(MathNet.Numerics.LinearAlgebra.Vector<double> x, MathNet.Numerics.LinearAlgebra.Vector<double> y, MathNet.Numerics.LinearAlgebra.Matrix<Complex> u)
        {
            var f = new Func<double, double, Complex>((x0, y0) =>
            {
                var fx_real = new double[u.RowCount];
                var fx_imaginary = new double[u.RowCount];

                for (int i = 0; i < u.RowCount; ++i)
                {
                    fx_real[i] = Interpolate.CubicSpline(x, u.Row(i).Real()).Interpolate(y0);
                    fx_imaginary[i] = Interpolate.CubicSpline(x, u.Row(i).Imaginary()).Interpolate(y0);
                }

                var fy_real = Interpolate.CubicSpline(y, fx_real).Interpolate(x0);
                var fy_imaginary = Interpolate.CubicSpline(y, fx_imaginary).Interpolate(x0);
                
                return new Complex(fy_real, fy_imaginary);
            });

            return new DiscreteFunction2DComplex(f);
        }
    }
}
