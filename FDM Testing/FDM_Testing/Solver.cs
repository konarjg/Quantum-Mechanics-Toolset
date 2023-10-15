using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using Quantum_Mechanics.DE_Solver;
using ScottPlot;
using ScottPlot.Drawing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FDM_Testing
{
    public static class Solver
    {
        private static Vector<double> Normalize(Vector<double> f, double dx, double dy)
        {
            var N2 = 0d;

            for (int j = 0; j < f.Count; ++j)
                N2 += f[j] * f[j] * dx * dy;

            f /= Math.Sqrt(N2);
            return f;
        }

        public static double Solve(int n)
        {
            var dx = 5d / (n - 1);
            var dy = 2 * Math.PI / (n - 1);

            var x = CreateVector.Sparse<double>(n);
            var y = CreateVector.Sparse<double>(n);
            var A = CreateMatrix.Sparse<double>(n * n, n * n);

            for (int i = 0; i < n; ++i)
            {
                for (int j = 0; j < n; ++j)
                {
                    x[i] = i * dx;
                    y[j] = j * dy;
                }
            }

            for (int i = 0; i < n * n; ++i)
            {
                if (i + 2 * n < n * n)
                    A[i, i + 2 * n] = 1 / (12 * dx * dx) + 1 / (12 * dx);

                if (i - 2 * n >= 0)
                    A[i, i - 2 * n] = -1 / (dx * dx) - 1 / (12 * dx);

                if (i + n < n * n)
                    A[i, i + n] = 4 / (dx * dx) - 2 / (3 * dx);

                if (i - n >= 0)
                    A[i, i - n] = 4 / (dx * dx) + 2 / (3 * dx);

                if (i + 2 < n * n)
                    A[i, i + 2] = -1 / (dy * dy) + 1 / (12 * dy);

                if (i - 2 >= 0)
                    A[i, i - 2] = -1 / (dy * dy) - 1 / (12 * dy);

                if (i + 1 < n * n)
                    A[i, i + 1] = 4 / (dy * dy) - 2 / (3 * dy);

                if (i - 1 >= 0)
                    A[i, i - 1] = 4 / (dy * dy) + 2 / (3 * dy);

                A[i, i] = -6 / (dx * dx) - 6 / (dy * dy);
            }

            for (int i = 0; i < n; ++i)
            {
                A[i, i] = 0;
                A[n * i, n * i] = 0;
                A[n * (n - 1) + i, n * (n - 1) + i] = 0;
                A[n * i + n - 1, n * i + n - 1] = 0;
            }

            var evd = A.Evd();
            var possible_U = evd.EigenVectors.EnumerateColumns().ToArray();
            var E = evd.EigenValues;

            var nx = 4;
            var U = Normalize(possible_U[nx], dx, dy);
            var u_real = CreateMatrix.Sparse<double>(n, n);
            var u = CreateMatrix.Sparse<double>(n, n);

            for (int i = 0; i < n; ++i)
            {
                for (int j = 0; j < n; ++j)
                    u[i, j] = U[n * i + j] * U[n * i + j];
            }

            var plot = new Plot();
            plot.SetAxisLimits(0, 5, 0, 5);
            var map = plot.AddHeatmap(u.ToArray());
            map.CellWidth = dx;
            map.CellHeight = dy;
            plot.AddColorbar(map);

            plot.SaveFig("plot.png");

            Process.Start("explorer.exe", "plot.png");

            return 0;
        }
    }
}
