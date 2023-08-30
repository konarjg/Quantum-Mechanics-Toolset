using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using ScottPlot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quantum_Mechanics.DE_Solver
{
    public class VisualizationTool
    {
        public void Plot1D(Vector<Complex32> function, double[] domain, int n)
        {
            var x = CreateVector.Sparse<double>(n);
            var dx = (domain[1] - domain[0]) / n;

            for (int i = 0; i < n; ++i)
                x += domain[0] + i * dx;

            var y = CreateVector.Sparse<double>(n);

            for (int i = 0; i < n; ++i)
                y[i] = function[i].MagnitudeSquared;

            var plot = new Plot();

            plot.AddSignalXY(x.ToArray(), y.ToArray());
            plot.SaveFig("plot.png");

            Process.Start("explorer.exe", "plot.png");
        }
    }
}
