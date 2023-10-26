﻿using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;
using System.Text;
using System.Threading.Tasks;
using Complex = System.Numerics.Complex;
using MathNet.Numerics;
using MathNet.Numerics.Integration;

namespace FEM
{
    public static class MathUtils
    {
        public static Complex Integrate(Vector<Complex> values, double[] domain, double a, double b, double dx)
        {
            if (a < domain[0] || b > domain[1])
                throw new ArgumentOutOfRangeException("Integral bounds outside the function's domain");

            var x = new double[values.Count];

            for (int i = 0; i < values.Count; i++)
                x[i] = domain[0] + i * dx;

            var y_real = Interpolate.CubicSpline(x, values.Real());
            var y_imaginary = Interpolate.CubicSpline(x, values.Imaginary());

            var y = new Complex(y_real.Integrate(a, b), y_imaginary.Integrate(a, b));
            return y;
        }

        public static Complex DoubleIntegrate(Matrix<Complex> values, double[,] domain, double a, double b, double c, double d, double dx, double dy, bool polar = false)
        {
            if (a < domain[0, 0] || b > domain[0, 1] || c < domain[1, 0] || d > domain[1, 1])
                throw new ArgumentOutOfRangeException("Integral bounds outside the function's domain");
            
            var n = values.RowCount;
            var x = new double[n];
            var y = new double[n];

            for (int i = 0; i < n; i++)
            {
                x[i] = domain[0, 0] + i * dx;

                if (polar)
                {
                    for (int j = 0; j < n; ++j)
                        values[i, j] *= x[i];
                }
            }

            for (int i = 0; i < n; i++)
                y[i] = domain[1, 0] + i * dy;

            var u_real_x = new double[n];
            var u_imaginary_x = new double[n];

            for (int i = 0; i < n; ++i)
            {
                u_real_x[i] = Interpolate.CubicSpline(x, values.Row(i).Real()).Integrate(c, d);
                u_imaginary_x[i] = Interpolate.CubicSpline(x, values.Row(i).Imaginary()).Integrate(c, d);
            }

            var u_real_y = Interpolate.CubicSpline(y, u_real_x).Integrate(a, b);
            var u_imaginary_y = Interpolate.CubicSpline(y, u_imaginary_x).Integrate(a, b);
            
            return new Complex(u_real_y, u_imaginary_y);
        }

    }
}