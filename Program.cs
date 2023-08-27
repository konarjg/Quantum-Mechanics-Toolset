using Quantum_Mechanics.DE_Solver;

Console.OutputEncoding = System.Text.Encoding.UTF8;
DESolver.SolveODE(DifferenceScheme.BACKWARD, new string[] { "1", "1", "0", "0" }, new BoundaryCondition[] { new BoundaryCondition(0, "0", "0"), new BoundaryCondition(1, "0", "1") }, new double[] { 0, 10 }, 1000);