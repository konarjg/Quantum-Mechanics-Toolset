using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accord.Math.Decompositions;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MatplotlibCS;
using Quantum_Mechanics.General;
using ScottPlot;
using ScottPlot.Statistics.Interpolation;

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
        public static Vector<Complex32> SolveSystem(Matrix<Complex32> A, Vector<Complex32> b)
        {
            var Y = A.Solve(b);

            return Y;
        }
        public static void SolveODE(DifferenceScheme scheme, string[] equation, BoundaryCondition[] boundaryConditions, double[] domain, int n)
        {
            float dt = (float)(domain[1] - domain[0]) / (n - 1);

            var t = CreateVector.Dense<double>(n);

            var A = CreateMatrix.Dense<Complex32>(n, n);
            var B = CreateVector.Dense<Complex32>(n);

            for (int i = 0; i < n; ++i)
                t[i] = domain[0] + i * dt;

            for (int i = 0; i < n; ++i)
            {
                var a = RPNParser.Calculate(equation[0], (float)t[i]);
                var b = RPNParser.Calculate(equation[1], (float)t[i]);
                var c = RPNParser.Calculate(equation[2], (float)t[i]);
                var d = RPNParser.Calculate(equation[3], (float)t[i]);

                switch (scheme)
                {
                    case DifferenceScheme.BACKWARD:
                        if (i > 1)
                            A[i, i - 2] = a;

                        if (i > 0)
                            A[i, i - 1] = -2 * a - b * dt;

                        A[i, i] = a + b * dt + c * dt * dt;
                        B[i] = d * dt * dt;
                        break;

                    case DifferenceScheme.CENTRAL:
                        if (i + 1 < n)
                            A[i, i + 1] = a + dt / 2 * b;

                        if (i > 0)
                            A[i, i - 1] = a - dt / 2 * b;

                        A[i, i] = d * dt * dt; 
                        B[i] = d * dt * dt;
                        break;

                    case DifferenceScheme.FORWARD:
                        if (i + 2 < n)
                            A[i, i + 2] = a;

                        if (i + 1 < n)
                            A[i, i + 1] = b * dt - 2 * a;

                        A[i, i] = a - b * dt + c * dt * dt;
                        B[i] = d * dt * dt;
                        break;
                }
            }

            for (int i = 0; i < boundaryConditions.Length; ++i)
            {
                var condition = boundaryConditions[i];
                int k = (int)((condition.Argument - (float)domain[0]).Real / dt);

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
                    B[k + 1] = dt * condition.Value;
                }
            }

            var y = SolveSystem(A, B);
            var F = CreateVector.Sparse<double>(n);
            var G = CreateVector.Sparse<double>(n);

            for (int i = 0; i < n; ++i)
            {
                F[i] = y[i].Real;
                G[i] = 1 - Math.Exp(-t[i]);
            }

            var plot = new Plot();
            plot.SetAxisLimits(0, 10, 0, 5);
            plot.AddSignalXY(t.ToArray(), F.ToArray());
            plot.AddSignalXY(t.ToArray(), G.ToArray(), Color.Red);
            plot.SaveFig("plot.png");

            Process.Start("explorer.exe", "plot.png");
        }
    }
}
