using MathNet.Numerics;
using MathNet.Numerics.Differentiation;
using MathNet.Numerics.Integration;
using MathNet.Numerics.LinearAlgebra;
using OxyPlot;
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
using System.Text;
using System.Threading.Tasks;

namespace Quantum_Mechanics.DE_Solver
{
    internal class MathUtils
    {
        public static double Round(double x)
        {
            var y = Math.Round(x, 9);

            if (y.ToString() == "-0")
                return 0;

            return y;
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

        public void Plot(double[] domain, string path, int points)
        {
            var plot = new Plot();

            var n = points;
            var x = new double[n];
            var y = new double[n];
            var dx = (domain[1] - domain[0]) / (n - 1);

            for (int i = 0; i < n; ++i)
            {
                x[i] = domain[0] + i * dx;
                y[i] = Evaluate(x[i]);
            }

            plot.SetAxisLimits(domain[0], domain[1], y.Min(), y.Max());
            plot.AddSignalXY(x, y);
            plot.SaveFig(path);

            Process.Start("explorer.exe", path);
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

    private DiscreteFunction2D ToPolarCoordinates(DiscreteFunction2D f, bool includeJacobian = false)
    {
        var g = new Func<double, double, double>((r, fi) =>
        {
            var value = f.Evaluate(r * Math.Cos(fi), r * Math.Sin(fi));

            if (includeJacobian)
                return r * value;

            return value;
        });

        return new DiscreteFunction2D(g);
    }

    private DiscreteFunction2D ToCartesianCoordinates(DiscreteFunction2D f)
    {
        var g = new Func<double, double, double>((x, y) =>
        {
            var r = Math.Sqrt(x * x + y * y);
            var fi = Math.Atan2(y, x);

            var value = f.Evaluate(r, fi);

            return value;
        });

        return new DiscreteFunction2D(g);
    }

    public double Evaluate(double x, double y)
    {
        return MathUtils.Round(Function.Invoke(x, y));
    }

    public double IntegratePolar(double a, double b, double c, double d)
    {
        return ToPolarCoordinates(this, true).Integrate(a, b, c, d);
    }

    public double Integrate(double a, double b, double c, double d)
    {
        return MathUtils.Round(GaussLegendreRule.Integrate(Function, a, b, c, d, 10));
    }

    public void Plot(double[,] domain, string path, int points)
    {
        var plot = new Plot();

        var n = points;
        var u = new double[n, n];

        var dx = (domain[0, 1] - domain[0, 0]) / (n - 1);
        var dy = (domain[1, 1] - domain[1, 0]) / (n - 1);

        for (int i = 0; i < n; ++i)
        {
            for (int j = 0; j < n; ++j)
            {
                var x = domain[0, 0] + i * dx;
                var y = domain[1, 0] + j * dy;
                u[i, j] = Evaluate(x, y);
            }
        }

        plot.SetAxisLimits(domain[0, 0], domain[0, 1], domain[1, 0], domain[1, 1]);
        var map = plot.AddHeatmap(u);
        map.CellWidth = dx;
        map.CellHeight = dy;

        plot.AddColorbar(map);
        plot.SaveFig(path);

        Process.Start("explorer.exe", path);
    }
}
