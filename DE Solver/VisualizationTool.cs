using MathNet.Numerics;
using MathNet.Numerics.Integration;
using MathNet.Numerics.LinearAlgebra;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using ScottPlot;
using ScottPlot.Drawing;
using ScottPlot.Plottable;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace Quantum_Mechanics.DE_Solver
{
    public static class VisualizationTool
    { 
        public static void Plot(Vector<Complex32> function, double[] domain)
        {
            var y = CreateVector.Sparse<double>(function.Count);

            for (int i = 0; i < function.Count; ++i)
                y[i] = function[i].MagnitudeSquared;

            var plot = new Plot();
            plot.SetAxisLimits(domain.Min(), domain.Max(), y.Min(), y.Max());
            plot.AddSignalXY(domain, y.ToArray());

            plot.SaveFig("plot.png");
            Process.Start("explorer.exe", "plot.png");
        }

        public static void Plot(string path, Vector<Complex32> function, double[] domain, int n)
        {
            var x = CreateVector.Sparse<double>(n);
            var dx = (domain[1] - domain[0]) / (n - 1);

            for (int i = 0; i < n; ++i)
                x[i] = domain[0] + i * dx;

            var y = CreateVector.Sparse<double>(n);

            for (int i = 0; i < n; ++i)
                y[i] = function[i].MagnitudeSquared;

            var plot = new Plot();

            plot.AddSignalXY(x.ToArray(), y.ToArray());
            plot.SetAxisLimits(domain[0], domain[1], y.Min(), y.Max());
            plot.SaveFig(path);

            Process.Start("explorer.exe", path);
        }

        public static void Plot(string path, Vector<double> function, double[] domain, int n)
        {
            var x = CreateVector.Sparse<double>(n);
            var dx = (domain[1] - domain[0]) / (n - 1);

            for (int i = 0; i < n; ++i)
                x[i] = domain[0] + i * dx;

            var y = CreateVector.Sparse<double>(n);

            for (int i = 0; i < n; ++i)
                y[i] = function[i];

            var plot = new Plot();

            plot.AddSignalXY(x.ToArray(), y.ToArray());
            plot.SetAxisLimits(domain[0], domain[1], y.Min(), y.Max());
            plot.SaveFig(path);

            Process.Start("explorer.exe", path);
        }

        public static void Plot(string path, Vector<double> function, Vector<double> x, int n)
        {
            var plot = new Plot();

            plot.AddSignalXY(x.ToArray(), function.ToArray());
            plot.SetAxisLimits(x.Min(), x.Max(), function.Min(), function.Max());
            plot.SaveFig(path);

            Process.Start("explorer.exe", path);
        }

        public static void Plot2D(string path, Vector<Complex32> function, double[,] domain, int n)
        {
            var r = CreateMatrix.Sparse<double>(n, n);

            for (int i = 0; i < n; ++i)
            {
                for (int j = 0; j < n; ++j)
                    r[i, j] = function[n * i + j].MagnitudeSquared;
            }

            var plot = new Plot();
            
            plot.SetAxisLimits(domain[0, 0], domain[0, 1], domain[1, 0], domain[1, 1]);

            var map = plot.AddHeatmap(r.ToArray());

            map.CellWidth = (domain[0, 1] - domain[0, 0]) / (n - 1);
            map.CellHeight = (domain[1, 1] - domain[1, 0]) / (n - 1);
            
            map.XMin = domain[0, 0];
            map.XMax = domain[0, 1];
            map.YMin = domain[1, 0];
            map.YMax = domain[1, 1];

            var bar = plot.AddColorbar();
            bar.DataAreaPadding = 10;

            plot.SaveFig(path);
            Process.Start("explorer.exe", path);
        }

        public static void Plot2D(string path, Matrix<Complex32> function, double[,] domain, int n)
        {
            var r = CreateMatrix.Sparse<double>(n, n);

            for (int i = 0; i < n; ++i)
            {
                for (int j = 0; j < n; ++j)
                    r[i, j] = function[i, j].MagnitudeSquared;
            }

            var plot = new Plot();

            plot.SetAxisLimits(domain[0, 0], domain[0, 1], domain[1, 0], domain[1, 1]);

            var map = plot.AddHeatmap(r.ToArray());

            map.CellWidth = (domain[0, 1] - domain[0, 0]) / (n - 1);
            map.CellHeight = (domain[1, 1] - domain[1, 0]) / (n - 1);

            map.XMin = domain[0, 0];
            map.XMax = domain[0, 1];
            map.YMin = domain[1, 0];
            map.YMax = domain[1, 1];

            var bar = plot.AddColorbar();
            bar.DataAreaPadding = 10;

            plot.SaveFig(path);
            Process.Start("explorer.exe", path);
        }
    }
}
