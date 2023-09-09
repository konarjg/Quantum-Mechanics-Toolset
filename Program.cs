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

var R = CreateVector.SparseOfArray(new double[] { 0, 5 });
var psi = new QuantumSystem1D(QuantumConstants.Me, 1, 5000*QuantumConstants.Me+"*x^2", R);

var max_x = psi.MaxProbabilityPoint();
var exp_x = psi.ExpectedPosition();

var max_k = psi.MaxProbabilityMomentum();
var exp_k = psi.ExpectedMomentum();

VisualizationTool.Plot("plot.png", psi.GetPlot(), R.ToArray(), 200);
VisualizationTool.Plot("plot1.png", psi.WaveFunction.GetMomentumSpaceValues(), R.ToArray(), 200);

Console.WriteLine("Energy level: E={0}", psi.WaveFunction.Energy);
Console.WriteLine("Max probability point: P(x={0})={1}", max_x, psi.GetProbabilityFunctionPositionSpace().Invoke(max_x));
Console.WriteLine("Expected point: P(x={0})={1}\n", exp_x, psi.GetProbabilityFunctionPositionSpace().Invoke(exp_x));
Console.WriteLine("Max probability momentum: P(p={0})={1}", max_k / QuantumConstants.H, psi.GetProbabilityFunctionMomentumSpace().Invoke(max_k));
Console.WriteLine("Expected momentum: P(p={0})={1}\n", exp_k / QuantumConstants.H, psi.GetProbabilityFunctionMomentumSpace().Invoke(exp_k));

var n = 1000;
var x_avg = 0d;
var p_avg = 0d;

for (int i = 0; i < n; ++i)
{
    var x = psi.MeasurePosition();
    //Console.WriteLine("Measured position x{0}={1}", i + 1, x);
    x_avg += x / n;
}

for (int i = 0; i < n; ++i)
{
    var p = psi.MeasureMomentum() / QuantumConstants.H;
    //Console.WriteLine("Measured momentum p{0}={1}", i + 1, p);
    p_avg += p / n;
}

Console.WriteLine("On average ~x={0}", x_avg);
Console.WriteLine("On average ~p={0}", p_avg);
