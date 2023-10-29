using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accord.Math.Decompositions;
using Accord.Math;
using Accord.Math.Comparers;
using Accord.Math.Integration;
using MathNet.Numerics;
using Complex = System.Numerics.Complex;
using ScottPlot;
using System.Diagnostics;
using Accord.Extensions.BinaryTree;

namespace FEM
{
    public static class SpectralSolver
    {
        public static void SolveODE(double[] domain, int n, int gridPoints)
        {
            var dx = (domain[1] - domain[0]) / (gridPoints - 1);
            var x = new double[gridPoints];

            for (int i = 0; i < gridPoints; ++i)
                x[i] = domain[0] + i * dx;

            var H = CreateMatrix.Sparse<double>(gridPoints, gridPoints);

            for (int i = 0; i < gridPoints; ++i)
            {
                if (i + 1 < gridPoints)
                    H[i, i + 1] = -0.978 / (dx * dx);

                if (i - 1 >= 0)
                    H[i, i - 1] = -0.978 / (dx * dx);

                H[i, i] = 2 * 0.978 / (dx * dx);
            }

            var evd = H.Evd();

            var E = evd.EigenValues;
            var U = evd.EigenVectors;
            var u = U.Column(n).PointwiseMultiply(U.Column(n));

            var N = 0d;

            for (int i = 0; i < gridPoints; ++i)
                N += u[i] * dx;

            u /= N;

            Console.WriteLine("E{0} = {1}", n + 1, E[n]);

            var plot = new Plot();
            plot.SetAxisLimits(domain[0], domain[1], u.Min(), u.Max() + 1e-6);

            plot.AddSignalXY(x, u.ToArray());
            plot.SaveFig("plot.png");
            Process.Start("explorer.exe", "plot.png");
        }

        public static void SolveODEBessel(double[] domain, int n, int l, int gridPoints)
        {
            var dx = (domain[1] - domain[0]) / (gridPoints - 1);
            var dy = 2 * Math.PI / (gridPoints - 1);
            var d = 2 * domain[1] / (gridPoints - 1);
            var x = new double[gridPoints];
            var y = new double[gridPoints];

            for (int i = 0; i < gridPoints; ++i)
            {
                x[i] = domain[0] + i * dx;
                y[i] = -Math.PI + i * dy;
            }

            var H = CreateMatrix.Sparse<Complex>(gridPoints, gridPoints);

            for (int i = 0; i < gridPoints; ++i)
            {
                if (i + 1 < gridPoints)
                    H[i, i + 1] = -0.978 / (dx * dx) - 0.978 / (2 * dx) * 1 / (x[i] + 0.1);

                if (i - 1 >= 0)
                    H[i, i - 1] = -0.978 / (dx * dx) + 0.978 / (2 * dx) * 1 / (x[i] + 0.1);

                H[i, i] = 2 * 0.978 / (dx * dx) - 1 / (x[i] * x[i] + 0.1);
            }

            var evd = H.Evd();

            var E = evd.EigenValues;
            var U = evd.EigenVectors;

            for (int i = 0; i < E.Count; ++i)
            {
                if (E[i].Real < 0)
                    ++n;
            }

            var L = CreateVector.Sparse<Complex>(gridPoints);
            var R = CreateVector.Sparse<Complex>(gridPoints);
            var Nr = 0d;
            var Nl = 0d;

            for (int i = 0; i < gridPoints; ++i)
            {
                L[i] = Complex.Exp(-Complex.ImaginaryOne * l * y[i]);
                R[i] = U.Column(n)[i];

                Nr += R[i].MagnitudeSquared() * x[i] * dx;
                Nl += L[i].MagnitudeSquared() * dy;
            }

            var fl_real = Interpolate.CubicSpline(y, L.Real() / Nl);
            var fl_imag = Interpolate.CubicSpline(y, L.Imag() / Nl);
            var fr = Interpolate.CubicSplineRobust(x, R / Nr);

            var frl = new Func<double, double, double>((x, y) =>
            {
                var r = Math.Sqrt(x * x + y * y);
                var l = Math.Atan2(y, x);

                return fr.Interpolate(r) * fl.Interpolate(l);
            });

            var u = CreateMatrix.Sparse<double>(gridPoints, gridPoints);

            for (int i = 0; i < gridPoints; ++i)
            {
                for (int j = 0; j < gridPoints; ++j)
                {
                    var x0 = -domain[1] + i * d;
                    var y0 = -domain[1] + j * d;

                    var f = frl(x0, y0);

                    if (!f.IsFinite())
                        u[i, j] = 0;
                    else
                        u[i, j] = f;
                }
            }

            Console.WriteLine("E{0} = {1}", n + 1, E[n]);

            var plot = new Plot();
            plot.SetAxisLimits(-domain[1], domain[1], -domain[1], domain[1]);
            var map = plot.AddHeatmap(u.ToArray());
            map.CellWidth = d;
            map.CellHeight = d;
            map.OffsetX = -domain[1];
            map.OffsetY = -domain[1];

            plot.AddColorbar(map);
            plot.SaveFig("plot.png");
            Process.Start("explorer.exe", "plot.png");
        }
    }
}
