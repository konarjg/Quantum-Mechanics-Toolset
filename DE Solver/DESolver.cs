using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MatplotlibCS;

namespace Quantum_Mechanics.DE_Solver
{
    public static class DESolver
    {
        public static Vector<Complex32> SolveSystem(Matrix<Complex32> A, Vector<Complex32> b, int n)
        {
            var Y = A.Inverse() * b;

            return Y;
        }

        public static void Solve(float[] domain, int n)
        {
            var dt = (domain[1] - domain[0]) / n;

            var t_elements = new Complex32[n];
            var A_elements = new Complex32[n, n];
            var b_elements = new Complex32[n];

            for (int i = 0; i < n; ++i)
                t_elements[i] = new Complex32(domain[0] + i * dt, 0);

            A_elements[0, 0] = 1;
            b_elements[0] = 0;

            for (int i = 0; i < n - 1; ++i)
            {
                A_elements[i, i + 1] = new Complex32(1, 0);
                b_elements[i] = dt;
            }

            var t = CreateVector.DenseOfArray(t_elements);
            var A = CreateMatrix.DenseOfArray(A_elements);
            var b = CreateVector.DenseOfArray(b_elements);
            var y = SolveSystem(A, b, n);

            var t_plot = new double[n];
            var y_plot = new double[n];

            for (int i = 0; i < n; ++i)
            {
                t_plot[i] = t[i].Real;
                y_plot[i] = y[i].Real;
            }

            var plot = new ScottPlot.Plot(400, 300);

            for (int i = 0; i < n; ++i)
                plot.AddPoint(t_plot[i], y_plot[i]);

            plot.SaveFig("plot.png");

            Process.Start("explorer.exe", "plot.png");
        }
    }
}
