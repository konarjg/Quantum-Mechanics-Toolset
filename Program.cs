using MathNet.Numerics;
using MathNet.Numerics.Differentiation;
using MathNet.Numerics.IntegralTransforms;
using MathNet.Numerics.Integration;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.Optimization;
using OxyPlot;
using Quantum_Mechanics.DE_Solver;
using Quantum_Mechanics.General;
using Quantum_Mechanics.Quantum_Mechanics;
using ScottPlot;
using System.Diagnostics;
using System.Runtime.Versioning;

Control.UseNativeMKL();
Console.OutputEncoding = System.Text.Encoding.UTF8;
/*
var R = CreateVector.SparseOfArray(new double[] { 0, 5 });
var psi = new QuantumSystem1D(QuantumConstants.Me, 1, "0", R.ToArray());

var p = psi.WaveFunction.GetMomentumSpaceValues();

VisualizationTool.Plot(p, R.ToArray(), 400);*/

var n = 1000;

var R = CreateVector.SparseOfArray(new double[] { -2.5, 2.5 });
var x = CreateVector.Sparse<double>(n);
var y = CreateVector.Sparse<Complex32>(n);

var dx = (R[1] - R[0]) / (n - 1);

for (int i = 0; i < n; ++i)
{
    x[i] = R[0] + i * dx;
    y[i] = Complex32.Sqrt(0.4f) * Complex32.Sin(MathF.PI * (float)x[i] / 5f);
}

var F = DiscreteFunctions.Fourier(R, y, n);
var f = CreateVector.Sparse<double>(n);
var N = 0f;

for (int i = 0; i < n; ++i)
{
    f[i] = F[i].MagnitudeSquared;
    N += (float)f[i];
}
f *= 1 / N;

var plot = new Plot();
plot.SetAxisLimits(R[0], R[1], f.Min(), f.Max());
plot.AddSignalXY(x.ToArray(), f.ToArray());
plot.SaveFig("plot.png");

Process.Start("explorer.exe", "plot.png");