using MathNet.Numerics.LinearAlgebra;
using Quantum_Mechanics.DE_Solver;
using Quantum_Mechanics.Quantum_Mechanics;
using ScottPlot;
using System.Numerics;

var R = new double[,] { { 0, 5 }, { 0, 5 } };
var P = new double[,] { { -5, 5 }, { -5, 5 } };
var K = new double[] { 0, 5 };

var psi = new QuantumSystem2D(20, 1, 1, QuantumConstants.Me, "0", R, P);
psi.PlotMomentumSpace(K);

var max = psi.MostProbableMomenta();

for (int i = 0; i < max.Count(); ++i)
    Console.WriteLine(max[i]);