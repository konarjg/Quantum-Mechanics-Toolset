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
using System.Net.Http.Headers;

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
            var dr = (domain[1] - domain[0]) / (gridPoints - 1);
            var dtheta = 2 * Math.PI / (gridPoints - 1);
            var dx = 2 * domain[1] / (gridPoints - 1);

            var x = new double[gridPoints * gridPoints];
            var y = new double[gridPoints * gridPoints];
            var r = new double[gridPoints];
            var theta = new double[gridPoints];

            //Kocham misiaczka <3

            for (int i = 0; i < gridPoints; ++i)
            {
                r[i] = domain[0] + i * dr;

                for (int j = 0; j < gridPoints; ++j)
                {
                    theta[j] = j * dtheta;
                    x[gridPoints * i + j] = r[i] * Math.Cos(theta[j]);
                    y[gridPoints * i + j] = r[i] * Math.Sin(theta[j]);
                }
            }

            var H = CreateMatrix.Sparse<Complex>(gridPoints, gridPoints);

            for (int i = 0; i < gridPoints; ++i)
            {
                if (i + 1 < gridPoints)
                    H[i, i + 1] = -0.978 / (dr * dr) - 0.978 / (2 * dr) * 1 / (r[i] + 0.1);

                if (i - 1 >= 0)
                    H[i, i - 1] = -0.978 / (dr * dr) + 0.978 / (2 * dr) * 1 / (r[i] + 0.1);

                H[i, i] = 2 * 0.978 / (dr * dr) + (l * l) / (r[i] * r[i] + 0.1) - 1 / (r[i] + 0.1);
            }

            var evd = H.Evd();

            var E = evd.EigenValues;
            var U = evd.EigenVectors;

            var L = CreateVector.Sparse<Complex>(gridPoints);
            var R = CreateVector.Sparse<Complex>(gridPoints);

            for (int i = 0; i < gridPoints; ++i)
            {
                L[i] = Complex.Exp(Complex.ImaginaryOne * l * theta[i]);
                R[i] = U.Column(n)[i];
            }

            var psi = R.OuterProduct(L);
            var N = 0d;

            for (int i = 0; i < gridPoints; ++i)
            {
                for (int j = 0; j < gridPoints; ++j)
                    N += psi[i, j].MagnitudeSquared() * r[i] * dr * dtheta;
            }

            psi /= Math.Sqrt(N);
            var u = CreateMatrix.Sparse<double>(gridPoints, gridPoints);

            for (int i = 0; i < gridPoints; ++i)
            {
                for (int j = 0; j < gridPoints; ++j)
                    u[i, j] = psi[i, j].MagnitudeSquared();
            }

            u = u.Transpose();
            Console.WriteLine("E{0} = {1}", n + 1, E[n]);

            var plot = new Plot();
            plot.SetAxisLimits(0, 2 * Math.PI, domain[0], domain[1]);
            var map = plot.AddHeatmap(u.ToArray());
            map.CellWidth = dtheta;
            map.CellHeight = dr;
            map.OffsetX = 0;
            map.OffsetY = domain[0];
            map.UseParallel = true;
            map.Opacity = 1;

            plot.AddColorbar(map);
            plot.SaveFig("plot.png");
            Process.Start("explorer.exe", "plot.png");
        }
    }
}
