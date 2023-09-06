using MathNet.Numerics;
using MathNet.Numerics.Differentiation;
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

var R = CreateMatrix.SparseOfArray(new double[,] { { 0, 5 }, { 0, 5 } });
var psi = new QuantumSystem2D(QuantumConstants.Me, 1, "0", R.ToArray());

var p = psi.WaveFunction.GetMomentumSpaceValues();

VisualizationTool.Plot(p, psi.WaveFunction.MomentumMagnitudeDomain.ToArray());
