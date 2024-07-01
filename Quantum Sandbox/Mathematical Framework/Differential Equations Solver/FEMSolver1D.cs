using Accord.Math;
using Accord.Math.Decompositions;
using FEM.Symbolics;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using Generate = MathNet.Numerics.Generate;

namespace FEM
{
    public static class FEMSolver1D
    {
        //Local to global matrix
        public static int[,] LTOG(int N, int order)
        {
            var T = new int[N - order, order + 1];

            for (int i = 0; i < N - order; ++i)
            {
                for (int j = 0; j < order + 1; ++j)
                    T[i, j] = i + j;
            }

            return T;
        }

        //Global to local coordinates transform
        public static double Local(double x0, int e, double[] x)
        {
            var h = x[1] - x[0];
            return 2 / h * (x0 - x[e]) - 1;
        }

        //Local to global coordinates transform
        public static double Global(double t, int e, double[] x)
        {
            var h = x[1] - x[0];
            return 0.5 * h * t + 0.5 * (x[e] + x[e + 1]);
        }

        //Shape functions
        public static double N(double t, int i, int order)
        {
            if (t < -1 || t > 1)
                return 0d;

            switch (order)
            {
                case 1:
                    if (i == 0)
                        return 0.5 * (1 - t);
                    
                    return 0.5 * (1 + t);

                case 2:
                    if (i == 0)
                        return 0.5 * t * (t - 1);
                    else if (i == 1)
                        return (1 + t) * (1 - t);
                    
                    return 0.5 * t * (t + 1);
            }

            throw new ArgumentException();
        }

        //Shape functions' derivatives
        public static double D(double t, int i, int order)
        {
            switch (order)
            {
                case 1:
                    if (i == 0)
                        return -0.5;

                    return 0.5;

                case 2:
                    if (i == 0)
                        return t - 0.5;
                    else if (i == 1)
                        return -2 * t;

                    return t + 0.5;
            }

            throw new ArgumentException();
        }

        //Element stiffness matrix component
        public static double K(double t, int i, int j, int order)
        {
            return N(t, i, order) * N(t, j, order);
        }

        //First order element stiffness matrix component
        public static double K1(double t, int i, int j, int order)
        {
            return N(t, i, order) * D(t, j, order);
        }

        //Second order element stiffness matrix component
        public static double K2(double t, int i, int j, int order)
        {
            return D(t, i, order) * D(t, j, order);
        }

        //Element load vector component
        public static double F(double t, int i, int order)
        {
            return N(t, i, order);
        }

        public static double Gauss(Func<double, double> f)
        {
            return Integrate.GaussLegendre(f, -1, 1, 5);
        }

        public static List<(double, Func<double, double>)> Solve(string[] equation, double x0, double L, int n, int order)
        {
            var x = Generate.LinearSpaced(n, x0, L);
            var h = x[1] - x[0];

            var T = LTOG(n, order);
            var A = CreateMatrix.Sparse<double>(n, n);
            var B = CreateMatrix.Sparse<double>(n, n);

            var a = new Expression[equation.Length];

            for (int i = 0; i < equation.Length; ++i)
                a[i] = new Expression(equation[i]);

            for (int e = 0; e < n - order; ++e)
            {
                for (int i = 0; i < order + 1; ++i)
                {
                    for (int j = 0; j < order + 1; ++j)
                    {
                        var I = T[e, i];
                        var J = T[e, j];
                        
                        A[I, J] += Gauss(t =>
                        {
                            var x0 = Global(t, e, x);

                            return -2 / h * a[0].Calculate(x0) * K2(t, i, j, order) + a[1].Calculate(x0) * K1(t, i, j, order) + h / 2 * a[2].Calculate(x0) * K(t, i, j, order);
                        });

                        B[I, J] += Gauss(t => h / 2 * K(t, i, j, order));
                    }
                }
            }

            A = A.RemoveRow(n - 1).RemoveColumn(n - 1).RemoveRow(0).RemoveColumn(0);
            B = B.RemoveRow(n - 1).RemoveColumn(n - 1).RemoveRow(0).RemoveColumn(0);

            var evd = new GeneralizedEigenvalueDecomposition(A.ToArray(), B.ToArray());
            var E = evd.RealEigenvalues;
            var U = evd.Eigenvectors;
            var solutions = new List<(double, double[])>();

            for (int i = 0; i < E.Length; ++i)
                solutions.Add((E[i], U.GetColumn(i)));

            solutions = solutions.OrderBy(x => x.Item1).ToList();
            var result = new List<(double, Func<double, double>)>();

            for (int j = 0; j < solutions.Count; ++j)
            {
                var q_reduced = solutions[j].Item2;
                var q = new double[n];

                for (int i = 1; i < n - 1; ++i)
                    q[i] = q_reduced[i - 1];

                var interp = Interpolate.CubicSpline(x, q);

                var f = new Func<double, double>(interp.Interpolate);

                result.Add((solutions[j].Item1, f));
            }

            return result;
        }
    }
}
