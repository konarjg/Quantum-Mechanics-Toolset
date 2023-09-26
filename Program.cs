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

var R = new double[,] { { 0, 5 }, { 0, 5 } };
var P = new double[,] { { -5, 5 }, { -5, 5 } };
var K = new double[] { 0, 5 };
var psi = new QuantumSystem2D((int)Math.Sqrt(500), 1, 1, QuantumConstants.Me, "0", R, P, K);

var exp = psi.ExpectedPosition();
var avg = Tuple.Create(0d, 0d);

var plot = new Plot();

for (int i = 0; i < 1000; ++i)
{
    var x = psi.MeasurePosition();
    plot.AddPoint(x.Item1, x.Item2, Color.Blue);
}

plot.SaveFig("position_space.png");
Process.Start("explorer.exe", "position_space.png");