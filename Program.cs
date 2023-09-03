using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using Quantum_Mechanics.DE_Solver;

Console.OutputEncoding = System.Text.Encoding.UTF8;

var u = DESolver.SolvePDE(DifferenceScheme.FORWARD, new string[] { "0", "0", "0", "1", "1", "0", "0", "0" }, null, new double[,] { { 0, 10 }, { 0, 10 }, { 0, 10 } }, 10); 
Console.WriteLine(u[0]);
Console.ReadKey();

