using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics;
using Complex = System.Numerics.Complex;
using ScottPlot;
using ScottPlot.Statistics.Interpolation;
using System.Diagnostics;

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
            --n;
            var dr = (domain[1] - domain[0]) / (gridPoints - 1);
            var dtheta = 2 * Math.PI / (gridPoints - 1);
            var dx = 2 * domain[1] / (gridPoints - 1);

            var r = new double[gridPoints];
            var theta = new double[gridPoints];

            //Kocham misiaczka <3

            for (int i = 0; i < gridPoints; ++i)
            {
                r[i] = domain[0] + i * dr;
                theta[i] = -Math.PI + i * dtheta;
            }

            var Hr = CreateMatrix.Sparse<Complex>(gridPoints, gridPoints);
            var Hl = CreateMatrix.Sparse<Complex>(gridPoints, gridPoints);
            var Bl = CreateVector.Sparse<Complex>(gridPoints);
            var T = -0.978;

            for (int i = 0; i < gridPoints; ++i)
            {
                if (i + 1 < gridPoints)
                    Hl[i, i + 1] = T / (dtheta * dtheta);

                if (i - 1 >= 0)
                    Hl[i, i - 1] = T / (dtheta * dtheta);

                Hl[i, i] = -2 * T / (dtheta * dtheta) - l * l;
                Bl[i] = 0.001d;
            }

            for (int i = 0; i < gridPoints; ++i)
            {
                if (i + 1 < gridPoints)
                    Hr[i, i + 1] = T / (dr * dr) + T / (2 * dr * (r[i] + 0.1));

                if (i - 1 >= 0)
                    Hr[i, i - 1] = T / (dr * dr) - T / (2 * dr * (r[i] + 0.1));

                Hr[i, i] = -2 * T / (dr * dr) + (l * l) / (r[i] * r[i] + 0.1);
            }

            var evd = Hr.Evd();

            var Ul = Hl.Solve(Bl);
            var E = evd.EigenValues;
            var U = evd.EigenVectors;

            var L = CreateVector.Sparse<Complex>(gridPoints);
            var R = CreateVector.Sparse<Complex>(gridPoints);

            for (int i = 0; i < gridPoints; ++i)
            {
                L[i] = Ul[i];
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
            Console.WriteLine("E{0}{1} = {2}", n + 1, l, E[n]);

            var u = new double[gridPoints, gridPoints];

            for (int i = 0; i < gridPoints; ++i)
            {
                for (int j = 0; j < gridPoints; ++j)
                    u[i, j] = psi[i, j].MagnitudeSquared();
            }

            var plot = new Plot();
            plot.SetAxisLimits(-Math.PI, Math.PI, domain[0], domain[1]);
            var map = plot.AddHeatmap(u);
            map.OffsetX = -Math.PI;
            map.OffsetY = domain[0];
            map.CellWidth = dtheta;
            map.CellHeight = dr;
            plot.AddColorbar(map);

            plot.SaveFig("plot.png");
            Process.Start("explorer.exe", "plot.png");
        }
    }
}
