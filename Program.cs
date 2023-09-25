using MathNet.Numerics.LinearAlgebra;
using Quantum_Mechanics.DE_Solver;
using Quantum_Mechanics.Quantum_Mechanics;
using ScottPlot;
using System.Diagnostics;
using System.Drawing;
using System.Numerics;

var R = new double[,] { { 0, 5 }, { 0, 2 * Math.PI } };

var psi = new QuantumSystemPolar((int)Math.Sqrt(500), 1, 1, QuantumConstants.Me, "0", R);

var exp = psi.ExpectedPosition();
var avg_x = 0d;
var avg_y = 0d;
var n = 1000;

var plot = new Plot();

var r = R[0, 1];
var dfi = (R[1, 1] - R[1, 0]) / (n - 1);
var x = new double[n];
var y = new double[n];

for (int i = 0; i < n; ++i)
{
    var fi = R[1, 0] + i * dfi;

    x[i] = r * Math.Cos(fi);
    y[i] = r * Math.Sin(fi);
}

plot.AddScatterLines(x, y);

for (int i = 0; i < n; ++i)
{
    var p = psi.MeasurePosition();
    avg_x += p.Item1 * Math.Cos(p.Item2) / n;
    avg_y += p.Item1 * Math.Sin(p.Item2) / n;
    plot.AddPoint(p.Item1 * Math.Cos(p.Item2), p.Item1 * Math.Sin(p.Item2), Color.Green);
}

plot.AddPoint(exp.Item1, exp.Item2, Color.Magenta, 10);
plot.AddPoint(avg_x, avg_y, Color.Red, 10);
plot.SetAxisLimits(-R[0, 1], R[0, 1], -R[0, 1], R[0, 1]);
plot.SaveFig("position_space.png");

Process.Start("explorer.exe", "position_space.png");
