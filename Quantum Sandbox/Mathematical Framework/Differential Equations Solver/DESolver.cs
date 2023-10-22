using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using Quantum_Mechanics.General;

namespace Quantum_Mechanics.DE_Solver
{
    public class BoundaryCondition
    {
        public int Order { get; set; }
        public System.Numerics.Complex Argument { get; set; }
        public System.Numerics.Complex Value { get; set; }

        public BoundaryCondition() { }

        public BoundaryCondition(int order, string argument, string value)
        {
            Order = order;
            Argument = RPNParser.Calculate(argument);
            Value = RPNParser.Calculate(value);
        }
    }

    public class PeriodicBoundaryCondition : BoundaryCondition
    {
        public double Period { get; set; }

        public PeriodicBoundaryCondition(double period) : base(2, "0", "0")
        {
            Period = period;
        }
    }

    public static class DESolver
    {
        /*//Normalizes the given function to square integrate to one
        private static Vector<System.Numerics.Complex> Normalize(Vector<System.Numerics.Complex> values, double dx, bool polar = false)
        {
            var N2 = System.Numerics.Complex.Zero;

            if (!polar)
            {
                for (int i = 0; i < values.Count; ++i)
                    N2 += values[i].MagnitudeSquared() * dx;
            }
            else
            {
                for (int i = 0; i < values.Count; ++i)
                    N2 += values[i].MagnitudeSquared() * i * dx * dx;
            }

            values *= System.Numerics.Complex.Sqrt(1d / N2);
            return values;
        }*/

        //Computes coefficient of FDM stencil with 2nd order accuracy in specified direction
        //n -> order of derivative
        //k -> orientation of the point for example +1, -1
        //d -> grid spacing in a target direction
        private static double Coefficient(int n, int k, double d)
        {
            if (n == 2)
            {
                switch (k)
                {
                    case -1:
                        return 1d / (d * d);

                    case 0:
                        return -2d / (d * d);

                    case 1:
                        return 1d / (d * d);
                }

                return double.NaN;
            }

            switch (k)
            {
                case -1:
                    return -1d / (2 * d);

                case 0:
                    return 0;

                case 1:
                    return 1d / (2 * d);
            }

            return double.NaN;
        }

        public static List<(double, DiscreteFunctionComplex)> SolveODE(string[] equation, double[] domain, BoundaryCondition[] boundaryConditions, int resolution)
        {
            var n = resolution;
            var dx = (domain[1] - domain[0]) / (n - 1);
            var x = CreateVector.Sparse<double>(n);
            var a = CreateMatrix.Sparse<System.Numerics.Complex>(n, equation.Length);

            for (int i = 0; i < n; ++i)
            {
                x[i] = domain[0] + i * dx;

                for (int j = 0; j < equation.Length; ++j)
                    a[i, j] = RPNParser.Calculate(equation[j], x[i]);
            }

            var H = CreateMatrix.Sparse<System.Numerics.Complex>(n, n);
            var solution = new List<(double, DiscreteFunctionComplex)>();

            for (int i = 0; i < n; ++i)
            {
                if (i + 1 < n)
                    H[i, i + 1] = a[i, 0] * Coefficient(2, 1, dx) + a[i, 1] * Coefficient(1, 1, dx);

                if (i - 1 >= 0)
                    H[i, i - 1] = a[i, 0] * Coefficient(2, -1, dx) + a[i, 1] * Coefficient(1, -1, dx);

                H[i, i] = a[i, 0] * Coefficient(2, 0, dx) + a[i, 2];
            }

            var evd = H.Evd();
            var E = evd.EigenValues;

            for (int i = 0; i < boundaryConditions.Length; ++i)
            {
                var j = (int)((boundaryConditions[i].Argument.Real - domain[0]) / dx);

                if (boundaryConditions[i].Order == 0)
                    H[j, j] = boundaryConditions[i].Value;
                else if (boundaryConditions[i].Order == 2)
                {
                    var condition = (PeriodicBoundaryCondition)boundaryConditions[i];
                    j = (int)((condition.Period - domain[0]) / dx);

                    var tmp = H[0, 0];
                    H[0, 0] = H[j, j];
                    H[j, j] = tmp;
                }
            }

            evd = H.Evd();
            var U = evd.EigenVectors.EnumerateColumns().ToArray();

            for (int i = 0; i < 10; ++i)
            {
                E[i] = E[i].Magnitude;
                var u = Interpolator.Cubic(x, U[i]);

                solution.Add((E[i].Real, u));
            }

            return solution;
        }
    }
}