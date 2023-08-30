using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics;
using MathNet.Numerics.Optimization;
using MathNet.Numerics.Differentiation;
using MathNet.Numerics.Interpolation;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Factorization;
using MatplotlibCS;
using Quantum_Mechanics.General;
using ScottPlot;
using ScottPlot.Plottable;
using ScottPlot.Statistics.Interpolation;
using Accord.Math.Optimization;
using MathNet.Numerics.RootFinding;
using Accord.Math;

namespace Quantum_Mechanics.DE_Solver
{
    public class BoundaryCondition
    {
        public int Order { get; set; }
        public Complex32 Argument { get; set; }
        public Complex32 Value { get; set; }

        public BoundaryCondition(int order, string argument, string value)
        {
            Order = order;
            Argument = RPNParser.Calculate(argument);
            Value = RPNParser.Calculate(value);
        }
    }

    public enum DifferenceScheme
    {
        FORWARD,
        CENTRAL,
        BACKWARD
    }

    public static class DESolver
    {
        public static Vector<Complex32> SolveODE(DifferenceScheme scheme, string[] equation, BoundaryCondition[] boundaryConditions, double[] domain, int n)
        {
            float dx = (float)(domain[1] - domain[0]) / (n - 1);

            var x = CreateVector.Dense<double>(n);

            var A = CreateMatrix.Dense<Complex32>(n, n);
            var B = CreateVector.Dense<Complex32>(n);

            for (int i = 0; i < n; ++i)
                x[i] = domain[0] + i * dx;

            for (int i = 0; i < n; ++i)
            {
                var a = RPNParser.Calculate(equation[0], (float)x[i]);
                var b = RPNParser.Calculate(equation[1], (float)x[i]);
                var c = RPNParser.Calculate(equation[2], (float)x[i]);
                var d = RPNParser.Calculate(equation[3], (float)x[i]);

                switch (scheme)
                {
                    case DifferenceScheme.BACKWARD:
                        if (i > 1)
                            A[i, i - 2] = a;

                        if (i > 0)
                            A[i, i - 1] = -2 * a - b * dx;

                        A[i, i] = a + b * dx + c * dx * dx;
                        B[i] = d * dx * dx;
                        break;

                    case DifferenceScheme.CENTRAL:
                        if (i + 1 < n)
                            A[i, i + 1] = a + dx / 2 * b;

                        if (i > 0)
                            A[i, i - 1] = a - dx / 2 * b;

                        A[i, i] = d * dx * dx;
                        B[i] = d * dx * dx;
                        break;

                    case DifferenceScheme.FORWARD:
                        if (i + 2 < n)
                            A[i, i + 2] = a;

                        if (i + 1 < n)
                            A[i, i + 1] = b * dx - 2 * a;

                        A[i, i] = a - b * dx + c * dx * dx;
                        B[i] = d * dx * dx;
                        break;
                }
            }

            for (int i = 0; i < boundaryConditions.Length; ++i)
            {
                var condition = boundaryConditions[i];
                int k = (int)((condition.Argument - (float)domain[0]).Real / dx);

                if (condition.Order == 0)
                {
                    for (int j = 0; j < n; ++j)
                        A[k, j] = 0;

                    A[k, k] = 1;
                    B[k] = condition.Value;
                }
                else
                {
                    for (int j = 0; j < n; ++j)
                        A[k + 1, j] = 0;

                    A[k + 1, k + 1] = 1;
                    A[k + 1, k] = -1;
                    B[k + 1] = dx * condition.Value;
                }
            }

            return A.Solve(B);
        }

        public static Dictionary<Complex32, Vector<Complex32>> SolveEigenvalueODE(DifferenceScheme scheme, string[] equation, BoundaryCondition[] boundaryConditions, double[] domain, int n)
        {
            var solution = new Dictionary<Complex32, Vector<Complex32>>();

            float dx = (float)(domain[1] - domain[0]) / (n - 1);

            var x = CreateVector.Dense<double>(n);
            var A = CreateMatrix.Dense<Complex32>(n, n);

            for (int i = 0; i < n; ++i)
                x[i] = domain[0] + i * dx;

            for (int i = 0; i < n; ++i)
            {
                var a = RPNParser.Calculate(equation[0], (float)x[i]);
                var b = RPNParser.Calculate(equation[1], (float)x[i]);
                var c = RPNParser.Calculate(equation[2], (float)x[i]);

                switch (scheme)
                {
                    case DifferenceScheme.BACKWARD:
                        if (i > 1)
                            A[i, i - 2] = a / (dx * dx);

                        if (i > 0)
                            A[i, i - 1] = -b / dx - 2 * a / dx;

                        A[i, i] = a / (dx * dx) + b / dx + c - E_guess;
                        break;

                    case DifferenceScheme.CENTRAL:
                        if (i + 1 < n)
                            A[i, i + 1] = a / (dx * dx) + b / (2 * dx);

                        if (i > 0)
                            A[i, i - 1] = a / (dx * dx) - b / (2 * dx);

                        A[i, i] = -2 * a / (dx * dx) + c - E_guess;
                        break;

                    case DifferenceScheme.FORWARD:
                        if (i + 2 < n)
                            A[i, i + 2] = a / (dx * dx);

                        if (i + 1 < n)
                            A[i, i + 1] = -2 * a / (dx * dx) + b / dx;

                        A[i, i] = a / (dx * dx) - b / dx + c - E_guess;
                        break;
                }
            }

            var evd = A.Evd();

            var Y = evd.EigenVectors;

            for (int i = 0; i < n; ++i)
            {
                var F = Y.Column(i);
                var valid = true;

                for (int j = 0; j < boundaryConditions.Length; ++j)
                {
                    var condition = boundaryConditions[j];
                    var k = (int)((condition.Argument.Real - domain[0]) / dx);

                    if (condition.Order == 0)
                    {
                        if (Math.Abs(F[k].Real - condition.Value.Real) > dx)
                        {
                            valid = false;
                            break;
                        }
                    }
                    else
                    {
                        var g = CreateVector.Sparse<double>(n);

                        for (int m = 0; m < n; ++m)
                            g[m] = F[m].Real;

                        var g_interpolated = Interpolate.Linear(x, g);

                        for (int m = 0; m < n; ++m)
                            g[m] = g_interpolated.Differentiate(x[m]);

                        if (g[k] != condition.Value.Real)
                        { 
                            valid = false; 
                            break; 
                        }
                    }
                }

                if (!valid)
                    continue;

                var E = evd.EigenValues[i + 2];

                solution.Add((Complex32)E, F);
            }

            return solution;
        }
        public static Vector<Complex32> SolvePDE(DifferenceScheme[] schemes, string[] equation, BoundaryCondition[,] boundaryConditions, double[,] domain, int n) { return null; }
        
        public static Dictionary<Complex32, Vector<Complex32>> SolveEigenvaluePDE(DifferenceScheme[] schemes, string[] equation, BoundaryCondition[,] boundaryConditions, double[,] domain, int n) { return null; }
    }
}