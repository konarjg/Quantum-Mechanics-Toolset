using MathNet.Numerics;
using MathNet.Numerics.Differentiation;
using MathNet.Numerics.Integration;
using MathNet.Numerics.LinearAlgebra;
using Quantum_Mechanics.DE_Solver;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quantum_Mechanics.DE_Solver
{
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
            return Math.Round(Function.Invoke(x), 9);
        }

        public double Integrate(double a, double b)
        {
            return GaussLegendreRule.Integrate(Function, a, b, 5);
        }
    }
}

public class DiscreteFunction2D
{
    private Func<double, double, double> Function;

    public DiscreteFunction2D(Func<double, double, double> function)
    {
        Function = function;
    }

    public Func<double, double, double> GetHandle()
    {
        return Function;
    }

    public double Evaluate(double x, double y)
    {
        return Math.Round(Function.Invoke(x, y), 5);
    }

  /*  public double IntegratePolar(double a, double b, double c, double d)
    {
        var n = (int)Math.Sqrt(1000);
        var dr = (b - a) / (n - 1);
        var dfi = (d - c) / (n - 1);

        var r = CreateVector.Sparse<double>(n);
        var fi = CreateVector.Sparse<double>(n);
        var u = CreateMatrix.Sparse<double>(n, n);

        for (int i = 0; i < n; ++i)
        {
            r[i] = a + i * dr;

            for (int j = 0; j < n; ++j)
            {
                fi[j] = c + j * dfi;

                var x = r[i] * Math.Cos(fi[j]);
                var y = r[i] * Math.Sin(fi[j]);

                u[i, j] = r[i] * Evaluate(x, y);
            }
        }

        var z = Interpolator.Bicubic(r, fi, u);
        return z.Integrate(a, b, c, d);
    }*/

    public double Integrate(double a, double b, double c, double d)
    {
        return GaussLegendreRule.Integrate(Function, a, b, c, d, 5);
    }
}
