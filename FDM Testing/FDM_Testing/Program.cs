using FDM_Testing;
using MathNet.Numerics.LinearAlgebra;
using Quantum_Mechanics.DE_Solver;
using ScottPlot;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Drawing.Text;
using System.Text;
using System.Threading.Tasks;

Console.OutputEncoding = Encoding.Unicode;

var max_e = 0.5;
var min_e = 0.01;

var k = (int)((max_e - min_e) / 0.01);

var x = new double[k];
var y = new double[k];

for (int i = 0; i <= k; ++i)
{
    var C = Solver.Solve(20, min_e + i * 0.01);
    Console.WriteLine("σ(e={0}) = {1}%", min_e + i * 0.01, C);

    x[i] = min_e + i * 0.01;
    y[i] = C;
}

var f = Interpolator.Cubic(CreateVector.SparseOfArray(x), CreateVector.SparseOfArray(y));
f.Plot(new double[] { min_e, max_e }, 1000);

Process.Start("explorer.exe", "plot.png");