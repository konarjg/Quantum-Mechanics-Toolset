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
var psi = new QuantumSystem1D(QuantumConstants.Me, 1, QuantumConstants.Me * 5000 + "*x^2", R);

var max_x = psi.MaxProbabilityPoint();
var exp_x = psi.ExpectedPosition();

var max_p = psi.MaxProbabilityMomentum();
var exp_p = psi.ExpectedMomentum();

VisualizationTool.Plot("position_space.png", psi.GetPlot(), R.ToArray(), 200);
VisualizationTool.Plot("momentum_space.png", psi.WaveFunction.GetMomentumSpaceValues(), R.ToArray(), 200);

Console.WriteLine("Energy level: E={0}", psi.WaveFunction.Energy);
Console.WriteLine("Max probability point: P(x={0})={1}", max_x, psi.GetProbabilityFunctionPositionSpace().Invoke(max_x));
Console.WriteLine("Expected point: P(x={0})={1}\n", exp_x, psi.GetProbabilityFunctionPositionSpace().Invoke(exp_x));
Console.WriteLine("Max probability momentum: P(p={0})={1}", max_p, psi.GetProbabilityFunctionMomentumSpace().Invoke(max_p));
Console.WriteLine("Expected momentum: P(p={0})={1}\n", exp_p, psi.GetProbabilityFunctionMomentumSpace().Invoke(exp_p));

var n = 100;
var x_avg = 0d;
var p_avg = 0d;

for (int i = 0; i < n; ++i)
{
    var x = psi.MeasurePosition();
    x_avg += x / n;
}

for (int i = 0; i < n; ++i)
{
    var p = psi.MeasureMomentum();
    p_avg += p / n;
}

Console.WriteLine("On average ~x={0}", x_avg);
Console.WriteLine("On average ~p={0}", p_avg);