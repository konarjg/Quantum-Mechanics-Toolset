using FDM_Testing;
using MathNet.Numerics.LinearAlgebra;
using Quantum_Mechanics.DE_Solver;
using ScottPlot;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Drawing.Text;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

Console.OutputEncoding = Encoding.Unicode;

var n = 1;
var u_n = DESolver.SolveODE(500, 5);

var E = u_n[n - 1].Item1;
var u = u_n[n - 1].Item2;

u.Plot(500, new double[] { 0, 5 });