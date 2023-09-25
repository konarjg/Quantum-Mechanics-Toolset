using MathNet.Numerics.LinearAlgebra;
using Quantum_Mechanics.DE_Solver;
using Quantum_Mechanics.Quantum_Mechanics;
using ScottPlot;
using System.Numerics;

var R = new double[,] { { 0, 5 }, { 0, 5 } };
var P = new double[,] { { -5, 5 }, { -5, 5 } };
var K = new double[] { 0, 5 };

var psi = new QuantumSystem2D(20, 1, 1, QuantumConstants.Me, "0", R, P, K);
psi.PlotMomentumSpace();

var exp_p = psi.ExpectedMomentum();
var avg_p = 0d;
var p = 0d;

for (int i = 0; i < 1000; ++i)
{
    p = psi.MeasureMomentum();
    avg_p += p / 1000;
    Console.WriteLine("p{0} = {1}", i + 1, p);
}

Console.WriteLine("\n<p> = {0}", exp_p);
Console.WriteLine("~p = {0}", avg_p);
