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
    public class WaveFunction1D
    {
        public Vector<double> PositionDomain { get; private set; }
        public Vector<double> MomentumDomain { get; private set; }

        private Vector<Complex32> PositionSpaceValues;
        private Func<double, Complex32> PositionSpaceHandle;
        private Vector<double> PositionSpaceProbabilities;

        public WaveFunction1D(double mass, int energyLevel, string potential, double[] positionDomain)
        {
            PositionDomain = CreateVector.SparseOfArray(positionDomain);
            MomentumDomain = CreateVector.SparseOfArray(positionDomain);

            var T = "(0-" + (QuantumConstants.H * QuantumConstants.H / (2 * mass)) + ")";
            var equation = new string[] { T, "0", potential };
            var boundaryConditions = new BoundaryCondition[]
            {
                new BoundaryCondition(0, positionDomain[0].ToString(), "0"),
                new BoundaryCondition(0, positionDomain[1].ToString(), "0")
            };

            var U = DESolver.SolveEigenvalueODE(DifferenceScheme.CENTRAL, equation, boundaryConditions, positionDomain, 400).Values.ElementAt(energyLevel - 1);
 
            var u = CreateVector.Sparse<double>(400);
            var psi = CreateVector.Sparse<Complex32>(400);
            var N = 0d;

            for (int i = 0; i < 400; ++i)
            {
                u[i] = U[i].MagnitudeSquared;
                psi[i] = U[i];

                N += u[i];
            }

            N = 1d / N;
         
            u.Multiply(N);
            psi.Multiply((float)Math.Sqrt(N));

            PositionSpaceValues = psi;
            PositionSpaceProbabilities = u;
            PositionSpaceHandle = Interpolator.InterpolateComplex(positionDomain, psi, 400);
        }

        #region Position Space

        public Func<double, Complex32> GetPositionSpace()
        {
            return PositionSpaceHandle;
        }

        public Vector<Complex32> GetPositionSpaceValues()
        {
            return PositionSpaceValues;
        }

        public Vector<double> GetProbabilityMapPositionSpace()
        {
            return PositionSpaceProbabilities;
        }

        #endregion

        #region Momentum Space
        public Func<double, Complex32> GetMomentumSpace()
        {
            var momentumSpace = CreateVector.SparseOfArray(PositionSpaceValues.ToArray());

            for (int i = 0; i < 400; ++i)
                momentumSpace[i] = PositionSpaceValues[i];

            var fourier = momentumSpace.Multiply(1 / MathF.Sqrt(2 * MathF.PI)).ToArray();
            var values = CreateVector.Sparse<Complex32>(400);

            var N = 0f;
            var dp = (MomentumDomain[1] - MomentumDomain[0]) / 399;

            for (int i = 0; i < 400; ++i)
            {
                N += fourier[i].MagnitudeSquared;
                values[i] = fourier[i];
            }
            values = values.Multiply(MathF.Sqrt(1 / N));

            return Interpolator.InterpolateComplex(MomentumDomain.ToArray(), values, 400);
        }

        public Vector<Complex32> GetMomentumSpaceValues()
        {
            var psi = GetMomentumSpace();
            var values = CreateVector.Sparse<Complex32>(400);
            var dp = (MomentumDomain[1] - MomentumDomain[0]) / 399;

            for (int i = 0; i < 400; ++i)
            {
                values[i] = psi.Invoke(MomentumDomain[0] + i * dp);
                Console.WriteLine("psi(p={0})={1}", MomentumDomain[0] + i * dp, values[i]);
            }
            return values;
        }

        public Vector<double> GetProbabilityMapMomentumSpace()
        {
            var map = CreateVector.Sparse<double>(400);
            var values = GetMomentumSpaceValues();

            for (int i = 0; i < 400; ++i)
                map[i] = values[i].MagnitudeSquared;

            return map;
        }

        #endregion
    }

    public class QuantumSystem1D
    {
        public double Mass { get; set; }
        public int EnergyLevel { get; set; }
        public string PotentialFunction { get; set; }
        public double[] PositionDomain { get; set; }
        public WaveFunction1D WaveFunction { get; set; }

        public QuantumSystem1D(double mass, int energyLevel, string potentialFunction, double[] positionDomain) 
        {
            Mass = mass;
            EnergyLevel = energyLevel;
            PotentialFunction = potentialFunction;
            PositionDomain = positionDomain;

            WaveFunction = new WaveFunction1D(Mass, EnergyLevel, PotentialFunction, PositionDomain);
        }

        public Vector<Complex32> GetPlot()
        {
            var dx = (PositionDomain[1] - PositionDomain[0]) / 399;
            var psi = CreateVector.Sparse<Complex32>(400);

            for (int i = 0; i < 400; ++i)
            {
                var x = PositionDomain[0] + i * dx;
                psi[i] = WaveFunction.GetPositionSpace().Invoke(x);
            }

            return psi;
        }

        public Func<double, double> GetProbabilityFunction()
        {
            return x =>
            {
                var dx = (PositionDomain[1] - PositionDomain[0]) / 399;

                if (x >= PositionDomain[0] && x <= PositionDomain[1])
                {
                    var map = WaveFunction.GetProbabilityMapPositionSpace();
                    var kx = (int)((x - PositionDomain[0]) / dx);

                    return map[kx];
                }

                throw new ArgumentException();
            };
        }

        public double MaxProbabilityPoint()
        {
            var p = GetProbabilityFunction();
            var dx = (PositionDomain[1] - PositionDomain[0]) / 399;

            var max_x = 0d;

            for (int i = 0; i < 400; ++i)
            {
                var x = PositionDomain[0] + i * dx;

                var P = p.Invoke(x);

                if (P >= p.Invoke(max_x))
                    max_x = x;
            }

            return max_x;
        }

        public double ExpectedPosition()
        {
            var P = WaveFunction.GetProbabilityMapPositionSpace();

            var dx = (PositionDomain[1] - PositionDomain[0]) / 399;
            var x_exp = 0d;

            for (int i = 0; i < 400; ++i)
            {
                var x = PositionDomain[0] + i * dx;
                x_exp += x * P[i];
            }

            return x_exp;
        }
    }
}
