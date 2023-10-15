using FDM_Testing;
using MathNet.Numerics.LinearAlgebra;
using ScottPlot;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Drawing.Text;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

Console.OutputEncoding = Encoding.Unicode;

var C = Solver.Solve(20);

Console.WriteLine("Error: {0}", C);