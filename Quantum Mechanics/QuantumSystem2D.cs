using MathNet.Numerics.Integration;
using MathNet.Numerics.LinearAlgebra;
using Quantum_Mechanics.DE_Solver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.IntegralTransforms;
using MathNet.Numerics;
using System.Data.Common;
using MathNet.Numerics.Providers.FourierTransform;

namespace Quantum_Mechanics.Quantum_Mechanics
{
    public static class QuantumConstants
    {
        //Reduced Planck's Constant H = h / 2pi
        public static double H = 1.054571817e-3;
        //Mass of a single electron
        public static double Me = 0.0091093837e-3;
        //Kinetic energy of a single electron
        public static double Te = -H * H / (2 * Me);
    }

    public class WaveFunction2D
    {
        public Matrix<double> PositionDomain { get; private set; }
        public Matrix<double> MomentumDomain { get; private set; }
        public Vector<double> MomentumMagnitudeDomain { get; private set; }

        private Matrix<Complex32> PositionSpaceValues;
        private Func<double, double, Complex32> PositionSpaceHandle;
        private Matrix<double> PositionSpaceProbabilities;

        public WaveFunction2D(double mass, int energyLevel, string potential, double[,] positionDomain)
        {
            PositionDomain = CreateMatrix.SparseOfArray(positionDomain);
            MomentumDomain = CreateMatrix.SparseOfArray(positionDomain);

            MomentumMagnitudeDomain = CreateVector.Sparse<double>(400);
            var dpx = (MomentumDomain[0, 1] - MomentumDomain[0, 0]) / 19;
            var dpy = (MomentumDomain[1, 1] - MomentumDomain[1, 0]) / 19;
            var dp = Math.Sqrt(dpx * dpx + dpy * dpy);

            for (int i = 0; i < 20; ++i)
            {
                for (int j = 0; j < 20; ++j)
                {
                    var px = MomentumDomain[0, 0] + i * dpx;
                    var py = MomentumDomain[1, 0] + j * dpy;

                    MomentumMagnitudeDomain[20 * i + j] = (20 * i + j) * dp;
                }
            }

            var T = "(0-" + (QuantumConstants.H * QuantumConstants.H / (2 * mass)) + ")";
            var equation = new string[] { T, T, "0", "0", potential };
            var boundaryConditions = new BoundaryConditionPDE[]
            {
                new BoundaryConditionPDE(0, 0, positionDomain[0, 0].ToString(), "0"),
                new BoundaryConditionPDE(0, 0, positionDomain[0, 1].ToString(), "0"),
                new BoundaryConditionPDE(0, 1, positionDomain[1, 0].ToString(), "0"),
                new BoundaryConditionPDE(0, 1, positionDomain[1, 1].ToString(), "0")
            };

            var U = DESolver.SolveEigenvaluePDE(DifferenceScheme.CENTRAL, equation, boundaryConditions, positionDomain, 20).Values.ElementAt(energyLevel - 1);
 
            var u = CreateMatrix.Sparse<double>(20, 20);
            var psi = CreateMatrix.Sparse<Complex32>(20, 20);
            var N = 0d;

            for (int i = 0; i < 20; ++i)
            {
                for (int j = 0; j < 20; ++j)
                {
                    u[i, j] = U[20 * i + j].MagnitudeSquared;
                    psi[i, j] = U[20 * i + j];

                    N += u[i, j];
                }
            }

            N = 1d / N;
         
            u.Multiply(N);
            psi.Multiply((float)Math.Sqrt(N));

            PositionSpaceValues = psi;
            PositionSpaceProbabilities = u;
            PositionSpaceHandle = Interpolator.InterpolateComplex(positionDomain, psi, 20);
        }

        #region Position Space

        public Func<double, double, Complex32> GetPositionSpace()
        {
            return PositionSpaceHandle;
        }

        public Vector<Complex32> GetPositionSpaceValues()
        {
            var values = CreateVector.Sparse<Complex32>(400);

            for (int i = 0; i < 20; ++i)
            {
                for (int j = 0; j < 20; ++j)
                    values[20 * i + j] = PositionSpaceValues[i, j];
            }

            return values;
        }

        public Matrix<double> GetProbabilityMapPositionSpace()
        {
            return PositionSpaceProbabilities;
        }

        #endregion

        #region Momentum Space
        public Func<double, Complex32> GetMomentumSpace()
        {
            var momentumSpace = CreateMatrix.Sparse<Complex32>(20, 20);
            PositionSpaceValues.CopyTo(momentumSpace);
            var values = CreateVector.Sparse<Complex32>(400);

            Fourier.Forward2D(momentumSpace.Multiply(1 / MathF.Sqrt(2 * MathF.PI)), FourierOptions.InverseExponent);

            var N = 0d;
            var dpx = (MomentumDomain[0, 1] - MomentumDomain[0, 0]) / 19;
            var dpy = (MomentumDomain[1, 1] - MomentumDomain[1, 0]) / 19;

            for (int i = 0; i < 20; ++i)
            {
                for (int j = 0; j < 20; ++j)
                {
                    values[20 * i + j] = momentumSpace[i, j];
                    N += momentumSpace[i, j].MagnitudeSquared;
                }
            }

            values.Multiply((float)Math.Sqrt(1 / N));
            return Interpolator.InterpolateComplex(MomentumMagnitudeDomain, values);
        }

        public Vector<Complex32> GetMomentumSpaceValues()
        {
            var p = GetMomentumSpace();
            var values = CreateVector.Sparse<Complex32>(400);

            for (int i = 0; i < 400; ++i)
                values[i] = p.Invoke(MomentumMagnitudeDomain[i]);

            return values;
        }

        public Matrix<double> GetProbabilityMapMomentumSpace()
        {
            var map = CreateMatrix.Sparse<double>(20, 20);
            var values = GetMomentumSpaceValues();

            for (int i = 0; i < 20; ++i)
            {
                for (int j = 0; j < 20; ++j)
                    map[i, j] = values[20 * i + j].MagnitudeSquared;
            }

            return map;
        }

        #endregion
    }

    public class QuantumSystem2D
    {
        public double Mass { get; set; }
        public int EnergyLevel { get; set; }
        public string PotentialFunction { get; set; }
        public double[,] PositionDomain { get; set; }
        public WaveFunction2D WaveFunction { get; set; }

        public QuantumSystem2D(double mass, int energyLevel, string potentialFunction, double[,] positionDomain) 
        {
            Mass = mass;
            EnergyLevel = energyLevel;
            PotentialFunction = potentialFunction;
            PositionDomain = positionDomain;

            WaveFunction = new WaveFunction2D(Mass, EnergyLevel, PotentialFunction, PositionDomain);
        }

        public Vector<Complex32> GetPlot()
        {
            var dx = (PositionDomain[0, 1] - PositionDomain[0, 0]) / 19;
            var dy = (PositionDomain[1, 1] - PositionDomain[1, 0]) / 19;
            var psi = CreateVector.Sparse<Complex32>(400);

            for (int i = 0; i < 20; ++i)
            {
                for (int j = 0; j < 20; ++j)
                {
                    var x = PositionDomain[0, 0] + i * dx;
                    var y = PositionDomain[1, 0] + j * dy;

                    psi[20 * i + j] = WaveFunction.GetPositionSpace().Invoke(x, y);
                }
            }

            return psi;
        }

        public Func<double, double, double> GetProbabilityFunction()
        {
            return (x, y) =>
            {
                var dx = (PositionDomain[0, 1] - PositionDomain[0, 0]) / 19;
                var dy = (PositionDomain[1, 1] - PositionDomain[1, 0]) / 19;

                if (x >= PositionDomain[0, 0] && x <= PositionDomain[0, 1] && y >= PositionDomain[1, 0] && y <= PositionDomain[1, 1])
                {
                    var map = WaveFunction.GetProbabilityMapPositionSpace();
                    var P = 0d;

                    var kx = (int)((x - PositionDomain[0, 0]) / dx);
                    var ky = (int)((y - PositionDomain[1, 0]) / dy);

                    return map[kx, ky];
                }

                throw new ArgumentException();
            };
        }

        public Vector<double> MaxProbabilityPoint()
        {
            var p = GetProbabilityFunction();
            var dx = (PositionDomain[0, 1] - PositionDomain[0, 0]) / 19;
            var dy = (PositionDomain[1, 1] - PositionDomain[1, 0]) / 19;

            var max_x = 0d;
            var max_y = 0d;

            for (int i = 0; i < 20; ++i)
            {
                for (int j = 0; j < 20; ++j)
                {
                    var x = PositionDomain[0, 0] + i * dx;
                    var y = PositionDomain[1, 0] + j * dy;

                    var P = p.Invoke(x, y);

                    if (P >= p.Invoke(max_x, max_y))
                    {
                        max_x = x;
                        max_y = y;
                    }
                }
            }

            return CreateVector.SparseOfArray(new double[] { max_x, max_y });
        }

        public Vector<double> ExpectedPosition()
        {
            var P = WaveFunction.GetProbabilityMapPositionSpace();

            var dx = (PositionDomain[0, 1] - PositionDomain[0, 0]) / 19;
            var dy = (PositionDomain[1, 1] - PositionDomain[1, 0]) / 19;

            var x_exp = 0d;
            var y_exp = 0d;

            for (int i = 0; i < 20; ++i)
            {
                for (int j = 0; j < 20; ++j)
                {
                    var x = PositionDomain[0, 0] + i * dx;
                    var y = PositionDomain[1, 0] + j * dy;

                    x_exp += x * P[i, j];
                    y_exp += y * P[i, j];
                }
            }

            return CreateVector.SparseOfArray<double>(new double[] { x_exp, y_exp });
        }
    }
}
