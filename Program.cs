using MathNet.Numerics.Integration;
using Quantum_Mechanics.DE_Solver;
using Quantum_Mechanics.Quantum_Mechanics;
using ScottPlot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.Distributions;

var R = new double[,] { { 0, 5 }, { 0, Math.PI * 2 } };
var P = new double[,] { { 0, 5 }, { 0, Math.PI * 2 } };
var psi = new QuantumSystemPolar((int)Math.Sqrt(500), 1, 1, QuantumConstants.Me, "(0-1)/(x + 0,0001)", R, P);

var exp = psi.ExpectedPosition();
var avg = Tuple.Create(0d, 0d);

var plot = new Plot(600, 600);
var r = R[0, 1];
var dfi = (R[1, 1] - R[1, 0]) / 999;

for (int i = 0; i < 1000; ++i)
{
    var fi = R[1, 0] + i * dfi;
    plot.AddPoint(r * Math.Cos(fi), r * Math.Sin(fi), Color.Black, 10);
}

for (int i = 0; i < 1000; ++i)
{
    var x = psi.MeasurePosition();
    avg = Tuple.Create(avg.Item1 + x.Item1 / 1000, avg.Item2 + x.Item2 / 1000);
    plot.AddPoint(x.Item1, x.Item2, Color.Blue);
}

plot.AddPoint(exp.Item1, exp.Item2, Color.Red, 10);
plot.AddPoint(avg.Item1, exp.Item2, Color.Green, 10);
plot.SetAxisLimits(-R[0, 1] - 0.5, R[0, 1] + 0.5, -R[0, 1] - 0.5, R[0, 1] + 0.5);
plot.SaveFig("position_space.png");
Process.Start("explorer.exe", "position_space.png");