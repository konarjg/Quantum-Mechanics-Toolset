using MathNet.Numerics.LinearAlgebra;
using Quantum_Mechanics.DE_Solver;
using ScottPlot;

var n = (int)Math.Sqrt(1000);
var dx = 2e9 / (n - 1);
var dy = 2e9 / (n - 1);
var x = CreateVector.Sparse<double>(n);
var y = CreateVector.Sparse<double>(n);
var u = CreateVector.Sparse<double>(n * n);

for (int i = 0; i < n; ++i)
{
    for (int j = 0; j < n; ++j)
    {
        x[i] = -1e9 + i * dx;
        y[j] = -1e9 + j * dy;

        u[n * i + j] = Math.Sin(x[i]) + Math.Sin(y[i]);
    }
}

var f = Interpolator.Bicubic(x, y, u);

Console.WriteLine(f.Evaluate(Math.PI / 4, Math.PI / 4));
