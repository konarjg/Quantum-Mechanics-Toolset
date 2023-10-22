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
using Quantum_Mechanics.General;
using ScottPlot;
using ScottPlot.Plottable;
using ScottPlot.Statistics.Interpolation;
using MathNet.Numerics.RootFinding;
using System.Text.Json.Serialization.Metadata;
using System.Numerics;

namespace Quantum_Mechanics.DE_Solver
{
    public class BoundaryCondition
    {
        public int Order { get; set; }
        public System.Numerics.Complex Argument { get; set; }
        public System.Numerics.Complex Value { get; set; }

        public BoundaryCondition() { }

        public BoundaryCondition(int order, string argument, string value)
        {
            Order = order;
            Argument = RPNParser.Calculate(argument);
            Value = RPNParser.Calculate(value);
        }
    }

    public class PeriodicBoundaryCondition : BoundaryCondition
    {
        public double Period { get; set; }

        public PeriodicBoundaryCondition(double period) : base(2, "0", "0")
        {
            Period = period;
        }
    }

    public class BoundaryConditionPDE
    {
        public int Order { get; set; }
        public int Variable { get; set; }
        public System.Numerics.Complex Argument { get; set; }
        public System.Numerics.Complex Value { get; set; }

        public BoundaryConditionPDE(int order, int variable, string argument, string value)
        {
            Order = order;
            Variable = variable;
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
        public static MathNet.Numerics.LinearAlgebra.Vector<System.Numerics.Complex> SolveODE(DifferenceScheme scheme, string[] equation, BoundaryCondition[] boundaryConditions, double[] domain, int n)
        {
            float dx = (float)(domain[1] - domain[0]) / (n - 1);

            var x = CreateVector.Dense<double>(n);

            var A = CreateMatrix.Dense<System.Numerics.Complex>(n, n);
            var B = CreateVector.Dense<System.Numerics.Complex>(n);

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

        public static Dictionary<System.Numerics.Complex, MathNet.Numerics.LinearAlgebra.Vector<System.Numerics.Complex>> SolveEigenvalueODE(CancellationToken token, DifferenceScheme scheme, string[] equation, BoundaryCondition[] boundaryConditions, double[] domain, int n)
        {
            var solution = new Dictionary<System.Numerics.Complex, MathNet.Numerics.LinearAlgebra.Vector<System.Numerics.Complex>>();

            float dx = (float)(domain[1] - domain[0]) / (n - 1);

            var x = CreateVector.Dense<double>(n);
            var A = CreateMatrix.Dense<System.Numerics.Complex>(n, n);
            var B = CreateMatrix.SparseDiagonal<System.Numerics.Complex>(n, 1);

            for (int i = 0; i < n; ++i)
            {
                x[i] = domain[0] + i * dx;
                token.ThrowIfCancellationRequested();
            }

            for (int i = 0; i < n; ++i)
            {
                var a = RPNParser.Calculate(equation[0], (float)x[i]);
                var b = RPNParser.Calculate(equation[1], (float)x[i]);
                var c = RPNParser.Calculate(equation[2], (float)x[i]);
                var d = RPNParser.Calculate(equation[3], (float)x[i]);

                token.ThrowIfCancellationRequested();

                switch (scheme)
                {
                    case DifferenceScheme.BACKWARD:
                        if (i > 1)
                            A[i, i - 2] = a / (dx * dx);

                        if (i > 0)
                            A[i, i - 1] = -b / dx - 2 * a / dx;

                        A[i, i] = a / (dx * dx) + b / dx + c;
                        token.ThrowIfCancellationRequested();
                        break;

                    case DifferenceScheme.CENTRAL:
                        if (i + 1 < n)
                            A[i, i + 1] = a / (dx * dx) + b / (2 * dx);

                        if (i > 0)
                            A[i, i - 1] = a / (dx * dx) - b / (2 * dx);

                        A[i, i] = -2 * a / (dx * dx) + c;
                        token.ThrowIfCancellationRequested();
                        break;

                    case DifferenceScheme.FORWARD:
                        if (i + 2 < n)
                            A[i, i + 2] = a / (dx * dx);

                        if (i + 1 < n)
                            A[i, i + 1] = -2 * a / (dx * dx) + b / dx;

                        A[i, i] = a / (dx * dx) - b / dx + c;
                        token.ThrowIfCancellationRequested();
                        break;
                }

                B[i, i] = d;
            }

            var evd = (A + B).Evd();
            token.ThrowIfCancellationRequested();

            var Y = evd.EigenVectors;

            for (int i = 0; i < n; ++i)
            {
                var F = Y.Column(i);
                var valid = true;
                token.ThrowIfCancellationRequested();

                for (int j = 0; j < boundaryConditions.Length; ++j)
                {
                    var condition = boundaryConditions[j];
                    var k = (int)((condition.Argument.Real - domain[0]) / dx);

                    token.ThrowIfCancellationRequested();

                    if (condition.Order == 0)
                    {
                        if (Math.Abs(F[k].Real - condition.Value.Real) > 0.1)
                        {
                            valid = false;
                            token.ThrowIfCancellationRequested();
                            break;
                        }
                    }
                    else if (condition.Order == 1)
                    {
                        var g = CreateVector.Sparse<double>(n);

                        for (int m = 0; m < n; ++m)
                        {
                            g[m] = F[m].Real;
                            token.ThrowIfCancellationRequested();
                        }

                        var g_interpolated = Interpolate.Linear(x, g);

                        for (int m = 0; m < n; ++m)
                        {
                            g[m] = g_interpolated.Differentiate(x[m]);
                            token.ThrowIfCancellationRequested();
                        }

                        if (g[k] != condition.Value.Real)
                        {
                            valid = false;
                            token.ThrowIfCancellationRequested();
                            break;
                        }
                    }
                    else
                    {
                        var periodic = (PeriodicBoundaryCondition)condition;
                        var ix = (int)((periodic.Period - domain[0]) / dx);

                        if (Math.Abs(F[0].Real - F[ix].Real) > 0.1)
                        {
                            valid = false;
                            token.ThrowIfCancellationRequested();
                            break;
                        }
                    }
                }

                if (!valid)
                    continue;

                var E = evd.EigenValues[i];
                token.ThrowIfCancellationRequested();

                solution.Add((System.Numerics.Complex)E, F);
            }

            token.ThrowIfCancellationRequested();
            return solution;
        }
        public static MathNet.Numerics.LinearAlgebra.Vector<System.Numerics.Complex> SolvePDE(DifferenceScheme scheme, string[] equation, BoundaryConditionPDE[] boundaryConditions, double[,] domain, int n)
        {
            var r = CreateMatrix.Sparse<double>(n, 2);
            var dr = CreateVector.Sparse<float>(2);
            var a = CreateVector.Sparse<System.Numerics.Complex>(equation.Length);

            var A = CreateMatrix.Sparse<System.Numerics.Complex>(n * n, n * n);
            var B = CreateVector.Sparse<System.Numerics.Complex>(n * n);
            var solution = CreateVector.Sparse<System.Numerics.Complex>(n * n);

            for (int j = 0; j < 2; ++j)
            {
                dr[j] = (float)(domain[j, 1] - domain[j, 0]) / (n - 1);

                for (int i = 0; i < n; ++i)
                    r[i, j] = domain[j, 0] + i * dr[j];
            }

            for (int i = 0; i < equation.Length; ++i)
            {
                for (int j = 0; j < n; ++j)
                {
                    for (int k = 0; k < n; ++k)
                        a[i] = RPNParser.Calculate(equation[i], (float)r[j, 0], (float)r[k, 1]);
                }
            }

            for (int k = 0; k < n * n; ++k)
            {
                switch (scheme)
                {
                    case DifferenceScheme.FORWARD:
                        if (k + 2 * n < n * n)
                            A[k, k + 2 * n] = a[0] / (dr[0] * dr[0]);

                        if (k + n < n * n)
                            A[k, k + n] = a[2] / dr[0] - 2 * a[0] / (dr[0] * dr[0]);

                        if (k + 2 < n * n)
                            A[k, k + 2] = a[1] / (dr[1] * dr[1]);

                        if (k + 1 < n * n)
                            A[k, k + 1] = a[3] / dr[1] - 2 * a[1] / (dr[1] * dr[1]);

                        A[k, k] = a[0] / (dr[0] * dr[0]) + a[1] / (dr[1] * dr[1]) - a[2] / dr[0] - a[3] / dr[1] + a[4];
                        B[k] = a[5];
                        break;

                    case DifferenceScheme.CENTRAL:
                        if (k + n < n * n)
                            A[k, k + n] = a[0] / (dr[0] * dr[0]);

                        if (k - n > 0)
                            A[k, k - n] = a[0] / (dr[0] * dr[0]) - a[2] / (2 * dr[0]);

                        if (k + 1 < n * n)
                            A[k, k + 1] = a[1] / (dr[1] * dr[1]);

                        if (k - 1 > 0)
                            A[k, k - 1] = a[1] / (dr[1] * dr[1]) - a[3] / (2 * dr[1]);

                        A[k, k] = a[4] - 2 * a[0] / (dr[0] * dr[0]) - 2 * a[1] / (dr[1] * dr[1]);
                        B[k] = a[5];
                        break;
                }
            }

            for (int i = 0; i < boundaryConditions.Length; ++i)
            {
                var condition = boundaryConditions[i];

                if (condition.Order == 0)
                {
                    if (condition.Variable == 0)
                    {
                        int x = (int)((condition.Argument.Real - domain[0, 0]) / dr[0]);

                        for (int k = 0; k < n; ++k)
                            A[n * x, n * k] = 0;

                        A[n * x, n * x] = 1;
                        B[n * x] = condition.Value;
                    }
                    else
                    {
                        int x = (int)((condition.Argument.Real - domain[1, 0]) / dr[1]);

                        for (int k = 0; k < n; ++k)
                            A[x, k] = 0;

                        A[x, x] = 1;
                        B[x] = condition.Value;
                    }
                }
            }

            solution = A.Solve(B);
            return solution;
        }

        public static Dictionary<System.Numerics.Complex, MathNet.Numerics.LinearAlgebra.Vector<System.Numerics.Complex>> SolveEigenvaluePDE(CancellationToken token, string[] equation, BoundaryConditionPDE[] boundaryConditions, double[,] domain, int n) 
        {
            var x = new double[n];
            var y = new double[n];
            var dx = (domain[0, 1] - domain[0, 0]) / (n - 1);
            var dy = (domain[1, 1] - domain[1, 0]) / (n - 1);

            var A = CreateMatrix.Sparse<System.Numerics.Complex>(n * n, n * n);

            var solution = new Dictionary<System.Numerics.Complex, MathNet.Numerics.LinearAlgebra.Vector<System.Numerics.Complex>>();
            token.ThrowIfCancellationRequested();

            for (int i = 0; i < n; ++i)
            {
                for (int j = 0; j < n; ++j)
                {
                    x[i] = domain[0, 0] + i * dx;
                    y[j] = domain[1, 0] + j * dy;
                }
            }

            for (int i = 0; i < n; ++i)
            {
                for (int j = 0; j < n; ++j)
                {
                    var a = RPNParser.Calculate(equation[0], x[i], y[j]); 
                    var b = RPNParser.Calculate(equation[1], x[i], y[j]); 
                    var c = RPNParser.Calculate(equation[2], x[i], y[j]); 
                    var d = RPNParser.Calculate(equation[3], x[i], y[j]); 
                    var e = RPNParser.Calculate(equation[4], x[i], y[j]);

                    var k = n * i + j;

                    if (k + n < n * n)
                        A[k, k + n] = a / (dx * dx) + b / (2 * dx);

                    if (k - n >= 0)
                        A[k, k - n] = a / (dx * dx) - b / (2 * dx);

                    if (k + 1 < n * n)
                        A[k, k + 1] = c / (dy * dy) + d / (2 * dy);

                    if (k - 1 >= 0)
                        A[k, k - 1] = c / (dy * dy) - d / (2 * dy);

                    A[k, k] = -2 * a / (dx * dx) - 2 * b / (dy * dy) + e;
                }
            }

            var evd = A.Evd();
            var U = evd.EigenVectors.EnumerateColumns().ToArray();
            var E = evd.EigenValues;

            for (int i = 0; i < E.Count; ++i)
            {
                if (E[i].Real < 0)
                    continue;

                var valid = true;

                Parallel.For(0, n, j =>
                {
                    if (!U[i][j].AlmostEqual(0, 0.01)
                        || !U[i][n * (n - 1) + j].AlmostEqual(0, 0.01)
                        || !U[i][n * j].AlmostEqual(0, 0.01)
                        || !U[i][n * j + n - 1].AlmostEqual(0, 0.01))
                    {
                        valid = false;
                        return;
                    }
                });

                if (!valid)
                    continue;

                solution.Add(E[i], U[i]);
            }

            return solution;
        }
    }
}