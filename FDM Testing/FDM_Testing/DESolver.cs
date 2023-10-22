using MathNet.Numerics;
using MathNet.Numerics.Integration;
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
    public static class DESolver
    {
        //Normalizes the given function to square integrate to one
        private static Vector<System.Numerics.Complex> Normalize(Vector<System.Numerics.Complex> values, double dx, bool polar = false)
        {
            var N2 = System.Numerics.Complex.Zero;

            if (!polar)
            {
                for (int i = 0; i < values.Count; ++i)
                    N2 += values[i].MagnitudeSquared() * dx;
            }
            else
            {
                for (int i = 0; i < values.Count; ++i)
                    N2 += values[i].MagnitudeSquared() * i * dx * dx;
            }

            values *= System.Numerics.Complex.Sqrt(1d / N2);
            return values;
        }

        //Computes coefficient of FDM stencil with 2nd order accuracy in specified direction
        //n -> order of derivative
        //k -> orientation of the point for example +1, -1
        //d -> grid spacing in a target direction
        private static double Coefficient(int n, int k, double d)
        {
            if (n == 2)
            {
                switch (k)
                {
                    case -1:
                        return 1d / (d * d);

                    case 0:
                        return -2d / (d * d);

                    case 1:
                        return 1d / (d * d);
                }

                return double.NaN;
            }

            switch (k)
            {
                case -1:
                    return -1d / (2 * d);

                case 0:
                    return 0;

                case 1:
                    return 1d / (2 * d);
            }

            return double.NaN;
        }

        public static List<(double, DiscreteFunctionComplex)> SolveODE(int resolution, double a)
        {
            var n = resolution;
            var dx = (a - 0.001) / (n - 1);
            var x = CreateVector.Sparse<double>(n);

            for (int i = 0; i < n; ++i)
                x[i] = 0.001 + i * dx;

            var H = CreateMatrix.Sparse<System.Numerics.Complex>(n, n);
            var solution = new List<(double, DiscreteFunctionComplex)>();

            for (int i = 0; i < n; ++i)
            {
                if (i + 1 < n)
                    H[i, i + 1] = Coefficient(2, 1, dx) + 1d / (x[i] * x[i] + 0.1) * Coefficient(1, 1, dx);

                if (i - 1 >= 0)
                    H[i, i - 1] = Coefficient(2, -1, dx) + 1d / (x[i] * x[i] + 0.1) * Coefficient(1, -1, dx);

                H[i, i] = Coefficient(2, 0, dx) + 1;
            }

            var evd = (-H).Evd();
            var E = evd.EigenValues;

            H[n - 1, n - 1] = 0;

            evd = (H).Evd();
            var U = evd.EigenVectors.EnumerateColumns().ToArray();

            for (int i = 0; i < 10; ++i)
            {
                //U[i] = Normalize(U[i], dx, true);
                var u = Interpolator.Cubic(x, U[i]);

                solution.Add((E[i].Magnitude, u));
            }

            return solution;
        }
    }
}
