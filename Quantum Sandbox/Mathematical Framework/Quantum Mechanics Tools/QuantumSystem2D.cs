﻿using MathNet.Numerics;
using MathNet.Numerics.Distributions;
using MathNet.Numerics.LinearAlgebra;
using Quantum_Mechanics.DE_Solver;
using ScottPlot;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Quantum_Mechanics.Quantum_Mechanics
{
    public class QuantumSystem2D
    {
        public DiscreteFunction2DComplex WaveFunction { get; private set; }
        public DiscreteFunction2DComplex WaveFunctionMomentumSpace { get; private set; }
        public DiscreteFunction2D PositionSpaceProbabilityDensity { get; private set; }
        public DiscreteFunction2D MomentumSpaceProbabilityDensity { get; private set; }

        private double[,] PositionSpaceDistributionParameters;
        private double[] MomentumSpaceDistributionParameters;

        public double Energy { get; private set; }

        private int EnergyLevel;
        private int AzimuthalLevel;
        private int Precision;
        private double[,] PositionDomain;
        private double[,] MomentumDomain;
        private double[] MomentumMagnitudeDomain;

        private Random Random;

        public QuantumSystem2D(CancellationToken token, int precision, int energyLevel, int azimuthalLevel, double mass, string potential, double[,] positionDomain, double[,] momentumDomain, double[] momentumMagnitudeDomain)
        {
            Random = new Random();
            token.ThrowIfCancellationRequested();

            PositionDomain = positionDomain;
            MomentumDomain = momentumDomain;
            MomentumMagnitudeDomain = momentumMagnitudeDomain;
            Precision = precision;
            EnergyLevel = energyLevel;
            AzimuthalLevel = azimuthalLevel;

            token.ThrowIfCancellationRequested();

            var T = -1 / (2 * mass);
            var V = potential;

            var boundaryConditions = new BoundaryConditionPDE[]
            {
                new BoundaryConditionPDE(0, 0, positionDomain[0, 0].ToString(), "0"),
                new BoundaryConditionPDE(0, 0, positionDomain[0, 1].ToString(), "0"),
                new BoundaryConditionPDE(0, 1, positionDomain[1, 0].ToString(), "0"),
                new BoundaryConditionPDE(0, 1, positionDomain[1, 1].ToString(), "0")
            };

            token.ThrowIfCancellationRequested();
            var schrodingerEquation = new string[] { T.ToString(), T.ToString(), "0", "0", V };

            var solution = DESolver.SolveEigenvaluePDE(token, DifferenceScheme.CENTRAL, schrodingerEquation, boundaryConditions, positionDomain, precision);
            token.ThrowIfCancellationRequested();

            Energy = solution.Keys.ElementAt(energyLevel - 1).Real;

            var dx = (positionDomain[0, 1] - positionDomain[0, 0]) / (precision - 1);
            var dy = (positionDomain[1, 1] - positionDomain[1, 0]) / (precision - 1);
            var x = CreateVector.Sparse<double>(precision);
            var y = CreateVector.Sparse<double>(precision);
            var u_vector = solution.Values.ElementAt(energyLevel - 1);
            var u = CreateMatrix.Sparse<Complex>(Precision, Precision);

            token.ThrowIfCancellationRequested();

            for (int i = 0; i < precision; ++i)
            {
                token.ThrowIfCancellationRequested();

                for (int j = 0; j < precision; ++j)
                {
                    x[i] = positionDomain[0, 0] + i * dx;
                    y[j] = positionDomain[1, 0] + j * dy;
                    u[i, j] = u_vector[precision * i + j];
                    token.ThrowIfCancellationRequested();
                }
            }

            WaveFunction = Interpolator.Bicubic(x, y, u);
            var density = WaveFunction.GetMagnitudeSquared();

            token.ThrowIfCancellationRequested();

            var N = Math.Sqrt(1d / density.Integrate(positionDomain[0, 0], positionDomain[0, 1], positionDomain[1, 0], positionDomain[1, 1]));
            token.ThrowIfCancellationRequested();

            WaveFunction = Interpolator.Bicubic(x, y, N * u);
            WaveFunctionMomentumSpace = WaveFunction.FourierTransform(positionDomain);
            density = WaveFunctionMomentumSpace.GetMagnitudeSquared();

            token.ThrowIfCancellationRequested();

            var Np = Math.Sqrt(1d / density.Integrate(momentumDomain[0, 0], momentumDomain[0, 1], momentumDomain[1, 0], momentumDomain[1, 1]));
            var dkx = (MomentumDomain[0, 1] - MomentumDomain[0, 0]) / (Precision - 1);
            var dky = (MomentumDomain[1, 1] - MomentumDomain[1, 0]) / (Precision - 1);
            var kx = CreateVector.Sparse<double>(Precision);
            var ky = CreateVector.Sparse<double>(Precision);
            var k = CreateMatrix.Sparse<Complex>(Precision, Precision);

            token.ThrowIfCancellationRequested();

            for (int i = 0; i < Precision; ++i)
            {
                token.ThrowIfCancellationRequested();

                for (int j = 0; j < Precision; ++j)
                {
                    kx[i] = MomentumDomain[0, 0] + i * dkx;
                    ky[j] = MomentumDomain[1, 0] + j * dky;

                    k[i, j] = Np * WaveFunctionMomentumSpace.Evaluate(kx[i], ky[j]);
                    token.ThrowIfCancellationRequested();
                }
            }

            WaveFunctionMomentumSpace = Interpolator.Bicubic(kx, ky, k);
            PositionSpaceProbabilityDensity = WaveFunction.GetMagnitudeSquared();
            MomentumSpaceProbabilityDensity = WaveFunctionMomentumSpace.GetMagnitudeSquared();
            PositionSpaceDistributionParameters = GetPositionSpaceDistributionParameters();
            MomentumSpaceDistributionParameters = GetMomentumSpaceDistributionParameters();
            token.ThrowIfCancellationRequested();
        }

        public void PlotPositionSpace(FormsPlot plot)
        {
            WaveFunction.Plot(plot, PositionDomain, Precision);
        }

        public void PlotMomentumSpace(FormsPlot plot)
        {
            var n = Precision * Precision;
            var dk = (MomentumMagnitudeDomain[1] - MomentumMagnitudeDomain[0]) / (n - 1);
            var k = CreateVector.Sparse<double>(n);
            var p = CreateVector.Sparse<double>(n);

            for (int i = 0; i < n; ++i)
            {
                k[i] = MomentumMagnitudeDomain[0] + i * dk;
                p[i] = MomentumSpaceProbabilityDensity.IntegratePolar(k[i] - dk, k[i] + dk, 0, Math.PI * 2);
            }

            var f = Interpolator.Cubic(k, p);
            f.Plot(plot, MomentumMagnitudeDomain, n);
        }

        private int RandomSign()
        {
            var sign = Random.Next(0, 2);

            if (sign == 0)
                return -1;

            return 1;
        }

        public Tuple<Tuple<double, double>, double> MeasurePositionMomentum()
        {
            var r = MeasurePosition();
            var x = r.Item1;
            var y = r.Item1;
            var p = MeasureMomentum();
            

            var R = Tuple.Create(x + RandomSign() * Random.NextDouble() * 0.5, y + RandomSign() * Random.NextDouble());
            return Tuple.Create(R, p + RandomSign() * Random.NextDouble() * 0.5);
        }

        public double MeasureAngularMomentum() 
        { 
            return AzimuthalLevel * (AzimuthalLevel + 1); 
        }

        #region Position Space

        private double[,] GetPositionSpaceDistributionParameters()
        {
            var mean_x = new DiscreteFunction2D((x, y) => x * PositionSpaceProbabilityDensity.Evaluate(x, y)).Integrate(PositionDomain[0, 0], PositionDomain[0, 1], PositionDomain[1, 0], PositionDomain[1, 1]);
            var mean_y = new DiscreteFunction2D((x, y) => y * PositionSpaceProbabilityDensity.Evaluate(x, y)).Integrate(PositionDomain[0, 0], PositionDomain[0, 1], PositionDomain[1, 0], PositionDomain[1, 1]);
            var var_x = new DiscreteFunction2D((x, y) => (x - mean_x) * (x - mean_x) * PositionSpaceProbabilityDensity.Evaluate(x, y)).Integrate(PositionDomain[0, 0], PositionDomain[0, 1], PositionDomain[1, 0], PositionDomain[1, 1]);
            var var_y = new DiscreteFunction2D((x, y) => (y - mean_y) * (y - mean_y) * PositionSpaceProbabilityDensity.Evaluate(x, y)).Integrate(PositionDomain[0, 0], PositionDomain[0, 1], PositionDomain[1, 0], PositionDomain[1, 1]);

            return new double[,] { { mean_x, Math.Sqrt(var_x) }, { mean_y, Math.Sqrt(var_y) } };
        }

        public double GetProbabilityPositionSpace(double x, double y)
        {
            var dx = (PositionDomain[0, 1] - PositionDomain[0, 0]) / (Precision - 1);
            var dy = (PositionDomain[1, 1] - PositionDomain[1, 0]) / (Precision - 1);
            return PositionSpaceProbabilityDensity.Integrate(x - dx, x + dx, y - dy, y + dy);
        }

        public Tuple<double, double> ExpectedPosition()
        {
            var fx = new DiscreteFunction2D(new Func<double, double, double>((x, y) => x * PositionSpaceProbabilityDensity.Evaluate(x, y)));
            var fy = new DiscreteFunction2D(new Func<double, double, double>((x, y) => y * PositionSpaceProbabilityDensity.Evaluate(x, y)));

            var exp_x = fx.Integrate(PositionDomain[0, 0], PositionDomain[0, 1], PositionDomain[1, 0], PositionDomain[1, 1]);
            var exp_y = fy.Integrate(PositionDomain[0, 0], PositionDomain[0, 1], PositionDomain[1, 0], PositionDomain[1, 1]);

            return Tuple.Create(exp_x, exp_y);
        }

        public Tuple<double, double> MeasurePosition()
        {
            var parameters = PositionSpaceDistributionParameters;

            var x = Normal.Sample(parameters[0, 0], parameters[0, 1]);
            var y = Normal.Sample(parameters[1, 0], parameters[1, 1]);

            while (x <= PositionDomain[0, 0] || x >= PositionDomain[0, 1])
                x = Normal.Sample(parameters[0, 0], parameters[0, 1]);

            while (y <= PositionDomain[1, 0] || y >= PositionDomain[1, 1])
                y = Normal.Sample(parameters[1, 0], parameters[1, 1]);

            return Tuple.Create(x, y);
        }

        #endregion

        #region Momentum Space

        private double[] GetMomentumSpaceDistributionParameters()
        {
            var mean = new DiscreteFunction2D((x, y) => Math.Sqrt(x * x + y * y) * MomentumSpaceProbabilityDensity.Evaluate(x, y)).Integrate(MomentumDomain[0, 0], MomentumDomain[0, 1], MomentumDomain[1, 0], MomentumDomain[1, 1]);
            var var = new DiscreteFunction2D((x, y) => (Math.Sqrt(x * x + y * y) - mean) * (Math.Sqrt(x * x + y * y) - mean) * MomentumSpaceProbabilityDensity.Evaluate(x, y)).Integrate(MomentumDomain[0, 0], MomentumDomain[0, 1], MomentumDomain[1, 0], MomentumDomain[1, 1]);

            return new double[] { mean, Math.Sqrt(var) };
        }

        public double GetProbabilityMomentumSpace(double k)
        {
            var n = Precision * Precision;
            var dk = (MomentumMagnitudeDomain[1] - MomentumMagnitudeDomain[0]) / (n - 1);
            return MomentumSpaceProbabilityDensity.IntegratePolar(k - dk, k + dk, 0, Math.PI * 2);
        }

        public double ExpectedMomentum()
        {
            var f = new DiscreteFunction2D(new Func<double, double, double>((kx, ky) => Math.Sqrt(kx * kx + ky * ky) * MomentumSpaceProbabilityDensity.Evaluate(kx, ky)));
            return f.Integrate(MomentumDomain[0, 0], MomentumDomain[0, 1], MomentumDomain[1, 0], MomentumDomain[1, 1]);
        }

        public double MeasureMomentum()
        {
            var parameters = MomentumSpaceDistributionParameters;
            var p = Normal.Sample(parameters[0], parameters[1]);

            while (p <= MomentumMagnitudeDomain[0] || p >= MomentumMagnitudeDomain[1])
                p = Normal.Sample(parameters[0], parameters[1]);

            return p; 
        }

        #endregion
    }
}
