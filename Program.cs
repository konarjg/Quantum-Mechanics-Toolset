using MathNet.Numerics.LinearAlgebra;
using Quantum_Mechanics.DE_Solver;
using ScottPlot;

var n = (int)Math.Sqrt(1000);
var dx = 2 * Math.PI / (n - 1);
var dy = 2 * Math.PI / (n - 1);
var x = CreateVector.Sparse<double>(n);
var y = CreateVector.Sparse<double>(n);
var u = CreateMatrix.Sparse<double>(n, n);

for (int i = 0; i < n; ++i)
{
    for (int j = 0; j < n; ++j)
    {
        x[i] = 0 + i * dx;
        y[j] = 0 + j * dy;

        u[i, j] = 0.08 * Math.Pow(Math.Sin(Math.PI * x[i] / 5) * Math.Sin(Math.PI * y[i] / 5), 2);    
    }
}

var f = Interpolator.Bicubic(x, y, u);

Console.WriteLine(f.Integrate(0, 5, 0, 5));
