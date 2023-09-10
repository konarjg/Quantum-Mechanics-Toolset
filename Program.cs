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
var psi = new QuantumSystem1D(QuantumConstants.Me, 12, "50*x^2", R);

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

var n = 1000;
var x_avg = 0d;
var p_avg = 0d;

for (int i = 0; i < n; ++i)
{
    var x = psi.MeasurePosition();
    Console.WriteLine("Measured position x{0}={1}", i + 1, x);
    x_avg += x / n;
}

for (int i = 0; i < n; ++i)
{
    var p = psi.MeasureMomentum();
    Console.WriteLine("Measured momentum p{0}={1}", i + 1, p);
    p_avg += p / n;
}

Console.WriteLine("On average ~x={0}", x_avg);
Console.WriteLine("On average ~p={0}", p_avg);*/

var R = CreateMatrix.SparseOfArray(new double[,] { { 0, 5 }, { 0, 5 } });
var X = CreateVector.Sparse<Complex32>(400);
var r = CreateMatrix.Sparse<Complex32>(20, 20);
var u = CreateMatrix.Sparse<Complex32>(20, 20);

var dx = (R[0, 1] - R[0, 0]) / 19;
var dy = (R[1, 1] - R[1, 0]) / 19;

for (int i = 0; i < 20; ++i)
{
    var x = R[0, 0] + i * dx;

    for (int j = 0; j < 20; ++j)
    {
        var y = R[1, 0] + j * dy;

        X[20 * i + j] = 
    }
}