using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Reflection.Emit;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Complex;
using MathNet.Numerics.LinearAlgebra.Factorization;
using Quantum_Mechanics.DE_Solver;
using Quantum_Mechanics.General;
using ScottPlot;

namespace FDM_Testing
{
    public static class Solver
    {

        //Solves the equation and outputs the sqrt(MSE) error for given grid resolution and error tolerance
        public static double Solve(int n, double epsilon)
        {
            var dx = 4d / (n - 1);
            var dy = 4d / (n - 1);

            var H = CreateMatrix.Sparse<Complex>(n * n, n * n);

            Parallel.For(0, n * n, k =>
            {
                lock (H)
                {
                    if (k + n < n * n)
                        H[k, k + n] = -1 / (dx * dx);

                    if (k - n >= 0)
                        H[k, k - n] = -1 / (dx * dx);

                    if (k + 1 < n * n)
                        H[k, k + 1] = -1 / (dy * dy);

                    if (k - 1 >= 0)
                        H[k, k - 1] = -1 / (dy * dy);

                    H[k, k] = 2 / (dx * dx) + 2 / (dy * dy);
                }
            });

            H[0, 0] = 0;
            H[n - 1, n - 1] = 0;
            H[n * (n - 1), n * (n - 1)] = 0;

            var A = H.Evd();

            var E = A.EigenValues;
            var U = A.EigenVectors.EnumerateColumns().ToArray();

            var solutions = new List<Tuple<double, MathNet.Numerics.LinearAlgebra.Vector<Complex>>>();
            var actualSolutions = new Dictionary<(int, int), double>();

            for(int i = 0; i < E.Count; ++i)
            {
                if (E[i].Real < 0)
                    continue;

                var valid = true;

                Parallel.For(0, n, j =>
                {
                    if (!U[i][j].AlmostEqual(0, epsilon)
                        || !U[i][n * (n - 1) + j].AlmostEqual(0, epsilon)
                        || !U[i][n * j].AlmostEqual(0, epsilon)
                        || !U[i][n * j + n - 1].AlmostEqual(0, epsilon))
                    {
                        valid = false;
                        return;
                    }
                });

                if (!valid)
                    continue;

                solutions.Add(Tuple.Create(E[i].Real, U[i]));
            }

            Parallel.For(0, solutions.Count, i =>
            {
                lock (solutions)
                {
                    var N2 = 0d;

                    for (int j = 0; j < solutions[i].Item2.Count; ++j)
                        N2 += solutions[i].Item2[j].MagnitudeSquared() * dx * dy;

                    solutions[i] = Tuple.Create(solutions[i].Item1, solutions[i].Item2 * Math.Sqrt(1 / N2));
                }
            });

            for (int nx = 1; nx <= 10; ++nx)
            {
                Parallel.For(1, 11, ny =>
                {
                    lock (actualSolutions)
                    {
                        var Exy = (nx * nx + ny * ny) * (Math.PI * Math.PI) / 32;
                        actualSolutions.Add((nx, ny), Exy);
                    }
                });
            }

            var E_real = actualSolutions.OrderBy(x => x.Value);
            var C = 0d;
            var X = 0.01 * (solutions.Max(x => x.Item1) - solutions.Min(x => x.Item1));

            Parallel.For(0, 10, i =>
            {
                var real = E_real.ElementAt(i).Value;
                var approx = solutions[i].Item1;
                var error = real - approx;

                C += error * error / 10;
            });

            return Math.Round(Math.Sqrt(C) / X, 3);
        }
    }
}
