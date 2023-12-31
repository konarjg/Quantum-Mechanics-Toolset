﻿using MathNet.Numerics;
using MathNet.Numerics.Distributions;
using MathNet.Numerics.LinearAlgebra;
using Quantum_Mechanics.DE_Solver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Quantum_Mechanics.Quantum_Mechanics
{
    public class QuantumSystem1D
    {
        public DiscreteFunctionComplex WaveFunction { get; private set; }
        public DiscreteFunctionComplex WaveFunctionMomentumSpace { get; private set; }
        public DiscreteFunction PositionSpaceProbabilityDensity { get; private set; }
        public DiscreteFunction MomentumSpaceProbabilityDensity { get; private set; }
        
        private double[] PositionSpaceDistributionParameters;
        private double[] MomentumSpaceDistributionParameters;

        public double Energy { get; private set; }
        public double OrbitalAngularMomentum { get => AzimuthalLevel * (AzimuthalLevel + 1); }

        private int EnergyLevel;
        private int AzimuthalLevel;
        private int Precision;
        private double[] PositionDomain;
        private double[] MomentumDomain;

        public QuantumSystem1D(int precision, int energyLevel, int azimuthalLevel, double mass, string potential, double[] positionDomain, double[] momentumDomain)
        {
            PositionDomain = positionDomain;
            MomentumDomain = momentumDomain;
            Precision = precision;
            EnergyLevel = energyLevel;

            var T = -1 / (2 * mass);
            var V = potential;

            var boundaryConditions = new BoundaryCondition[]
            {
                new BoundaryCondition(0, positionDomain[0].ToString(), "0"),
                new BoundaryCondition(0, positionDomain[1].ToString(), "0")
            };

            var schrodingerEquation = new string[] { T.ToString(), "0", V };

            var solution = DESolver.SolveEigenvalueODE(DifferenceScheme.CENTRAL, schrodingerEquation, boundaryConditions, positionDomain, precision);
            Energy = solution.Keys.ElementAt(energyLevel - 1).Real;

            var dx = (positionDomain[1] - positionDomain[0]) / (precision - 1);
            var x = CreateVector.Sparse<double>(precision);
            var y = solution.Values.ElementAt(energyLevel - 1);

            for (int i = 0; i < precision; ++i)
                x[i] = positionDomain[0] + i * dx;

            WaveFunction = Interpolator.Cubic(x, y);
            var density = WaveFunction.GetMagnitudeSquared();

            var N = Math.Sqrt(1d / density.Integrate(positionDomain[0], positionDomain[1]));

            WaveFunction = Interpolator.Cubic(x, N * y);
            WaveFunctionMomentumSpace = WaveFunction.FourierTransform(positionDomain);
            density = WaveFunctionMomentumSpace.GetMagnitudeSquared();

            var Np = Math.Sqrt(1d / density.Integrate(momentumDomain[0], momentumDomain[1]));
            var dk = (MomentumDomain[1] - MomentumDomain[0]) / (Precision - 1);
            var k = CreateVector.Sparse<double>(Precision);
            var p = CreateVector.Sparse<Complex>(Precision);

            for (int i = 0; i < Precision; ++i)
            {
                k[i] = MomentumDomain[0] + i * dk;
                p[i] = Np * WaveFunctionMomentumSpace.Evaluate(k[i]);
            }

            WaveFunctionMomentumSpace = Interpolator.Cubic(k, p);
            PositionSpaceProbabilityDensity = WaveFunction.GetMagnitudeSquared();
            MomentumSpaceProbabilityDensity = WaveFunctionMomentumSpace.GetMagnitudeSquared();
            PositionSpaceDistributionParameters = GetPositionSpaceDistributionParameters();
            MomentumSpaceDistributionParameters = GetMomentumSpaceDistributionParameters();
        }

        #region Position Space

        private double[] GetPositionSpaceDistributionParameters()
        {
            var mean = new DiscreteFunction(x => x * PositionSpaceProbabilityDensity.Evaluate(x)).Integrate(PositionDomain[0], PositionDomain[1]);
            var std = Math.Sqrt(new DiscreteFunction(x => (x - mean) * (x - mean) * PositionSpaceProbabilityDensity.Evaluate(x)).Integrate(PositionDomain[0], PositionDomain[1]));

            return new double[] { mean, std };
        }

        private double[] GetMomentumSpaceDistributionParameters()
        {
            var mean = new DiscreteFunction(p => p * MomentumSpaceProbabilityDensity.Evaluate(p)).Integrate(MomentumDomain[0], MomentumDomain[1]);
            var std = Math.Sqrt(new DiscreteFunction(p => (p - mean) * (p - mean) * MomentumSpaceProbabilityDensity.Evaluate(p)).Integrate(MomentumDomain[0], MomentumDomain[1]));

            return new double[] { mean, std };
        }

        public double GetProbabilityPositionSpace(double x)
        {
            var dx = (PositionDomain[1] - PositionDomain[0]) / (Precision - 1);
            return PositionSpaceProbabilityDensity.Integrate(x - dx, x + dx);
        }

        public double ExpectedPosition()
        {
            var f = new Func<double, double>(x => x * PositionSpaceProbabilityDensity.Evaluate(x));

            return new DiscreteFunction(f).Integrate(PositionDomain[0], PositionDomain[1]);
        }

        public double MeasurePosition()
        {
            var mean = PositionSpaceDistributionParameters[0];
            var std = PositionSpaceDistributionParameters[1];
            var x = Normal.Sample(mean, std);

            while (x <= PositionDomain[0] || x >= PositionDomain[1])
                x = Normal.Sample(mean, std);   

            return x;
        }

        #endregion

        #region Momentum Space

        public double GetProbabilityMomentumSpace(double k)
        {
            var dk = (MomentumDomain[1] - MomentumDomain[0]) / (Precision - 1);
            return PositionSpaceProbabilityDensity.Integrate(k - dk, k + dk);
        }

        public double ExpectedMomentum()
        {
            var f = new Func<double, double>(k => k * MomentumSpaceProbabilityDensity.Evaluate(k));

            return new DiscreteFunction(f).Integrate(MomentumDomain[0], MomentumDomain[1]);
        }

        public double MeasureMomentum()
        {
            var mean = MomentumSpaceDistributionParameters[0];
            var std = MomentumSpaceDistributionParameters[1];
            var p = Normal.Sample(mean, std);

            while (p <= MomentumDomain[0] || p >= MomentumDomain[1])
                p = Normal.Sample(mean, std);
            
            return p;
        }

        #endregion
    }
}
